using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Services.Docker;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services.Docker
{
    public class DockerImageListView : Panel
    {
        protected GridView ImagesGridView;

        protected TextBoxCell IdCell;
        protected TextBoxCell RepoTagsCell;
        protected TextBoxCell RepoDigestsCell;
        protected TextBoxCell CreatedCell;
        protected TextBoxCell SizeCell;
        protected TextBoxCell VirtualSizeCell;

        private readonly ObservableCollectionWatcher<DockerImageViewModel> containersCollectionWatcher;

        public DockerImageListView()
        {
            XamlReader.Load(this);

            //todo: workaround for bug in Eto.Forms (probably related to https://github.com/picoe/Eto/issues/515)
            containersCollectionWatcher = new ObservableCollectionWatcher<DockerImageViewModel>(ImagesGridView.Invalidate);

            DataContextChanged += (sender, args) =>
            {
                DockerImageListViewModel viewModel = DataContext as DockerImageListViewModel;
                containersCollectionWatcher.SetCollection(viewModel?.Images);
            };

            IdCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.Id);
            RepoTagsCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.RepoTagsAsText);
            RepoDigestsCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.RepoDigestsAsText);
            CreatedCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.CreatedAsText);
            SizeCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.SizeAsText);
            VirtualSizeCell.Binding = Binding.Property<DockerImageViewModel, string>(c => c.VirtualSizeAsText);
            ImagesGridView.SelectionChanged += ImagesGridViewOnSelectionChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                containersCollectionWatcher.SetCollection(null);
            }
            base.Dispose(disposing);
        }

        private void ImagesGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            List<DockerImageViewModel> images = ImagesGridView
                .SelectedItems
                .Cast<DockerImageViewModel>()
                .ToList();
            DockerImageListViewModel viewModel = (DockerImageListViewModel) DataContext;
            viewModel.SelectedImages.Clear();
            foreach (DockerImageViewModel image in images)
            {
                viewModel.SelectedImages.Add(image);
            }
            viewModel.SelectedImage = (images.Count == 1) ? images[0] : null;
        }
    }
}