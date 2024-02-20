using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public class WindowDialogActivationBehavior : DialogActivationBehavior
    {
        public static readonly DependencyProperty StartupLocationProperty =
            DependencyProperty.Register("StartupLocation", typeof(System.Windows.WindowStartupLocation), typeof(WindowDialogActivationBehavior));

        public static System.Windows.WindowStartupLocation GetStartupLocation(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(StartupLocationProperty);
            if (value == null)
            {
                return System.Windows.WindowStartupLocation.Manual;
            }

            return (System.Windows.WindowStartupLocation)value;
        }

        public static void SetStartupLocation(DependencyObject owner, object value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(StartupLocationProperty, value);
        }

        protected override IWindow CreateWindow()
        {
            return new WindowWrapper();
        }
    }
}
