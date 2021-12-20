
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
    }
}
