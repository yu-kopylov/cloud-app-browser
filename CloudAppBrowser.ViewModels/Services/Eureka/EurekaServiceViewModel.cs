using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CloudAppBrowser.Core.Services.Eureka;

namespace CloudAppBrowser.ViewModels.Services.Eureka
{
    public class EurekaServiceViewModel : ISubsystemViewModel
    {
        private readonly EurekaService service;

        public string Name { get; set; }

        public ObservableCollection<EurekaApplicationViewModel> Applications { get; } = new ObservableCollection<EurekaApplicationViewModel>();

        public ObservableCollection<EurekaApplicationViewModel> SelectedApplications { get; } = new ObservableCollection<EurekaApplicationViewModel>();

        public BasicCommand ConnectCommand { get; }
        public BasicCommand DisconnectCommand { get; }
        public BasicCommand RefreshCommand { get; }
        public BasicCommand DeregisterApplicationsCommand { get; }

        public EurekaServiceViewModel(EurekaService service)
        {
            this.service = service;
            Name = service.Name;

            ConnectCommand = new BasicCommand(() => !service.Connected, o => service.Connect());
            DisconnectCommand = new BasicCommand(() => service.Connected, o => service.Disconnect());
            RefreshCommand = new BasicCommand(() => service.Connected, o => service.RefreshApplications());
            DeregisterApplicationsCommand = new BasicCommand(() => service.Connected && SelectedApplications.Count > 0, o => DeregisterApplications());

            SelectedApplications.CollectionChanged += (sender, args) => { DeregisterApplicationsCommand.UpdateState(); };

            service.StateChanged += () => ViewContext.Instance.Invoke(UpdateViewModel);
        }

        private void UpdateViewModel()
        {
            Applications.Clear();
            foreach (EurekaApplication application in service.Applications.OrderBy(a => a.Name))
            {
                foreach (EurekaApplicationInstance instance in application.Instances)
                {
                    EurekaApplicationViewModel appViewModel = new EurekaApplicationViewModel();
                    appViewModel.AppName = application.Name;
                    appViewModel.InstanceId = instance.InstanceId;
                    appViewModel.HostName = instance.HostName;
                    Applications.Add(appViewModel);
                }
            }

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