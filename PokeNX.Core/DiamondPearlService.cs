namespace PokeNX.Core
{
    using System;
    using System.Linq;
    using System.Net;
    using Extensions;
    using Models;
    using Models.Enums;
    using static Models.Enums.Game;
    using static Utils.DiamondPearlOffsets;

    public class DiamondPearlService : SysBotService
    {
        public Game Game { get; private set; } = None;

        public void Connect(string ipAddress, int port)
        {
            if (Connected)
                return;

            Connect(IPAddress.Parse(ipAddress), port);

            var titleId = GetTitleID();
            Game = titleId switch
            {
                BrilliantDiamondID => BrilliantDiamond,
                ShiningPearlID => ShiningPearl,
                _ => throw new ArgumentOutOfRangeException(nameof(titleId), titleId, $"Only compatible with {nameof(BrilliantDiamond)} or {nameof(ShiningPearl)}")
            };
        }

        public (ulong S0, ulong S1) MainRNG()
        {
            const int size = sizeof(ulong) * 2;
            var tmpInitialState = ReadPointer(MainPointer, size);

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

        public (ushort TID, ushort SID) GetTrainerInfo()
        {
            var baseAddress = GetPlayerPrefsProvider();

            //var trainerInfo = ReadBytesAbsolute(baseAddress + 0xe8, sizeof(uint));
            var trainerInfo = ReadBytesAbsolute(baseAddress + 0xe8, sizeof(long))
                .Reverse()
                .ToUlong();

            var tid = (ushort)(trainerInfo >> 16);
            var sid = (ushort)trainerInfo;

            return (tid, sid);
        }

        public EggDetails GetDayCareDetails()
        {
            var baseAddress = GetDayCareAddress();

            var eggSeed = ReadBytesAbsolute(baseAddress, sizeof(long))
                .Take(sizeof(int)) // Is this correct?
                .Reverse()
                .ToUlong();

            var eggStepCount = ReadBytesAbsolute(baseAddress + 0x8, sizeof(long))
                .Reverse()
                .ToUshort();

            return new EggDetails
            {
                Exists = eggSeed > 0,
                Seed = eggSeed,
                StepCount = eggStepCount
            };
        }

        private ulong GetDayCareAddress() => GetPlayerPrefsProvider() + 0x460;

        private ulong GetPlayerPrefsProvider()
        {
            const int size = sizeof(ulong);

            var offset = Game switch
            {
                BrilliantDiamond => DiamondPlayerPrefsProviderInstance,
                ShiningPearl => PearlPlayerPrefsProviderInstance,
                _ => throw new ArgumentOutOfRangeException(nameof(Game), Game, $"Only compatible with {nameof(BrilliantDiamond)} or {nameof(ShiningPearl)}")
            };

            var tmp = ReadBytesMain(offset, size).Reverse().ToUlong();

            var addresses = new ulong[] { 0x18, 0xc0, 0x28, 0xb8, 0 };
            return addresses.Aggregate(tmp, (current, addition) => ReadBytesAbsolute(current + addition, size).Reverse().ToUlong());
        }
    }
}
