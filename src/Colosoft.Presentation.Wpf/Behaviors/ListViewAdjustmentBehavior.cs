using System;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation.Behaviors
{
    public static class ListViewAdjustmentBehavior
    {
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(ListViewAdjustmentBehavior),
            new PropertyMetadata(false, AttachChanged));

        public static bool GetAttach(DependencyObject owner)
        {
            return owner != null && (bool)owner.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject owner, bool value)
        {
            if (owner != null)
            {
                owner.SetValue(AttachProperty, value);
            }
        }

        private static void AttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs a)
        {
            var ctrl = d as ListView;
            if (ctrl == null)
            {
                return;
            }

            if (ctrl.View == null)
            {
                d.AddChangeListener(ListView.ViewProperty, ViewChanged);
            }
            else if (!ctrl.IsInitialized)
            {
                ctrl.Initialized += CtrlInitialized;
            }
            else
            {
                ViewChanged(d, EventArgs.Empty);
            }
        }

        private static void CtrlInitialized(object sender, EventArgs e)
        {
            ViewChanged(sender, e);
            ((ListView)sender).Initialized -= CtrlInitialized;
        }

        private static void ViewChanged(object sender, EventArgs a)
        {
            var ctrl = sender as ListView;
            if (ctrl == null)
            {
                return;
            }

            var view = ctrl.View as GridView;
            if ((view == null) || (view.Columns.Count == 0))
            {
                return;
            }

            if (view.Columns.Count == 1)
            {
                SetAttach(ctrl, false);
                ctrl.RemoveChangeListener(ListView.ViewProperty, ViewChanged);
                return;
            }

            foreach (GridViewColumn col in view.Columns)
            {
                col.Width = col.ActualWidth;
                col.Width = double.NaN;
            }

            SetAttach(ctrl, false);
            ctrl.RemoveChangeListener(ListView.ViewProperty, ViewChanged);
        }
    }
}