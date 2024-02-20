using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class SearchTextBoxBehavior
    {
        public static readonly DependencyProperty LabelTextProperty =
           DependencyProperty.RegisterAttached(
               "LabelText",
               typeof(string),
               typeof(SearchTextBoxBehavior));

        public static readonly DependencyProperty LabelTextColorProperty =
            DependencyProperty.RegisterAttached(
                "LabelTextColor",
                typeof(System.Windows.Media.Brush),
                typeof(SearchTextBoxBehavior));

        public static void SetLabelText(DependencyObject owner, string value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(LabelTextProperty, value);
        }

        public static string GetLabelText(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return owner.GetValue(LabelTextProperty) as string;
        }

        public static void SetLabelTextColor(DependencyObject owner, System.Windows.Media.Brush value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(LabelTextColorProperty, value);
        }

        public static System.Windows.Media.Brush GetLabelTextColor(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (System.Windows.Media.Brush)owner.GetValue(LabelTextColorProperty);
        }
    }
}
