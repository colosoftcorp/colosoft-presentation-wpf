using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public static class ExpanderFocusBehavior
    {
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(ExpanderFocusBehavior),
            new PropertyMetadata(false, AttachChanged));

        public static readonly DependencyProperty IsFirstProperty = DependencyProperty.RegisterAttached(
            "IsFirst",
            typeof(bool),
            typeof(ExpanderFocusBehavior),
            new PropertyMetadata(false, IsFirstChanged));

        public static bool GetAttach(DependencyObject owner)
        {
            return owner != null && ((bool)owner.GetValue(AttachProperty));
        }

        public static void SetAttach(DependencyObject owner, bool value)
        {
            if (owner != null)
            {
                owner.SetValue(AttachProperty, value);
            }
        }

        public static bool GetIsFirst(DependencyObject owner)
        {
            return owner != null && ((bool)owner.GetValue(IsFirstProperty));
        }

        public static void SetIsFirst(DependencyObject owner, bool value)
        {
            if (owner != null)
            {
                owner.SetValue(IsFirstProperty, value);
            }
        }

        private static void AttachChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
        {
            var expander = item as Expander;
            if (expander == null)
            {
                return;
            }

            expander.GotFocus += ExpanderGotFocus;
        }

        private static void IsFirstChanged(DependencyObject item, DependencyPropertyChangedEventArgs args)
        {
            var control = item as FrameworkElement;
            if (control == null)
            {
                return;
            }

            control.IsVisibleChanged += ControlIsVisibleChanged;
        }

        private static void ExpanderGotFocus(object sender, System.Windows.RoutedEventArgs args)
        {
            var expander = (Expander)sender;

            if (!expander.IsExpanded)
            {
                expander.IsExpanded = true;
            }
        }

        private static void ControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var item = (DependencyObject)sender;
            if ((Visibility)item.GetValue(FrameworkElement.VisibilityProperty) == Visibility.Visible)
            {
                Keyboard.Focus((IInputElement)item);
            }
        }
    }
}
