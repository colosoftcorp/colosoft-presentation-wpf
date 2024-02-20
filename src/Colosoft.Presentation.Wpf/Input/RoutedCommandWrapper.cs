using System;
using System.Windows.Input;

namespace Colosoft.Presentation.Input
{
    internal class RoutedCommandWrapper : System.Windows.Input.RoutedUICommand, IRoutedCommand
    {
        private readonly ICommand command;
        private readonly InputGestureCollection innerInputGestures;

        public CommandDescription Description { get; }

        public object Target { get; set; }

        public bool IsExecuting { get; private set; }

        public virtual bool HasRequirements => true;

        public virtual bool HasImage => true;

        public virtual Uri ImageUri => null;

        InputGestureCollection IRoutedCommand.InputGestures => innerInputGestures;

        public RoutedCommandWrapper(
            ICommand command,
            string name,
            Type ownerType,
            CommandDescription description,
            System.Windows.Input.InputGestureCollection gestures)
            : base(
                  (description?.Text ?? Guid.NewGuid().ToString().GetFormatter())?.Format(System.Globalization.CultureInfo.CurrentUICulture),
                  name ?? Guid.NewGuid().ToString(),
                  ownerType,
                  gestures)
        {
            this.command = command;

            this.innerInputGestures = new InputGestureCollection();
            if (gestures != null)
            {
                foreach (System.Windows.Input.InputGesture i in gestures)
                {
                    this.innerInputGestures.Add(new InputGestureWrapper(i));
                }
            }
        }

        public CommandContext CreateCanExecuteContext(object bindingSource, object commandSource, object commandParameter)
        {
            return this.OnCreateCanExecuteContext(bindingSource, commandSource, commandParameter);
        }

        public bool CanExecute(CommandContext commandContext)
        {
            return this.OnCanExecute(commandContext);
        }

        public CommandContext CreateExecuteContext(object bindingSource, object commandSource, object commandParameter)
        {
            return this.OnCreateExecuteContext(bindingSource, commandSource, commandParameter);
        }

        public void Execute(CommandContext commandContext)
        {
            if (this.IsExecuting)
            {
                throw new InvalidOperationException();
            }

            this.IsExecuting = true;
            try
            {
                this.OnExecute(commandContext);
            }
            catch
            {
                // ignore
            }
            finally
            {
                this.IsExecuting = false;
            }
        }

        protected virtual CommandContext OnCreateCanExecuteContext(object bindingSource, object commandSource, object commandParameter)
        {
            return this.OnCreateContext(bindingSource, commandSource, commandParameter);
        }

        protected virtual bool OnCanExecute(CommandContext commandContext) =>
            this.command.CanExecute(commandContext.CommandParameter);

        protected virtual CommandContext OnCreateExecuteContext(object bindingSource, object commandSource, object commandParameter)
        {
            return this.OnCreateContext(bindingSource, commandSource, commandParameter);
        }

        protected virtual CommandContext OnCreateContext(object bindingSource, object commandSource, object commandParameter)
        {
            return new CommandContext(bindingSource, commandSource, commandParameter);
        }

        protected virtual void OnExecute(CommandContext commandContext) => this.command.Execute(commandContext.CommandParameter);
    }
}
