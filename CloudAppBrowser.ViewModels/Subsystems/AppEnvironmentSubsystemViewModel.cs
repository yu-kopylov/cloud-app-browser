using System.Collections.Generic;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class AppEnvironmentSubsystemViewModel : ISubsystemViewModel
    {
        private readonly AppEnvironment environment;

        private readonly List<ServiceViewModel> services = new List<ServiceViewModel>();

        public AppEnvironmentSubsystemViewModel(AppEnvironment environment)
        {
            this.environment = environment;
            Name = environment.Name;
            foreach (IService service in environment.Services)
            {
                if (service is DockerService)
                {
                    services.Add(new ServiceViewModel(service));
                }
            }
        }

        public string Name { get; }

        public List<ServiceViewModel> Services
        {
            get { return services; }
        }

        public void RemoveService(ServiceViewModel service)
        {
            environment.RemoveService(service.Service);
        }
    }
}