using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PokeNX.DesktopApp.Views
{
    public partial class Gen8Stationary : UserControl
    {
        public Gen8Stationary()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
