using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using NewFileOrder.Models;
using NewFileOrder.ViewModels;

namespace NewFileOrder.Views
{
    public class MainWindow : Window
    {
        internal WindowNotificationManager _notificationArea;
        private MyDbContext _db;
        public MainWindow() { }
        public MainWindow(MyDbContext db)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _db = db;
           _notificationArea = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 3
            };
            DataContext = new MainWindowViewModel(_db, _notificationArea);

            (DataContext as MainWindowViewModel).MainWindow = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
       
    }
}
