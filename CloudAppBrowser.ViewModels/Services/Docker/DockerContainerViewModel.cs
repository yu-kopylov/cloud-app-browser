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
        public DockerService Service { get; }
        public DockerContainer Container { get; }

        public string Id { get; }
        private string image;
        private string imageId;
        private string created;
        private string state;
        private string portsAsText;

        private string log;

        public BasicCommand EnableLogsCommand { get; }

        public DockerContainerViewModel(DockerService service, DockerContainer container)
        {
            Service = service;
            Container = container;
            Id = container.Id;

            Update();

            EnableLogsCommand = new BasicCommand(() => Service.Connected, o => service.EnableLogs(Id));
        }

        public string Image
        {
            get { return image; }
            private set
            {
                if (image == value)
                {
                    return;
                }
                image = value;
                OnPropertyChanged();
            }
        }

        public string ImageId
        {
            get { return imageId; }
            private set
            {
                if (imageId == value)
                {
                    return;
                }
                imageId = value;
                OnPropertyChanged();
            }
        }

        public string Created
        {
            get { return created; }
            private set
            {
                if (created == value)
                {
                    return;
                }
                created = value;
                OnPropertyChanged();
            }
        }

        public string State
        {
            get { return state; }
            private set
            {
                if (state == value)
                {
                    return;
                }
                state = value;
                OnPropertyChanged();
            }
        }

        public string PortsAsText
        {
            get { return portsAsText; }
            private set
            {
                if (portsAsText == value)
                {
                    return;
                }
                portsAsText = value;
                OnPropertyChanged();
            }
        }

        public string Log
        {
            get { return log; }
            set
            {
                if (log == value)
                {
                    return;
                }
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
            Image = Container.Image;
            ImageId = Container.ImageId;
            Created = Container.Created.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            State = Container.State;
            PortsAsText = string.Join(", ", Container.Ports.Select(p => $"{p.PrivatePort}->{p.PublicPort} ({p.PortType})"));
        }
    }
}