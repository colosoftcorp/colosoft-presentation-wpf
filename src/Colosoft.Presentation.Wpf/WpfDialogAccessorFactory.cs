using Colosoft.Threading;

namespace Colosoft.Presentation
{
    public class WpfDialogAccessorFactory : IDialogAccessorFactory
    {
        private readonly IViewLocator viewLocator;
        private readonly IDispatcher dispatcher;

        public WpfDialogAccessorFactory(
            IViewLocator viewLocator,
            IDispatcher dispatcher)
        {
            this.viewLocator = viewLocator;
            this.dispatcher = dispatcher;
        }

        public IDialogAccessor Create(object viewModel)
        {
            return new WpfDialogAccessor(viewModel, this.dispatcher, this.viewLocator);
        }
    }
}
