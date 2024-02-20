using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation
{
    public class Dialog : UserControl, IDialog, Behaviors.IWindowContainer, System.ComponentModel.INotifyPropertyChanged
    {
        private Behaviors.IWindow window;
        private EventHandler loadedHandler;
        private WindowShowMode showMode;
        private System.ComponentModel.PropertyChangedEventHandler propertyChangedEvent;

        public event System.ComponentModel.CancelEventHandler Closing;

        public event EventHandler Closed;

        event EventHandler IDialog.Loaded
        {
            add
            {
                this.loadedHandler += value;
            }
            remove
            {
                this.loadedHandler -= value;
            }
        }

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler Resized;

        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
#pragma warning disable CA1033 // Interface methods should be callable by child types
            add
            {
                this.propertyChangedEvent += value;
            }
            remove
            {
                this.propertyChangedEvent -= value;
            }
#pragma warning restore CA1033 // Interface methods should be callable by child types
        }

        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(Dialog));

        public static void SetRegionName(DependencyObject regionTarget, string regionName)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            regionTarget.SetValue(RegionNameProperty, regionName);
        }

        public static string GetRegionName(DependencyObject regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            return regionTarget.GetValue(RegionNameProperty) as string;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Dialog), new PropertyMetadata(null, TitlePropertyChanged));

        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.Register(
                "WindowStartupLocation",
                typeof(Behaviors.WindowStartupLocation),
                typeof(Dialog),
                new PropertyMetadata(Behaviors.WindowStartupLocation.CenterScreen, StartupLocationPropertyChanged));

        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dialog = (Dialog)d;

            if (dialog.Window != null)
            {
                dialog.Window.Title = (e.NewValue ?? string.Empty).ToString();
            }

            dialog.RaisePropertyChanged("Title");
        }

        private static void StartupLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dialog = (Dialog)d;

            if (dialog.Window != null)
            {
                dialog.Window.WindowStartupLocation = (Behaviors.WindowStartupLocation)e.NewValue;
            }

            dialog.RaisePropertyChanged("StartupLocation");
        }

        public WindowShowMode ShowMode
        {
            get { return this.showMode; }
        }

        public virtual string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public virtual Behaviors.WindowStartupLocation WindowStartupLocation
        {
            get { return (Behaviors.WindowStartupLocation)this.GetValue(WindowStartupLocationProperty); }
            set { this.SetValue(WindowStartupLocationProperty, value); }
        }

        public Behaviors.IWindow Window
        {
            get { return this.window; }
            set
            {
                if (this.window != null)
                {
                    this.window.Closed -= this.WindowClosed;
                    this.window.Closing -= this.WindowClosing;
                    this.window.Activated -= this.WindowActivated;
                    this.window.Deactivated -= this.WindowDesactivated;
                    this.window.Resized -= this.WindowResized;
                }

                this.window = value;

                if (this.window != null)
                {
                    try
                    {
                        this.window.Title = this.Title;

                        this.window.Width = this.Width;
                        this.window.Height = this.Height;

                        var widthDiff = 0.0;
                        var heightDiff = 0.0;

                        if (this.window is Behaviors.WindowWrapper wrapper)
                        {
                            var bounds = System.Windows.Media.VisualTreeHelper.GetDescendantBounds(wrapper.Window);

                            if (!double.IsInfinity(bounds.Width))
                            {
                                widthDiff = this.window.Width - bounds.Width;
                            }

                            if (!double.IsInfinity(bounds.Height))
                            {
                                heightDiff = this.window.Height - bounds.Height;
                            }
                        }

                        this.window.Width = this.Width + widthDiff;
                        this.window.Height = this.Height + heightDiff;
                        this.window.MinWidth = this.MinWidth;
                        this.window.MinHeight = this.MinHeight;
                        this.window.WindowStartupLocation = this.WindowStartupLocation;
                    }
                    catch
                    {
                        // ignore
                    }

                    this.window.Closed += this.WindowClosed;
                    this.window.Closing += this.WindowClosing;
                    this.window.Activated += this.WindowActivated;
                    this.window.Deactivated += this.WindowDesactivated;
                    this.window.Resized += this.WindowResized;
                }
            }
        }

        public bool? DialogResult { get; set; }

        public bool IsActive
        {
            get
            {
                return this.Window != null ? this.Window.IsActive : false;
            }
        }

        protected Prism.Regions.IRegionManager RegionManager
        {
            get
            {
                return (Prism.Regions.IRegionManager)this.GetValue(Prism.Regions.RegionManager.RegionManagerProperty);
            }

            set
            {
                this.SetValue(Prism.Regions.RegionManager.RegionManagerProperty, value);
            }
        }

        protected string RegionName
        {
            get
            {
                return (string)this.GetValue(RegionNameProperty);
            }

            set
            {
                this.SetValue(RegionNameProperty, value);
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public IServiceProvider ServiceLocator { get; set; }

        public Dialog()
        {
            this.WindowStartupLocation = Behaviors.WindowStartupLocation.CenterScreen;
            this.Loaded += new System.Windows.RoutedEventHandler(this.Dialog_Loaded);
            this.DataContextChanged += this.OnDataContextChanged;
            this.RegionName = DialogSettings.PopupRegion;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IDialogViewModel oldDialogViewModel && oldDialogViewModel.DialogAccessor != null)
            {
                if (oldDialogViewModel.DialogAccessor is IDialogObserver accessor)
                {
                    accessor.ShowDialogRequested -= this.ShowDialogRequested;
                    accessor.ShowRequested -= this.ShowRequested;
                    accessor.CloseRequested -= this.CloseRequested;
                    accessor.HideRequested -= this.HideRequested;
                    accessor.FocusRequested -= this.FocusRequested;
                }

                oldDialogViewModel.DialogAccessor.PropertyChanged -= this.DialogAccessorPropertyChanged;
            }

            if (e.NewValue is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor != null)
            {
                if (dialogViewModel.DialogAccessor is IDialogObserver accessor)
                {
                    accessor.ShowDialogRequested += this.ShowDialogRequested;
                    accessor.ShowRequested += this.ShowRequested;
                    accessor.CloseRequested += this.CloseRequested;
                    accessor.HideRequested += this.HideRequested;
                    accessor.FocusRequested += this.FocusRequested;
                }

                dialogViewModel.DialogAccessor.PropertyChanged += this.DialogAccessorPropertyChanged;
            }
        }

        private void DialogAccessorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDialogAccessor.DialogResult) && this.DataContext is IDialogViewModel dialogViewModel)
            {
                this.DialogResult = dialogViewModel.DialogAccessor?.DialogResult;
            }
        }

        private Task<bool?> ShowDialogRequested(object sender, CancellationToken cancellationToken) => this.ShowDialog(cancellationToken);

        private Task ShowRequested(object sender, CancellationToken cancellationToken) => this.Show(cancellationToken: cancellationToken);

        private Task CloseRequested(object sender, CancellationToken cancellationToken) => this.Close(cancellationToken);

        private Task HideRequested(object sender, CancellationToken cancellationToken) => this.Hide(cancellationToken);

        private Task FocusRequested(object sender, CancellationToken cancellationToken)
        {
            this.Focus();
            return Task.CompletedTask;
        }

        private void Dialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.loadedHandler?.Invoke(this, e);
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            this.OnClosed();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.OnClosing(e);
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            this.OnActivated();
        }

        private void WindowDesactivated(object sender, EventArgs e)
        {
            this.OnDesativated();
        }

        private void WindowResized(object sender, EventArgs e)
        {
            this.OnResized();
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames != null && propertyNames.Length > 0 && this.propertyChangedEvent != null)
            {
                foreach (var i in propertyNames)
                {
                    this.propertyChangedEvent(this, new System.ComponentModel.PropertyChangedEventArgs(i));
                }
            }
        }

        protected virtual void OnClosed()
        {
            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        protected virtual void OnActivated()
        {
            if (!this.IsFocused)
            {
                this.window.Focus();
            }

            this.Activated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDesativated()
        {
            this.Deactivated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnResized()
        {
            if (this.Window != null)
            {
                var widthDiff = 0.0;
                var heightDiff = 0.0;

                if (this.Window is Behaviors.WindowWrapper wrapper)
                {
                    var bounds = System.Windows.Media.VisualTreeHelper.GetDescendantBounds(wrapper.Window);

                    if (!double.IsInfinity(bounds.Width))
                    {
                        this.Width = bounds.Width;
                        widthDiff = this.Window.Width - bounds.Width;
                    }
                    else
                    {
#pragma warning disable CA2245 // Do not assign a property to itself
                        this.Width = this.Width;
                    }

                    if (!double.IsInfinity(bounds.Height))
                    {
                        this.Height = bounds.Height;
                        heightDiff = this.Window.Height - bounds.Height;
                    }
                    else
                    {
                        this.Height = this.Height;
                    }
                }
                else
                {
                    this.Width = this.Width;
                    this.Height = this.Height;
                }
#pragma warning restore CA2245 // Do not assign a property to itself

                this.Window.MinWidth = Math.Max(this.MinWidth + (!double.IsNaN(widthDiff) ? widthDiff : 0), 0);
                this.Window.MinHeight = Math.Max(this.MinHeight + (!double.IsNaN(heightDiff) ? heightDiff : 0), 0);
            }

            this.Resized?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool BeforeClose()
        {
            return true;
        }

        public virtual Task<bool?> ShowDialog(CancellationToken cancellationToken)
        {
            this.showMode = WindowShowMode.ShowDialog;

            var regionManager = this.RegionManager ?? (Prism.Regions.IRegionManager)this.ServiceLocator?.GetService(typeof(Prism.Regions.IRegionManager));

            if (regionManager != null)
            {
                var position = this.RegionName ?? "PopupRegion";
                var region = regionManager.Regions[position];
                region.Add(this);

                try
                {
                    region.Activate(this);
                }
                finally
                {
                    region.Remove(this);
                }
            }

            return Task.FromResult(this.DialogResult);
        }

        public virtual bool? ShowDialog(object regionManager, string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new ArgumentException($"'{nameof(regionName)}' cannot be null or empty.", nameof(regionName));
            }

            this.showMode = WindowShowMode.ShowDialog;

            if (!(regionManager is Prism.Regions.IRegionManager))
            {
                throw new ArgumentException("Invalid region manager", nameof(regionManager));
            }

            this.RegionManager = regionManager as Prism.Regions.IRegionManager;
            this.RegionName = regionName;

            var region = this.RegionManager.Regions[regionName];

            if (region.Views == null ||
                !region.Views.Contains(this))
            {
                region.Add(this);
            }

            try
            {
                region.Activate(this);
                return this.DialogResult;
            }
            finally
            {
                region.Remove(this);
            }
        }

        public virtual bool? ShowDialog(string regionName)
        {
            this.showMode = WindowShowMode.ShowDialog;

            var regionManager = this.RegionManager;

            if (regionManager == null)
            {
                var serviceLocator = this.ServiceLocator;
                regionManager = (Prism.Regions.IRegionManager)serviceLocator.GetService(typeof(Prism.Regions.IRegionManager));
            }

            if (regionManager == null)
            {
                Behaviors.DialogActivationBehavior behavior;
                behavior = new Behaviors.WindowDialogActivationBehavior();
                behavior.HostControl = Application.Current.MainWindow;
                return behavior.PrepareContentDialog(this);
            }
            else
            {
                return this.ShowDialog(regionManager, regionName);
            }
        }

        public Task Show(CancellationToken cancellationToken)
        {
            this.showMode = WindowShowMode.Show;

            var regionManager = this.RegionManager ?? (Prism.Regions.IRegionManager)this.ServiceLocator.GetService(typeof(Prism.Regions.IRegionManager));

            if (regionManager != null)
            {
                var region = regionManager.Regions[this.RegionName];
                region.Add(this);

                region.Activate(this);
            }

            return Task.CompletedTask;
        }

        public virtual Task Close(CancellationToken cancellationToken)
        {
            if (this.Window != null)
            {
                this.Window.Close();
            }
            else
            {
                this.OnClosed();
            }

            return Task.CompletedTask;
        }

        public Task Hide(CancellationToken cancellationToken)
        {
            if (this.Window != null)
            {
                this.Window.Hide();
            }

            return Task.CompletedTask;
        }

        Task IDialog.Focus(CancellationToken cancellationToken)
        {
            if (this.Window != null)
            {
                this.Window.Focus();
            }

            return Task.CompletedTask;
        }
    }
}
