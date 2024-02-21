using Microsoft.Extensions.DependencyInjection;
using System;

namespace WpfSample
{
    public class AppBootstrapper
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        public void Run()
        {
            var services = new ServiceCollection();
            this.ConfigureServices(services);
            this.ServiceProvider = services.BuildServiceProvider();

            Colosoft.Notifications.Notification.Dispatcher = this.ServiceProvider.GetRequiredService<Colosoft.Notifications.INotificationDispatcher>();
        }

        private void AddViews(IServiceCollection services)
        {
            var viewForType = typeof(Colosoft.Presentation.IViewFor);
            var viewForViewModelType = typeof(Colosoft.Presentation.IViewFor<>);
            var viewModelType = typeof(Colosoft.Presentation.IViewModel);

            foreach (var type in typeof(AppBootstrapper).Assembly.GetTypes())
            {
                if (!type.IsAbstract && type.IsClass)
                {
                    foreach (var i in type.GetInterfaces())
                    {
                        if (i != viewForType &&
                            ((i.IsGenericType && i.GetGenericTypeDefinition() == viewForViewModelType) ||
                            viewForType.IsAssignableFrom(i)))
                        {
                            services.AddTransient(i, type);
                        }
                    }

                    if (viewModelType.IsAssignableFrom(type))
                    {
                        services.AddScoped(type, type);
                    }
                }
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Colosoft.Presentation.Input.IRoutedCommandFactory, Colosoft.Presentation.Input.RoutedCommandFactory>();
            services.AddScoped<Colosoft.Presentation.PresentationData.ICommandDataConfigurator, Colosoft.Presentation.PresentationData.CommandDataConfigurator>();
            services.AddScoped<Colosoft.Presentation.IAttacher, Colosoft.Presentation.WpfAttacher>();
            services.AddScoped<Colosoft.Presentation.IViewLocator, Colosoft.Presentation.WpfViewLocator>();
            services.AddScoped<Colosoft.Presentation.IDialogAccessorFactory, Colosoft.Presentation.WpfDialogAccessorFactory>();
            services.AddScoped<Colosoft.Notifications.INotificationDispatcher, Colosoft.Presentation.WpfNotificationDispatcherDialog>();
            services.AddScoped<Colosoft.Threading.IDispatcher, Colosoft.Presentation.Threading.WpfPresentationDispatcher>();
            services.AddScoped<Colosoft.Presentation.IOpenFileDialogFactory, Colosoft.Presentation.WpfOpenFileDialogFactory>();
            services.AddScoped<Colosoft.Presentation.IFolderBrowserDialogFactory, Colosoft.Presentation.WpfFolderBrowserDialogFactory>();
            this.AddViews(services);
        }
    }
}
