using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Colosoft.Presentation.Behaviors
{
    public static class TreeViewSelectedItemBehavior
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(object),
            typeof(TreeViewSelectedItemBehavior),
            new PropertyMetadata(new object(), OnSelectedItemChanged));

        public static object GetSelectedItem(TreeView treeView)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            return treeView.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(TreeView treeView, object value)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            treeView.SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var treeView = d as TreeView;
            if (treeView == null)
            {
                return;
            }

            treeView.SelectedItemChanged -= TreeViewItemChanged;

            var treeViewItem = SelectTreeViewItemForBinding(args.NewValue, treeView);

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
            }

            treeView.SelectedItemChanged += TreeViewItemChanged;
        }

        private static void TreeViewItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((TreeView)sender).SetValue(SelectedItemProperty, e.NewValue);
        }

        private static TreeViewItem SelectTreeViewItemForBinding(object dataItem, ItemsControl itemControl)
        {
            if (itemControl == null || dataItem == null)
            {
                return null;
            }

            IItemContainerGenerator generator = itemControl.ItemContainerGenerator;

            using (generator.StartAt(generator.GeneratorPositionFromIndex(-1), GeneratorDirection.Forward))
            {
                foreach (var item in itemControl.Items)
                {
                    bool isNewlyRealized;
                    var treeViewItem = generator.GenerateNext(out isNewlyRealized);

                    if (isNewlyRealized)
                    {
                        generator.PrepareItemContainer(treeViewItem);
                    }

                    if (item == dataItem)
                    {
                        return treeViewItem as TreeViewItem;
                    }

                    var tmp = SelectTreeViewItemForBinding(dataItem, treeViewItem as ItemsControl);
                    if (tmp != null)
                    {
                        return tmp;
                    }
                }
            }

            return null;
        }
    }
}
