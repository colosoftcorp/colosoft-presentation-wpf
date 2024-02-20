using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class ComboBoxBehavior
    {
        public static readonly DependencyProperty NullableProperty =
            DependencyProperty.RegisterAttached("Nullable", typeof(bool), typeof(ComboBoxBehavior), new PropertyMetadata(NullableChanged));

        public static void SetNullable(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(NullableProperty, value);
        }

        public static bool GetNullable(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(NullableProperty);
            if (value == null)
            {
                return false;
            }

            return (bool)value;
        }

        private static void NullableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.ComboBox combo)
            {
                if ((bool)e.NewValue)
                {
                    combo.KeyDown += new System.Windows.Input.KeyEventHandler(ComboBoxKeyDown);
                }
                else if ((bool)e.OldValue)
                {
                    combo.KeyDown -= new System.Windows.Input.KeyEventHandler(ComboBoxKeyDown);
                }
            }
        }

        private static void ComboBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete &&
                sender is System.Windows.Controls.ComboBox)
            {
                var combo = (System.Windows.Controls.ComboBox)sender;
                combo.SelectedItem = null;
            }
        }
    }
}
