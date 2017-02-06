﻿using System.Collections.Generic;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.Core.Services.Eureka;
using CloudAppBrowser.ViewModels.Settings;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class AppEnvironmentSubsystemViewModel : ISubsystemViewModel
    {
        private readonly MainFormViewModel mainFormViewModel;
        private readonly AppEnvironment environment;

        private readonly List<ServiceViewModel> services = new List<ServiceViewModel>();

        public AppEnvironmentSubsystemViewModel(MainFormViewModel mainFormViewModel, AppEnvironment environment)
        {
            this.mainFormViewModel = mainFormViewModel;
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
            service.MachineName = "default";

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