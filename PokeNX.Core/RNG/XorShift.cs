namespace PokeNX.Core.RNG
{
    public class XorShift
    {
        private ulong _s0;
        private ulong _s1;

        public XorShift(ulong s0, ulong s1)
        {
            _s0 = s0;
            _s1 = s1;
        }

        public XorShift(XorShift rng)
        {
            _s0 = rng._s0;
            _s1 = rng._s1;
        }

        public (ulong s0, ulong s1) Seed()
        {
            return (_s0, _s1);
        }

        public void Advance(uint advances)
        {
            for (var i = 0; i < advances; i++)
                Next();
        }

        public uint Next()
        {
            var t = _s0 & 0xFFFFFFFF;
            var s = _s1 >> 32;

            t ^= (t << 11) & 0xFFFFFFFF;
            t ^= t >> 8;
            t ^= s ^ (s >> 19);

            _s0 = ((_s1 & 0xFFFFFFFF) << 32) | (_s0 >> 32);
            _s1 = t << 32 | (_s1 >> 32);

            return (uint)(t % 0xFFFFFFFF + 0x80000000);
        }

        public override string ToString()
        {
            return $"S[0]: {_s0 & 0xFFFFFFFFFFFFFFFF:X16}  S[1]: {_s1 & 0xFFFFFFFFFFFFFFFF:X16}";
        }
    }
}
