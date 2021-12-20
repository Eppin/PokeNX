namespace PokeNX.Core.Utils
{
    public static class DiamondPearlOffsets
    {
        public const string ShiningPearlID = "010018E011D92000";
        public const string BrilliantDiamondID = "0100000011D90000";

        public const string MainPointer = "[main+4F8CCD0]";

        public const uint DiamondPlayerPrefsProviderInstance = 0x4c49098;
        public const uint DiamondFieldManager = 0x4c13650;
        public const string DiamondBoxStartPointer = "[[[[main+4C1DCF8]+B8]+10]+A0]+20"; // v1.1.1 => [[[[main+4C1DCF8]+B8]+10]+A0]+20, v1.1.2 => [[[[main+4E34DD0]+B8]+10]+A0]+20

        public const uint PearlPlayerPrefsProviderInstance = 0x4e60170;
        public const uint PearlFieldManager = 0x4e2a728;
    }
}
