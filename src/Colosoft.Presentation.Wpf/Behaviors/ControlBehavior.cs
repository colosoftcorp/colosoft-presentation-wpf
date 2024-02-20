using System;
using System.Linq;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class ControlBehavior
    {
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool?), typeof(ControlBehavior), new PropertyMetadata(OnIsReadOnlyChanged));

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isLocked = (bool?)e.NewValue;

            if (isLocked == null)
            {
                return;
            }

            foreach (var item in UIHelpers.NavigateChildren(d))
            {
                var properties = item.GetType().GetProperties();
                var name = properties.FirstOrDefault(f => /*f.Name == "IsReadOnly" ||*/ f.Name == "IsEnabled");
                if (name != null && name.Name == "IsEnabled")
                {
                    name.SetValue(item, !isLocked, null);
                }
            }
        }

        public static void SetIsReadOnly(DependencyObject owner, bool? value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(IsReadOnlyProperty, value);
        }

        public static bool? GetIsReadOnly(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(IsReadOnlyProperty);
            if (value == null)
            {
                return false;
            }

            return (bool?)value;
        }
    }
}
