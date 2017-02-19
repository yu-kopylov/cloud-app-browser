using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Services.Docker
{
    public class DockerContainerViewModel : INotifyPropertyChanged
    {
        public string Id { get; private set; }
        public string Image { get; private set; }
        public string ImageId { get; private set; }
        public string Created { get; private set; }
        public string State { get; private set; }
        public string PortsAsText { get; private set; }

        public BasicCommand EnableLogsCommand { get; }

        private readonly DockerService service;

        private string log;

        public DockerContainerViewModel(DockerService service, DockerContainer container)
        {
            this.service = service;
            Container = container;

            Update();

            EnableLogsCommand = new BasicCommand(() => service.Connected, o => service.EnableLogs(Id));
        }

        public DockerContainer Container { get; }

        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            Id = Container.Id;
            Image = Container.Image;
            ImageId = Container.ImageId;
            Created = Container.Created.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            State = Container.State;
            PortsAsText = string.Join(", ", Container.Ports.Select(p => $"{p.PrivatePort}->{p.PublicPort} ({p.PortType})"));
        }
    }
}