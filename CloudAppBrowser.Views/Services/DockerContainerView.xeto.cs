using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services
{
    public class DockerContainerView : Panel
    {
        public DockerContainerView()
        {
            XamlReader.Load(this);
        }
    }
}