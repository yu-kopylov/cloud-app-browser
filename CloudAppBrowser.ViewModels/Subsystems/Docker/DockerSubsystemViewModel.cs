using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Subsystems.Docker
{
    public class DockerSubsystemViewModel : ISubsystemViewModel, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public ObservableCollection<DockerContainerViewModel> Containers { get; } = new ObservableCollection<DockerContainerViewModel>();
        private DockerContainerViewModel selectedContainer;

        private readonly DockerService service;

        public DockerSubsystemViewModel(DockerService service)
        {
            this.service = service;
            Name = service.Name;

            service.ContainersChanged += () => ViewContext.Instance.Invoke(UpdateContainerList);
            service.LogChanged += UpdateLog;

            UpdateContainerList();
        }

        public DockerContainerViewModel SelectedContainer
        {
            get { return selectedContainer; }
            set
            {
                selectedContainer = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateLog(string containerId, string log)
        {
            ViewContext.Instance.Invoke(() =>
            {
                DockerContainerViewModel containerViewModel = Containers.FirstOrDefault(c => c.Id == containerId);
                if (containerViewModel != null)
                {
                    containerViewModel.Log = log;
                }
            });
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
                DockerContainerViewModel containerViewModel = new DockerContainerViewModel(service, container);
                containerViewModel.Log = service.GetLog(container.Id);
                Containers.Add(containerViewModel);
            }
        }
    }
}