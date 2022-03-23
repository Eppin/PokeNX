namespace PokeNX.DesktopApp;

using ViewModels;

public static class DesignData
{
    public static Gen8EggsViewModel Gen8EggsViewModel { get; } = new(null!);

    public static Gen8LegendaryViewModel Gen8LegendaryViewModel { get; } = new(null!);

    public static Gen8WildViewModel Gen8WildViewModel { get; } = new(null!);
}
