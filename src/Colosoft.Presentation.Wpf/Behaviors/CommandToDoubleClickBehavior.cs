using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public static class CommandToDoubleClickBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(CommandToDoubleClickBehavior),
            new PropertyMetadata(null, CommandChanged));

        public static ICommand GetCommand(DependencyObject owner)
        {
            return owner != null ? (ICommand)owner.GetValue(CommandProperty) : null;
        }

        public static void SetCommand(DependencyObject owner, ICommand value)
        {
            if (owner != null)
            {
                owner.SetValue(CommandProperty, value);
            }
        }

        private static void CommandChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = owner as Control;
            if (ctrl == null)
            {
                return;
            }

            var command = e.NewValue as ICommand;
            if (command == null)
            {
                if (e.OldValue != null)
                {
                    ctrl.MouseDoubleClick -= DoubleClicked;
                }
            }
            else
            {
                ctrl.MouseDoubleClick += DoubleClicked;
            }
        }

        private static void DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is IContentHost))
            {
                var element = e.OriginalSource as FrameworkElement;
                var isChild = (element.FindVisualAncestorByType<ListViewItem>() != null) ||
                    (element.FindVisualAncestorByType<DataGridRow>() != null);

                if (!isChild)
                {
                    return;
                }
            }

            var item = sender as DependencyObject;
            if (item == null)
            {
                return;
            }

            var cmd = GetCommand(item);
            if (cmd == null)
            {
                return;
            }

            if (cmd.CanExecute(item))
            {
                cmd.Execute(item);
            }
        }
    }
}