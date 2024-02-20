using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Colosoft.Presentation.Behaviors
{
    public static class DelayedUpdateBehavior
    {
        public const string TargetPropertyPropertyName = "TargetProperty";
        public const string MillisecondsPropertyName = "Milliseconds";

        public static readonly DependencyProperty TargetPropertyProperty = DependencyProperty.RegisterAttached(
          TargetPropertyPropertyName,
          typeof(DependencyProperty),
          typeof(DelayedUpdateBehavior),
          new FrameworkPropertyMetadata(null, OnTargetPropertyChanged));

        public static void SetTargetProperty(DependencyObject element, DependencyProperty value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(TargetPropertyProperty, value);
        }

        public static DependencyProperty GetTargetProperty(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (DependencyProperty)element.GetValue(TargetPropertyProperty);
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var prop = e.NewValue as DependencyProperty;

            if (prop == null)
            {
                return;
            }

#pragma warning disable CA1806 // Do not ignore method results
            d.Dispatcher.BeginInvoke(
                (Action<DependencyObject, DependencyProperty>)((target, p) => new PropertyChangeTimer(target, p)),
                DispatcherPriority.ApplicationIdle,
                d,
                prop);
#pragma warning restore CA1806 // Do not ignore method results

        }

        public static readonly DependencyProperty MillisecondsProperty = DependencyProperty.RegisterAttached(
          MillisecondsPropertyName,
          typeof(int),
          typeof(DelayedUpdateBehavior),
          new FrameworkPropertyMetadata(1000));

        public static void SetMilliseconds(DependencyObject element, int value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(MillisecondsProperty, value);
        }

        public static int GetMilliseconds(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (int)element.GetValue(MillisecondsProperty);
        }

        private sealed class PropertyChangeTimer
        {
            private readonly BindingExpression expression;
            private DispatcherTimer timer;

            public PropertyChangeTimer(DependencyObject target, DependencyProperty property)
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(target));
                }

                if (property == null)
                {
                    throw new ArgumentNullException(nameof(property));
                }

                if (!BindingOperations.IsDataBound(target, property))
                {
                    return;
                }

                this.expression = BindingOperations.GetBindingExpression(target, property);
                if (this.expression == null)
                {
                    throw new InvalidOperationException($"No binding was found on property {property.Name} on object {target.GetType().FullName}");
                }

                DependencyPropertyDescriptor.FromProperty(property, target.GetType()).AddValueChanged(target, this.OnPropertyChanged);
            }

            private void OnPropertyChanged(object sender, EventArgs e)
            {
                if (this.timer == null)
                {
                    this.timer = new DispatcherTimer();
                    int ms = DelayedUpdateBehavior.GetMilliseconds(sender as DependencyObject);
                    this.timer.Interval = TimeSpan.FromMilliseconds(ms);
                    this.timer.Tick += this.OnTimerTick;
                    this.timer.Start();
                    return;
                }

                this.timer.Stop();
                this.timer.Start();
            }

            private void OnTimerTick(object sender, EventArgs e)
            {
                this.expression.UpdateSource();
                this.expression.UpdateTarget();
                this.timer.Stop();
                this.timer = null;
            }
        }
    }
}
