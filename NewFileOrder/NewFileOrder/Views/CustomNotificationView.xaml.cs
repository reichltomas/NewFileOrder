using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NewFileOrder.Views
{
    public class CustomNotificationView : UserControl
    {
        public CustomNotificationView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
