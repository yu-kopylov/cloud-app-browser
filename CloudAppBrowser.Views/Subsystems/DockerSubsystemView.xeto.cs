using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Subsystems;
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

        public DockerSubsystemView()
        {
            XamlReader.Load(this);
            IdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Id);
            ImageCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Image);
            ImageIdCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.ImageId);
            CreatedCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.Created);
            StateCell.Binding = Binding.Delegate<DockerContainerViewModel, string>(c => c.State);
        }

        protected void RefreshContainerList(object sender, EventArgs e)
        {
            DockerSubsystemViewModel viewModel = (DockerSubsystemViewModel)DataContext;
            viewModel.RefreshContainerList();
        }

        protected void StartContainers(object sender, EventArgs e)
        {
            List<string> containerIds = ContainersGridView
                .SelectedItems
                .Cast<DockerContainerViewModel>()
                .Select(c => c.Id)
                .ToList();
            if (!containerIds.Any())
            {
                MessageBox.Show(this.Parent, "No containers are selected.");
                return;
            }
            DockerSubsystemViewModel viewModel = (DockerSubsystemViewModel) DataContext;
            viewModel.StartContainers(containerIds);
        }

        protected void StopContainers(object sender, EventArgs e)
        {
            List<string> containerIds = ContainersGridView
                .SelectedItems
                .Cast<DockerContainerViewModel>()
                .Select(c => c.Id)
                .ToList();
            if (!containerIds.Any())
            {
                MessageBox.Show(this.Parent, "No containers are selected.");
                return;
            }
            DockerSubsystemViewModel viewModel = (DockerSubsystemViewModel)DataContext;
            viewModel.StopContainers(containerIds);
        }
    }
}