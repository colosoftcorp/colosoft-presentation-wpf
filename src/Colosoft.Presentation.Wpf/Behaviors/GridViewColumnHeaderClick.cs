using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public static class GridViewColumnHeaderClick
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(GridViewColumnHeaderClick),
                new UIPropertyMetadata(null, GridViewColumnHeaderClick.CommandChanged));

        public static readonly DependencyProperty CommandBehaviourProperty =
            DependencyProperty.RegisterAttached(
                "CommandBehaviour",
                typeof(GridViewColumnHeaderClickCommandBehavior),
                typeof(GridViewColumnHeaderClick),
                new UIPropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(CommandProperty, value);
        }

        public static GridViewColumnHeaderClickCommandBehavior GetCommandBehaviour(DependencyObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (GridViewColumnHeaderClickCommandBehavior)obj.GetValue(CommandBehaviourProperty);
        }

        public static void SetCommandBehaviour(DependencyObject obj, GridViewColumnHeaderClickCommandBehavior value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(CommandBehaviourProperty, value);
        }

        private static void CommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GridViewColumnHeaderClick.GetOrCreateBehaviour(sender).Command = e.NewValue as ICommand;
        }

        private static GridViewColumnHeaderClickCommandBehavior GetOrCreateBehaviour(DependencyObject element)
        {
            GridViewColumnHeaderClickCommandBehavior returnVal = GridViewColumnHeaderClick.GetCommandBehaviour(element);

            if (returnVal == null)
            {
                var typedElement = element as ListView;

                if (typedElement == null)
                {
                    throw new InvalidOperationException("GridViewColumnHeaderClick.Command property can only be set on instances of ListView");
                }

                returnVal = new GridViewColumnHeaderClickCommandBehavior(typedElement);

                GridViewColumnHeaderClick.SetCommandBehaviour(element, returnVal);
            }

            return returnVal;
        }
    }
}