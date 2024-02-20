using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public sealed class WindowWrapper : IWindow
    {
        private readonly Window window;
        private string title;

        public bool? DialogResult
        {
            get { return this.window.DialogResult; }
            set { this.window.DialogResult = value; }
        }

        public event System.ComponentModel.CancelEventHandler Closing
        {
            add { this.window.Closing += value; }
            remove { this.window.Closing -= value; }
        }

        public event EventHandler Closed
        {
            add { this.window.Closed += value; }
            remove { this.window.Closed -= value; }
        }

        public event EventHandler Activated
        {
            add { this.window.Activated += value; }
            remove { this.window.Activated -= value; }
        }

        public event EventHandler Deactivated
        {
            add { this.window.Deactivated += value; }
            remove { this.window.Deactivated -= value; }
        }

        public event EventHandler Resized;

        public bool IsActive
        {
            get { return this.window.IsActive; }
        }

        public object Content
        {
            get { return this.window.Content; }
            set { this.window.Content = value; }
        }

        public object Owner
        {
            get { return this.window.Owner; }
            set
            {
                this.window.Owner = value as Window;
            }
        }

        public Style Style
        {
            get { return this.window.Style; }
            set { this.window.Style = value; }
        }

        public Window Window
        {
            get { return this.window; }
        }

        public bool IsFocused
        {
            get { return this.window.IsFocused; }
        }

        public System.Windows.WindowStartupLocation StartupLocation
        {
            get { return this.window.WindowStartupLocation; }
            set { this.window.WindowStartupLocation = value; }
        }

        public string Title
        {
#pragma warning disable S4275 // Getters and setters should access the expected fields
            get => this.window.Title;
#pragma warning restore S4275 // Getters and setters should access the expected fields
            set
            {
                this.window.Title = value ?? string.Empty;
                this.title = value ?? string.Empty;
            }
        }

        object IWindow.Style
        {
            get { return this.window.Style; }
            set { this.window.Style = (Style)value; }
        }

        WindowStartupLocation IWindow.WindowStartupLocation
        {
            get
            {
                return Convert(this.window.WindowStartupLocation);
            }
            set
            {
                switch (value)
                {
                    case WindowStartupLocation.CenterOwner:
                        this.window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                        break;
                    case WindowStartupLocation.CenterScreen:
                        this.window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        break;
                    default:
                        this.window.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                        break;
                }
            }
        }

        double IWindow.ActualHeight
        {
            get { return this.window.ActualHeight; }
        }

        double IWindow.ActualWidth
        {
            get { return this.window.ActualWidth; }
        }

        double IWindow.Height
        {
            get { return this.window.Height; }
            set
            {
                this.window.Height = value;
            }
        }

        double IWindow.Width
        {
            get { return this.window.Width; }
            set
            {
                this.window.Width = value;
            }
        }

        double IWindow.MinHeight
        {
            get { return this.window.MinHeight; }
            set
            {
                this.window.MinHeight = value;
            }
        }

        double IWindow.MinWidth
        {
            get { return this.window.MinWidth; }
            set
            {
                this.window.MinWidth = value;
            }
        }

        public WindowWrapper(Window window)
        {
            this.window = window;
            this.window.SetValue(FocusFirstItemBehavior.FocusFirstItemProperty, true);
            this.window.SizeChanged += this.WindowResized;
            this.title = this.window.Title;
        }

        public WindowWrapper()
            : this(new Window())
        {
        }

        public static WindowStartupLocation Convert(System.Windows.WindowStartupLocation location)
        {
            switch (location)
            {
                case System.Windows.WindowStartupLocation.CenterOwner:
                    return WindowStartupLocation.CenterOwner;
                case System.Windows.WindowStartupLocation.CenterScreen:
                    return WindowStartupLocation.CenterScreen;
                default:
                    return WindowStartupLocation.Manual;
            }
        }

        private void WindowResized(object sender, EventArgs e)
        {
            this.Resized?.Invoke(this, e);
        }

        public void Show()
        {
            this.window.Title = this.title;
            this.window.Show();
        }

        public bool? ShowDialog()
        {
            this.window.Title = this.title;
            return this.window.ShowDialog();
        }

        public void Close()
        {
            this.window.Close();
        }

        public void Hide()
        {
            this.window.Hide();
        }

        public void Focus()
        {
            this.window.Focus();
        }
    }
}
