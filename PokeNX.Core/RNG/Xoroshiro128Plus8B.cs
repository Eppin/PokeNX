namespace PokeNX.Core.RNG
{
    public class Xoroshiro128Plus8B : Xoroshiro
    {
        public Xoroshiro128Plus8B(ulong seed)
            : base(SplitMix(seed, 0x9E3779B97F4A7C15), SplitMix(seed, 0x3C6EF372FE94F82A))
        {
        }

        private static ulong SplitMix(ulong seed, ulong state)
        {
            seed += state;
            seed = 0xBF58476D1CE4E5B9 * (seed ^ (seed >> 30));
            seed = 0x94D049BB133111EB * (seed ^ (seed >> 27));

            return seed ^ (seed >> 31);
        }
    }
}
