using Colosoft.Presentation;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfSample
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new AppBootstrapper();
            bootstrapper.Run();
            var serviceProvider = bootstrapper.ServiceProvider!;
            var attacher = (WpfAttacher)serviceProvider.GetRequiredService<IAttacher>();

            var viewLocator = serviceProvider.GetRequiredService<IViewLocator>();
            var viewModel = serviceProvider.GetRequiredService<ViewModels.MainWindowViewModel>();
            var view = viewLocator.ResolveDialog(viewModel);

            attacher.Initialize(view as UIElement);
            await view.ShowDialog(default);
        }
    }
}
