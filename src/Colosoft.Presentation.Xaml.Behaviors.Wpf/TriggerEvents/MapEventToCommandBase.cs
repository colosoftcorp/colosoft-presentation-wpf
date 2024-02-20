using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;

namespace Colosoft.Presentation.TriggerEvents
{
    public abstract class MapEventToCommandBase<TEventArgsType> : TriggerAction<FrameworkElement>
        where TEventArgsType : EventArgs
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(MapEventToCommandBase<TEventArgsType>), new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }

            System.Windows.Input.ICommand cmd = this.Command;
            if (cmd.CanExecute(parameter))
            {
                cmd.Execute(parameter);
            }
        }

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }
    }
}
