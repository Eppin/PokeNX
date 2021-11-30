namespace PokeNX.DesktopApp.ViewModels
{
    using Core;
    using Core.Models;
    using PokeNX.Core.RNG;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        public Gen8EggsViewModel Gen8EggsViewModel { get; set; } = new();

        public DiamondPearlService DiamondPearlService { get; set; }

        private ulong _seed0;
        public ulong Seed0 { get => _seed0; set => this.RaiseAndSetIfChanged(ref _seed0, value); }

        private ulong _seed1;
        public ulong Seed1 { get => _seed1; set => this.RaiseAndSetIfChanged(ref _seed1, value); }

        private uint _advances;

        public uint Advances
        {
            get => _advances;
            set
            {
                this.RaiseAndSetIfChanged(ref _advances, value);

                if (Gen8EggsViewModel.TargetAdvances > 0)
                    Gen8EggsViewModel.AdvancesLeft = (int)(Gen8EggsViewModel.TargetAdvances - value);
            }
        }

        // TODO move to egg viewmodel
        private ulong _eggSeed;
        public ulong EggSeed { get => _eggSeed; set => this.RaiseAndSetIfChanged(ref _eggSeed, value); }

        private uint _stepCount;
        public uint StepCount { get => 180 - _stepCount; set => this.RaiseAndSetIfChanged(ref _stepCount, value); }

        public void TestMainRNG(DiamondPearlService b, XorShift tmpRNG)
        {
            Advances = 0;

            var (tmpS0, tmpS1) = tmpRNG.Seed();

            while (true)
            {
                var (ramS0, ramS1) = b.MainRNG();

                while (ramS0 != tmpS0 || ramS1 != tmpS1)
                {
                    tmpRNG.Next();
                    (tmpS0, tmpS1) = tmpRNG.Seed();
                    Advances++;

                    if (ramS0 == tmpS0 && ramS1 == tmpS1)
                    {
                        Seed0 = ramS0;
                        Seed1 = ramS1;
                    }
                }
            }
        }

        public void GetEggDetails(DiamondPearlService b)
        {
            var eggDetails = b.GetDayCareDetails(Game.BrilliantDiamond);
            EggSeed = Gen8EggsViewModel.EggSeed = eggDetails.Seed;
            StepCount = Gen8EggsViewModel.StepCount = eggDetails.StepCount;
        }
    }
}
