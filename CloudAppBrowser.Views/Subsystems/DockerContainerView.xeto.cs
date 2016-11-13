using System;
using CloudAppBrowser.ViewModels.Subsystems;
using CloudAppBrowser.ViewModels.Subsystems.Docker;
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

        protected void EnableLogs(object sender, EventArgs e)
        {
            DockerContainerViewModel viewModel = (DockerContainerViewModel) DataContext;
            viewModel.EnableLogs();
        }
    }
}