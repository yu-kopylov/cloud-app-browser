using System;
using System.Collections.Generic;
using System.IO;
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

        public void AddDockerService()
        {
            DockerService service = new DockerService();

            service.Name = "Docker";
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            service.CertificateFile = Path.Combine(userProfile, @".docker\machine\machines\default\cert.pem");
            service.CertificateKeyFile = Path.Combine(userProfile, @".docker\machine\machines\default\key.pem");
            service.Url = "http://192.168.99.100:2376";

            environment.AddService(service);
        }

        public void RemoveService(ServiceViewModel service)
        {
            environment.RemoveService(service.Service);
        }
    }
}