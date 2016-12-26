using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Subsystems.Docker;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class DockerSubsystemView : Panel
    {
        protected GridView ContainersGridView;

        protected TextBoxCell IdCell;
        protected TextBoxCell ImageCell;
        protected TextBoxCell ImageIdCell;
        protected TextBoxCell CreatedCell;
        protected TextBoxCell StateCell;

        protected DockerContainerView ContainerView;

        public DockerSubsystemView()
        {
            XamlReader.Load(this);
            IdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Id);
            ImageCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Image);
            ImageIdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.ImageId);
            CreatedCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Created);
            StateCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.State);
            ContainersGridView.SelectionChanged += ContainersGridViewOnSelectionChanged;
        }

        private void ContainersGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            List<DockerContainerViewModel> containers = ContainersGridView
                .SelectedItems
                .Cast<DockerContainerViewModel>()
                .ToList();
            DockerSubsystemViewModel viewModel = (DockerSubsystemViewModel) DataContext;
            viewModel.SelectedContainers.Clear();
            foreach (DockerContainerViewModel containerModel in containers)
            {
                viewModel.SelectedContainers.Add(containerModel);
            }
            DockerContainerViewModel container = (containers.Count == 1) ? containers[0] : null;
            viewModel.SelectedContainer = container;
            ContainerView.DataContext = viewModel.SelectedContainer;
        }
    }
}