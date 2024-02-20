namespace Colosoft.Collections
{
#pragma warning disable CA1010 // Generic interface should also be implemented
    public class ObservableCollectionViewFactory : System.ComponentModel.ICollectionViewFactory, System.Collections.IEnumerable
#pragma warning restore CA1010 // Generic interface should also be implemented
    {
        private readonly System.Collections.IEnumerable source;

        public System.Collections.IEnumerable Source
        {
            get { return this.source; }
        }

        public ObservableCollectionViewFactory(System.Collections.IEnumerable source)
        {
            this.source = source ?? throw new System.ArgumentNullException(nameof(source));
        }

        public System.ComponentModel.ICollectionView CreateView()
        {
            return new CollectionViewProxy(new ObservableCollectionView(this.Source));
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.source.GetEnumerator();
        }
    }
}
