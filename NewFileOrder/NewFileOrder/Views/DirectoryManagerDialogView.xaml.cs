using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NewFileOrder.Views
{
    public class DirectoryManagerDialogView : Window
    {
        public DirectoryManagerDialogView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
