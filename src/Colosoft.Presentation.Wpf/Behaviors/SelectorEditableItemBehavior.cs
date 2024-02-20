using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class SelectorEditableItemBehavior
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.RegisterAttached(
                "IsEditable",
                typeof(bool),
                typeof(SelectorEditableItemBehavior),
                new PropertyMetadata(false, OnIsEditableChanged));

        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as System.Windows.Controls.Primitives.Selector;
            if (selector == null)
            {
                return;
            }

            selector.KeyDown += new System.Windows.Input.KeyEventHandler(SelectorKeyDown);
        }

        private static void SelectorKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F2)
            {
                SetCurrentItemInEditMode((System.Windows.Controls.Primitives.Selector)sender, true);
            }
        }

        private static void SetCurrentItemInEditMode(System.Windows.Controls.Primitives.Selector selector, bool editMode)
        {
            var selectedItem = selector.ItemContainerGenerator.ContainerFromIndex(selector.SelectedIndex);

            var editableControl = selectedItem as Controls.IEditableControl;

            if (editableControl == null)
            {
                foreach (var child in UIHelpers.NavigateChildren(selectedItem))
                {
                    if (child is Controls.IEditableControl control)
                    {
                        editableControl = control;
                        if (editableControl.IsEditable)
                        {
                            editableControl.IsInEditMode = editMode;
                        }
                    }
                }
            }
            else
            {
                if (editableControl.IsEditable)
                {
                    editableControl.IsInEditMode = editMode;
                }
            }
        }

        public static void SetIsEditable(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(IsEditableProperty, value);
        }

        public static bool GetIsEditable(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (bool)owner.GetValue(IsEditableProperty);
        }
    }
}
