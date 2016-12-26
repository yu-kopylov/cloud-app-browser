using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class EurekaSubsystemView : Panel
    {
        public EurekaSubsystemView()
        {
            XamlReader.Load(this);
        }
    }
}