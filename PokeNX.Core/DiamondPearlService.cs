namespace PokeNX.Core
{
    using System;
    using System.Linq;
    using System.Net;
    using Extensions;
    using Models;
    using Utils;
    using static Models.Game;

    public class  DiamondPearlService : SysBotService
    {
        public void Connect(string ipAddress, int port)
        {
            if (Connected)
                return;

            Connect(IPAddress.Parse(ipAddress), port);
        }

        public (ulong S0, ulong S1) MainRNG()
        {
            const int size = sizeof(ulong) * 2;
            var tmpInitialState = ReadPointer(DiamondPearlOffsets.MainPointer, size);

            var half = tmpInitialState.Length / 2;

            // First half is S0
            var s0 = tmpInitialState
                .Take(half)
                .Reverse()
                .ToUlong();
            
            // Second half is S1
            var s1 = tmpInitialState
                .Skip(half)
                .Reverse()
                .ToUlong();

            return (s0, s1);
        }

        public EggDetails GetDayCareDetails(Game game)
        {
            var baseAddress = GetDayCareAddress(game);

            var eggSeed = ReadBytesAbsolute(baseAddress, sizeof(long)).Reverse().ToUlong();
            var eggStepCount = ReadBytesAbsolute(baseAddress + 0x8, sizeof(long)).Reverse().ToUshort();

            return new EggDetails
            {
                Exists = eggSeed > 0,
                Seed = eggSeed,
                StepCount = eggStepCount
            };
        }

        private ulong GetDayCareAddress(Game game) => GetPlayerPrefsProvider(game) + 0x460;

        private ulong GetPlayerPrefsProvider(Game game)
        {
            const int size = sizeof(ulong);

            var offset = game switch
            {
                BrilliantDiamond => DiamondPearlOffsets.DiamondPlayerPrefsProviderInstance,
                ShiningPearl => DiamondPearlOffsets.PearlPlayerPrefsProviderInstance,
                _ => throw new ArgumentOutOfRangeException(nameof(game), $"Only compatible with {nameof(BrilliantDiamond)} or {nameof(ShiningPearl)}")
            };

            var tmp = ReadBytesMain(offset, size).Reverse().ToUlong();

            var addresses = new ulong[] { 0x18, 0xc0, 0x28, 0xb8, 0 };
            return addresses.Aggregate(tmp, (current, addition) => ReadBytesAbsolute(current + addition, size).Reverse().ToUlong());
        }
    }
}
