using System;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation.Behaviors
{
    public static class TreeViewItemBehavior
    {
        public static bool GetIsSelectable(TreeViewItem obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (bool)obj.GetValue(IsSelectableProperty);
        }

        public static void SetIsSelectable(TreeViewItem obj, bool value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(IsSelectableProperty, value);
        }

        public static readonly DependencyProperty IsSelectableProperty =
            DependencyProperty.RegisterAttached(
                "IsSelectable",
                typeof(bool),
                typeof(TreeViewItemBehavior),
                new UIPropertyMetadata(true, IsSelectablePropertyChangedCallback));

        private static void IsSelectablePropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            var i = (TreeViewItem)o;
            i.Selected -= OnSelected;
            if (!GetIsSelectable(i))
            {
                i.Selected += OnSelected;
            }
        }

        private static void OnSelected(object sender, System.Windows.RoutedEventArgs args)
        {
            if (sender == args.Source)
            {
                TreeViewItem i = (TreeViewItem)sender;
                i.IsSelected = false;
            }
        }
    }
}
