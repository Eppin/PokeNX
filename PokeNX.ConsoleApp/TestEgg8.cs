namespace PokeNX.ConsoleApp
{
    using System;
    using System.Runtime.CompilerServices;

    public class TestEgg8
    {
        public void Generate(UInt64 seed0, UInt64 seed1)
        {
            var rng = new Core.RNG.XorShift(seed0, seed1);

            for (int cnt = 0; cnt < 10; cnt++, rng.Next())
            {
                var check = new Core.RNG.XorShift(rng);
                
                // var n2 = rng.Next2();
                // var next = check.Next3();
                // if ((next % 100) < 21)
                // {
                // check.Next();
                Console.WriteLine($"{cnt}, {check.Next():X8} ");
                // }
            }
        }

        public class XorShift
        {
            public UInt64[] state;

            public XorShift(UInt64 seed0, UInt64 seed1)
            {
                state = new[]
                {
                    (seed0 >> 32),
                    (seed0 & 0xffffffff),
                    (seed1 >> 32),
                    (seed1 & 0xffffffff)
                };
            }

            public XorShift(XorShift other)
            {
                state = new[] { other.state[1], other.state[0], other.state[3], other.state[2] };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UInt64 Next()
            {
                UInt64 t = state[1];
                UInt64 s = state[2];

                t ^= t << 11;
                t ^= t >> 8;
                t ^= s ^ (s >> 19);

                state[1] = state[0];
                state[0] = state[3];
                state[3] = state[2];
                state[2] = t;

                return ((t % 0xffffffff) + 0x80000000) & 0xFFFFFFFF;
            }

            public UInt64 Next(UInt32 max)
            {
                return Next() % max;
            }

            public void Advance(UInt32 advances)
            {
                for (UInt32 advance = 0; advance < advances; advance++)
                    Next();
            }
        }
    }
}
