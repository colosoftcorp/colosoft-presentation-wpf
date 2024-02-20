using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class WindowBehavior
    {
        private sealed class MoveWindowState : IDisposable
        {
            private Window window;
#pragma warning disable SA1305 // Field names should not use Hungarian notation
            private UIElement uiElement;
#pragma warning restore SA1305 // Field names should not use Hungarian notation

#pragma warning disable SA1305 // Field names should not use Hungarian notation
            public MoveWindowState(UIElement uiElement)
#pragma warning restore SA1305 // Field names should not use Hungarian notation
            {
                this.uiElement = uiElement;
                this.uiElement.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.uiElement_MouseDown);
            }

            private void uiElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                if (this.window == null)
                {
                    this.window = UIHelpers.TryFindParent<Window>(this.uiElement);

                    if (this.uiElement == null)
                    {
                        var dialog = UIHelpers.TryFindParent<Dialog>(this.uiElement);

                        if (dialog != null && dialog.Window is WindowWrapper wrapper)
                        {
                            this.uiElement = wrapper.Window;
                        }
                    }

                    if (this.uiElement == null)
                    {
                        throw new InvalidOperationException("Parent Window not found");
                    }
                }

                try
                {
                    this.window.DragMove();
                }
                catch
                {
                    // Ignore
                }
            }

            public void Dispose()
            {
                this.uiElement.MouseDown -= this.uiElement_MouseDown;
            }
        }

        public static readonly DependencyProperty CanMoveProperty =
            DependencyProperty.RegisterAttached("CanMove", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(false, CanMoveChanged));

        private static readonly DependencyProperty MoveWindowStateProperty =
            DependencyProperty.RegisterAttached("MoveWindowState", typeof(MoveWindowState), typeof(WindowBehavior));

        public static readonly DependencyProperty CloseButtonProperty =
            DependencyProperty.RegisterAttached("CloseButton", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(false, CloseButtonChanged));

        public static readonly DependencyProperty MaximizeButtonProperty =
            DependencyProperty.RegisterAttached("MaximizeButton", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(false, MaximizeButtonChanged));

        public static readonly DependencyProperty MinimizeButtonProperty =
            DependencyProperty.RegisterAttached("MinimizeButton", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(false, MinimizeButtonChanged));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height", typeof(double), typeof(WindowBehavior), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.RegisterAttached("MinHeight", typeof(double), typeof(WindowBehavior), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(double), typeof(WindowBehavior), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(WindowBehavior), new PropertyMetadata(double.NaN));

        private static void CanMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            var state = d.GetValue(MoveWindowStateProperty) as MoveWindowState;

            if (((bool)e.NewValue) && state == null)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                state = new MoveWindowState(element);
#pragma warning restore CA2000 // Dispose objects before losing scope
                d.SetCurrentValue(MoveWindowStateProperty, state);
            }
            else if (state != null)
            {
                state.Dispose();
                d.SetCurrentValue(MoveWindowStateProperty, null);
            }
        }

        private static void CloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;

            if (button != null)
            {
                button.Click += (sender, e2) =>
                {
                    var window = UIHelpers.TryFindParent<Window>(d);

                    if (window == null)
                    {
                        var dialog = UIHelpers.TryFindParent<Dialog>(d);

                        if (dialog != null && dialog.Window is WindowWrapper wrapper)
                        {
                            window = wrapper.Window;
                        }
                    }

                    if (window == null)
                    {
                        throw new InvalidOperationException("Parent Window not found");
                    }

                    window.Close();
                };
            }
        }

        private static void MaximizeButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;

            if (button != null)
            {
                button.Click += (sender, e2) =>
                {
                    var window = UIHelpers.TryFindParent<Window>(d);

                    if (window == null)
                    {
                        var dialog = UIHelpers.TryFindParent<Dialog>(d);

                        if (dialog != null && dialog.Window is WindowWrapper wrapper)
                        {
                            window = wrapper.Window;
                        }
                    }

                    if (window == null)
                    {
                        throw new InvalidOperationException("Parent Window not found");
                    }

                    if (window.Width == SystemParameters.WorkArea.Width && window.Height == SystemParameters.WorkArea.Height)
                    {
                        window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        window.Width = SystemParameters.WorkArea.Width / 1.4;
                        window.Height = SystemParameters.WorkArea.Height / 1.4;
                    }
                    else
                    {
                        window.Width = SystemParameters.WorkArea.Width;
                        window.Height = SystemParameters.WorkArea.Height;
                        window.Left = 0;
                        window.Top = 0;
                    }

                    window.WindowState = WindowState.Maximized;
                };
            }
        }

        private static void MinimizeButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;

            if (button != null)
            {
                button.Click += (sender, e2) =>
                {
                    var window = UIHelpers.TryFindParent<Window>(d);

                    if (window == null)
                    {
                        var dialog = UIHelpers.TryFindParent<Dialog>(d);

                        if (dialog != null && dialog.Window is WindowWrapper wrapper)
                        {
                            window = wrapper.Window;
                        }
                    }

                    if (window == null)
                    {
                        throw new InvalidOperationException("Parent Window not found");
                    }

                    window.WindowState = WindowState.Minimized;
                };
            }
        }

        public static bool GetMaximizeButton(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(MaximizeButtonProperty);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static void SetMaximizeButton(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(MaximizeButtonProperty, value);
        }

        public static bool GetMinimizeButton(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(MinimizeButtonProperty);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static void SetMinimizeButton(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(MinimizeButtonProperty, value);
        }

        public static bool GetCloseButton(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(CloseButtonProperty);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static void SetCloseButton(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(CloseButtonProperty, value);
        }

        public static bool GetCanMove(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(CanMoveProperty);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static void SetCanMove(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(CanMoveProperty, value);
        }

        public static double GetHeight(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (double)owner.GetValue(HeightProperty);
        }

        public static void SetHeight(DependencyObject owner, double value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(HeightProperty, value);
        }

        public static double GetMinHeight(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (double)owner.GetValue(MinHeightProperty);
        }

        public static void SetMinHeight(DependencyObject owner, double value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(MinHeightProperty, value);
        }

        public static double GetWidth(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (double)owner.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject owner, double value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(WidthProperty, value);
        }

        public static double GetMinWidth(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (double)owner.GetValue(MinWidthProperty);
        }

        public static void SetMinWidth(DependencyObject owner, double value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(MinWidthProperty, value);
        }
    }
}
