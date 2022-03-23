namespace PokeNX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Models;
    using Models.Enums;
    using PKHeX.Core;
    using RNG;
    using static Models.Enums.Game;
    using static Models.Enums.Version;
    using static Utils.DiamondPearlOffsets;
    using Ability = Models.Enums.Ability;
    using Nature = Models.Enums.Nature;
    using Version = Models.Enums.Version;

    public class DiamondPearlService : SysBotService
    {
        private GameOffset _gameOffset;
        private uint _mainAdvances;

        public Game Game { get; private set; } = None;

        public Version Version { get; private set; } = Unknown;

        public void Connect(string ipAddress, int port)
        {
            if (Connected)
                return;

            Connect(IPAddress.Parse(ipAddress), port);

            var titleId = GetTitleId();
            var buildId = GetBuildId();

            (Game, _gameOffset) = GetGameOffset(titleId, buildId);
            Version = _gameOffset.Version;
        }

        public void CalulateMainRNG(Action<ulong, ulong, uint> callback, CancellationToken cts)
        {
            ulong seed0 = 0;
            ulong seed1 = 0;

            var (s0, s1) = MainRNG();
            var rng = new XorShift(s0, s1);

            var (tmpS0, tmpS1) = rng.Seed();

            while (true)
            {
                if (cts.IsCancellationRequested)
                    return;

                var (ramS0, ramS1) = MainRNG();

                while (ramS0 != tmpS0 || ramS1 != tmpS1)
                {
                    if (cts.IsCancellationRequested)
                        return;

                    rng.Next();
                    (tmpS0, tmpS1) = rng.Seed();
                    _mainAdvances++;

                    if (ramS0 == tmpS0 && ramS1 == tmpS1)
                    {
                        seed0 = ramS0;
                        seed1 = ramS1;
                    }

                    callback(seed0, seed1, _mainAdvances);
                }
            }
        }

        public void ResetAdvances() => _mainAdvances = 0;

        private (ulong S0, ulong S1) MainRNG()
        {
            const int size = sizeof(ulong) * 2;
            var tmpInitialState = ReadPointer(_gameOffset.MainPointer, size);

            var s0 = BitConverter.ToUInt64(tmpInitialState, 0);
            var s1 = BitConverter.ToUInt64(tmpInitialState, 8);

            return (s0, s1);
        }

        public (ushort TID, ushort SID) GetTrainerInfo()
        {
            var baseAddress = GetPlayerPrefsProvider();

            var trainerInfoBytes = ReadBytesAbsolute(baseAddress + 0xe8, sizeof(uint));
            var trainerInfo = BitConverter.ToUInt32(trainerInfoBytes);

            var sid = (ushort)(trainerInfo >> 16);
            var tid = (ushort)trainerInfo;

            return (tid, sid);
        }

        public EggDetails GetDayCareDetails()
        {
            var baseAddress = GetDayCareAddress();

            var eggSeedBytes = ReadBytesAbsolute(baseAddress, sizeof(ulong))
                .Take(sizeof(uint))
                .ToArray();

            var eggSeed = BitConverter.ToUInt32(eggSeedBytes);

            var eggStepCount = BitConverter.ToUInt16(ReadBytesAbsolute(baseAddress + 0x8, sizeof(ulong)));

            return new EggDetails
            {
                Exists = eggSeed > 0,
                Seed = eggSeed,
                StepCount = eggStepCount
            };
        }

        public Wild GetWild()
        {
            var tmp = GetBattleSetupAddress();

            var addresses = new ulong[] { 0x58, 0x28, 0x10, 0x20, 0x20, 0x18 };
            tmp = addresses.Aggregate(tmp, (current, addition) => BitConverter.ToUInt64(ReadBytesAbsolute(current + addition, sizeof(ulong))));

            var result = ReadBytesAbsolute(tmp + 0x20, 0x148);

            var pk = new PK8(result);

            var ivs = new[]
            {
                pk.IVs[0],
                pk.IVs[1],
                pk.IVs[2],
                pk.IVs[4],
                pk.IVs[5],
                pk.IVs[3]
            };

            return new Wild(pk.Species, pk.EncryptionConstant, pk.PID, (Nature)pk.Nature, (Ability)pk.AbilityNumber, ivs);
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
                var species = (Roamer) BitConverter.ToUInt32(speciesBytes);

                yield return (species, seed);
            }
        }

        private ulong GetDayCareAddress() => GetPlayerPrefsProvider() + 0x460;

        private ulong GetBattleSetupAddress()
        {
            var offset = _gameOffset.Version == V130
                ? (ulong) 0x800
                : (ulong) 0xF0;

            return BitConverter.ToUInt64(ReadBytesAbsolute(GetPlayerPrefsProvider() + offset, sizeof(ulong)));
        }

        private ulong GetPlayerPrefsProvider()
        {
            const int size = sizeof(ulong);

            var tmp = BitConverter.ToUInt64(ReadBytesMain(_gameOffset.PlayerPrefsProviderInstance, size));

            var addresses = new ulong[] { 0x18, 0xc0, 0x28, 0xb8, 0 };
            return addresses.Aggregate(tmp, (current, addition) => BitConverter.ToUInt64(ReadBytesAbsolute(current + addition, size)));
        }
    }
}
