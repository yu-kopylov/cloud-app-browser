using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class DockerContainerView : Panel
    {
        public DockerContainerView()
        {
            XamlReader.Load(this);
        }
    }
}