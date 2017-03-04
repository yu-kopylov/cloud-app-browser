using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services.Docker
{
    public class DockerServiceView : Panel
    {
        public DockerServiceView()
        {
            XamlReader.Load(this);
        }
    }
}