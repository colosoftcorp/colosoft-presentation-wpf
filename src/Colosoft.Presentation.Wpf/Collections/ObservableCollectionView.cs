using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Colosoft.Collections
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
#pragma warning disable CA1010 // Generic interface should also be implemented
    public class ObservableCollectionView : NotificationObject, ICollectionView
#pragma warning restore CA1010 // Generic interface should also be implemented
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private static readonly CurrentChangingEventArgs UncancelableCurrentChangingEventArgs = new CurrentChangingEventArgs(false);
        private static object newItemPlaceholder = new { Name = "NewItemPlaceholder" };

        private readonly System.Collections.IEnumerable innerCollection;
        private readonly Threading.SimpleMonitor currentChangedMonitor;
        private object currentItem;
        private int currentPosition;
        private CollectionViewFlags flags;
        private int timestamp;
        private int deferLevel;

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        public static object NewItemPlaceholder
        {
            get
            {
                return newItemPlaceholder;
            }
        }

        public virtual object CurrentItem
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.currentItem;
            }
        }

        public virtual int CurrentPosition
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.currentPosition;
            }
        }

        protected bool IsRefreshDeferred
        {
            get
            {
                return this.deferLevel > 0;
            }
        }

        public virtual bool NeedsRefresh
        {
            get
            {
                return this.CheckFlag(CollectionViewFlags.NeedsRefresh);
            }
        }

        public bool CanFilter
        {
            get { return this.innerCollection is IFilteredObservableCollection; }
        }

        public bool CanGroup
        {
            get { return false; }
        }

        public bool CanSort
        {
            get { return false; }
        }

        public int Count
        {
            get
            {
                if (this.innerCollection is System.Collections.ICollection collection)
                {
                    return collection.Count;
                }

                int num = 0;

                System.Collections.IEnumerator enumerator = this.innerCollection.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        num++;
                    }
                }
                catch
                {
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    throw;
                }

                return num;
            }
        }

        public bool Contains(object item)
        {
            if (this.innerCollection is System.Collections.IList list)
            {
                return list.Contains(item);
            }

            return this.IndexOf(item) >= 0;
        }

        public System.Globalization.CultureInfo Culture { get; set; } = System.Globalization.CultureInfo.CurrentCulture;

        private bool IsCurrentInView
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return (this.CurrentPosition >= 0) && (this.CurrentPosition < this.Count);
            }
        }

        protected bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return this.GetItemAt(this.CurrentPosition) == this.CurrentItem;
                }

                return this.CurrentItem == null;
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return SortDescriptionCollection.Empty;
            }
        }

        public System.Collections.IEnumerable SourceCollection
        {
            get { return this.innerCollection; }
        }

        public Predicate<object> Filter
        {
            get
            {
                if (this.innerCollection is IFilteredObservableCollection collection)
                {
                    return collection.Filter;
                }

                throw new NotSupportedException();
            }
            set
            {
                if (this.innerCollection is IFilteredObservableCollection collection)
                {
                    collection.Filter = value;
                }

                throw new NotSupportedException();
            }
        }

