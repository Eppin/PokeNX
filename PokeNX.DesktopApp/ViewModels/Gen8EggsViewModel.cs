namespace PokeNX.DesktopApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using Core;
    using Core.Generators;
    using Core.Models;
    using Core.Models.Enums;
    using Models;
    using ReactiveUI;
    using Utils;

    public class Gen8EggsViewModel : ViewModelBase
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

        private int _compatibility;
        public int Compatibility { get => _compatibility; set => this.RaiseAndSetIfChanged(ref _compatibility, value); }

        private FilterStats _filterStats = new();
        public FilterStats FilterStats { get => _filterStats; set => this.RaiseAndSetIfChanged(ref _filterStats, value); }

        private ParentExtended _parentA = new();
        public ParentExtended ParentA { get => _parentA; set => this.RaiseAndSetIfChanged(ref _parentA, value); }

        private ParentExtended _parentB = new();
        public ParentExtended ParentB { get => _parentB; set => this.RaiseAndSetIfChanged(ref _parentB, value); }

        private ulong _eggSeed;
        public ulong EggSeed { get => _eggSeed; set => this.RaiseAndSetIfChanged(ref _eggSeed, value); }

        private uint _stepCount;
        public uint StepCount { get => 180 - _stepCount; set => this.RaiseAndSetIfChanged(ref _stepCount, value); }

        private uint _targetAdvances;
        public uint TargetAdvances { get => _targetAdvances; set => this.RaiseAndSetIfChanged(ref _targetAdvances, value); }

        private int _advancesLeft;
        public int AdvancesLeft { get => _advancesLeft; set => this.RaiseAndSetIfChanged(ref _advancesLeft, value); }

        private bool _masuda = true;
        public bool Masuda { get => _masuda; set => this.RaiseAndSetIfChanged(ref _masuda, value); }

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

        public IEnumerable<string> Compatibilities { get; set; } = new[] { "The two don't seem to like each other", "The two seem to get along", "The two seem to get along very well" };

        public IEnumerable<string> Abilities { get; set; } = new[] { "1", "2", "H" };

        public IEnumerable<string> Genders { get; set; } = new[] { "Male", "Female", "Genderless", "Ditto" };

        public IEnumerable<KeyValue<Nature, string>> Natures { get; set; } = KeyValues.Natures;

        public IEnumerable<string> Items { get; set; } = new[] { "None", "Everstone", "Destiny Knot" };

        private ushort TID;

        private ushort SID;

        private bool OvalCharm;

        private bool ShinyCharm;

        #endregion

        public Gen8EggsViewModel(DiamondPearlService diamondPearlService)
        {
            _diamondPearlService = diamondPearlService;

            EventAggregator.RegisterHandler<UseSeedMessage>(message =>
            {
                Seed0 = message.Seed0;
                Seed1 = message.Seed1;
            });

            EventAggregator.RegisterHandler<ProfileMessage>(message =>
            {
                if (message.TID.HasValue) TID = message.TID.Value;
                if (message.SID.HasValue) SID = message.SID.Value;
                if (message.OvalCharm.HasValue) OvalCharm = message.OvalCharm.Value;
                if (message.ShinyCharm.HasValue) ShinyCharm = message.ShinyCharm.Value;
            });

            OnGenerateCommand = ReactiveCommand.Create(GenerateExecute);
            OnDayCareDetailsCommand = ReactiveCommand.Create(EggDetailsExecute);
        }

        public ReactiveCommand<Unit, Unit> OnGenerateCommand { get; }

        public ReactiveCommand<Unit, Unit> OnDayCareDetailsCommand { get; }

        private void EggDetailsExecute()
        {
            var eggDetails = _diamondPearlService.GetDayCareDetails();
            EggSeed = eggDetails.Seed;
            StepCount = eggDetails.StepCount;
        }

        private void GenerateExecute()
        {
            if (!CompatibleParents(ParentA.Gender, ParentB.Gender))
            {
                ErrorText = "Parents aren't compatible!";

                return;
            }

            if (ReorderParents(ParentA.Gender, ParentB.Gender))
                (ParentA, ParentB) = (ParentB, ParentA);

            var compatibility = GetCompatibility(Compatibility, OvalCharm);
            var natureFilter = KeyValues.NaturesFilter[FilterStats.Nature].Key;
            var genderRatio = KeyValues.GenderRatio[FilterStats.GenderRatio].Key;

            var request = new Egg8Request
            {
                TrainerId = TID,
                SecretId = SID,
                ParentA = ParentA,
                ParentB = ParentB,
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
                IsMasuda = Masuda,
                IsShinyCharm = ShinyCharm,
                Compatibility = compatibility
            };

            var eggGen8 = new EggGenerator8(InitialAdvances, MaximumAdvances);

            if (Seed0 + Seed1 == 0)
            {
                ErrorText = "S0 and S1 cannot be 0!";

                return;
            }

            // Clear on success!
            ErrorText = string.Empty;

            var results = eggGen8.Generate(Seed0, Seed1, request);

            Results.Clear();

            foreach (var eggResult in results)
                Results.Add(new GenerateTableResult(eggResult));
        }

        private static byte GetCompatibility(int compatibilityIndex, bool ovalCharm)
        {
            var compatibility = compatibilityIndex switch
            {
                0 => 20,
                1 => 50,
                2 => 70,
                _ => throw new ArgumentOutOfRangeException(nameof(compatibilityIndex), compatibilityIndex, null)
            };

            if (ovalCharm)
                compatibility = compatibility switch
                {
                    20 => 40,
                    50 => 80,
                    _ => 88
                };

            return (byte)compatibility;
        }

        private static bool CompatibleParents(int parent1, int parent2)
        {
            switch (parent1)
            {
                // Male/Female
                case 0 when parent2 == 1:
                case 1 when parent2 == 0:

                // Ditto/Female
                case 3 when parent2 == 1:
                case 1 when parent2 == 3:

                // Male/Ditto
                case 0 when parent2 == 3:
                case 3 when parent2 == 0:

                // Genderless/Ditto
                case 2 when parent2 == 3:
                case 3 when parent2 == 2:
                    return true;

                default:
                    return false;
            }
        }

        private static bool ReorderParents(int parent1, int parent2)
        {
            // Female/Male -> Male/Female
            var flag = parent1 == 1 && parent2 == 0;

            // Female/Ditto -> Ditto/Female
            flag |= parent1 == 1 && parent2 == 3;

            // Ditto/Male -> Male/Ditto
            flag |= parent1 == 3 && parent2 == 0;

            // Ditto/Genderless -> Genderless/Ditto
            flag |= parent1 == 3 && parent2 == 2;

            return flag;
        }
    }
}
