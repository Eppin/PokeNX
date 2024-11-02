
namespace PokeNX.DesktopApp.Controls
{
    using System.Collections.ObjectModel;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Models;


    public partial class ResultsTable : UserControl
    {
        public ResultsTable()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly DirectProperty<ResultsTable, ObservableCollection<GenerateTableResult>> ResultsProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, ObservableCollection<GenerateTableResult>>(
                nameof(Results),
                o => o.Results,
                (o, v) => o.Results = v,
                default!,
                BindingMode.TwoWay,
                true);

        private ObservableCollection<GenerateTableResult> _results = new();
        public ObservableCollection<GenerateTableResult> Results
        {
            get => _results;
            set => SetAndRaise(ResultsProperty, ref _results, value);
        }

        public static readonly DirectProperty<ResultsTable, bool> ShowEggSeedProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, bool>(
                nameof(ShowEggSeed),
                o => o.ShowEggSeed,
                (o, v) => o.ShowEggSeed = v,
                default!,
                BindingMode.TwoWay,
                true);

        private bool _showEggSeed = true;
        public bool ShowEggSeed
        {
            get => _showEggSeed;
            set => SetAndRaise(ShowEggSeedProperty, ref _showEggSeed, value);
        }

        public static readonly DirectProperty<ResultsTable, bool> ShowAbilityProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, bool>(
                nameof(ShowAbility),
                o => o.ShowAbility,
                (o, v) => o.ShowAbility = v,
                default!,
                BindingMode.TwoWay,
                true);

        private bool _showAbility = true;
        public bool ShowAbility
        {
            get => _showAbility;
            set => SetAndRaise(ShowAbilityProperty, ref _showAbility, value);
        }

        public static readonly DirectProperty<ResultsTable, bool> ShowGenderProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, bool>(
                nameof(ShowGender),
                o => o.ShowGender,
                (o, v) => o.ShowGender = v,
                default!,
                BindingMode.TwoWay,
                true);

        private bool _showGender = true;
        public bool ShowGender
        {
            get => _showGender;
            set => SetAndRaise(ShowGenderProperty, ref _showGender, value);
        }

        public static readonly DirectProperty<ResultsTable, bool> ShowHeldItemProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, bool>(
                nameof(ShowHeldItem),
                o => o.ShowHeldItem,
                (o, v) => o.ShowHeldItem = v,
                default!,
                BindingMode.TwoWay,
                true);

        private bool _showHeldItem = true;
        public bool ShowHeldItem
        {
            get => _showHeldItem;
            set => SetAndRaise(ShowHeldItemProperty, ref _showHeldItem, value);
        }

        public static readonly DirectProperty<ResultsTable, bool> ShowEncounterSlotProperty =
            AvaloniaProperty.RegisterDirect<ResultsTable, bool>(
                nameof(ShowEncounterSlot),
                o => o.ShowEncounterSlot,
                (o, v) => o.ShowEncounterSlot = v,
                default!,
                BindingMode.TwoWay,
                true);

        private bool _showEncounterSlot = true;
        public bool ShowEncounterSlot
        {
            get => _showEncounterSlot;
            set => SetAndRaise(ShowEncounterSlotProperty, ref _showEncounterSlot, value);
        }
    }
}
