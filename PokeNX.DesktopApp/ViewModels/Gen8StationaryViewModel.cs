namespace PokeNX.DesktopApp.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Reactive;
    using Core;
    using Core.Generators;
    using Core.Models;
    using Core.Models.Enums;
    using Models;
    using ReactiveUI;
    using Utils;

    public class Gen8StationaryViewModel : ViewModelBase
    {
        private readonly DiamondPearlService _diamondPearlService;

        #region Properties

        public ObservableCollection<GenerateTableResult> Results { get; set; } = new();

        private uint _initialAdvances;
        public uint InitialAdvances { get => _initialAdvances; set => this.RaiseAndSetIfChanged(ref _initialAdvances, value); }

        private uint _maximumAdvances = 10_000;
        public uint MaximumAdvances { get => _maximumAdvances; set => this.RaiseAndSetIfChanged(ref _maximumAdvances, value); }

        private uint _delay;
        public uint Delay { get => _delay; set => this.RaiseAndSetIfChanged(ref _delay, value); }

        private ulong _seed0;
        public ulong Seed0 { get => _seed0; set => this.RaiseAndSetIfChanged(ref _seed0, value); }

        private ulong _seed1;
        public ulong Seed1 { get => _seed1; set => this.RaiseAndSetIfChanged(ref _seed1, value); }

        private FilterStats _filterStats = new();
        public FilterStats FilterStats { get => _filterStats; set => this.RaiseAndSetIfChanged(ref _filterStats, value); }

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

        #endregion

        public Gen8StationaryViewModel(DiamondPearlService diamondPearlService)
        {
            _diamondPearlService = diamondPearlService;

            OnGenerateCommand = ReactiveCommand.Create(GenerateExecute);
        }

        public ReactiveCommand<Unit, Unit> OnGenerateCommand { get; }

        private void GenerateExecute()
        {
            var natureFilter = KeyValues.NaturesFilter[FilterStats.Nature].Key;
            var genderRatio = KeyValues.GenderRatio[FilterStats.GenderRatio].Key;

            var request = new Stationary8Request
            {
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
            };

            var stationaryGenerator8 = new StationaryGenerator8(InitialAdvances, MaximumAdvances);

            if (Seed0 + Seed1 == 0)
            {
                ErrorText = "S0 and S1 cannot be 0!";

                return;
            }

            // Clear on success!
            ErrorText = string.Empty;

            var results = stationaryGenerator8.Generate(Seed0, Seed1, request);

            Results.Clear();

            foreach (var eggResult in results)
                Results.Add(new GenerateTableResult(eggResult));
        }
    }
}
