using Prism.Regions;
using Prism.Regions.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public abstract class DialogActivationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public const string BehaviorKey = "DialogActivation";

        private readonly Stack<IWindow> dialogs = new Stack<IWindow>();

        private IWindow CurrentDialog
        {
            get { return this.dialogs.Count > 0 ? this.dialogs.Peek() : null; }
        }

        public DependencyObject HostControl { get; set; }

        private static void SetMinWidth(IWindow dialogWindow, DependencyObject view)
        {
            if (dialogWindow is null)
            {
                throw new ArgumentNullException(nameof(dialogWindow));
            }

            var windowMinWidth = WindowBehavior.GetMinWidth(view);
            if (!double.IsNaN(windowMinWidth))
            {
                dialogWindow.MinWidth = windowMinWidth;
            }
        }

        private static void SetMinHeight(IWindow dialogWindow, DependencyObject view)
        {
            if (dialogWindow is null)
            {
                throw new ArgumentNullException(nameof(dialogWindow));
            }

            var windowMinHeight = WindowBehavior.GetMinHeight(view);
            if (!double.IsNaN(windowMinHeight))
            {
                dialogWindow.MinHeight = windowMinHeight;
            }
        }

        private static void SetWidth(IWindow dialogWindow, DependencyObject view)
        {
            if (dialogWindow is null)
            {
                throw new ArgumentNullException(nameof(dialogWindow));
            }

            var windowWidth = WindowBehavior.GetWidth(view);
            if (!double.IsNaN(windowWidth))
            {
                dialogWindow.Width = windowWidth;
            }
        }

        private static void SetHeight(IWindow dialogWindow, DependencyObject view)
        {
            if (dialogWindow is null)
            {
                throw new ArgumentNullException(nameof(dialogWindow));
            }

            var windowHeight = WindowBehavior.GetHeight(view);
            if (!double.IsNaN(windowHeight))
            {
                dialogWindow.Height = windowHeight;
            }
        }

        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
        }

        protected abstract IWindow CreateWindow();

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.PrepareContentDialog(e.NewItems[0]);
            }
        }

        private Style GetStyleForView()
        {
            return this.HostControl.GetValue(RegionPopupBehaviors.ContainerWindowStyleProperty) as Style;
        }

        private Style GetStyleForView(object view)
        {
            var dependencyObject = view as DependencyObject;

            if (dependencyObject != null)
            {
                return dependencyObject.GetValue(RegionPopupBehaviors.ContainerWindowStyleProperty) as Style;
            }

            return null;
        }

        internal bool? PrepareContentDialog(object view)
        {
            var dialogWindow = this.CreateWindow();

            var style = this.GetStyleForView(view);

            if (style == null)
            {
                style = this.GetStyleForView();
            }

            dialogWindow.Content = view;
            try
            {
                if (((IDialog)view).ShowMode != WindowShowMode.Show)
                {
                    var hostControl =
                        Application.Current.Windows.OfType<Window>()
                            .FirstOrDefault(x => x.IsFocused) ??
                            (Application.Current.Windows.OfType<Window>()
                                        .FirstOrDefault(f => f.IsActive) ?? this.HostControl);

                    dialogWindow.Owner = hostControl;
                }
            }
            catch (InvalidOperationException)
            {
                dialogWindow.Owner = null;
            }

            var viewDependencyObject = view as DependencyObject;

            dialogWindow.Closed += this.ContentDialogClosed;
            dialogWindow.Style = style;

            if (viewDependencyObject != null)
            {
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(WindowBehavior.WidthProperty, view.GetType())
                    .AddValueChanged(viewDependencyObject, (sender, e) =>
                        {
                            SetWidth(dialogWindow, viewDependencyObject);
                        });

                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(WindowBehavior.HeightProperty, view.GetType())
                    .AddValueChanged(viewDependencyObject, (sender, e) =>
                    {
                        SetHeight(dialogWindow, viewDependencyObject);
                    });

                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(WindowBehavior.MinWidthProperty, view.GetType())
                    .AddValueChanged(viewDependencyObject, (sender, e) =>
                    {
                        SetMinWidth(dialogWindow, viewDependencyObject);
                    });

                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(WindowBehavior.MinHeightProperty, view.GetType())
                    .AddValueChanged(viewDependencyObject, (sender, e) =>
                    {
                        SetMinHeight(dialogWindow, viewDependencyObject);
                    });

                SetHeight(dialogWindow, viewDependencyObject);
                SetWidth(dialogWindow, viewDependencyObject);
                SetMinHeight(dialogWindow, viewDependencyObject);
                SetMinWidth(dialogWindow, viewDependencyObject);

                dialogWindow.WindowStartupLocation = WindowWrapper.Convert(WindowDialogActivationBehavior.GetStartupLocation(viewDependencyObject));
            }

            if (view is IWindowContainer dialog)
            {
                dialog.Window = dialogWindow;
            }

            this.dialogs.Push(dialogWindow);

            var wrapper = dialogWindow as WindowWrapper;

            if (wrapper != null)
            {
                wrapper.Window.SetBinding(Window.TitleProperty, new System.Windows.Data.Binding("Content.Title")
                {
                    RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.Self),
                });

                try
                {
                    if ((view is IDialog) &&
                        ((IDialog)view).ShowMode == WindowShowMode.Show)
                    {
                        wrapper.Window.Show();
                        return false;
                    }
                    else
                    {
                        return wrapper.Window.ShowDialog();
                    }
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
                catch (InvalidOperationException)
                {
                    // ignore
                }
            }
            else
            {
                dialogWindow.Show();
            }

            return null;
        }

        private void CloseContentDialog()
        {
            if (this.CurrentDialog != null)
            {
                this.CurrentDialog.Closed -= this.ContentDialogClosed;
                this.CurrentDialog.Close();
                this.CurrentDialog.Content = null;
                this.CurrentDialog.Owner = null;
                this.dialogs.Pop();
            }
        }

        private void ContentDialogClosed(object sender, System.EventArgs e)
        {
            try
            {
                this.Region.Deactivate(this.CurrentDialog.Content);
            }
            catch
            {
                // ignore
            }

            this.CloseContentDialog();
        }
    }
}
