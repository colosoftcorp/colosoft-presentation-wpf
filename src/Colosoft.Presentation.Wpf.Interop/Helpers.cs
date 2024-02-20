using System.Windows;

namespace Colosoft.Presentation.Interop
{
    internal static class Helpers
    {
        internal static Window GetDefaultOwnerWindow()
        {
            Window defaultWindow = null;

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                defaultWindow = Application.Current.MainWindow;
            }

            return defaultWindow;
        }
    }
}
