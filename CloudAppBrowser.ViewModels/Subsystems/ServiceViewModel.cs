using System;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class ServiceViewModel
    {
        public ServiceViewModel(IService service)
        {
            Service = service;
            ServiceName = service.Name;
            ServiceType = GetServiceType(service);
        }

        internal IService Service { get; }

        public string ServiceName { get; }
        public ServiceType ServiceType { get; }

        private static ServiceType GetServiceType(IService service)
        {
            if (service is DockerService)
            {
                return ServiceType.Docker;
            }
            if (service is EurekaService)
            {
                return ServiceType.Eureka;
            }
            throw new Exception("Unknown service type.");
        }
    }
}