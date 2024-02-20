using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public static class InputBindingsManager
    {
        public static readonly DependencyProperty UpdatePropertySourceWhenEnterPressedProperty = DependencyProperty.RegisterAttached(
           "UpdatePropertySourceWhenEnterPressed", typeof(DependencyProperty), typeof(InputBindingsManager), new PropertyMetadata(null, OnUpdatePropertySourceWhenEnterPressedPropertyChanged));

        public static void SetUpdatePropertySourceWhenEnterPressed(DependencyObject dp, DependencyProperty value)
        {
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            dp.SetValue(UpdatePropertySourceWhenEnterPressedProperty, value);
        }

        public static DependencyProperty GetUpdatePropertySourceWhenEnterPressed(DependencyObject dp)
        {
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            return (DependencyProperty)dp.GetValue(UpdatePropertySourceWhenEnterPressedProperty);
        }

        private static void OnUpdatePropertySourceWhenEnterPressedPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dp as UIElement;

            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (e.NewValue != null)
            {
                element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        private static void DoUpdateSource(object source)
        {
            DependencyProperty property =
                GetUpdatePropertySourceWhenEnterPressed(source as DependencyObject);

            if (property == null)
            {
                return;
            }

            UIElement elt = source as UIElement;

            if (elt == null)
            {
                return;
            }

            var binding = BindingOperations.GetBindingExpression(elt, property);

            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }
}
