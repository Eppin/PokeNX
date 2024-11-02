namespace PokeNX.Core.Generators
{
    using System.Collections.Generic;
    using Models;
    using Models.Enums;
    using RNG;

    public class EventGenerator8 : Generator8
    {
        public EventGenerator8(uint initialAdvances, uint maximumAdvances = 1_000)
            : base(initialAdvances, maximumAdvances)
        {
        }

        public List<GenerateResult> Generate(ulong s0, ulong s1, Event8Request request)
        {
            var rng = new XorShift(s0, s1);
            rng.Advance(InitialAdvances);

            var results = new List<GenerateResult>();

            for (uint advances = 0; advances < MaximumAdvances; advances++, rng.Next())
            {
                var gen = new XorShift(rng);
                gen.Next(); // We need to roll..

                var result = new GenerateResult
                {
                    Advances = advances,
                    EC = gen.Next()
                };

                var pid = gen.Next();
                result.PID = pid;

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

                result.Nature = (Nature)(gen.Next() % 25);

                if (Filter(request.Filter, result))
                    results.Add(result);
            }

            return results;
        }
    }
}
