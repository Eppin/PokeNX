namespace PokeNX.DesktopApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Avalonia.Media;
    using Core.Generators;
    using ReactiveUI;

    public class Gen8EggsViewModel : ViewModelBase
    {
        public ObservableCollection<Person> Persons { get; set; } = new();

        private uint _initialAdvances;
        public uint InitialAdvances { get => _initialAdvances; set => this.RaiseAndSetIfChanged(ref _initialAdvances, value); }

        private uint _maximumAdvances = 1_000;
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

        public IEnumerable<string> AbilitiesFilter { get; set; } = new[] { "Any", "1", "2", "H" };

        public IEnumerable<string> Genders { get; set; } = new[] { "Male", "Female", "Genderless", "Ditto" };

        public IEnumerable<string> GendersFilter { get; set; } = new[] { "Any", "Male", "Female" };

        public IEnumerable<string> GendersRatioFilter { get; set; } = new[] { "Genderless", "50% ♂ / 50% ♀", "25% ♂ / 75% ♀", "75% ♂ / 25% ♀", "87,5% ♂ / 12,5% ♀", "100% ♂", "100% ♀" };

        public IEnumerable<string> Natures { get; set; } = GetNatures();

        public IEnumerable<string> NaturesFilter { get; set; } = GetNatures(true);

        public IEnumerable<string> Items { get; set; } = new[] { "None", "Everstone", "Destiny Knot" };

        public IEnumerable<string> Shinies { get; set; } = new[] { "Any", "Star", "Square", "Star/Square" };

        private static IEnumerable<string> GetNatures(bool addAny = false)
        {
            var natures = Enum.GetValues<Nature>()
                .Select(n => n.ToString())
                .ToList();

            if (addAny)
                natures.Insert(0, "Any");

            return natures;
        }
    }

    public class FilterStats
    {
        public Values HP { get; set; } = new();

        public Values Atk { get; set; } = new();

        public Values Def { get; set; } = new();

        public Values SpA { get; set; } = new();

        public Values SpD { get; set; } = new();

        public Values Speed { get; set; } = new();

        public byte[] MinimumValues => new[] { (byte)HP.Minimum, (byte)Atk.Minimum, (byte)Def.Minimum, (byte)SpA.Minimum, (byte)SpD.Minimum, (byte)Speed.Minimum };

        public byte[] MaximumValues => new[] { (byte)HP.Maximum, (byte)Atk.Maximum, (byte)Def.Maximum, (byte)SpA.Maximum, (byte)SpD.Maximum, (byte)Speed.Maximum };

        public int Gender { get; set; }

        public int GenderRatio { get; set; }

        public int Ability { get; set; }

        public int Nature { get; set; }

        public int Shiny { get; set; }
    }

    public class Values
    {
        public int Minimum { get; set; }

        public int Maximum { get; set; } = 31;
    }

    public class Person
    {
        public uint Advances { get; set; }

        public uint Seed { get; set; }

        public uint PID { get; set; }

        public Shiny Shiny { get; set; }

        public Nature Nature { get; set; } // TODO sbyte

        public uint Ability { get; set; } // TODO sbyte

        public Gender Gender { get; set; }

        public IVs HP { get; set; }

        public IVs Atk { get; set; }

        public IVs Def { get; set; }

        public IVs SpA { get; set; }

        public IVs SpD { get; set; }

        public IVs Speed { get; set; }

        public bool IsShiny { get; set; }

        public IBrush? RowColor => IsShiny ? new SolidColorBrush(Color.FromRgb(144, 0, 0)) : null;
    }

    public class IVs
    {
        public byte Value { get; set; }

        public Inheritance? Inheritance { get; set; }

        public bool ShowInheritance => Inheritance != null;

        public IVs(byte value, Inheritance? inheritance = null)
        {
            Value = value;
            Inheritance = inheritance;
        }
    }

    public enum Inheritance
    {
        A,
        B
    }

    public class ParentExtended : Parent
    {
        public byte HP { get => IVs[0]; set => IVs[0] = value; }

        public byte Atk { get => IVs[1]; set => IVs[1] = value; }

        public byte Def { get => IVs[2]; set => IVs[2] = value; }

        public byte SpA { get => IVs[3]; set => IVs[3] = value; }

        public byte SpD { get => IVs[4]; set => IVs[4] = value; }

        public byte Speed { get => IVs[5]; set => IVs[5] = value; }

        public new int Ability { get => base.Ability; set => base.Ability = (byte)value; }

        public int Gender { get; set; }

        public new int Nature { get => (int)base.Nature; set => base.Nature = (Nature)value; }

        public new int HeldItem { get => (int)base.HeldItem.GetValueOrDefault(); set => base.HeldItem = (Item)value; }
    }
}
