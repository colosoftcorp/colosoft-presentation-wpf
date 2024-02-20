using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty SelectAllModeProperty = DependencyProperty.RegisterAttached(
            "SelectAllMode",
            typeof(SelectAllMode?),
            typeof(TextBoxBehavior),
            new PropertyMetadata(SelectAllModePropertyChanged));

        private static void SelectAllModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox)
            {
                var textBox = d as System.Windows.Controls.TextBox;

                if (e.NewValue != null)
                {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null)
            {
                return;
            }

            var textBox = (System.Windows.Controls.TextBox)dependencyObject;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is System.Windows.Controls.TextBox))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as System.Windows.Controls.TextBox;
            if (textBox == null)
            {
                return;
            }

            var selectAllMode = GetSelectAllMode(textBox);

            if (selectAllMode == SelectAllMode.Never)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = 0;
            }
            else
            {
                textBox.SelectAll();
            }

            if (selectAllMode == SelectAllMode.OnFirstFocusThenNever)
            {
                SetSelectAllMode(textBox, SelectAllMode.Never);
            }
            else if (selectAllMode == SelectAllMode.OnFirstFocusThenLeaveOff)
            {
                SetSelectAllMode(textBox, null);
            }
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
        public static SelectAllMode? GetSelectAllMode(DependencyObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (SelectAllMode)obj.GetValue(SelectAllModeProperty);
        }

        public static void SetSelectAllMode(DependencyObject obj, SelectAllMode? value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(SelectAllModeProperty, value);
        }
    }
}
