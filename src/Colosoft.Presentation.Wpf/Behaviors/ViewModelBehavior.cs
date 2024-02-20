using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class ViewModelBehavior
    {
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(ViewModelBehavior),
                new PropertyMetadata(AttachChanged));

        public static void SetAttach(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(AttachProperty);
            if (value == null)
            {
                return false;
            }

            return (bool)value;
        }

        private static void AttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var enumerator = LogicalTreeHelper.GetChildren(d).GetEnumerator();

            if (enumerator.MoveNext())
            {
                var child = enumerator.Current;

                var element = d as System.Windows.Controls.ContentControl;

                if (element != null)
                {
                    element.Content = null;

                    var adorner = new Controls.AdornedControl()
                    {
                        VerticalAdornerPlacement = Controls.AdornerPlacement.Inside,
                        HorizontalAdornerPlacement = Controls.AdornerPlacement.Inside,
                        Content = child as FrameworkElement,
                    };

                    adorner.SetBinding(
                        Controls.AdornedControl.IsAdornerVisibleProperty,
                        new System.Windows.Data.Binding("Content.DataContext.IsBusy")
                        {
                            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.Self),
                        });

                    var parent = d as System.Windows.Markup.IAddChild;

                    if (parent != null)
                    {
                        parent.AddChild(adorner);
                    }
                }
            }
        }
    }
}
