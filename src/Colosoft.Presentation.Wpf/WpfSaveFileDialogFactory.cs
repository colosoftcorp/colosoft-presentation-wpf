using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation
{
    public class WpfSaveFileDialogFactory : ISaveFileDialogFactory
    {
        public Task<ISaveFileDialog> Create(CancellationToken cancellationToken) =>
            Task.FromResult<ISaveFileDialog>(new WpfSaveDialog());
    }
}
