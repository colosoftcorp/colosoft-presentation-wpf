using Colosoft.Threading;

namespace Colosoft.Presentation.Threading
{
    public class WpfPresentationDispatcherManager : IDispatcherManager
    {
        public IDispatcher FromThread(System.Threading.Thread thread)
        {
            var dispatcher = System.Windows.Threading.Dispatcher.FromThread(thread);

            return dispatcher == null ? null : (IDispatcher)new WpfPresentationDispatcher(dispatcher);
        }

        public void DoEvents()
        {
            DispatcherHelper.DoEvents();
        }
    }
}
