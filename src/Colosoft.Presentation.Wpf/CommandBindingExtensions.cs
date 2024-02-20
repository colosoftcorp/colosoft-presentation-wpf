namespace Colosoft
{
    public static class CommandBindingExtensions
    {
        private static void CommandBindingCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            var command = e.Command as Presentation.Input.Command;
            if (command != null)
            {
                CommandBindingCanExecute(sender, e, command);
            }
            else if (e.Command is Presentation.Input.IRoutedCommand routedCommand)
            {
                using (var commandContext = new Presentation.Input.CommandContext(sender, e.OriginalSource, e.Parameter))
                {
                    e.CanExecute = routedCommand.CanExecute(commandContext);
                    e.Handled = true;
                }
            }
            else
            {
                e.CanExecute = e.Command.CanExecute(e.Parameter);
                e.Handled = true;
            }
        }

        private static void CommandBindingCanExecute(
            object sender,
            System.Windows.Input.CanExecuteRoutedEventArgs e,
            Presentation.Input.Command command)
        {
            if (!command.HasRequirements)
            {
                e.CanExecute = true;
                e.Handled = true;
                return;
            }

            using (var commandContext =
                command.CreateCanExecuteContext(sender, e.OriginalSource, e.Parameter))
            {
                if (commandContext == null)
                {
                    return;
                }

                e.CanExecute = command.CanExecute(commandContext);
                e.Handled = true;
            }
        }

        private static void CommandBindingExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command is Presentation.Input.Command command)
            {
                using (var commandContext = command.CreateCanExecuteContext(sender, e.OriginalSource, e.Parameter))
                {
                    command.Execute(commandContext);
                    e.Handled = true;
                }
            }
            else if (e.Command is Presentation.Input.IRoutedCommand routedCommand)
            {
                using (var commandContext = new Presentation.Input.CommandContext(sender, e.OriginalSource, e.Parameter))
                {
                    routedCommand.Execute(commandContext);
                    e.Handled = true;
                }
            }
            else
            {
                e.Command.Execute(e.Parameter);
                e.Handled = true;
            }
        }

        public static System.Windows.Input.CommandBinding CreateCommandBinding(this System.Windows.Input.ICommand command)
        {
            return new System.Windows.Input.CommandBinding(command, CommandBindingExecuted, CommandBindingCanExecute);
        }
    }
}
