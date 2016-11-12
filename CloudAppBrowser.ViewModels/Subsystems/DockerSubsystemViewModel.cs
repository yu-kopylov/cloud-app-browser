using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class DockerSubsystemViewModel : ISubsystemViewModel
    {
        private readonly DockerService service;

        public DockerSubsystemViewModel(DockerService service)
        {
            this.service = service;
            Name = service.Name;
        }

        public string Name { get; set; }
    }
}