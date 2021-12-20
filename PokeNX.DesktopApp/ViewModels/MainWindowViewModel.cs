namespace PokeNX.DesktopApp.ViewModels
{
    using System.Reactive;
    using System.Threading;
    using Core;
    using Core.RNG;
    using Models;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        private CancellationTokenSource _cancellationTokenSource = new();

        public DiamondPearlService DiamondPearlService { get; }

        public Gen8EggsViewModel Gen8EggsViewModel { get; }

        public Gen8StationaryViewModel Gen8StationaryViewModel { get; }

        #region Properties
        private string _ipAddress = string.Empty;
        public string IPAddress { get => _ipAddress; set => this.RaiseAndSetIfChanged(ref _ipAddress, value); }

        private int _port = 6000; // Default Sys-bot port
        public int Port { get => _port; set => this.RaiseAndSetIfChanged(ref _port, value); }

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

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                this.RaiseAndSetIfChanged(ref _isConnected, value);
                this.RaisePropertyChanged(nameof(ConnectionString));
            }
        }

        public string ConnectionString => IsConnected ? "Disconnect" : "Connect";

        private ConnectedGame _connectedGame = new();
        public ConnectedGame ConnectedGame { get => _connectedGame; set => this.RaiseAndSetIfChanged(ref _connectedGame, value); }
        #endregion

        public MainWindowViewModel()
        {
            DiamondPearlService = new DiamondPearlService();
            Gen8EggsViewModel = new Gen8EggsViewModel(DiamondPearlService);
            Gen8StationaryViewModel = new Gen8StationaryViewModel(DiamondPearlService);

            OnConnectCommand = ReactiveCommand.Create(OnConnectExecute);
            OnUseSeedCommand = ReactiveCommand.Create(OnUseSeedExecute);
        }

        public ReactiveCommand<Unit, Unit> OnConnectCommand { get; }

        public ReactiveCommand<Unit, Unit> OnUseSeedCommand { get; }

        private void OnConnectExecute()
        {
            if (IsConnected)
            {
                _cancellationTokenSource.Cancel();

                for (var i = 0; i < 500; i++)
                    Thread.Sleep(1);

                DiamondPearlService.Disconnect();
                IsConnected = false;

            }
            else
            {
                DiamondPearlService.Connect(IPAddress, Port);
                var (tid, sid) = DiamondPearlService.GetTrainerInfo();

                ConnectedGame = new ConnectedGame
                {
                    Game = DiamondPearlService.Game,
                    TID = tid,
                    SID = sid
                };

                IsConnected = true;

                DiamondPearlService.TestBoxPointer();
                return;

                _cancellationTokenSource = new CancellationTokenSource();

                var thread = new Thread(() => CalulateMainRNG(_cancellationTokenSource.Token));
                thread.Start();
            }
        }

        private void OnUseSeedExecute()
        {
            // TODO.. Notifier pattern
        }

        private void CalulateMainRNG(CancellationToken cts)
        {
            Advances = 0;

            var (s0, s1) = DiamondPearlService.MainRNG();
            var rng = new XorShift(s0, s1);

            var (tmpS0, tmpS1) = rng.Seed();

            while (true)
            {
                if (cts.IsCancellationRequested) break;

                var (ramS0, ramS1) = DiamondPearlService.MainRNG();

                while (ramS0 != tmpS0 || ramS1 != tmpS1)
                {
                    if (cts.IsCancellationRequested) break;

                    rng.Next();
                    (tmpS0, tmpS1) = rng.Seed();
                    Advances++;

                    if (ramS0 != tmpS0 || ramS1 != tmpS1)
                        continue;

                    Seed0 = ramS0;
                    Seed1 = ramS1;
                }
            }
        }
    }
}
