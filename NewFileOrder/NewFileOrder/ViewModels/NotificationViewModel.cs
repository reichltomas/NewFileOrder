using Avalonia.Controls.Notifications;
using ReactiveUI;
using System;
using System.Reactive;

namespace NewFileOrder.ViewModels
{
    public class NotificationViewModel:INotification
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

        public NotificationType Type =>NotificationType.Information;

        public TimeSpan Expiration => TimeSpan.Zero;

        public Action OnClick => null;

        public Action OnClose => null;
    }
}