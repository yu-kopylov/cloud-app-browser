using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services.Docker
{
    public class DockerContainerView : Panel
    {
        public DockerContainerView()
        {
            XamlReader.Load(this);
        }
    }
}