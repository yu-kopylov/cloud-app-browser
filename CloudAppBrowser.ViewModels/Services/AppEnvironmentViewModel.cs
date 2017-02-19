using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.Core.Services.Eureka;
using CloudAppBrowser.ViewModels.Services.Docker;
using CloudAppBrowser.ViewModels.Services.Eureka;
using CloudAppBrowser.ViewModels.Settings;

namespace CloudAppBrowser.ViewModels.Services
{
    public class AppEnvironmentViewModel : IModuleViewModel
    {
        private readonly ObservableCollectionMapper<IService, IServiceViewModel> serviceMapper;
        private readonly AppBrowserViewModel appBrowserViewModel;
        private readonly AppEnvironment environment;

        public AppEnvironmentViewModel(AppBrowserViewModel appBrowserViewModel, AppEnvironment environment)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            this.environment = environment;

            serviceMapper = new ObservableCollectionMapper<IService, IServiceViewModel>(
                CreateServiceViewModel,
                viewModel => viewModel.Service,
                (service, viewModel) => viewModel.Update(),
                (viewModel1, viewModel2) => string.CompareOrdinal(viewModel1.ModuleName, viewModel2.ModuleName)
            );

            environment.ServiceListChanged += Update;
            Services.CollectionChanged += (sender, args) => SubModulesChanged?.Invoke();
            Update();
        }

        public string ModuleName { get; private set; }

        public IEnumerable<IModuleViewModel> GetSubModules()
        {
            return Services;
        }

        public event Action SubModulesChanged;

        public AppEnvironment Environment
        {
            get { return environment; }
        }

        public ObservableCollection<IServiceViewModel> Services { get; } = new ObservableCollection<IServiceViewModel>();

        public void AddDockerService()
        {
            DockerSettingsViewModel settings = new DockerSettingsViewModel();
            settings.ServiceName = "Docker";
            settings.MachineName = "default";

            if (!appBrowserViewModel.ViewContext.ShowDialog(settings))
            {
                return;
            }

            DockerService service = new DockerService();
            service.Name = settings.ServiceName;
            service.MachineName = settings.MachineName;

            environment.AddService(service);

            IServiceViewModel serviceViewModel = Services.FirstOrDefault(s => s.Service == service);
            appBrowserViewModel.MainForm.ModulesTree.SelectedModule = serviceViewModel;
        }

        public void AddEurekaService()
        {
            EurekaSettingsViewModel settings = new EurekaSettingsViewModel();
            settings.ServiceName = "Eureka";
            settings.Url = "http://<host>:<port>/eureka/apps";

            if (!appBrowserViewModel.ViewContext.ShowDialog(settings))
            {
                return;
            }

            EurekaService service = new EurekaService();
            service.Name = settings.ServiceName;
            service.Url = settings.Url;

            environment.AddService(service);

            IServiceViewModel serviceViewModel = Services.FirstOrDefault(s => s.Service == service);
            appBrowserViewModel.MainForm.ModulesTree.SelectedModule = serviceViewModel;
        }

        public void RemoveService(IServiceViewModel service)
        {
            environment.RemoveService(service.Service);
        }

        public void Update()
        {
            ModuleName = environment.Name;
            serviceMapper.UpdateCollection(environment.Services, Services);
        }

        private IServiceViewModel CreateServiceViewModel(IService service)
        {
            if (service is DockerService)
            {
                return new DockerServiceViewModel(appBrowserViewModel, (DockerService) service);
            }
            if (service is EurekaService)
            {
                return new EurekaServiceViewModel(appBrowserViewModel, (EurekaService) service);
            }
            throw new InvalidOperationException($"Cannot create view-model for service of type '{service?.GetType().Name}'.");
        }
    }
}