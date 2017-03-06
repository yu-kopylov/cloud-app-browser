using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Services.Docker
{
    public class DockerImageListViewModel : IModuleViewModel, INotifyPropertyChanged
    {
        private readonly ObservableCollectionMapper<DockerImage, DockerImageViewModel> imagesMapper;

        public BasicCommand RefreshCommand { get; }

        public ObservableCollection<DockerImageViewModel> Images { get; } = new ObservableCollection<DockerImageViewModel>();
        public ObservableCollection<DockerImageViewModel> SelectedImages { get; } = new ObservableCollection<DockerImageViewModel>();
        private DockerImageViewModel selectedImage;

        private readonly AppBrowserViewModel appBrowserViewModel;
        private readonly DockerService service;

        public DockerImageListViewModel(AppBrowserViewModel appBrowserViewModel, DockerService service)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            this.service = service;
            ModuleName = "Images";

            imagesMapper = new ObservableCollectionMapper<DockerImage, DockerImageViewModel>(
                image => new DockerImageViewModel(service, image),
                viewModel => viewModel.Image,
                (image, viewModel) => viewModel.Update(),
                (viewModel1, viewModel2) =>
                {
                    int r = string.CompareOrdinal(viewModel1.RepoTagsAsText, viewModel2.RepoTagsAsText);
                    if (r == 0)
                    {
                        r = string.CompareOrdinal(viewModel1.RepoDigestsAsText, viewModel2.RepoDigestsAsText);
                    }
                    if (r == 0)
                    {
                        r = string.CompareOrdinal(viewModel1.Id, viewModel2.Id);
                    }
                    return r;
                }
            );

            RefreshCommand = new BasicCommand(() => service.Connected, o => service.Refresh());

            service.StageChanged += () => appBrowserViewModel.ViewContext.Invoke(Update);

            Update();
        }

        public string ModuleName { get; }

        public IEnumerable<IModuleViewModel> GetSubModules()
        {
            return Enumerable.Empty<IModuleViewModel>();
        }

        public event Action SubModulesChanged;

        public void Update()
        {
            imagesMapper.UpdateCollection(service.GetImages(), Images);

            RefreshCommand.UpdateState();
        }

        public DockerImageViewModel SelectedImage
        {
            get { return selectedImage; }
            set
            {
                selectedImage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}