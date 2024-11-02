namespace PokeNX.Core.Generators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RNG;

    public class EggGenerator8
    {
        private readonly uint _initialAdvances;
        private readonly uint _maxAdvances;

        //private readonly Egg8Request Egg8Request = new Egg8Request
        //{
        //    TrainerId = 64785,
        //    SecretId = 18176,
        //    ParentA = new Parent
        //    {
        //        Nature = Nature.Adamant,
        //        Ability = 1,
        //        IVs = new byte[] { 31, 0, 31, 31, 31, 31 }//{ 4, 5, 6, 7, 8, 9 }
        //    },
        //    ParentB = new Parent
        //    {
        //        Nature = Nature.Sassy,
        //        Ability = 2,
        //        IVs = new byte[] { 31, 0, 31, 31, 31, 31 },
        //        HeldItem = Item.DestinyKnot

        //    },
        //    Filter = new Filter
        //    {
        //        Gender = 255,
        //        //Natures = new List<uint>(),
        //        Ability = 255,
        //        MinIVs = new byte[] { 31, 0, 31, 31, 31, 31 },
        //        MaxIVs = new byte[] { 31, 0, 31, 31, 31, 31 },
        //        Shiny = Shiny.Any
        //    },
        //    IsMasuda = true,
        //    Compatibility = 50
        //};

        //private const ushort TrainerId = 64785;
        //private const ushort SecretId = 18176;

        //private const uint GenderRatio = 0;

        //private const bool IsMasuda = true;
        //private const bool IsShinyCharm = false;
        //private const bool IsVolbeatNidoran = false;

        //private const uint Compatibility = 50; // { 20 (The two don't seem to like each other), 50 (The two seem to get along), 70 (The two seem to get along very well) };

        public EggGenerator8(uint initialAdvances, uint maximumAdvances = 1_000)
        {
            _initialAdvances = initialAdvances;
            _maxAdvances = maximumAdvances;
        }

        public List<EggResult> Generate(ulong s0, ulong s1, Egg8Request request)
        {
            var tsv = (uint)(request.TrainerId ^ request.SecretId);

            var rng = new XorShift(s0, s1);
            rng.Advance(_initialAdvances);

            sbyte pidRolls = 0;
            if (request.IsMasuda)
                pidRolls += 6;
            if (request.IsShinyCharm)
                pidRolls += 2;

            var results = new List<EggResult>();

            for (uint advances = 0; advances < _maxAdvances; advances++, rng.Next())
            {
                var check = new XorShift(rng);

                if (check.Next() % 100 < request.Compatibility)
                {
                    var eggSeed = check.Next();

                    var result = new EggResult
                    {
                        Advances = advances,
                        Seed = eggSeed
                    };

                    // Sign extend to 64 bit
                    long seedLong = unchecked((int)eggSeed);
                    var seed = unchecked((ulong)seedLong);

                    var gen = new XoroshiroBDSP(seed);

                    // Nidoran, Illumise/Volbeat, Indeedee
                    if (request.IsVolbeatNidoran)
                        gen.Next();

                    result.Gender = request.GenderRatio switch
                    {
                        255 => 2,
                        254 => 1,
                        0 => 0,
                        _ => (uint)(gen.NextUInt(252) + 1 < request.GenderRatio ? 1 : 0)
                    };

                    var nature = gen.NextUInt(25);

                    if (request.ParentA.HeldItem == Item.Everstone && request.ParentB.HeldItem == Item.Everstone)
                    {
                        var parent = gen.NextUInt(2);

                        nature = parent switch
                        {
                            0 => (uint)request.ParentA.Nature,
                            1 => (uint)request.ParentB.Nature,
                            _ => throw new ArgumentOutOfRangeException(nameof(parent), parent, null)
                        };
                    }
                    else if (request.ParentA.HeldItem == Item.Everstone)
                        nature = (uint)request.ParentA.Nature;
                    else if (request.ParentB.HeldItem == Item.Everstone)
                        nature = (uint)request.ParentB.Nature;

                    result.Nature = (Nature)nature;

                    var ability = gen.NextUInt(100);
                    var parentAbility = request.ParentB.Ability;

                    result.Ability = parentAbility switch
                    {
                        2 => (uint)(ability < 20 ? 0 : ability < 40 ? 1 : 2),
                        1 => (uint)(ability < 20 ? 0 : 1),
                        _ => (uint)(ability < 80 ? 0 : 1)
                    };

                    var inheritance = request.ParentA.HeldItem == Item.DestinyKnot || request.ParentB.HeldItem == Item.DestinyKnot
                        ? (sbyte)5
                        : (sbyte)3;

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

                    if (request.Filter.CompareShiny(result.Shiny) && request.Filter.CompareAbility(result.Ability) && request.Filter.CompareGender(result.Gender) && request.Filter.CompareNature(result.Nature) && request.Filter.CompareIVs(ivsArr))
                        results.Add(result);

                    //Console.WriteLine();

                    //Console.WriteLine($"Advances: [{advances}], egg seed: [{eggSeed:X08},{eggSeed}], seed: {rng}, pid={pid}, shiny={shiny} {check}, nature: [{(Nature)nature},{nature}], inheritance: {string.Join(',', inheritanceArr)}");
                    //Debug.WriteLine($"Advances: [{advances}], egg seed: [{eggSeed:X08},{eggSeed}], seed: {rng}, pid={pid:X08}, shiny={shiny} {check}, nature: [{(Nature)nature},{nature}], inheritance: {string.Join(',', inheritanceArr)}");
                }
            }

            return results;
        }

        private static Shiny SetShiny(uint tsv, uint psv)
        {
            const int compare = 16;
            var shiny = Shiny.All;

            if (tsv == psv)
                shiny = Shiny.Square;
            else if ((tsv ^ psv) < compare)
                shiny = Shiny.Star;

            return shiny;
        }

        private bool Filter(EggResult result)
        {
            return false;
        }
    }


    // Request
    public class Egg8Request
    {
        public ushort TrainerId { get; set; }

        public ushort SecretId { get; set; }

        public uint GenderRatio { get; set; }

        public bool IsMasuda { get; set; }

        public bool IsShinyCharm { get; set; }

        public bool IsVolbeatNidoran { get; set; }

        public byte Compatibility { get; set; }

        public Filter Filter { get; set; }

        public Parent ParentA { get; set; }

        public Parent ParentB { get; set; }
    }

    public class Parent
    {
        public byte Ability { get; set; }

        public Nature Nature { get; set; }

        public Item? HeldItem { get; set; }

        public byte[] IVs { get; set; } = new byte[6];
    }

    public class Filter
    {
        public uint Gender { get; set; }

        public uint Ability { get; set; }

        public Shiny Shiny { get; set; }

        public byte[]? MinIVs { get; set; }

        public byte[]? MaxIVs { get; set; }

        public IEnumerable<uint>? Natures { get; set; }

        public bool CompareShiny(Shiny shiny)
        {
            return Shiny switch
            {
                Shiny.All => true,
                Shiny.Any => shiny is Shiny.Star or Shiny.Square,
                Shiny.Star => shiny is Shiny.Star,
                Shiny.Square => shiny is Shiny.Square,
                _ => throw new ArgumentOutOfRangeException(nameof(Shiny), Shiny, null)
            };
        }

        public bool CompareAbility(uint ability) => Ability == 255 || Ability == ability;

        public bool CompareGender(uint gender) => Gender == 255 || Gender == gender;

        public bool CompareNature(Nature nature) => Natures?.Contains((uint)nature) ?? true; // TODO test if working correctly..

        public bool CompareIVs(uint[] ivs)
        {
            if (MinIVs == null || MaxIVs == null)
                return true;

            for (var i = 0; i < 6; i++)
            {
                var iv = ivs[i];

                if (iv < MinIVs[i] || iv > MaxIVs[i])
                    return false;
            }

            return true;
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Genderless,
        Ditto
    }

    public enum Item
    {
        None = 0,
        Everstone = 1,
        DestinyKnot = 2
    }

    public enum Shiny
    {
        All,
        Any,
        Star,
        Square
    }

    // Response

    public class EggResult
    {
        public uint Advances { get; set; }

        public uint Seed { get; set; }

        public uint PID { get; set; }

        public uint EC { get; set; }

        public Nature Nature { get; set; }

        public uint Ability { get; set; }

        public uint Gender { get; set; }

        public IVs HP { get; set; }

        public IVs Atk { get; set; }

        public IVs Def { get; set; }


        public IVs SpA { get; set; }

        public IVs SpD { get; set; }

        public IVs Speed { get; set; }

        public Shiny Shiny { get; set; }
    }

    public class IVs
    {
        public byte Value { get; set; }

        public Inheritance? Inheritance { get; set; }

        public IVs(byte value, Inheritance? inheritance)
        {
            Value = value;
            Inheritance = inheritance;
        }
    }

    public enum Inheritance
    {
        A,
        B
    }

    public enum Nature : sbyte
    {
        Hardy = 0,
        Lonely = 1,
        Brave = 2,
        Adamant = 3,
        Naughty = 4,
        Bold = 5,
        Docile = 6,
        Relaxed = 7,
        Impish = 8,
        Lax = 9,
        Timid = 10,
        Hasty = 11,
        Serious = 12,
        Jolly = 13,
        Naive = 14,
        Modest = 15,
        Mild = 16,
        Quiet = 17,
        Bashful = 18,
        Rash = 19,
        Calm = 20,
        Gentle = 21,
        Sassy = 22,
        Careful = 23,
        Quirky = 24
    }
}
