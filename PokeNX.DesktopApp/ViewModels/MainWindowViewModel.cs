namespace PokeNX.DesktopApp.ViewModels;

using System.Reactive;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Core;
using Models;
using ReactiveUI;
using Utils;

public class MainWindowViewModel : ViewModelBase
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

    private AppConfig _appConfig = new();

    private CancellationTokenSource _cancellationTokenSource = new();

    public DiamondPearlService DiamondPearlService { get; }

    public Gen8EggsViewModel Gen8EggsViewModel { get; }

    public Gen8LegendaryViewModel Gen8LegendaryViewModel { get; }

    public Gen8WildViewModel Gen8WildViewModel { get; }

    #region Properties
    private string _ipAddress = string.Empty;
    public string IPAddress
    {
        get => _ipAddress;
        set
        {
            this.RaiseAndSetIfChanged(ref _ipAddress, value);
            _appConfig.HostAddress = value;
        }
    }

    private int _port;
    public int Port
    {
        get => _port;
        set
        {
            this.RaiseAndSetIfChanged(ref _port, value);
            _appConfig.Port = value;
        }
    }

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

            if (Gen8LegendaryViewModel.TargetAdvances > 0)
                Gen8LegendaryViewModel.AdvancesLeft = (int)(Gen8LegendaryViewModel.TargetAdvances - value);
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

    public string ConnectionString => IsConnected ? "Connected" : "Connect";

    private ConnectedGame _connectedGame = new();
    public ConnectedGame ConnectedGame { get => _connectedGame; set => this.RaiseAndSetIfChanged(ref _connectedGame, value); }
    #endregion

    public MainWindowViewModel()
    {
        DiamondPearlService = new DiamondPearlService();
        Gen8EggsViewModel = new Gen8EggsViewModel(DiamondPearlService);
        Gen8LegendaryViewModel = new Gen8LegendaryViewModel(DiamondPearlService);
        Gen8WildViewModel = new Gen8WildViewModel(DiamondPearlService);

        OnConnectCommand = ReactiveCommand.Create(OnConnectExecute);
        OnUseSeedCommand = ReactiveCommand.Create(OnUseSeedExecute);

        LoadAppConfig();

        EventAggregator.RegisterHandler<OnExitMessage>(_ => _cancellationTokenSource.Cancel());
    }

    public ReactiveCommand<Unit, Unit> OnConnectCommand { get; }

    public ReactiveCommand<Unit, Unit> OnUseSeedCommand { get; }

    private void OnConnectExecute()
    {
        if (IsConnected)
            return;

        WriteAppConfig();

        DiamondPearlService.Connect(IPAddress, Port);
        var (tid, sid) = DiamondPearlService.GetTrainerInfo();

        ConnectedGame = new ConnectedGame
        {
            Game = DiamondPearlService.Game,
            Version = DiamondPearlService.Version,
            TID = tid,
            SID = sid
        };

        EventAggregator.PostMessage(new ProfileMessage(tid, sid));
        EventAggregator.PostMessage(new ConnectionMessage(true));

        IsConnected = true;

        _cancellationTokenSource = new CancellationTokenSource();

        var thread = new Thread(() => DiamondPearlService.CalulateMainRNG((seed0, seed1, advances) =>
        {
            Seed0 = seed0;
            Seed1 = seed1;
            Advances = advances;
        }, _cancellationTokenSource.Token));

        thread.Start();
    }

    private void OnUseSeedExecute()
    {
        EventAggregator.PostMessage(new UseSeedMessage(Seed0, Seed1));
        DiamondPearlService.ResetAdvances();
    }

    private void LoadAppConfig()
    {
        try
        {
            if (File.Exists(_configPath))
                _appConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(_configPath), JsonSerializerOptions);
            else
                File.WriteAllText(_configPath, JsonSerializer.Serialize(_appConfig, JsonSerializerOptions));
        }
        catch (Exception)
        {
            // Ignore
        }

        if (_appConfig == null)
            return;

        IPAddress = _appConfig.HostAddress;
        Port = _appConfig.Port;
    }

    private void WriteAppConfig()
    {
        File.WriteAllText(_configPath, JsonSerializer.Serialize(_appConfig, JsonSerializerOptions));
    }
}
