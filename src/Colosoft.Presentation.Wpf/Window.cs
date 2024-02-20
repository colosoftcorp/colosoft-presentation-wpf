using Colosoft.Presentation.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Colosoft.Presentation
{
    public class Window<TViewModel> :
        Window,
        IViewFor<TViewModel>,
        IDialog,
        IWindowContainer,
        IDisposable
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(TViewModel),
                typeof(Window<TViewModel>),
                new PropertyMetadata(null));

        private readonly IDictionary<EventHandler, System.Windows.RoutedEventHandler> loadedEvents =
            new Dictionary<EventHandler, System.Windows.RoutedEventHandler>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly IWindow windowWrapper;
        private bool disposedValue;

        public Window()
            : base()
        {
            this.windowWrapper = new WindowWrapper(this);
            this.DataContextChanged += this.OnDataContextChanged;
            this.Loaded += this.OnLoaded;
        }

        ~Window()
        {
            this.Dispose(disposing: false);
        }

        public TViewModel BindingRoot => this.ViewModel;

        public TViewModel ViewModel
        {
            get => (TViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }

        public WindowShowMode ShowMode => WindowShowMode.ShowDialog;

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (TViewModel)value;
        }

        IWindow IWindowContainer.Window
        {
#pragma warning disable CA1033 // Interface methods should be callable by child types
            get => this.windowWrapper;
#pragma warning restore CA1033 // Interface methods should be callable by child types
            set
            {
                // ignore
            }
        }

        event EventHandler IDialog.Loaded
        {
            add
            {
                if (value != null)
                {
                    var event2 = new System.Windows.RoutedEventHandler((sender, e) => value.Invoke(sender, e));
                    this.Loaded += event2;
                    this.loadedEvents.Add(value, event2);
                }
            }

            remove
            {
                if (this.loadedEvents.TryGetValue(value, out var event2))
                {
                    this.Loaded -= event2;
                    this.loadedEvents.Remove(value);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                Task.Factory.StartNew(
                    async () => await dialogEventsNotifier.NotifyClosing(this, e, this.cancellationTokenSource.Token),
                    this.cancellationTokenSource.Token,
                    TaskCreationOptions.None,
                    new CurrentThreadScheduler());
            }
        }

        private void NotifyDialogEvent(Func<object, CancellationToken, Task> action)
        {
            Task.Factory.StartNew(
                async () => await action(this, this.cancellationTokenSource.Token),
                this.cancellationTokenSource.Token,
                TaskCreationOptions.None,
                new CurrentThreadScheduler());
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyLoaded);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyClosed);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyActivated);
            }
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyDeactivated);
            }
        }

        protected override void OnGotFocus(System.Windows.RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyGotFocus);
            }
        }

        protected override void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (this.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.DialogAccessor is IDialogEventsNotifier dialogEventsNotifier)
            {
                this.NotifyDialogEvent(dialogEventsNotifier.NotifyLostFocus);
            }
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
            if (e.PropertyName == nameof(IDialogAccessor.DialogResult) && this.ViewModel is IDialogViewModel dialogViewModel)
            {
                this.DialogResult = dialogViewModel.DialogAccessor?.DialogResult;
            }
        }

        private Task<bool?> ShowDialogRequested(object sender, CancellationToken cancellationToken) => Task.FromResult(this.ShowDialog());

        private Task ShowRequested(object sender, CancellationToken cancellationToken)
        {
            this.Show();
            return Task.CompletedTask;
        }

        private Task CloseRequested(object sender, CancellationToken cancellationToken)
        {
            this.Close();
            return Task.CompletedTask;
        }

        private Task HideRequested(object sender, CancellationToken cancellationToken)
        {
            this.Hide();
            return Task.CompletedTask;
        }

        private Task FocusRequested(object sender, CancellationToken cancellationToken)
        {
            this.Focus();
            return Task.CompletedTask;
        }

        Task IDialog.Close(CancellationToken cancellationToken)
        {
            this.Close();
            return Task.CompletedTask;
        }

        Task IDialog.Focus(CancellationToken cancellationToken)
        {
            this.Focus();
            return Task.CompletedTask;
        }

        Task IDialog.Hide(CancellationToken cancellationToken)
        {
            this.Hide();
            return Task.CompletedTask;
        }

        Task IDialog.Show(CancellationToken cancellationToken)
        {
            this.Show();
            return Task.CompletedTask;
        }

        Task<bool?> IDialog.ShowDialog(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.ShowDialog());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.cancellationTokenSource.Dispose();

                foreach (var i in this.loadedEvents.Values)
                {
                    this.Loaded -= i;
                }

                this.loadedEvents.Clear();
                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private sealed class CurrentThreadScheduler : TaskScheduler
        {
            private readonly System.Windows.Threading.Dispatcher dispatcher;

            public CurrentThreadScheduler()
            {
                this.dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            }

            protected override void QueueTask(Task task)
            {
                this.dispatcher.BeginInvoke(new Func<bool>(() => this.TryExecuteTask(task)));
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return true;
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return Enumerable.Empty<Task>();
            }
        }
    }
}
