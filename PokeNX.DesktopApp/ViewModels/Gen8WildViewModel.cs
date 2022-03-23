namespace PokeNX.DesktopApp.ViewModels;

using System.Collections.ObjectModel;
using System.Reactive;
using Core;
using Core.Generators;
using Core.Models;
using Core.Models.Enums;
using Models;
using ReactiveUI;
using Utils;

public class Gen8WildViewModel : ViewModelBase
{
    private readonly DiamondPearlService _diamondPearlService;

    #region Properties

    public ObservableCollection<GenerateTableResult> Results { get; set; } = new();

    private uint _initialAdvances;
    public uint InitialAdvances { get => _initialAdvances; set => this.RaiseAndSetIfChanged(ref _initialAdvances, value); }

    private uint _maximumAdvances = 10_000;
    public uint MaximumAdvances { get => _maximumAdvances; set => this.RaiseAndSetIfChanged(ref _maximumAdvances, value); }

    private ulong _seed0;
    public ulong Seed0 { get => _seed0; set => this.RaiseAndSetIfChanged(ref _seed0, value); }

    private ulong _seed1;
    public ulong Seed1 { get => _seed1; set => this.RaiseAndSetIfChanged(ref _seed1, value); }

    private FilterStats _filterStats = new();
    public FilterStats FilterStats { get => _filterStats; set => this.RaiseAndSetIfChanged(ref _filterStats, value); }

    private ulong _encounterEC;
    public ulong EncounterEC { get => _encounterEC; set => this.RaiseAndSetIfChanged(ref _encounterEC, value); }

    private ulong _encounterPID;
    public ulong EncounterPID { get => _encounterPID; set => this.RaiseAndSetIfChanged(ref _encounterPID, value); }

    private uint _targetAdvances;
    public uint TargetAdvances { get => _targetAdvances; set => this.RaiseAndSetIfChanged(ref _targetAdvances, value); }

    private int _advancesLeft;
    public int AdvancesLeft { get => _advancesLeft; set => this.RaiseAndSetIfChanged(ref _advancesLeft, value); }

    private bool _isConnected;
    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }

    private string _errorText = string.Empty;
    public string ErrorText
    {
        get => _errorText;
        set
        {
            this.RaiseAndSetIfChanged(ref _errorText, value);
            this.RaisePropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);

    private ushort _tid;

    private ushort _sid;

    #endregion

    public Gen8WildViewModel(DiamondPearlService diamondPearlService)
    {
        _diamondPearlService = diamondPearlService;

        EventAggregator.RegisterHandler<UseSeedMessage>(message =>
        {
            Seed0 = message.Seed0;
            Seed1 = message.Seed1;
        });

        EventAggregator.RegisterHandler<ProfileMessage>(message =>
        {
            if (message.TID.HasValue) _tid = message.TID.Value;
            if (message.SID.HasValue) _sid = message.SID.Value;
        });

        EventAggregator.RegisterHandler<ConnectionMessage>(message =>
        {
            IsConnected = message.IsConnected;
        });

        OnGenerateCommand = ReactiveCommand.Create(GenerateExecute);
        OnEncounterDetailsCommand = ReactiveCommand.Create(EncounterDetailsExecute);
    }

    public ReactiveCommand<Unit, Unit> OnGenerateCommand { get; }

    public ReactiveCommand<Unit, Unit> OnEncounterDetailsCommand { get; }

    private void EncounterDetailsExecute()
    {
        var wild = _diamondPearlService.GetWild();

        EncounterEC = wild.EC;
        EncounterPID = wild.PID;
    }

    private void GenerateExecute()
    {
        var natureFilter = KeyValues.NaturesFilter[FilterStats.Nature].Key;
        var genderRatio = KeyValues.GenderRatio[FilterStats.GenderRatio].Key;

        var request = new Wild8Request
        {
            TrainerId = _tid,
            SecretId = _sid,
            GenderRatio = genderRatio,
            Filter = new Filter
            {
                Gender = FilterStats.Gender switch
                {
                    0 => GenderFilter.Any,
                    _ => (GenderFilter)FilterStats.Gender - 1
                },
                Natures = new[] { natureFilter },
                Ability = FilterStats.Ability switch
                {
                    0 => AbilityFilter.Any,
                    _ => (AbilityFilter)FilterStats.Ability - 1
                },
                MinIVs = FilterStats.MinimumValues,
                MaxIVs = FilterStats.MaximumValues,
                Shiny = KeyValues.Shinies[FilterStats.Shiny].Key
            },
            Encounter = Encounter.Grass // Default value, make it selectable
        };

        var wildGen8 = new WildGenerator8(InitialAdvances, MaximumAdvances);

        if (Seed0 + Seed1 == 0)
        {
            ErrorText = "S0 and S1 cannot be 0!";

            return;
        }

        // Clear on success!
        ErrorText = string.Empty;

        var results = wildGen8.Generate(Seed0, Seed1, request);

        Results.Clear();

        foreach (var eggResult in results)
            Results.Add(new GenerateTableResult(eggResult));
    }
}