using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Colosoft.Presentation.Controls
{
    public class AdornedControl : ContentControl
    {
        public static readonly DependencyProperty IsAdornerVisibleProperty =
            DependencyProperty.Register(
                "IsAdornerVisible",
                typeof(bool),
                typeof(AdornedControl),
                new FrameworkPropertyMetadata(IsAdornerVisible_PropertyChanged));

        public static readonly DependencyProperty AdornerContentProperty =
            DependencyProperty.Register(
                "AdornerContent",
                typeof(FrameworkElement),
                typeof(AdornedControl),
                new FrameworkPropertyMetadata(AdornerContent_PropertyChanged));

        public static readonly DependencyProperty HorizontalAdornerPlacementProperty =
            DependencyProperty.Register(
                "HorizontalAdornerPlacement",
                typeof(AdornerPlacement),
                typeof(AdornedControl),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty VerticalAdornerPlacementProperty =
            DependencyProperty.Register(
                "VerticalAdornerPlacement",
                typeof(AdornerPlacement),
                typeof(AdornedControl),
                new FrameworkPropertyMetadata(AdornerPlacement.Inside));

        public static readonly DependencyProperty AdornerOffsetXProperty =
            DependencyProperty.Register(
                "AdornerOffsetX",
                typeof(double),
                typeof(AdornedControl));

        public static readonly DependencyProperty AdornerOffsetYProperty =
            DependencyProperty.Register(
                "AdornerOffsetY",
                typeof(double),
                typeof(AdornedControl));

        public static readonly RoutedCommand ShowAdornerCommand = new RoutedCommand("ShowAdorner", typeof(AdornedControl));

        public static readonly RoutedCommand HideAdornerCommand = new RoutedCommand("HideAdorner", typeof(AdornedControl));

        private AdornerLayer adornerLayer;
        private FrameworkElementAdorner adorner;

        public AdornedControl()
        {
            this.Focusable = false;
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.AdornedControl_DataContextChanged);
        }

        private void AdornedControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => this.UpdateAdornerDataContext();

        private void UpdateAdornerDataContext()
        {
            if (this.AdornerContent != null)
            {
                this.AdornerContent.DataContext = this.DataContext;
            }
        }

        public void ShowAdorner()
        {
            this.IsAdornerVisible = true;
        }

        public void HideAdorner()
        {
            this.IsAdornerVisible = false;
        }

        public bool IsAdornerVisible
        {
            get
            {
                return (bool)this.GetValue(IsAdornerVisibleProperty);
            }
            set
            {
                this.SetValue(IsAdornerVisibleProperty, value);
            }
        }

        public FrameworkElement AdornerContent
        {
            get
            {
                return (FrameworkElement)this.GetValue(AdornerContentProperty);
            }
            set
            {
                this.SetValue(AdornerContentProperty, value);
            }
        }

        public AdornerPlacement HorizontalAdornerPlacement
        {
            get
            {
                return (AdornerPlacement)this.GetValue(HorizontalAdornerPlacementProperty);
            }
            set
            {
                this.SetValue(HorizontalAdornerPlacementProperty, value);
            }
        }

        public AdornerPlacement VerticalAdornerPlacement
        {
            get
            {
                return (AdornerPlacement)this.GetValue(VerticalAdornerPlacementProperty);
            }
            set
            {
                this.SetValue(VerticalAdornerPlacementProperty, value);
            }
        }

        public double AdornerOffsetX
        {
            get
            {
                return (double)this.GetValue(AdornerOffsetXProperty);
            }
            set
            {
                this.SetValue(AdornerOffsetXProperty, value);
            }
        }

        public double AdornerOffsetY
        {
            get
            {
                return (double)this.GetValue(AdornerOffsetYProperty);
            }
            set
            {
                this.SetValue(AdornerOffsetYProperty, value);
            }
        }

        private static readonly CommandBinding ShowAdornerCommandBinding = new CommandBinding(ShowAdornerCommand, ShowAdornerCommand_Executed);
        private static readonly CommandBinding HideAdornerCommandBinding = new CommandBinding(HideAdornerCommand, HideAdornerCommand_Executed);

        static AdornedControl()
        {
            CommandManager.RegisterClassCommandBinding(typeof(AdornedControl), ShowAdornerCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(AdornedControl), HideAdornerCommandBinding);
        }

        private static void ShowAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            AdornedControl c = (AdornedControl)target;
            c.ShowAdorner();
        }

        private static void HideAdornerCommand_Executed(object target, ExecutedRoutedEventArgs e)
        {
            AdornedControl c = (AdornedControl)target;
            c.HideAdorner();
        }

        private static void IsAdornerVisible_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AdornedControl c = (AdornedControl)o;
            c.ShowOrHideAdornerInternal();
        }

#pragma warning disable S4144 // Methods should not have identical implementations
        private static void AdornerContent_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            AdornedControl c = (AdornedControl)o;
            c.ShowOrHideAdornerInternal();
        }

        private void ShowOrHideAdornerInternal()
        {
            if (this.IsAdornerVisible)
            {
                this.ShowAdornerInternal();
            }
            else
            {
                this.HideAdornerInternal();
            }
        }

        private void ShowAdornerInternal()
        {
            if (this.adorner != null)
            {
                return;
            }

            if (this.AdornerContent != null)
            {
                if (this.adornerLayer == null)
                {
                    this.adornerLayer = AdornerLayer.GetAdornerLayer(this);
                }

                if (this.adornerLayer != null && this.AdornerContent.Parent == null)
                {
                    if (this.adorner != null)
                    {
                        this.adorner.DisconnectChild();
                    }

                    try
                    {
                        this.adorner = new FrameworkElementAdorner(
                            this.AdornerContent,
                            this,
                            this.HorizontalAdornerPlacement,
                            this.VerticalAdornerPlacement,
                            this.AdornerOffsetX,
                            this.AdornerOffsetY);
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }

                    this.adornerLayer.Add(this.adorner);
                    this.UpdateAdornerDataContext();
                }
            }
        }

        private void HideAdornerInternal()
        {
            if (this.adornerLayer == null || this.adorner == null)
            {
                return;
            }

            this.adornerLayer.Remove(this.adorner);
            this.adorner.DisconnectChild();

            this.adorner = null;
            this.adornerLayer = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ShowOrHideAdornerInternal();
        }
    }
}
