namespace PokeNX.Core.Generators;

using Models;
using Models.Enums;
using RNG;

public class WildGenerator8 : Generator8
{
    public WildGenerator8(uint initialAdvances, uint maximumAdvances = 1_000)
        : base(initialAdvances, maximumAdvances)
    {
    }

    public List<GenerateResult> Generate(ulong s0, ulong s1, Wild8Request request)
    {
        var rng = new XorShift(s0, s1);
        rng.Advance(InitialAdvances);

        var results = new List<GenerateResult>();

        for (uint advances = 0; advances < MaximumAdvances; advances++, rng.Next())
        {
            var gen = new XorShift(rng);

            var slotPercent = gen.Next(0, 100);
            gen.Advance(84);

            var result = new GenerateResult
            {
                Advances = advances,
                EncounterSlot = GetEncounterSlot(slotPercent, request.Encounter),
                EC = gen.Next()
            };

            var shinyRandom = gen.Next();

            var pid = gen.Next();
            result.PID = pid;

            var psv = shinyRandom & 0xFFFF ^ shinyRandom >> 0x10;
            var tsv = pid >> 0x10 ^ pid & 0xFFFF;

            var shiny = Shiny.None;

            if ((psv ^ tsv) < 0x10)
                shiny = (psv ^ tsv) == 0
                    ? Shiny.Square
                    : Shiny.Star;

            result.Shiny = shiny;

            var ivs = new[] { -1, -1, -1, -1, -1, -1 };

            for (var i = 0; i < ivs.Length; i++)
            {
                if (ivs[i] == -1)
                    ivs[i] = (int)(gen.Next() % 32);
            }

            result.HP = new IVs((byte)ivs[0]);
            result.Atk = new IVs((byte)ivs[1]);
            result.Def = new IVs((byte)ivs[2]);
            result.SpA = new IVs((byte)ivs[3]);
            result.SpD = new IVs((byte)ivs[4]);
            result.Speed = new IVs((byte)ivs[5]);

            result.Ability = (Ability)(gen.Next() % 2);

            // Unown form check
            //var unownForm = gen.Next() % 28;

            switch (request.GenderRatio)
            {
                case 255:
                    result.Gender = Gender.Genderless;
                    break;
                case 254:
                    result.Gender = Gender.Female;
                    break;
                case 0:
                    result.Gender = Gender.Male;
                    break;
                default:
                    {
                        if (request.Lead is Lead.CuteCharmMale or Lead.CuteCharmFemale && gen.Next() % 3 > 0)
                        {
                            result.Gender = request.Lead == Lead.CuteCharmFemale
                                ? Gender.Male
                                : Gender.Female;
                        }
                        else
                            result.Gender = (Gender)(gen.Next() % 253 + 1 < request.GenderRatio ? 1 : 0);

                        break;
                    }
            }

            if (request.Lead != Lead.Synchronize)
                result.Nature = (Nature)(gen.Next() % 25);

            // 2 calls height, weight
            gen.Advance(4);

            var item = gen.Next(0, 100);
            result.HeldItem = GetHeldItem(item);

            if (Filter(request.Filter, result))
                results.Add(result);
        }

        return results;
    }

    private static HeldItem GetHeldItem(uint value)
    {
        return value > 54
            ? HeldItem.No
            : value < 50
                ? HeldItem._50
                : HeldItem._5;
    }

    private static int GetEncounterSlot(uint value, Encounter encounter)
    {
        return encounter switch
        {
            Encounter.Surfing or Encounter.OldRod => CalculateSlot(value, new[] { 60, 90, 95, 99, 100 }),
            Encounter.GoodRod or Encounter.SuperRod => CalculateSlot(value, new[] { 40, 80, 95, 99, 100 }),
            Encounter.Grass => CalculateSlot(value, new[] { 20, 40, 50, 60, 70, 80, 85, 90, 94, 98, 99, 100 }),
            _ => throw new ArgumentOutOfRangeException(nameof(encounter), encounter, null),
        };
    }

    private static int CalculateSlot(uint percent, IReadOnlyList<int> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            if (percent < slots[i])
                return i;
        }

        return 255;
    }
}
