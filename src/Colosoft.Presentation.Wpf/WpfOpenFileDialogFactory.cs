using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfOpenFileDialogFactory : IOpenFileDialogFactory
    {
        public Task<IOpenFileDialog> Create(CancellationToken cancellationToken) =>
            Task.FromResult<IOpenFileDialog>(new WpfOpenDialog());
    }
}
