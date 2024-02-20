using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public class TwoListSynchronizer : IWeakEventListener
    {
        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        private static readonly IListItemConverter DefaultConverter = new DoNothingListItemConverter();
        private readonly IList masterList;
        private readonly IListItemConverter masterTargetConverter;
        private readonly IList targetList;
        private readonly Colosoft.Threading.IDispatcher dispatcher;

        public TwoListSynchronizer(IList masterList, IList targetList, Colosoft.Threading.IDispatcher dispatcher, IListItemConverter masterTargetConverter)
        {
            this.masterList = masterList;
            this.targetList = targetList;
            this.masterTargetConverter = masterTargetConverter;
            this.dispatcher = dispatcher;
        }

        public TwoListSynchronizer(IList masterList, IList targetList, Colosoft.Threading.IDispatcher dispatcher)
            : this(masterList, targetList, dispatcher, DefaultConverter)
        {
        }

        public void StartSynchronizing()
        {
            this.ListenForChangeEvents(this.masterList);
            this.ListenForChangeEvents(this.targetList);

            this.SetListValuesFromSource(this.targetList, this.masterList, this.ConvertFromTargetToMaster);

            if (!this.TargetAndMasterCollectionsAreEqual())
            {
                this.SetListValuesFromSource(this.targetList, this.masterList, this.ConvertFromTargetToMaster);
            }
        }

        public void StopSynchronizing()
        {
            this.StopListeningForChangeEvents(this.masterList);
            this.StopListeningForChangeEvents(this.targetList);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            this.HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);
#pragma warning restore CA1062 // Validate arguments of public methods
            return true;
        }

        protected void ListenForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
            }
        }

        protected void StopListeningForChangeEvents(IList list)
        {
            if (list is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
            }
        }

        private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.NewItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                int insertionPoint = e.NewStartingIndex + i;

                if (insertionPoint > list.Count)
                {
                    list.Add(converter(e.NewItems[i]));
                }
                else
                {
                    list.Insert(insertionPoint, converter(e.NewItems[i]));
                }
            }
        }

        private object ConvertFromMasterToTarget(object masterListItem)
        {
            return this.masterTargetConverter == null ? masterListItem : this.masterTargetConverter.Convert(masterListItem);
        }

        private object ConvertFromTargetToMaster(object targetListItem)
        {
            return this.masterTargetConverter == null ? targetListItem : this.masterTargetConverter.ConvertBack(targetListItem);
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList sourceList = sender as IList;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.PerformActionOnAllLists(this.AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.PerformActionOnAllLists(this.MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.PerformActionOnAllLists(this.RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.PerformActionOnAllLists(this.ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.UpdateListsFromSource(sender as IList);
                    break;
                default:
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (sourceList == this.masterList)
            {
                this.PerformActionOnList(this.targetList, action, collectionChangedArgs, this.ConvertFromMasterToTarget);
            }
            else
            {
                this.PerformActionOnList(this.masterList, action, collectionChangedArgs, this.ConvertFromTargetToMaster);
            }
        }

        private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
        {
            this.StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            this.ListenForChangeEvents(list);
        }

        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int itemCount = e.OldItems.Count;

            for (int i = 0; i < itemCount && list.Count > 0; i++)
            {
                list.RemoveAt(e.OldStartingIndex);
            }
        }

#pragma warning disable S4144 // Methods should not have identical implementations
        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
        {
            if (!this.dispatcher.CheckAccess())
            {
                this.dispatcher.Invoke(
                    new Action<IList, IList, Converter<object, object>>(this.SetListValuesFromSource),
                    sourceList,
                    targetList,
                    converter);
            }
            else
            {
                this.StopListeningForChangeEvents(targetList);
                targetList.Clear();

                foreach (object o in sourceList)
                {
                    targetList.Add(converter(o));
                }

                this.ListenForChangeEvents(targetList);
            }
        }

        private bool TargetAndMasterCollectionsAreEqual()
        {
            return this.masterList.Cast<object>().SequenceEqual(this.targetList.Cast<object>().Select(item => this.ConvertFromTargetToMaster(item)));
        }

        private void UpdateListsFromSource(IList sourceList)
        {
            if (sourceList == this.masterList)
            {
                this.SetListValuesFromSource(this.masterList, this.targetList, this.ConvertFromMasterToTarget);
            }
            else
            {
                this.SetListValuesFromSource(this.targetList, this.masterList, this.ConvertFromTargetToMaster);
            }
        }

        private sealed class DoNothingListItemConverter : IListItemConverter
        {
            public object Convert(object masterListItem)
            {
                return masterListItem;
            }

            public object ConvertBack(object targetListItem)
            {
                return targetListItem;
            }
        }
    }
}
