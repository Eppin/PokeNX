namespace PokeNX.Core.RNG
{
    using System.Runtime.CompilerServices;

    public abstract class Xoroshiro
    {
        private ulong _s0, _s1;

        protected Xoroshiro(ulong s0, ulong s1)
        {
            _s0 = s0;
            _s1 = s1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Next()
        {
            var s0 = _s0;
            var s1 = _s1;
            var result = s0 + s1;

            s1 ^= s0;
            _s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
            _s1 = RotateLeft(s1, 37);

            return result;
        }

        public uint NextUInt() => (uint)(Next() >> 32);

        public uint NextUInt(uint max) => NextUInt() % max;

        public void Advance(uint advances)
        {
            for (var advance = 0; advance < advances; advance++)
                Next();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }
    }
}