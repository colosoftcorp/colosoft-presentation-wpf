using System;

namespace Colosoft.Presentation.Behaviors
{
    public interface IWindow
    {
        event EventHandler Closed;

        event System.ComponentModel.CancelEventHandler Closing;

        event EventHandler Activated;

        event EventHandler Deactivated;

        event EventHandler Resized;

        bool IsActive { get; }

        bool? DialogResult { get; set; }

        object Content { get; set; }

        object Owner { get; set; }

        object Style { get; set; }

        WindowStartupLocation WindowStartupLocation { get; set; }

        double ActualHeight { get;  }

        double ActualWidth { get; }

        double Height { get; set; }

        double Width { get; set; }

        double MinWidth { get; set; }

        double MinHeight { get; set; }

        bool IsFocused { get; }

        string Title { get; set; }

        void Show();

        bool? ShowDialog();

        void Close();

        void Hide();

        void Focus();
    }
}
