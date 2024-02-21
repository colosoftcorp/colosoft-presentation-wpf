using Colosoft;
using Colosoft.Notifications;
using Colosoft.Presentation;
using Colosoft.Presentation.PresentationData;
using Colosoft.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfSample.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IViewLocator viewLocator;
        private readonly INotificationDispatcher notificationDispatcher;
        private readonly IDispatcher dispatcher;
        private readonly IOpenFileDialogFactory openFileDialogFactory;
        private readonly IFolderBrowserDialogFactory folderBrowserDialogFactory;
        private readonly IDialogAccessorFactory dialogAccessorFactory;
        private int testCount;
        private string text;

        public MainWindowViewModel(
            IViewLocator viewLocator,
            INotificationDispatcher notificationDispatcher,
            IDispatcher dispatcher,
            IOpenFileDialogFactory openFileDialogFactory,
            IFolderBrowserDialogFactory folderBrowserDialogFactory,
            IDialogAccessorFactory dialogAccessorFactory)
        {
            this.ShowAboutCommand = new Colosoft.Presentation.Input.AwaitableDelegateCommand(this.ShowAbout);
            this.viewLocator = viewLocator;
            this.notificationDispatcher = notificationDispatcher;
            this.dispatcher = dispatcher;
            this.openFileDialogFactory = openFileDialogFactory;
            this.folderBrowserDialogFactory = folderBrowserDialogFactory;
            this.dialogAccessorFactory = dialogAccessorFactory;
            this.TestButtonData = new ButtonData(this.Test, () => this.testCount % 2 == 0)
            {
                Gestures = "F3",
            };

            this.ResetButtonData = new ButtonData(this.Reset)
            {
                Gestures = "F2",
            };

            this.OpenFileButtonData = new ButtonData(this.OpenFile)
            {
                Gestures = "Ctrl+O",
            };
        }

        public ICommand ShowAboutCommand { get; }

        [Attach]
        public ICommandData TestButtonData { get; }

        [Attach]
        public ICommandData ResetButtonData { get; }

        [Attach]
        public ICommandData OpenFileButtonData { get; }

        public string Title => "Test";

        public string Text
        {
            get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(nameof(this.Text));
                }
            }
        }

        private async Task OpenFile(CancellationToken cancellationToken)
        {
            var dialog = await this.openFileDialogFactory.Create(cancellationToken);
            await dialog.ShowDialog(cancellationToken);

            var folderDialog = await this.folderBrowserDialogFactory.Create(cancellationToken);
            await folderDialog.ShowDialog(cancellationToken);
        }

        private Task Test(CancellationToken cancellationToken)
        {
            new System.Threading.Thread(async () =>
            {
                await this.dispatcher.Invoke(async (cancellationToken2) =>
                {
                    this.testCount++;
                    this.Text = this.testCount.ToString();
                    await this.notificationDispatcher.Dispatch("Teste".GetFormatter(), NotificationType.Information);
                },
                cancellationToken);
            }).Start();

            return Task.CompletedTask;
        }

        private Task Reset(CancellationToken cancellationToken)
        {
            this.testCount++;
            return Task.CompletedTask;
        }

        private async Task ShowAbout(CancellationToken cancellationToken)
        {
            await this.notificationDispatcher.Dispatch("Teste".GetFormatter(), NotificationType.Information, cancellationToken);
            var viewModel = new AboutViewModel(this.dialogAccessorFactory);
            this.viewLocator.ResolveView(viewModel);
            await viewModel.Show(cancellationToken);
        }
    }
}
