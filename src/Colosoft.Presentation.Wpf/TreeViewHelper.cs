using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Colosoft.Presentation
{
    public static class TreeViewHelper
    {
        public static void ExpandAll(this TreeView treeView)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            ExpandSubContainers(treeView);
        }

        private static void ExpandSubContainers(ItemsControl parentContainer)
        {
            foreach (var item in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    currentContainer.IsExpanded = true;

                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        EventHandler eh = null;
                        eh = new EventHandler((sender, e) =>
                        {
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                ExpandSubContainers(currentContainer);
                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });

                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else
                    {
                        ExpandSubContainers(currentContainer);
                    }
                }
            }
        }

        public static void SelectItem(this TreeView treeView, object item)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            ExpandAndSelectItem(treeView, item);
        }

        public static void SelectValue(this TreeView treeView, object value)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            ExpandAndSelectItem(treeView, value, true);
        }

        private static void ExpandAndSelectItem(TreeView treeView, object obj, bool isValue)
        {
            if (treeView is null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }

            ExpandAndSelectItem(treeView, obj, isValue, null, null);
        }

        internal static void ExpandAndSelectItem(TreeView treeView, object obj, bool isValue, Action<TreeViewItem> beforeSelection, Action<TreeViewItem> afterSelection)
        {
            if (treeView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                EventHandler eh = null;
                eh = new EventHandler((sender, e) =>
                {
                    if (treeView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                    {
                        ExpandAndSelectItem(treeView, obj, isValue, treeView.SelectedValuePath, beforeSelection, afterSelection);
                        treeView.ItemContainerGenerator.StatusChanged -= eh;
                    }
                });

                treeView.ItemContainerGenerator.StatusChanged += eh;
            }
            else
            {
                ExpandAndSelectItem(treeView, obj, isValue, treeView.SelectedValuePath, beforeSelection, afterSelection);
            }
        }

        private static bool ExpandAndSelectItem(ItemsControl parentContainer, object itemToSelect, bool isValue, string valuePath, Action<TreeViewItem> beforeSelection, Action<TreeViewItem> afterSelection)
        {
            foreach (object item in parentContainer.Items)
            {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                object equalItem = item;
                if (isValue)
                {
                    equalItem = BindingHelper.Eval<object>(item, valuePath);
                }

                if (currentContainer != null)
                {
                    if (equalItem.Equals(itemToSelect))
                    {
                        if (beforeSelection != null)
                        {
                            beforeSelection(currentContainer);
                        }

                        if (!currentContainer.IsSelected)
                        {
                            currentContainer.IsSelected = true;
                        }

                        currentContainer.BringIntoView();
                        currentContainer.Focus();
                        if (afterSelection != null)
                        {
                            afterSelection(currentContainer);
                        }

                        return true;
                    }
                    else if (currentContainer.IsSelected)
                    {
                        currentContainer.IsSelected = false;
                    }
                }
            }

            foreach (object item in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    bool wasExpanded = currentContainer.IsExpanded;

                    currentContainer.IsExpanded = true;

                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        EventHandler eh = null;
                        eh = new EventHandler((sender, e) =>
                        {
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                if (!ExpandAndSelectItem(currentContainer, itemToSelect, isValue, valuePath, beforeSelection, afterSelection))
                                {
                                    currentContainer.IsExpanded = false;
                                }

                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else
                    {
                        if (!ExpandAndSelectItem(currentContainer, itemToSelect, isValue, valuePath, beforeSelection, afterSelection))
                        {
                            currentContainer.IsExpanded = wasExpanded;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool ExpandAndSelectItem(ItemsControl parentContainer, object itemToSelect)
        {
            foreach (var item in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (item == itemToSelect && currentContainer != null)
                {
                    currentContainer.IsSelected = true;
                    currentContainer.BringIntoView();
                    currentContainer.Focus();
                    return true;
                }
            }

            foreach (var item in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    bool wasExpanded = currentContainer.IsExpanded;

                    currentContainer.IsExpanded = true;

                    if (currentContainer.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                    {
                        EventHandler eh = null;
                        eh = new EventHandler((sender, e) =>
                        {
                            if (currentContainer.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                            {
                                if (!ExpandAndSelectItem(currentContainer, itemToSelect))
                                {
                                    currentContainer.IsExpanded = false;
                                }

                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else
                    {
                        if (!ExpandAndSelectItem(currentContainer, itemToSelect))
                        {
                            currentContainer.IsExpanded = wasExpanded;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static void ApplyActionToAllTreeViewItems(this ItemsControl itemsControl, Action<TreeViewItem> itemAction)
        {
            if (itemAction is null)
            {
                throw new ArgumentNullException(nameof(itemAction));
            }

            Stack<ItemsControl> itemsControlStack = new Stack<ItemsControl>();
            itemsControlStack.Push(itemsControl);

            while (itemsControlStack.Count != 0)
            {
                var currentItem = itemsControlStack.Pop();
                TreeViewItem currentTreeViewItem = currentItem as TreeViewItem;
                if (currentTreeViewItem != null)
                {
                    itemAction(currentTreeViewItem);
                }

                if (currentItem != null)
                {
                    foreach (object dataItem in currentItem.Items)
                    {
                        ItemsControl childElement = (ItemsControl)currentItem.ItemContainerGenerator.ContainerFromItem(dataItem);

                        itemsControlStack.Push(childElement);
                    }
                }
            }
        }
    }
}
