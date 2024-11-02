namespace PokeNX.Core.Generators
{
    using System.Collections.Generic;
    using Models;
    using Models.Enums;
    using RNG;

    public class StationaryGenerator8
    {
        private readonly uint _initialAdvances;
        private readonly uint _maximumAdvances;

        public StationaryGenerator8(uint initialAdvances, uint maximumAdvances = 1_000)
        {
            _initialAdvances = initialAdvances;
            _maximumAdvances = maximumAdvances;
        }

        public List<GenerateResult> Generate(ulong s0, ulong s1, Stationary8Request request)
        {
            var rng = new XorShift(s0, s1);
            rng.Advance(_initialAdvances);

            var results = new List<GenerateResult>();

            for (uint advances = 0; advances < _maximumAdvances; advances++, rng.Next())
            {
                var gen = new XorShift(rng);

                var result = new GenerateResult
                {
                    Advances = advances,
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

                if (request.SetIVs)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        uint index;

                        while (true)
                        {
                            var ivRandom = gen.Next();
                            index = ivRandom - ivRandom / 6 * 6;

                            if (ivs[index] == -1)
                                break;
                        }

                        ivs[index] = 31;
                    }
                }

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

                result.Gender = request.GenderRatio switch
                {
                    255 => Gender.Genderless,
                    254 => Gender.Female,
                    0 => Gender.Male,
                    _ => (Gender)(gen.Next() % 253 + 1 < request.GenderRatio ? 1 : 0)
                };

                result.Nature = (Nature)(gen.Next() % 25);

                if (Filter(request.Filter, result))
                    results.Add(result);
            }

            return results;
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
