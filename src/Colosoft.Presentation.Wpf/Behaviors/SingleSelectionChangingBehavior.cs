using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class SingleSelectionChangingBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(System.Windows.Input.ICommand), typeof(SingleSelectionChangingBehavior), new PropertyMetadata(CommandPropertyChanged));

        private static readonly DependencyProperty SelectorBehaviorProperty =
            DependencyProperty.RegisterAttached("SelectorBehavior", typeof(SelectorBehaviorWrapper), typeof(SingleSelectionChangingBehavior));

        private static readonly DependencyProperty TreeViewBehaviorProperty =
            DependencyProperty.RegisterAttached("TreeViewBehavior", typeof(TreeViewBehaviorWrapper), typeof(SingleSelectionChangingBehavior));

        public static string GetCommand(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return owner.GetValue(CommandProperty) as string;
        }

        public static void SetCommand(DependencyObject owner, string value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(CommandProperty, value);
        }

        private static void CommandPropertyChanged(DependencyObject hostControl, DependencyPropertyChangedEventArgs e)
        {
            var selector = hostControl as System.Windows.Controls.Primitives.Selector;

            if (selector != null)
            {
                var behavior = (SelectorBehaviorWrapper)hostControl.GetValue(SelectorBehaviorProperty);

                if (behavior != null)
                {
                    behavior.Dispose();
                }

                if (e.NewValue != null)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    behavior = new SelectorBehaviorWrapper(selector, (System.Windows.Input.ICommand)e.NewValue);
#pragma warning restore CA2000 // Dispose objects before losing scope
                    hostControl.SetValue(SelectorBehaviorProperty, behavior);
                }
            }
            else
            {
                var treeView = hostControl as System.Windows.Controls.TreeView;

                if (treeView != null)
                {
                    var behavior = (TreeViewBehaviorWrapper)hostControl.GetValue(TreeViewBehaviorProperty);

                    if (behavior != null)
                    {
                        behavior.Dispose();
                    }

                    if (e.NewValue != null)
                    {
#pragma warning disable CA2000 // Dispose objects before losing scope
                        behavior = new TreeViewBehaviorWrapper(treeView, (System.Windows.Input.ICommand)e.NewValue);
#pragma warning restore CA2000 // Dispose objects before losing scope
                        hostControl.SetValue(TreeViewBehaviorProperty, behavior);
                    }
                }
            }
        }

        private sealed class SelectorBehaviorWrapper : IDisposable
        {
            private readonly System.Windows.Controls.Primitives.Selector selector;
            private readonly System.Windows.Input.ICommand command;

            public SelectorBehaviorWrapper(System.Windows.Controls.Primitives.Selector selector, System.Windows.Input.ICommand command)
            {
                this.selector = selector;
                this.command = command;
                this.selector.SelectionChanged += this.Selector_SelectionChanged;
            }

            private void Selector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
                this.selector.SelectionChanged -= this.Selector_SelectionChanged;

                try
                {
                    var oldSelection = e.RemovedItems.Count == 0 ? null : e.RemovedItems[0];
                    var newSelection = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;

                    var args = new Controls.SingleSelectionChangedEventArgs(oldSelection, newSelection, this.selector.SelectedIndex);

                    if (this.command.CanExecute(args))
                    {
                        this.command.Execute(args);

                        if (!args.Cancel)
                        {
                            return;
                        }
                    }

                    this.selector.SelectedItem = oldSelection;
                }
                finally
                {
                    this.selector.SelectionChanged += this.Selector_SelectionChanged;
                }
            }

            public void Dispose()
            {
                this.selector.SelectionChanged -= this.Selector_SelectionChanged;
            }
        }

        private sealed class TreeViewBehaviorWrapper : IDisposable
        {
            private readonly System.Windows.Controls.TreeView treeView;
            private readonly System.Windows.Input.ICommand command;

            public TreeViewBehaviorWrapper(System.Windows.Controls.TreeView treeView, System.Windows.Input.ICommand command)
            {
                this.treeView = treeView;
                this.command = command;

                this.treeView.SelectedItemChanged += this.TreeView_SelectedItemChanged;
            }

            private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            {
                this.treeView.SelectedItemChanged -= this.TreeView_SelectedItemChanged;

                var operation = this.treeView.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        var oldSelection = e.OldValue;
                        var newSelection = e.NewValue;

                        var args = new Controls.SingleSelectionChangedEventArgs(oldSelection, newSelection, 0);

                        if (this.command.CanExecute(args))
                        {
                            this.command.Execute(args);

                            // Verifica se não é para cancelar
                            if (!args.Cancel)
                            {
                                return;
                            }
                        }

                        this.treeView.SelectItem(oldSelection);
                    }),
                    System.Windows.Threading.DispatcherPriority.ContextIdle);

                operation.Completed += new EventHandler((s, ea) =>
                {
                    this.treeView.SelectedItemChanged += this.TreeView_SelectedItemChanged;
                });
            }

            public void Dispose()
            {
                this.treeView.SelectedItemChanged -= this.TreeView_SelectedItemChanged;
            }
        }
    }
}
