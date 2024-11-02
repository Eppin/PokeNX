namespace PokeNX.DesktopApp
{
    using ViewModels;

    public static class DesignData
    {
        public static Gen8EggsViewModel Gen8EggsViewModel { get; } = new(null!);

        public static Gen8StationaryViewModel Gen8StationaryViewModel { get; } = new(null!);
    }
}
