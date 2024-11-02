namespace PokeNX.DesktopApp.Controls;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Core.Models.Enums;
using Models;
using Utils;

public partial class ResultsFilter : UserControl
{
    public ResultsFilter()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly DirectProperty<ResultsFilter, FilterStats> FilterStatsProperty =
        AvaloniaProperty.RegisterDirect<ResultsFilter, FilterStats>(
            nameof(FilterStats),
            o => o.FilterStats,
            (o, v) => o.FilterStats = v,
            default!,
            BindingMode.TwoWay,
            true);

    private FilterStats _filterStats = new();
    public FilterStats FilterStats
    {
        get => _filterStats;
        set => SetAndRaise(FilterStatsProperty, ref _filterStats, value);
    }

    public static IEnumerable<string> GendersFilter => new[] { "Any", "Male", "Female" };

    public static IEnumerable<KeyValue<uint, string>> GendersRatioFilter => KeyValues.GenderRatio;

    public static IEnumerable<string> AbilitiesFilter => new[] { "Any", "1", "2", "H" };

    public static IEnumerable<KeyValue<NatureFilter, string>> NaturesFilter => KeyValues.NaturesFilter;

    public static IEnumerable<KeyValue<ShinyFilter, string>> Shinies => KeyValues.Shinies;
}
