using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Services.Docker
{
    public class DockerImageViewModel : INotifyPropertyChanged
    {
        public DockerService Service { get; }
        public DockerImage Image { get; }

        public string Id { get; }

        private string createdAsText;
        private string sizeAsText;
        private string virtualSizeAsText;
        private string repoTagsAsText;

        public DockerImageViewModel(DockerService service, DockerImage image)
        {
            Service = service;
            Image = image;
            Id = image.Id;

            Update();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CreatedAsText
        {
            get { return createdAsText; }
            set
            {
                createdAsText = value;
                OnPropertyChanged();
            }
        }

        public string SizeAsText
        {
            get { return sizeAsText; }
            set
            {
                sizeAsText = value;
                OnPropertyChanged();
            }
        }

        public string VirtualSizeAsText
        {
            get { return virtualSizeAsText; }
            set
            {
                virtualSizeAsText = value;
                OnPropertyChanged();
            }
        }

        public string RepoTagsAsText
        {
            get { return repoTagsAsText; }
            set
            {
                repoTagsAsText = value;
                OnPropertyChanged();
            }
        }

        public void Update()
        {
            CreatedAsText = Image.Created.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            SizeAsText = FormatSize(Image.Size);
            VirtualSizeAsText = FormatSize(Image.VirtualSize);
            RepoTagsAsText = string.Join(", ", Image.RepoTags.OrderBy(t => t));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string FormatSize(long size)
        {
            const long kb = 1024;
            const long mb = kb * kb;

            if (size < 16 * kb)
            {
                return $"{size}B";
            }
            if (size < 16 * mb)
            {
                return $"{size / kb}KB";
            }
            return $"{size / mb}MB";
        }
    }
}