namespace PokeNX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Extensions;
    using Models;
    using Models.Enums;
    using PKHeX.Core;
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

            var trainerInfo = ReadBytesAbsolute(baseAddress + 0xe8, sizeof(uint))
                .Reverse()
                .ToUlong();

            var sid = (ushort)(trainerInfo >> 16);
            var tid = (ushort)trainerInfo;

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

        public (uint EC, uint PID) GetWild()
        {
            var tmp = GetBattleSetupAddress();

            var addresses = new ulong[] { 0x58, 0x28, 0x10, 0x20, 0x20, 0x18 };
            tmp = addresses.Aggregate(tmp, (current, addition) => BitConverter.ToUInt64(ReadBytesAbsolute(current + addition, sizeof(ulong))));

            var result = ReadBytesAbsolute(tmp + 0x20, 0x148);

            var pk = new PK8(result);

            return (pk.EncryptionConstant, pk.PID);
        }

        public IEnumerable<(Roamer Roamer, ulong Seed)> GetRoamers()
        {
            var roamersList = BitConverter.ToUInt64(ReadBytesAbsolute(GetPlayerPrefsProvider() + 0x2a0, sizeof(ulong)));
            var readSize = BitConverter.ToUInt32(ReadBytesAbsolute(roamersList + 0x18, sizeof(uint)));

            var size = Math.Min(readSize, 10);

            for (ulong i = 0; i < size; i++)
            {
                var roamer = ReadBytesAbsolute(roamersList + 0x20 + (0x20 * i), 0x20);

                var indexes = new[,]
                {
                    { 0, 4 }, // Area ID
                    { 4, 8 }, // RNG Seed
                    { 12, 4 }, // Species
                    { 16, 4 }, // HP
                    { 20, 1 } // Level
                };

                //var areaIdBytes = roamer.Skip(indexes[0, 0]).Take(indexes[0, 1]).ToArray();
                var seedBytes = roamer.Skip(indexes[1, 0]).Take(indexes[1, 1]).ToArray();
                var speciesBytes = roamer.Skip(indexes[2, 0]).Take(indexes[2, 1]).ToArray();
                //var hpBytes = roamer.Skip(indexes[3, 0]).Take(indexes[3, 1]).ToArray();
                //var levelBytes = roamer.Skip(indexes[4, 0]).Take(indexes[4, 1]).ToArray();

                var seed = BitConverter.ToUInt64(seedBytes);
                var species = (Roamer)BitConverter.ToUInt32(speciesBytes);

                yield return (species, seed);
            }
        }

        private ulong GetDayCareAddress() => GetPlayerPrefsProvider() + 0x460;

        private ulong GetBattleSetupAddress() { return ReadBytesAbsolute(GetPlayerPrefsProvider() + 0x7e8, sizeof(ulong)).Reverse().ToUlong(); }

        private ulong GetPlayerPrefsProvider()
        {
            const int size = sizeof(ulong);

            var tmp = ReadBytesMain(PlayerPrefsProviderInstance, size).Reverse().ToUlong();

            var addresses = new ulong[] { 0x18, 0xc0, 0x28, 0xb8, 0 };
            return addresses.Aggregate(tmp, (current, addition) => ReadBytesAbsolute(current + addition, size).Reverse().ToUlong());
        }
    }
}
