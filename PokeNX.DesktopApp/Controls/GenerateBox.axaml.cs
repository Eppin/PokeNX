namespace PokeNX.DesktopApp.Controls
{
    using System.Collections.Generic;
    using System.Reactive;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public partial class GenerateBox : UserControl
    {
        public GenerateBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly DirectProperty<GenerateBox, uint> InitialAdvancesProperty =
            AvaloniaProperty.RegisterDirect<GenerateBox, uint>(
                nameof(InitialAdvances),
                o => o.InitialAdvances,
                (o, v) => o.InitialAdvances = v,
                default!,
                BindingMode.TwoWay,
                true);

        private uint _initialAdvances;
        public uint InitialAdvances
        {
            get => _initialAdvances;
            set => SetAndRaise(InitialAdvancesProperty, ref _initialAdvances, value);
        }

        public static readonly DirectProperty<GenerateBox, uint> MaximumAdvancesProperty =
            AvaloniaProperty.RegisterDirect<GenerateBox, uint>(
                nameof(MaximumAdvances),
                o => o.MaximumAdvances,
                (o, v) => o.MaximumAdvances = v,
                default!,
                BindingMode.TwoWay,
                true);

        private uint _maximumAdvances;
        public uint MaximumAdvances
        {
            get => _maximumAdvances;
            set => SetAndRaise(MaximumAdvancesProperty, ref _maximumAdvances, value);
        }

        public static readonly DirectProperty<GenerateBox, ulong> Seed0Property =
            AvaloniaProperty.RegisterDirect<GenerateBox, ulong>(
                nameof(Seed0),
                o => o.Seed0,
                (o, v) => o.Seed0 = v,
                default!,
                BindingMode.TwoWay,
                true);

        private ulong _seed0;
        public ulong Seed0
        {
            get => _seed0;
            set => SetAndRaise(Seed0Property, ref _seed0, value);
        }

        public static readonly DirectProperty<GenerateBox, ulong> Seed1Property =
            AvaloniaProperty.RegisterDirect<GenerateBox, ulong>(
                nameof(Seed1),
                o => o.Seed1,
                (o, v) => o.Seed1 = v,
                default!,
                BindingMode.TwoWay,
                true);

        private ulong _seed1;
        public ulong Seed1
        {
            get => _seed1;
            set => SetAndRaise(Seed1Property, ref _seed1, value);
        }

        public static readonly DirectProperty<GenerateBox, int?> CompatibilityProperty =
            AvaloniaProperty.RegisterDirect<GenerateBox, int?>(
                nameof(Compatibility),
                o => o.Compatibility,
                (o, v) => o.Compatibility = v,
                default!,
                BindingMode.TwoWay,
                true);

        private int? _compatibility;
        public int? Compatibility
        {
            get => _compatibility == -1 ? 0 : _compatibility;
            set => SetAndRaise(CompatibilityProperty, ref _compatibility, value);
        }

        public static readonly DirectProperty<GenerateBox, int?> GeneratorProperty =
            AvaloniaProperty.RegisterDirect<GenerateBox, int?>(
                nameof(Generator),
                o => o.Generator,
                (o, v) => o.Generator = v,
                default!,
                BindingMode.TwoWay,
                true);

        private int? _generator;
        public int? Generator
        {
            get => _generator == -1 ? 0 : _generator;
            set => SetAndRaise(GeneratorProperty, ref _generator, value);
        }

        public static readonly DirectProperty<GenerateBox, ReactiveCommand<Unit, Unit>> GenerateCommandProperty =
            AvaloniaProperty.RegisterDirect<GenerateBox, ReactiveCommand<Unit, Unit>>(
                nameof(GenerateCommand),
                o => o.GenerateCommand,
                (o, v) => o.GenerateCommand = v,
                default!,
                BindingMode.TwoWay,
                true);

        private ReactiveCommand<Unit, Unit> _generateCommand;
        public ReactiveCommand<Unit, Unit> GenerateCommand
        {
            get => _generateCommand;
            set => SetAndRaise(GenerateCommandProperty, ref _generateCommand, value);
        }

        public bool ShowCompatibilities => Compatibility != null;

        public bool ShowGenerators => Generator != null;

        public static IEnumerable<string> Compatibilities => new[] { "The two don't seem to like each other", "The two seem to get along", "The two seem to get along very well" };

        public static IEnumerable<string> Generators => new[] { "Stationary", "Roamer", "Event" };
    }
}
