using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NewFileOrder.Models.Managers;
using NewFileOrder.ViewModels;

namespace NewFileOrder.Views
{
    public class ManagerDialogWindow : Window
    {
        public ManagerDialogWindow() { }

        public ManagerDialogWindow(MainWindow mw)
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.DataContext = new ManagerDialogViewModel(this, mw);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Window AsTagManagerDialog(FileManager fm, TagManager tm)
        {
            (DataContext as ManagerDialogViewModel).CreateTagManagerDialog(fm, tm);
            return this;
        }

        public Window AsDirectoryManagerDialog(FileManager fm)
        {
            (DataContext as ManagerDialogViewModel).CreateDirectoryManagerDialog(fm);
            return this;
        }
    }
}
