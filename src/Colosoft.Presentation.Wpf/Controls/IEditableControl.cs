namespace Colosoft.Presentation.Controls
{
    public interface IEditableControl
    {
        bool IsEditable { get; set; }

        bool IsInEditMode { get; set; }
    }
}
