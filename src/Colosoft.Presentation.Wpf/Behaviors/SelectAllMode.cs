using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public enum SelectAllMode
    {
        OnFirstFocusThenLeaveOff = 0,
        OnFirstFocusThenNever = 1,
        OnEveryFocus = 2,
        Never = 4,
    }
}
