using System;

namespace Colosoft.Presentation
{
    public class WpfViewLocator : DefaultViewLocator
    {
        private readonly IAttacher attacher;

        public WpfViewLocator(
            IServiceProvider serviceProvider,
            IAttacher attacher)
            : base(serviceProvider)
        {
            this.attacher = attacher;
        }

        public override IViewFor ResolveView<T>(T viewModel)
        {
            var view = base.ResolveView(viewModel);

            if (view is System.Windows.FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = viewModel;
            }

            if (view is IViewFor viewFor)
            {
                viewFor.ViewModel = viewModel;
            }

            this.attacher.AttachViewModel(view, viewModel);

            return view;
        }

        public override IViewFor ResolveView(object viewModel, Type viewModelType)
        {
            if (viewModelType == null)
            {
                throw new ArgumentNullException(nameof(viewModelType));
            }

            var view = base.ResolveView(viewModel, viewModelType);

            if (view is System.Windows.FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = viewModel;
            }

            if (view is IViewFor viewFor)
            {
                viewFor.ViewModel = viewModel;
            }

            this.attacher.AttachViewModel(view, viewModel);

            return view;
        }
    }
}
