using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PokeNX.DesktopApp.Views
{
    using System.Threading;
    using Avalonia.Interactivity;
    using Core;
    using Core.RNG;
    using ViewModels;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel d)
                return;

            d.DiamondPearlService = new DiamondPearlService();
            d.DiamondPearlService.Connect("192.168.25.220", 6000);

            var (s0, s1) = d.DiamondPearlService.MainRNG();
            var tmpRNG = new XorShift(s0, s1);

            //while (true)
            //{
            //    //var eggDetails = s.GetDayCareDetails(Game.BrilliantDiamond);
            //    //Console.WriteLine($"Exists: {eggDetails.Exists}, Seed: {eggDetails.Seed:X8}, Step count: {eggDetails.StepCount}");
            //    //await Task.Delay(100);
            //}

            var _thread = new Thread(() => d.TestMainRNG(d.DiamondPearlService, tmpRNG));
            _thread.Start();
        }

        private void Button1_OnClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel d)
                return;

            d.Gen8EggsViewModel.Seed0 = d.Seed0;
            d.Gen8EggsViewModel.Seed1 = d.Seed1;
            d.Advances = 0;

            // Move to seperate methode/action
            d.GetEggDetails(d.DiamondPearlService);
        }
    }
}
