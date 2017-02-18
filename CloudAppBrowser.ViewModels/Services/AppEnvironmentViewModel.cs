using System.Collections.Generic;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.Core.Services.Eureka;
using CloudAppBrowser.ViewModels.Settings;

namespace CloudAppBrowser.ViewModels.Services
{
    public class AppEnvironmentViewModel : ISubsystemViewModel
    {
        private readonly MainFormViewModel mainFormViewModel;
        private readonly AppEnvironment environment;

        private readonly List<ServiceViewModel> services = new List<ServiceViewModel>();

        public AppEnvironmentViewModel(MainFormViewModel mainFormViewModel, AppEnvironment environment)
        {
            this.mainFormViewModel = mainFormViewModel;
            this.environment = environment;
            Name = environment.Name;
            foreach (IService service in environment.Services)
            {
                services.Add(new ServiceViewModel(service));
            }
        }

        public string Name { get; }

        public List<ServiceViewModel> Services
        {
            get { return services; }
        }

        public void AddDockerService()
        {
            DockerSettingsViewModel settings = new DockerSettingsViewModel();
            settings.Name = "Docker";
            settings.MachineName = "default";

            if (!ViewContext.Instance.ShowDialog(settings))
            {
                return;
            }

            DockerService service = new DockerService();
            service.Name = settings.Name;
            service.MachineName = settings.MachineName;

            environment.AddService(service);
            mainFormViewModel.SelectService(service);
        }

        public void AddEurekaService()
        {
            EurekaSettingsViewModel settings = new EurekaSettingsViewModel();
            settings.Name = "Eureka";
            settings.Url = "http://<host>:<port>/eureka/apps";

            if (!ViewContext.Instance.ShowDialog(settings))
            {
                return;
            }

            EurekaService service = new EurekaService();
            service.Name = settings.Name;
            service.Url = settings.Url;

            environment.AddService(service);
            mainFormViewModel.SelectService(service);
        }

        public void RemoveService(ServiceViewModel service)
        {
            environment.RemoveService(service.Service);
        }
    }
}