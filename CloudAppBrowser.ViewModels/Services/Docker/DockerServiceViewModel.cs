using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Services.Docker
{
    public class DockerServiceViewModel : IServiceViewModel, INotifyPropertyChanged
    {
        public BasicCommand ConnectCommand { get; }
        public BasicCommand DisconnectCommand { get; }
        public BasicCommand RefreshCommand { get; }

        private readonly AppBrowserViewModel appBrowserViewModel;
        private readonly DockerService service;

        private readonly DockerImageListViewModel imageList;
        private readonly DockerContainerListViewModel containerList;

        public DockerServiceViewModel(AppBrowserViewModel appBrowserViewModel, DockerService service)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            this.service = service;
            ModuleName = service.Name;

            imageList = new DockerImageListViewModel(appBrowserViewModel, service);
            containerList = new DockerContainerListViewModel(appBrowserViewModel, service);

            ConnectCommand = new BasicCommand(() => !service.Connected, o => service.Connect());
            DisconnectCommand = new BasicCommand(() => service.Connected, o => service.Disconnect());
            RefreshCommand = new BasicCommand(() => service.Connected, o => service.Refresh());

            service.StageChanged += () => appBrowserViewModel.ViewContext.Invoke(Update);

            Update();
        }

        public string ModuleName { get; private set; }

        public IEnumerable<IModuleViewModel> GetSubModules()
        {
            return new List<IModuleViewModel> {imageList, containerList};
        }

        public event Action SubModulesChanged;

        public IService Service
        {
            get { return service; }
        }

        public ServiceType ServiceType
        {
            get { return ServiceType.Docker; }
        }

        public void Update()
        {
            RefreshCommand.UpdateState();
            ConnectCommand.UpdateState();
            DisconnectCommand.UpdateState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}