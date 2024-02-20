using System;

namespace Colosoft.Collections
{
    internal class CollectionViewProxy : System.Windows.Data.CollectionView
    {
        private readonly ObservableCollectionView view;

        public CollectionViewProxy(ObservableCollectionView view)
            : base(Array.Empty<object>())
        {
            this.view = view;
        }

        public override bool CanFilter
        {
            get { return this.view.CanFilter; }
        }

        public override bool CanSort
        {
            get { return this.view.CanSort; }
        }

        public override bool CanGroup
        {
            get { return this.view.CanGroup; }
        }

        protected override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { this.view.CollectionChanged += value; }
            remove { this.view.CollectionChanged -= value; }
        }

        public override bool Contains(object item)
        {
            return this.view.Contains(item);
        }

        public override int Count
        {
            get { return this.view.Count; }
        }

        public override System.Globalization.CultureInfo Culture
        {
            get { return this.view.Culture; }
            set { this.view.Culture = value; }
        }

        public override event EventHandler CurrentChanged
        {
            add { this.view.CurrentChanged += value; }
            remove { this.view.CurrentChanged -= value; }
        }

        public override event System.ComponentModel.CurrentChangingEventHandler CurrentChanging
        {
            add { this.view.CurrentChanging += value; }
            remove { this.view.CurrentChanging -= value; }
        }

        public override object CurrentItem
        {
            get { return this.view.CurrentItem; }
        }

        public override int CurrentPosition
        {
            get { return this.view.CurrentPosition; }
        }

        public override IDisposable DeferRefresh()
        {
            return this.view.DeferRefresh();
        }

        public override Predicate<object> Filter
        {
            get
            {
                return this.view.Filter;
            }
            set
            {
                this.view.Filter = value;
            }
        }

        protected override System.Collections.IEnumerator GetEnumerator()
        {
            return this.view.GetEnumerator();
        }

        public override object GetItemAt(int index)
        {
            return this.view.GetItemAt(index);
        }

        public override System.Collections.ObjectModel.ObservableCollection<System.ComponentModel.GroupDescription> GroupDescriptions
        {
            get
            {
                return this.view.GroupDescriptions;
            }
        }

        public override System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return this.view.Groups;
            }
        }

        public override int IndexOf(object item)
        {
            return this.view.IndexOf(item);
        }

        public override bool IsCurrentAfterLast
        {
            get
            {
                return this.view.IsCurrentAfterLast;
            }
        }

        public override bool IsCurrentBeforeFirst
        {
            get
            {
                return this.view.IsCurrentBeforeFirst;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return this.view.IsEmpty;
            }
        }

        public override bool MoveCurrentTo(object item)
        {
            return this.view.MoveCurrentTo(item);
        }

        public override bool MoveCurrentToFirst()
        {
            return this.view.MoveCurrentToFirst();
        }

        public override bool MoveCurrentToLast()
        {
            return this.view.MoveCurrentToLast();
        }

        public override bool MoveCurrentToNext()
        {
            return this.view.MoveCurrentToNext();
        }

        public override bool MoveCurrentToPosition(int position)
        {
            return this.view.MoveCurrentToPosition(position);
        }

        public override bool MoveCurrentToPrevious()
        {
            return this.view.MoveCurrentToPrevious();
        }

        public override bool NeedsRefresh
        {
            get
            {
                return this.view.NeedsRefresh;
            }
        }

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            this.view.OnCollectionChanged(args);
        }

        protected override void OnCurrentChanged()
        {
            this.view.OnCurrentChanged();
        }

        protected override void OnCurrentChanging(System.ComponentModel.CurrentChangingEventArgs args)
        {
            this.view.OnCurrentChanging(args);
        }

        public override bool PassesFilter(object item)
        {
            return this.view.PassesFilter(item);
        }

        protected override event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            add { this.view.PropertyChanged += value; }
            remove { this.view.PropertyChanged -= value; }
        }

        public override void Refresh()
        {
            this.view.Refresh();
        }

        protected override void RefreshOverride()
        {
            this.view.RefreshOverride();
        }

        public override System.Collections.IEnumerable SourceCollection
        {
            get
            {
                return this.view.SourceCollection;
            }
        }
    }
}
