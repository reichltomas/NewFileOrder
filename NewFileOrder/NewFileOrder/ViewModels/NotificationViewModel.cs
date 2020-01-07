using Avalonia.Controls.Notifications;
using ReactiveUI;
using System.Reactive;

namespace NewFileOrder.ViewModels
{
    public class NotificationViewModel
    {

        public NotificationViewModel(IManagedNotificationManager manager)
        {
            ToTagCommand = ReactiveCommand.Create(() =>
            {
                manager.Show(new Avalonia.Controls.Notifications.Notification("Tag hele", "aslkdflajsdlf;ja"));
                //Todo implement what now
            });
        }

        public string Title { get; set; }
        public string Message { get; set; }
        public ReactiveCommand<Unit, Unit> ToTagCommand { get; }
    }
}