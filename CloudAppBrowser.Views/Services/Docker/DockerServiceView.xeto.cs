using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CloudAppBrowser.ViewModels.Services.Docker;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services.Docker
{
    public class DockerServiceView : Panel
    {
        protected GridView ContainersGridView;

        protected TextBoxCell IdCell;
        protected TextBoxCell ImageCell;
        protected TextBoxCell ImageIdCell;
        protected TextBoxCell CreatedCell;
        protected TextBoxCell StateCell;

        protected DockerContainerView ContainerView;

        public DockerServiceView()
        {
            XamlReader.Load(this);

            DataContextChanged += (sender, args) =>
            {
                DockerServiceViewModel viewModel = DataContext as DockerServiceViewModel;
                if (viewModel == null)
                {
                    return;
                }
                if (viewModel.Containers == null)
                {
                    return;
                }
                viewModel.Containers.CollectionChanged += (o, eventArgs) => BindCollectionItems(viewModel.Containers);
            };

            IdCell.Binding = Binding.Property<DockerContainerViewModel, string>(c => c.Id);
            ImageCell.Binding = Binding.Property<DockerContainerViewModel, string>(c => c.Image);
            ImageIdCell.Binding = Binding.Property<DockerContainerViewModel, string>(c => c.ImageId);
            CreatedCell.Binding = Binding.Property<DockerContainerViewModel, string>(c => c.Created);
            StateCell.Binding = Binding.Property<DockerContainerViewModel, string>(c => c.State);
            ContainersGridView.SelectionChanged += ContainersGridViewOnSelectionChanged;
        }

        //todo: workaround for bug in Eto.Forms (probably related to https://github.com/picoe/Eto/issues/515)
        private void BindCollectionItems(ObservableCollection<DockerContainerViewModel> containers)
        {
            Dictionary<DockerContainerViewModel, PropertyChangedEventHandler> propertyChangedHandlers = new Dictionary<DockerContainerViewModel, PropertyChangedEventHandler>();
            foreach (DockerContainerViewModel container in containers)
            {
                if (propertyChangedHandlers.ContainsKey(container))
                {
                    return;
                }
                //todo: there is a potential memory leak here
                PropertyChangedEventHandler handler = (sender, args) => { ContainersGridView.Invalidate(); };
                container.PropertyChanged += handler;
                propertyChangedHandlers.Add(container, handler);
            }
        }

        private void ContainersGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            List<DockerContainerViewModel> containers = ContainersGridView
                .SelectedItems
                .Cast<DockerContainerViewModel>()
                .ToList();
            DockerServiceViewModel viewModel = (DockerServiceViewModel) DataContext;
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