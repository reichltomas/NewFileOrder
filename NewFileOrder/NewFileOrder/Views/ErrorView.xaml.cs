using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NewFileOrder.Views
{
    public class ErrorView : UserControl
    {
        public ErrorView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
