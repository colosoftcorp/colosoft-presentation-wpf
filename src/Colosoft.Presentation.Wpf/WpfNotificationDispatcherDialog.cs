using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfNotificationDispatcherDialog : Notifications.INotificationDispatcher
    {
        private static System.Windows.MessageBoxResult Convert(Notifications.MessageResult result)
        {
            switch (result)
            {
                case Notifications.MessageResult.OK:
                    return System.Windows.MessageBoxResult.OK;
                case Notifications.MessageResult.Cancel:
                    return System.Windows.MessageBoxResult.Cancel;
                case Notifications.MessageResult.No:
                    return System.Windows.MessageBoxResult.No;
                case Notifications.MessageResult.Yes:
                    return System.Windows.MessageBoxResult.Yes;
                default:
                    return System.Windows.MessageBoxResult.None;
            }
        }

        private static System.Windows.MessageBoxButton Convert(Notifications.MessageResultOption option)
        {
            switch (option)
            {
                case Notifications.MessageResultOption.OK:
                    return System.Windows.MessageBoxButton.OK;
                case Notifications.MessageResultOption.OKCancel:
                    return System.Windows.MessageBoxButton.OKCancel;
                case Notifications.MessageResultOption.YesNo:
                    return System.Windows.MessageBoxButton.YesNo;
                case Notifications.MessageResultOption.YesNoCancel:
                    return System.Windows.MessageBoxButton.YesNoCancel;

                default:
                    return System.Windows.MessageBoxButton.OK;
            }
        }

        private static System.Windows.MessageBoxImage Convert(Notifications.NotificationType type)
        {
            switch (type)
            {
                case Notifications.NotificationType.Error:
                    return System.Windows.MessageBoxImage.Error;
                case Notifications.NotificationType.Exclamation:
                    return System.Windows.MessageBoxImage.Exclamation;
                case Notifications.NotificationType.Information:
                    return System.Windows.MessageBoxImage.Information;
                case Notifications.NotificationType.Question:
                    return System.Windows.MessageBoxImage.Question;
                case Notifications.NotificationType.Warning:
                    return System.Windows.MessageBoxImage.Warning;
                default:

                    return System.Windows.MessageBoxImage.None;
            }
        }

        private static Notifications.MessageResult Convert(System.Windows.MessageBoxResult result)
        {
            switch (result)
            {
                case System.Windows.MessageBoxResult.OK:
                    return Notifications.MessageResult.OK;
                case System.Windows.MessageBoxResult.Cancel:
                    return Notifications.MessageResult.Cancel;
                case System.Windows.MessageBoxResult.No:
                    return Notifications.MessageResult.No;
                case System.Windows.MessageBoxResult.Yes:
                    return Notifications.MessageResult.Yes;
                default:
                    return Notifications.MessageResult.None;
            }
        }

        private static Notifications.MessageResult Dispatch2(Notifications.NotificationInfo notification2)
        {
            var owner = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(x => x.IsFocused) ??
                System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(f => f.IsActive);

            if (owner != null)
            {
                return Convert(System.Windows.MessageBox.Show(
                    owner,
                    notification2.Message.Format(System.Globalization.CultureInfo.CurrentCulture),
                    notification2.Caption != null ? notification2.Caption.Format(System.Globalization.CultureInfo.CurrentCulture) : null,
                    Convert(notification2.Option),
                    Convert(notification2.Type),
                    Convert(notification2.DefaultMessageResult)));
            }
            else
            {
                return Convert(System.Windows.MessageBox.Show(
                    notification2.Message.Format(System.Globalization.CultureInfo.CurrentCulture),
                    notification2.Caption != null ? notification2.Caption.Format(System.Globalization.CultureInfo.CurrentCulture) : null,
                    Convert(notification2.Option),
                    Convert(notification2.Type),
                    Convert(notification2.DefaultMessageResult)));
            }
        }

        public Notifications.MessageResult Dispatch(Notifications.NotificationInfo notification)
        {
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            try
            {
                if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                {
                    return Dispatch2(notification);
                }
                else
                {
                    return (Notifications.MessageResult)System.Windows.Application.Current.Dispatcher.Invoke(
                        new Func<Notifications.NotificationInfo, Notifications.MessageResult>(Dispatch2),
                        System.Windows.Threading.DispatcherPriority.Normal,
                        notification);
                }
            }
            catch (InvalidOperationException)
            {
                return notification.DefaultMessageResult;
            }
        }

        public Notifications.MessageResult Dispatch(Colosoft.Threading.IDispatcher dispatcher, Notifications.NotificationInfo notification)
        {
            if (dispatcher is null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (dispatcher.CheckAccess())
            {
                return this.Dispatch(notification);
            }
            else
            {
                return (Notifications.MessageResult)dispatcher.Invoke(
                    new Func<Notifications.NotificationInfo, Notifications.MessageResult>(this.Dispatch),
                    notification);
            }
        }

#pragma warning disable S4457 // Parameter validation in "async"/"await" methods should be wrapped
        public async Task<Notifications.MessageResult> Dispatch(Notifications.NotificationInfo notification, CancellationToken cancellationToken)
#pragma warning restore S4457 // Parameter validation in "async"/"await" methods should be wrapped
        {
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            try
            {
                var result = Notifications.MessageResult.None;

                await System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    new Action<Notifications.NotificationInfo>(notification2 =>
                    {
                        result = Dispatch2(notification2);
                    }),
                    System.Windows.Threading.DispatcherPriority.Normal,
                    notification).Task;

                return result;
            }
            catch (InvalidOperationException)
            {
                return notification.DefaultMessageResult;
            }
        }
    }
}
