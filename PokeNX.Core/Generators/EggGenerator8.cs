namespace PokeNX.Core.Generators
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Models.Enums;
    using RNG;
    using Gender = Models.Enums.Gender;

    public class EggGenerator8
    {
        private readonly uint _initialAdvances;
        private readonly uint _maximumAdvances;

        public EggGenerator8(uint initialAdvances, uint maximumAdvances = 1_000)
        {
            _initialAdvances = initialAdvances;
            _maximumAdvances = maximumAdvances;
        }

        public List<GenerateResult> Generate(ulong s0, ulong s1, Egg8Request request)
        {
            var tsv = (uint)(request.TrainerId ^ request.SecretId);

            var rng = new XorShift(s0, s1);
            rng.Advance(_initialAdvances);

            sbyte pidRolls = 0;
            if (request.IsMasuda)
                pidRolls += 6;
            if (request.IsShinyCharm)
                pidRolls += 2;

            var results = new List<GenerateResult>();

            for (uint advances = 0; advances < _maximumAdvances; advances++, rng.Next())
            {
                var check = new XorShift(rng);

                if (check.Next() % 100 >= request.Compatibility)
                    continue;

                var eggSeed = check.Next();

                var result = new GenerateResult
                {
                    Advances = advances,
                    Seed = eggSeed
                };

                // Sign extend to 64 bit
                long seedLong = unchecked((int)eggSeed);
                var seed = unchecked((ulong)seedLong);

                var gen = new Xoroshiro128Plus8B(seed);

                // Nidoran, Illumise/Volbeat, Indeedee
                if (request.IsVolbeatNidoran)
                    gen.Next();

                result.Gender = request.GenderRatio switch
                {
                    255 => Gender.Genderless,
                    254 => Gender.Female,
                    0 => Gender.Male,
                    _ => (Gender)(gen.NextUInt(252) + 1 < request.GenderRatio ? 1 : 0)
                };

                var nature = gen.NextUInt(25);

                if (request.ParentA.HeldItem == EggHeldItem.Everstone && request.ParentB.HeldItem == EggHeldItem.Everstone)
                {
                    var parent = gen.NextUInt(2);

                    nature = parent switch
                    {
                        0 => (uint)request.ParentA.Nature,
                        1 => (uint)request.ParentB.Nature,
                        _ => throw new ArgumentOutOfRangeException(nameof(parent), parent, null)
                    };
                }
                else if (request.ParentA.HeldItem == EggHeldItem.Everstone)
                    nature = (uint)request.ParentA.Nature;
                else if (request.ParentB.HeldItem == EggHeldItem.Everstone)
                    nature = (uint)request.ParentB.Nature;

                result.Nature = (Nature)nature;

                var ability = gen.NextUInt(100);
                var parentAbility = request.ParentB.Ability;

                result.Ability = parentAbility switch
                {
                    2 => (Ability)(ability < 20 ? 0 : ability < 40 ? 1 : 2),
                    1 => (Ability)(ability < 20 ? 0 : 1),
                    _ => (Ability)(ability < 80 ? 0 : 1)
                };

                var inheritance = request.ParentA.HeldItem == EggHeldItem.DestinyKnot || request.ParentB.HeldItem == EggHeldItem.DestinyKnot
                    ? (byte)5
                    : (byte)3;

                var inheritanceArr = new uint[] { 0, 0, 0, 0, 0, 0 };

                // Determine inheritance
                for (sbyte i = 0; i < inheritance;)
                {
                    var index = gen.NextUInt(6);

                    if (inheritanceArr[index] != 0)
                        continue;

                    inheritanceArr[index] = gen.NextUInt(2) + 1;
                    i++;
                }

                var ivsArr = new uint[] { 0, 0, 0, 0, 0, 0 };

                // Assign IVs and inheritance
                for (byte i = 0; i < 6; i++)
                {
                    var iv = gen.NextUInt(32);

                    ivsArr[i] = inheritanceArr[i] switch
                    {
                        1 => request.ParentA.IVs[i],
                        2 => request.ParentB.IVs[i],
                        _ => iv
                    };
                }

                result.HP = new IVs((byte)ivsArr[0], inheritanceArr[0] == 1 ? Inheritance.A : inheritanceArr[0] == 2 ? Inheritance.B : null);
                result.Atk = new IVs((byte)ivsArr[1], inheritanceArr[1] == 1 ? Inheritance.A : inheritanceArr[1] == 2 ? Inheritance.B : null);
                result.Def = new IVs((byte)ivsArr[2], inheritanceArr[2] == 1 ? Inheritance.A : inheritanceArr[2] == 2 ? Inheritance.B : null);
                result.SpA = new IVs((byte)ivsArr[3], inheritanceArr[3] == 1 ? Inheritance.A : inheritanceArr[3] == 2 ? Inheritance.B : null);
                result.SpD = new IVs((byte)ivsArr[4], inheritanceArr[4] == 1 ? Inheritance.A : inheritanceArr[4] == 2 ? Inheritance.B : null);
                result.Speed = new IVs((byte)ivsArr[5], inheritanceArr[5] == 1 ? Inheritance.A : inheritanceArr[5] == 2 ? Inheritance.B : null);

                // Encryption constant
                result.EC = gen.NextUInt();

                uint pid = 0;
                uint psv = 0;

                for (byte roll = 0; roll < pidRolls; roll++)
                {
                    pid = gen.NextUInt(0xffffffff);
                    psv = (pid >> 16) ^ (pid & 0xffff);

                    if ((psv ^ tsv) < 16)
                        break;
                }

                result.PID = pid;
                result.Shiny = SetShiny(tsv, psv);

                if (Filter(request.Filter, result))
                    results.Add(result);
            }

            return results;
        }

        private static Shiny SetShiny(uint tsv, uint psv)
        {
            const int compare = 16;
            var shiny = Shiny.None;

            if (tsv == psv)
                shiny = Shiny.Square;
            else if ((tsv ^ psv) < compare)
                shiny = Shiny.Star;

            return shiny;
        }

        private static bool Filter(Filter filter, GenerateResult result)
        {
            return filter.CompareShiny(result.Shiny) &&
                   filter.CompareAbility(result.Ability) &&
                   filter.CompareGender(result.Gender) &&
                   filter.CompareNature(result.Nature) &&
                   filter.CompareIVs(result.IVs);
        }
    }
}
