using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class DockerSubsystemViewModel : ISubsystemViewModel
    {
        public string Name { get; set; }
        public ObservableCollection<DockerContainerViewModel> Containers { get; } = new ObservableCollection<DockerContainerViewModel>();

        private readonly DockerService service;

        public DockerSubsystemViewModel(DockerService service)
        {
            this.service = service;
            Name = service.Name;

            service.ContainersChanged += () => ViewContext.Instance.Invoke(UpdateContainerList);
            UpdateContainerList();
        }

        public void RefreshContainerList()
        {
            service.RefreshContainerList();
        }

        public void StartContainers(List<string> containerIds)
        {
            service.StartContainers(containerIds);
        }

        public void StopContainers(List<string> containerIds)
        {
            service.StopContainers(containerIds);
        }

        private void UpdateContainerList()
        {
            Containers.Clear();
            foreach (DockerContainer container in service.GetContainers().OrderBy(s => s.Image).ThenBy(s => s.Id))
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