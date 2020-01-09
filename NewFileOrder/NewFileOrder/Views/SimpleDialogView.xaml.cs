using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NewFileOrder.Views
{
    public class SimpleDialogView : Window
    {
        public SimpleDialogView() { }
        public SimpleDialogView(string message)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.FindControl<Button>("OKButton").Click += delegate { Close(); };

            this.FindControl<TextBlock>("Message").Text = message;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
