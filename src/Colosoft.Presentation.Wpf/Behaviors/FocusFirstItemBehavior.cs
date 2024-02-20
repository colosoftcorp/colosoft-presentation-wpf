using Colosoft.Presentation.Format;
using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class FocusFirstItemBehavior
    {
        public static readonly DependencyProperty FocusFirstItemProperty =
            DependencyProperty.RegisterAttached("FocusFirstItem", typeof(bool), typeof(FocusFirstItemBehavior), new PropertyMetadata(FocusFirstItemChanged));

        public static void SetFocusFirstItemBehavior(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(FocusFirstItemProperty, value);
        }

        public static bool GetFocusFirstItemBehavior(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(FocusFirstItemProperty);
            if (value == null)
            {
                return false;
            }

            return (bool)value;
        }

        private static void FocusFirstItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(e.NewValue?.ToString(), "True") &&
                d is Window window)
            {
                window.Activated += new EventHandler(WindowActivated);
            }
        }

        private static void WindowActivated(object sender, EventArgs e)
        {
            var window = sender as Window;

            if (System.Windows.Input.FocusManager.GetFocusedElement(window) == null)
            {
                var focusableElement = window.FindFirstFocusableChild();
                if (focusableElement != null)
                {
                    focusableElement.Focus();
                }
            }
        }
    }
}
