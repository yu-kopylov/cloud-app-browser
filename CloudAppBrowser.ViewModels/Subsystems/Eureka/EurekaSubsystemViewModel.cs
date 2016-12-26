using CloudAppBrowser.Core.Services.Eureka;

namespace CloudAppBrowser.ViewModels.Subsystems.Eureka
{
    public class EurekaSubsystemViewModel : ISubsystemViewModel
    {
        private readonly EurekaService service;

        public string Name { get; set; }

        public EurekaSubsystemViewModel(EurekaService service)
        {
            this.service = service;
            Name = service.Name;
        }
    }
}