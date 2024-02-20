namespace Colosoft.Presentation.Controls
{
    public delegate void SingleSelectionChangedHandler(object sender, SingleSelectionChangedEventArgs e);

    public class SingleSelectionChangedEventArgs
    {
        public object NewSelection { get; }

        public int NewSelectionIndex { get; }

        public object OldSelection { get; }

        public bool Cancel { get; set; }

        public SingleSelectionChangedEventArgs(object oldSelection, object newSelecion, int newSelectionIndex)
        {
            this.NewSelection = newSelecion;
            this.OldSelection = oldSelection;
            this.NewSelectionIndex = newSelectionIndex;
        }
    }
}
