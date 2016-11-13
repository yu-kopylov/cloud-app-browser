using System.Collections.Generic;
using System.Globalization;
using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class DockerSubsystemViewModel : ISubsystemViewModel
    {
        public string Name { get; set; }
        public List<DockerContainerViewModel> Containers { get; } = new List<DockerContainerViewModel>();

        private readonly DockerService service;

        public DockerSubsystemViewModel(DockerService service)
        {
            this.service = service;
            Name = service.Name;
            UpdateContainerList();
        }

        private void UpdateContainerList()
        {
            Containers.Clear();
            foreach (DockerContainer container in service.Containers)
            {
                DockerContainerViewModel containerViewModel = new DockerContainerViewModel(container);
                Containers.Add(containerViewModel);
            }
        }
    }

    public class DockerContainerViewModel
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string ImageId { get; set; }
        public string Created { get; set; }
        public string State { get; set; }

        public DockerContainerViewModel(DockerContainer container)
        {
            Id = container.Id;
            Image = container.Image;
            ImageId = container.ImageId;
            Created = container.Created.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            State = container.State;
        }
    }
}