using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Colosoft.Presentation.Behaviors
{
    public static class MultiSelectorBehavior
    {
        public static readonly DependencyProperty SynchronizedSelectedItems = DependencyProperty.RegisterAttached(
            "SynchronizedSelectedItems", typeof(IList), typeof(MultiSelectorBehavior), new PropertyMetadata(null, OnSynchronizedSelectedItemsChanged));

        private static readonly DependencyProperty SynchronizationManagerProperty = DependencyProperty.RegisterAttached(
            "SynchronizationManager", typeof(SynchronizationManager), typeof(MultiSelectorBehavior), new PropertyMetadata(null));

        public static IList GetSynchronizedSelectedItems(DependencyObject dependencyObject)
        {
            if (dependencyObject is null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            return (IList)dependencyObject.GetValue(SynchronizedSelectedItems);
        }

        public static void SetSynchronizedSelectedItems(DependencyObject dependencyObject, IList value)
        {
            if (dependencyObject is null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            dependencyObject.SetValue(SynchronizedSelectedItems, value);
        }

        private static SynchronizationManager GetSynchronizationManager(DependencyObject dependencyObject)
        {
            return (SynchronizationManager)dependencyObject.GetValue(SynchronizationManagerProperty);
        }

        private static void SetSynchronizationManager(DependencyObject dependencyObject, SynchronizationManager value)
        {
            dependencyObject.SetValue(SynchronizationManagerProperty, value);
        }

        private static void OnSynchronizedSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                SynchronizationManager synchronizer = GetSynchronizationManager(dependencyObject);
                synchronizer.StopSynchronizing();

                SetSynchronizationManager(dependencyObject, null);
            }

            var list = e.NewValue as IList;
            var selector = dependencyObject as Selector;

            if (list != null && selector != null)
            {
                SynchronizationManager synchronizer = GetSynchronizationManager(dependencyObject);
                if (synchronizer == null)
                {
                    synchronizer = new SynchronizationManager(selector);
                    SetSynchronizationManager(dependencyObject, synchronizer);
                }

                synchronizer.StartSynchronizingList();
            }
        }

        private sealed class SynchronizationManager
        {
            private readonly Selector multiSelector;
            private TwoListSynchronizer synchronizer;

            internal SynchronizationManager(Selector selector)
            {
                this.multiSelector = selector;
            }

            public static IList GetSelectedItemsCollection(Selector selector)
            {
                if (selector is MultiSelector)
                {
                    return (selector as MultiSelector).SelectedItems;
                }
                else if (selector is System.Windows.Controls.ListBox listBox)
                {
                    return listBox.SelectedItems;
                }
                else
                {
                    throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
                }
            }

            public void StartSynchronizingList()
            {
                IList list = GetSynchronizedSelectedItems(this.multiSelector);

                if (list != null)
                {
                    this.synchronizer = new TwoListSynchronizer(
                        GetSelectedItemsCollection(this.multiSelector),
                        list,
                        new Threading.WpfPresentationDispatcher(this.multiSelector.Dispatcher));

                    this.synchronizer.StartSynchronizing();
                }
            }

            public void StopSynchronizing()
            {
                this.synchronizer.StopSynchronizing();
            }
        }
    }
}
