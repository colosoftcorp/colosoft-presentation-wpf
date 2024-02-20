using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace Colosoft.Presentation.TriggerEvents
{
    public class InvokeCommand : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(object), typeof(InvokeCommand), new PropertyMetadata(null, OnParameterPropertyChanged));

        private bool parameterDefined;

        private static void OnParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as InvokeCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(ParameterProperty, e.NewValue);
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as InvokeCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
                invokeCommand.parameterDefined = true;
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }

            if (this.parameterDefined)
            {
                parameter = this.Parameter;
            }

            if (this.Command.CanExecute(parameter))
            {
                this.Command.Execute(parameter);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public object Parameter
        {
            get { return this.GetValue(ParameterProperty); }
            set
            {
                this.SetValue(ParameterProperty, value);
                this.parameterDefined = true;
            }
        }
    }
}
