using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public static class MouseDoubleClickBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(MouseDoubleClickBehavior),
                new UIPropertyMetadata(CommandChanged));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(MouseDoubleClickBehavior),
                new UIPropertyMetadata(null));

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return target.GetValue(CommandParameterProperty);
        }

        private static void OnMouseDoubleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var control = sender as Control;
            var command = (ICommand)control.GetValue(CommandProperty);
            var commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
        }
    }
}