#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
        public System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions => null;

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups => null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

        public virtual bool IsCurrentAfterLast
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        public ObservableCollectionView(System.Collections.IEnumerable collection)
            : this(collection, 0)
        {
        }

        internal ObservableCollectionView(System.Collections.IEnumerable collection, int moveToFirst)
        {
            this.currentChangedMonitor = new Threading.SimpleMonitor();
            this.flags = CollectionViewFlags.NeedsRefresh | CollectionViewFlags.ShouldProcessCollectionChanged;
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.innerCollection = collection;
            INotifyCollectionChanged changed = collection as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
                this.SetFlag(CollectionViewFlags.IsDynamic, true);
            }

            object current = null;
            int num = -1;
            if (moveToFirst >= 0)
            {
                var enumerator = collection.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    num = 0;
                }
            }

            this.currentItem = current;
            this.currentPosition = num;
            this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, this.currentPosition < 0);
            this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, this.currentPosition < 0);
            this.SetFlag(CollectionViewFlags.CachedIsEmpty, this.currentPosition < 0);
        }

        protected void OnCurrentChanging()
        {
            this.currentPosition = -1;
            this.OnCurrentChanging(UncancelableCurrentChangingEventArgs);
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            this.CollectionChanged?.Invoke(this, args);
        }

        protected internal virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            this.timestamp++;
            this.CollectionChanged?.Invoke(this, args);

            if ((args.Action != NotifyCollectionChangedAction.Replace) && (args.Action != NotifyCollectionChangedAction.Move))
            {
                this.OnPropertyChanged(nameof(this.Count));
            }

            var isEmpty = this.IsEmpty;
            if (isEmpty != this.CheckFlag(CollectionViewFlags.CachedIsEmpty))
            {
                this.SetFlag(CollectionViewFlags.CachedIsEmpty, isEmpty);
                this.OnPropertyChanged(nameof(this.IsEmpty));
            }
        }

        protected internal virtual void RefreshOverride()
        {
            if (this.SortDescriptions.Count > 0)
            {
                throw new InvalidOperationException("Refresh not supported with sort");
            }

            object item = this.currentItem;
            bool flag = this.CheckFlag(CollectionViewFlags.IsCurrentAfterLast);
            bool flag2 = this.CheckFlag(CollectionViewFlags.IsCurrentBeforeFirst);
            int num = this.currentPosition;
            this.OnCurrentChanging();

            if (this.IsEmpty || flag2)
            {
                this._MoveCurrentToPosition(-1);
            }
            else if (flag)
            {
                this._MoveCurrentToPosition(this.Count);
            }
            else if (item != null)
            {
                int index = this.IndexOf(item);
                if (index < 0)
                {
                    index = 0;
                }

                this._MoveCurrentToPosition(index);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnCurrentChanged();

            if (this.IsCurrentAfterLast != flag)
            {
                this.OnPropertyChanged(nameof(this.IsCurrentAfterLast));
            }

            if (this.IsCurrentBeforeFirst != flag2)
            {
                this.OnPropertyChanged(nameof(this.IsCurrentBeforeFirst));
            }

            if (num != this.CurrentPosition)
            {
                this.OnPropertyChanged(nameof(this.CurrentPosition));
            }

            if (item != this.CurrentItem)
            {
                this.OnPropertyChanged(nameof(this.CurrentItem));
            }
        }

        protected internal virtual void OnCurrentChanged()
        {
            if ((this.CurrentChanged != null) && !this.currentChangedMonitor.Busy)
            {
                this.currentChangedMonitor.Enter();
                using (this.currentChangedMonitor)
                {
                    this.CurrentChanged(this, EventArgs.Empty);
                }
            }
        }

        protected internal virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (this.currentChangedMonitor.Busy)
            {
                if (args.IsCancelable)
                {
                    args.Cancel = true;
                }
            }
            else
            {
                this.CurrentChanging?.Invoke(this, args);
            }
        }

        protected bool OKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            this.OnCurrentChanging(args);
            return !args.Cancel;
        }

        protected void SetCurrent(object newItem, int newPosition)
        {
            var count = 0;
            if (newItem == null && !this.IsEmpty)
            {
                count = this.Count;
            }

            this.SetCurrent(newItem, newPosition, count);
        }

        protected void SetCurrent(object newItem, int newPosition, int count)
        {
            if (newItem != null)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, false);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }

            this.currentItem = newItem;
            this.currentPosition = newPosition;
        }

        private bool CheckFlag(CollectionViewFlags flags)
        {
            return (this.flags & flags) != 0;
        }

        private void SetFlag(CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                this.flags |= flags;
            }
            else
            {
                this.flags &= ~flags;
            }
        }

        private void _MoveCurrentToPosition(int position)
        {
            if (position < 0)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetCurrent(null, -1);
            }
            else if (position >= this.Count)
            {
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast, true);
                this.SetCurrent(null, this.Count);
            }
            else
            {
                this.SetFlag(CollectionViewFlags.IsCurrentAfterLast | CollectionViewFlags.IsCurrentBeforeFirst, false);
                this.SetCurrent(this.GetItemAt(position), position);
            }
        }

        private void EndDefer()
        {
            this.deferLevel--;
            if ((this.deferLevel == 0) && this.CheckFlag(CollectionViewFlags.NeedsRefresh))
            {
                this.Refresh();
            }
        }

        public IDisposable DeferRefresh()
        {
            IEditableCollectionView view = this as IEditableCollectionView;
            if ((view != null) && (view.IsAddingNew || view.IsEditingItem))
            {
                throw new InvalidOperationException("DeferRefresh not allowed during add or edit");
            }

            this.deferLevel++;
            return new DeferHelper(this);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.innerCollection.GetEnumerator();
        }

        public virtual bool MoveCurrentTo(object item)
        {
            this.VerifyRefreshNotDeferred();
            if ((object.Equals(this.CurrentItem, item) || object.Equals(NewItemPlaceholder, item)) && ((item != null) || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }

            int position = -1;
            if (this.PassesFilter(item))
            {
                position = this.IndexOf(item);
            }

            return this.MoveCurrentToPosition(position);
        }

        public virtual bool MoveCurrentToFirst()
        {
            this.VerifyRefreshNotDeferred();
            int position = 0;
            IEditableCollectionView view = this as IEditableCollectionView;
            if ((view != null) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
            {
                position = 1;
            }

            return this.MoveCurrentToPosition(position);
        }

        public virtual bool MoveCurrentToLast()
        {
            this.VerifyRefreshNotDeferred();
            int position = this.Count - 1;
            IEditableCollectionView view = this as IEditableCollectionView;
            if ((view != null) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
            {
                position--;
            }

            return this.MoveCurrentToPosition(position);
        }

        public virtual bool MoveCurrentToNext()
        {
            this.VerifyRefreshNotDeferred();
            int position = this.CurrentPosition + 1;
            int count = this.Count;
            IEditableCollectionView view = this as IEditableCollectionView;
            if (((view != null) && (position == 0)) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
            {
                position = 1;
            }

            if (((view != null) && (position == (count - 1))) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
            {
                position = count;
            }

            return (position <= count) && this.MoveCurrentToPosition(position);
        }

        public virtual bool MoveCurrentToPosition(int position)
        {
            this.VerifyRefreshNotDeferred();
            if ((position < -1) || (position > this.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            IEditableCollectionView view = this as IEditableCollectionView;
            if (((view == null) || (((position != 0) || (view.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtBeginning)) &&
                ((position != (this.Count - 1)) || (view.NewItemPlaceholderPosition != NewItemPlaceholderPosition.AtEnd)))) &&
                (((position != this.CurrentPosition) || !this.IsCurrentInSync) && this.OKToChangeCurrent()))
            {
                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                this._MoveCurrentToPosition(position);
                this.OnCurrentChanged();
                if (this.IsCurrentAfterLast != isCurrentAfterLast)
                {
                    this.OnPropertyChanged(nameof(this.IsCurrentAfterLast));
                }

                if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
                {
                    this.OnPropertyChanged(nameof(this.IsCurrentBeforeFirst));
                }

                this.OnPropertyChanged(nameof(this.CurrentPosition), nameof(this.CurrentItem));
            }

            return this.IsCurrentInView;
        }

        public virtual bool MoveCurrentToPrevious()
        {
            this.VerifyRefreshNotDeferred();
            int position = this.CurrentPosition - 1;
            int count = this.Count;

            IEditableCollectionView view = this as IEditableCollectionView;
            if (((view != null) && (position == (count - 1))) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd))
            {
                position = count - 2;
            }

            if (((view != null) && (position == 0)) && (view.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning))
            {
                position = -1;
            }

            return (position >= -1) && this.MoveCurrentToPosition(position);
        }

        public virtual void Refresh()
        {
            IEditableCollectionView view = this as IEditableCollectionView;
            if ((view != null) && (view.IsAddingNew || view.IsEditingItem))
            {
                throw new InvalidOperationException("Refresh not allowed during add or edit");
            }

            this.RefreshInternal();
        }

        public virtual object GetItemAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (this.innerCollection is System.Collections.IList list)
            {
                return list[index];
            }

            int num = 0;

            System.Collections.IEnumerator enumerator = this.innerCollection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (num == index)
                    {
                        return enumerator.Current;
                    }

                    num++;
                }
            }
            catch
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                throw;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public virtual int IndexOf(object item)
        {
            this.VerifyRefreshNotDeferred();

            if (this.innerCollection is System.Collections.IList list)
            {
                return list.IndexOf(item);
            }

            int num = 0;

            System.Collections.IEnumerator enumerator = this.innerCollection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == item)
                    {
                        return num;
                    }

                    num++;
                }
            }
            catch
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                throw;
            }

            return -1;
        }

        public virtual bool PassesFilter(object item)
        {
            if (this.CanFilter && (this.Filter != null))
            {
                return this.Filter(item);
            }

            return true;
        }

        internal void RefreshInternal()
        {
            this.RefreshOverride();
            this.SetFlag(CollectionViewFlags.NeedsRefresh, false);
        }

        internal void VerifyRefreshNotDeferred()
        {
            if (this.IsRefreshDeferred)
            {
                throw new InvalidOperationException("NoCheckOrChangeWhenDeferred");
            }
        }

        private sealed class DeferHelper : IDisposable
        {
            private ObservableCollectionView collectionView;

            public DeferHelper(ObservableCollectionView collectionView)
            {
                this.collectionView = collectionView;
            }

            public void Dispose()
            {
                if (this.collectionView != null)
                {
                    this.collectionView.EndDefer();
                    this.collectionView = null;
                }
            }
        }
    }
}
