using Colosoft.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfFolderBrowserDialogFactory : IFolderBrowserDialogFactory
    {
        private readonly IDispatcher dispatcher;

        public WpfFolderBrowserDialogFactory(Threading.IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<IFolderBrowserDialog> Create(CancellationToken cancellationToken) =>
            Task.FromResult<IFolderBrowserDialog>(new WpfFolderBrowserDialog(this.dispatcher));
    }
}
