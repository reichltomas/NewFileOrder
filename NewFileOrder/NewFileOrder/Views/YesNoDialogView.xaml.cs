using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NewFileOrder.Views
{
    public class YesNoDialogView : Window
    {
        public YesNoDialogView()
        {
        }

        public YesNoDialogView(string message)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.FindControl<Button>("YesButton").Click += delegate { Close(true); };

            this.FindControl<Button>("NoButton").Click += delegate { Close(false); };

            this.FindControl<TextBlock>("Message").Text = message;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
