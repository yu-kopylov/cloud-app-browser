using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Subsystems.Docker
{
    public class DockerSubsystemViewModel : ISubsystemViewModel, INotifyPropertyChanged, IDisposable
    {
        public string Name { get; set; }
        public ObservableCollection<DockerContainerViewModel> Containers { get; } = new ObservableCollection<DockerContainerViewModel>();
        public ObservableCollection<DockerContainerViewModel> SelectedContainers { get; } = new ObservableCollection<DockerContainerViewModel>();
        private DockerContainerViewModel selectedContainer;

        public BasicCommand ConnectCommand { get; }
        public BasicCommand DisconnectCommand { get; }
        public BasicCommand RefreshCommand { get; }
        public BasicCommand StartContainersCommand { get; }
        public BasicCommand StopContainersCommand { get; }

        private readonly DockerService service;

        private readonly Timer timer;

        private readonly HashSet<string> updatedLogs = new HashSet<string>();
        private readonly object updatedLogsMonitor = new object();

        public DockerSubsystemViewModel(DockerService service)
        {
            this.service = service;
            Name = service.Name;

            ConnectCommand = new BasicCommand(() => !service.Connected, o => service.Connect());
            DisconnectCommand = new BasicCommand(() => service.Connected, o => service.Disconnect());
            RefreshCommand = new BasicCommand(() => service.Connected, o => service.RefreshContainerList());
            StartContainersCommand = new BasicCommand(() => service.Connected && SelectedContainers.Count > 0, o => StartContainers());
            StopContainersCommand = new BasicCommand(() => service.Connected && SelectedContainers.Count > 0, o => StopContainers());

            SelectedContainers.CollectionChanged += (sender, args) =>
            {
                StartContainersCommand.UpdateState();
                StopContainersCommand.UpdateState();
            };

            service.ContainersChanged += () => ViewContext.Instance.Invoke(UpdateContainerList);
            service.LogChanged += UpdateLog;

            UpdateContainerList();

            timer = new Timer(TimerCallback, null, 0, 500);
        }

        //todo: don't forget to call dispose
        public void Dispose()
        {
            timer.Dispose();
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

        private void TimerCallback(object state)
        {
            string containerId = FetchUpdatedLogContainerId();
            while (containerId != null)
            {
                string log = service.GetLog(containerId);

                if (log != null)
                {
                    string localContainerId = containerId;
                    ViewContext.Instance.Invoke(() =>
                    {
                        DockerContainerViewModel containerViewModel = Containers.FirstOrDefault(c => c.Id == localContainerId);
                        if (containerViewModel != null)
                        {
                            containerViewModel.Log = log;
                        }
                    });
                }

                containerId = FetchUpdatedLogContainerId();
            }
        }

        private string FetchUpdatedLogContainerId()
        {
            lock (updatedLogsMonitor)
            {
                if (updatedLogs.Count == 0)
                {
                    return null;
                }
                string containerId = updatedLogs.First();
                updatedLogs.Remove(containerId);
                return containerId;
            }
        }

        private void UpdateLog(string containerId)
        {
            lock (updatedLogsMonitor)
            {
                updatedLogs.Add(containerId);
            }
        }

        public async Task StartContainers()
        {
            List<string> containerIds = SelectedContainers.Select(c => c.Id).ToList();
            await service.StartContainers(containerIds);
        }

        public async Task StopContainers()
        {
            List<string> containerIds = SelectedContainers.Select(c => c.Id).ToList();
            await service.StopContainers(containerIds);
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

            RefreshCommand.UpdateState();
            ConnectCommand.UpdateState();
            DisconnectCommand.UpdateState();
            StartContainersCommand.UpdateState();
            StopContainersCommand.UpdateState();
        }
    }
}