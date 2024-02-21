using Colosoft.Presentation;
using System.Threading;
using System.Threading.Tasks;

namespace WpfSample.ViewModels
{
    public class AboutViewModel : DialogViewModel
    {
        public AboutViewModel(IDialogAccessorFactory dialogAccessorFactory)
            : base(dialogAccessorFactory)
        {
            this.DialogAccessor.Closing += this.DialogAccessor_Closing;
        }

        private Task DialogAccessor_Closing(object sender, System.ComponentModel.CancelEventArgs e, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string? title;

        public string? Title
        {
            get => this.title;
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Task Show(CancellationToken cancellationToken)
        {
            return this.DialogAccessor.ShowDialog(cancellationToken);
        }
    }
}
