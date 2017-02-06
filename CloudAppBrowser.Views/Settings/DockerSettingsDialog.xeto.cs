using System;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Settings
{
    public class DockerSettingsDialog : Dialog<bool>
    {
        public DockerSettingsDialog()
        {
            XamlReader.Load(this);
        }

        protected void DefaultButton_Click(object sender, EventArgs e)
        {
            Close(true);
        }

        protected void AbortButton_Click(object sender, EventArgs e)
        {
            Close(false);
        }
    }
}