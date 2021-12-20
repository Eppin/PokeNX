namespace PokeNX.DesktopApp.Controls
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;

    public partial class StatRange : UserControl
    {
        public StatRange()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly DirectProperty<StatRange, int> MinValueProperty =
            AvaloniaProperty.RegisterDirect<StatRange, int>(
                nameof(MinValue),
                o => o.MinValue,
                (o, v) => o.MinValue = v,
                default!,
                BindingMode.TwoWay,
                true);

        private int _minValue;
        public int MinValue
        {
            get => _minValue;
            set => SetAndRaise(MinValueProperty, ref _minValue, value);
        }

        public static readonly DirectProperty<StatRange, int> MaxValueProperty =
            AvaloniaProperty.RegisterDirect<StatRange, int>(
                nameof(MaxValue),
                o => o.MaxValue,
                (o, v) => o.MaxValue = v,
                default!,
                BindingMode.TwoWay,
                true);

        private int _maxValue;
        public int MaxValue
        {
            get => _maxValue;
            set => SetAndRaise(MaxValueProperty, ref _maxValue, value);
        }
    }
}
