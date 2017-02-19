using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Eureka;

namespace CloudAppBrowser.ViewModels.Services.Eureka
{
    public class EurekaServiceViewModel : IServiceViewModel
    {
        private readonly ObservableCollectionMapper<Tuple<EurekaApplication, EurekaApplicationInstance>, EurekaApplicationViewModel> appMapper;

        private readonly AppBrowserViewModel appBrowserViewModel;
        private readonly EurekaService service;

        public ObservableCollection<EurekaApplicationViewModel> Applications { get; } = new ObservableCollection<EurekaApplicationViewModel>();

        public ObservableCollection<EurekaApplicationViewModel> SelectedApplications { get; } = new ObservableCollection<EurekaApplicationViewModel>();

        public BasicCommand ConnectCommand { get; }
        public BasicCommand DisconnectCommand { get; }
        public BasicCommand RefreshCommand { get; }
        public BasicCommand DeregisterApplicationsCommand { get; }

        public EurekaServiceViewModel(AppBrowserViewModel appBrowserViewModel, EurekaService service)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            this.service = service;

            appMapper = new ObservableCollectionMapper<Tuple<EurekaApplication, EurekaApplicationInstance>, EurekaApplicationViewModel>(
                tuple => new EurekaApplicationViewModel(tuple.Item1, tuple.Item2),
                viewModel => Tuple.Create(viewModel.Application, viewModel.Instance),
                (tuple, viewModel) => viewModel.Update(),
                (viewModel1, viewModel2) =>
                {
                    int r = string.CompareOrdinal(viewModel1.AppName, viewModel2.AppName);
                    if (r == 0)
                    {
                        r = string.CompareOrdinal(viewModel1.HostName, viewModel2.HostName);
                    }
                    if (r == 0)
                    {
                        r = string.CompareOrdinal(viewModel1.InstanceId, viewModel2.InstanceId);
                    }
                    return r;
                }
            );

            ConnectCommand = new BasicCommand(() => !service.Connected, o => service.Connect());
            DisconnectCommand = new BasicCommand(() => service.Connected, o => service.Disconnect());
            RefreshCommand = new BasicCommand(() => service.Connected, o => service.RefreshApplications());
            DeregisterApplicationsCommand = new BasicCommand(() => service.Connected && SelectedApplications.Count > 0, o => DeregisterApplications());

            SelectedApplications.CollectionChanged += (sender, args) => { DeregisterApplicationsCommand.UpdateState(); };
            service.StateChanged += () => appBrowserViewModel.ViewContext.Invoke(Update);

            Update();
        }

        public string ModuleName { get; set; }

        public IEnumerable<IModuleViewModel> GetSubModules()
        {
            return Enumerable.Empty<IModuleViewModel>();
        }

        public event Action SubModulesChanged;

        public IService Service
        {
            get { return service; }
        }

        public ServiceType ServiceType
        {
            get { return ServiceType.Eureka; }
        }

        public void Update()
        {
            ModuleName = service.Name;

            appMapper.UpdateCollection(service.Applications.SelectMany(app => app.Instances.Select(inst => Tuple.Create(app, inst))), Applications);

            RefreshCommand.UpdateState();
            ConnectCommand.UpdateState();
            DisconnectCommand.UpdateState();
            DeregisterApplicationsCommand.UpdateState();
        }

        private async Task DeregisterApplications()
        {
            List<EurekaApplicationViewModel> apps = SelectedApplications.ToList();
            foreach (EurekaApplicationViewModel app in apps)
            {
                await service.Deregister(app.AppName, app.InstanceId);
            }
        }
    }
}