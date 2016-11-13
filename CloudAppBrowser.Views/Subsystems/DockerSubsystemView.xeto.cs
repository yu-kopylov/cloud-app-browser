using CloudAppBrowser.ViewModels.Subsystems;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class DockerSubsystemView : Panel
    {
        protected TextBoxCell IdCell;
        protected TextBoxCell ImageCell;
        protected TextBoxCell ImageIdCell;
        protected TextBoxCell CreatedCell;
        protected TextBoxCell StateCell;

        public DockerSubsystemView()
        {
            XamlReader.Load(this);
            IdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Id);
            ImageCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Image);
            ImageIdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.ImageId);
            CreatedCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Created);
            StateCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.State);
        }
    }
}