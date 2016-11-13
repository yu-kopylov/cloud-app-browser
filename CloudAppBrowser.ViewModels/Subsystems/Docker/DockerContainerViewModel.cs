using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Subsystems.Docker
{
    public class DockerContainerViewModel : INotifyPropertyChanged
    {
        public string Id { get; }
        public string Image { get; }
        public string ImageId { get; }
        public string Created { get; }
        public string State { get; }

        private readonly DockerService service;

        private string log;

        public DockerContainerViewModel(DockerService service, DockerContainer container)
        {
            this.service = service;
            Id = container.Id;
            Image = container.Image;
            ImageId = container.ImageId;
            Created = container.Created.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            State = container.State;
        }

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

        public void EnableLogs()
        {
            service.EnableLogs(Id);
        }
    }
}