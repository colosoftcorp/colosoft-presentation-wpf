using Colosoft.Threading;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    internal class WpfDialogAccessor : IDialogAccessor
    {
        private readonly object viewModel;
        private readonly IDispatcher dispatcher;
        private readonly IViewLocator viewLocator;

        private IDialog dialog;

        public WpfDialogAccessor(
            object viewModel,
            IDispatcher dispatcher,
            IViewLocator viewLocator)
        {
            this.viewModel = viewModel;
            this.dispatcher = dispatcher;
            this.viewLocator = viewLocator;
        }

        private void DialogDeactivated(object sender, System.EventArgs e)
        {
            if (this.Deactivated != null)
            {
                this.Deactivated.Invoke(this, default).Wait();
            }
        }

        private void DialogActivated(object sender, System.EventArgs e)
        {
            if (this.Activated != null)
            {
                this.Activated.Invoke(this, default).Wait();
            }
        }

        private void DialogClosed(object sender, System.EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed.Invoke(sender, default).Wait();
            }
        }

        private void DialogClosing(object sender, CancelEventArgs e)
        {
            if (this.Closing != null)
            {
                this.Closing.Invoke(this, e, default).Wait();
            }
        }

        public bool? DialogResult { get; set; }

        public bool IsFocused
        {
            get => this.Dialog.IsActive;
        }

        public event DialogClosingEventHandler Closing;
        public event ActionDialogRequestedEventHandler Closed;
        public event ActionDialogRequestedEventHandler Activated;
        public event ActionDialogRequestedEventHandler Deactivated;
        public event ActionDialogRequestedEventHandler GotFocus;
        public event ActionDialogRequestedEventHandler LostFocus;
        public event PropertyChangedEventHandler PropertyChanged;

        private IDialog Dialog
        {
            get
            {
                if (this.dialog == null)
                {
                    this.dialog = (IDialog)this.dispatcher.Invoke(() => this.viewLocator.ResolveView(this.viewModel, this.viewModel.GetType()));

                    this.dialog.Closing += this.DialogClosing;
                    this.dialog.Closed += this.DialogClosed;
                    this.dialog.Activated += this.DialogActivated;
                    this.dialog.Deactivated += this.DialogDeactivated;
                }

                return this.dialog;
            }
        }

        public Task Close(CancellationToken cancellationToken) => this.Dialog.Close(cancellationToken);

        public Task Focus(CancellationToken cancellationToken) => this.Dialog.Focus(cancellationToken);

        public Task Hide(CancellationToken cancellationToken) => this.Dialog.Hide(cancellationToken);

        public Task Show(CancellationToken cancellationToken) => this.Dialog.Show(cancellationToken);

        public async Task<bool?> ShowDialog(CancellationToken cancellationToken)
        {
            var result = await this.Dialog.ShowDialog(cancellationToken);
            this.DialogResult = result;

            return result;
        }
    }
}