using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class AppEnvironmentSubsystemView : Panel
    {
        public AppEnvironmentSubsystemView()
        {
            XamlReader.Load(this);
        }
    }
}
