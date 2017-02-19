using CloudAppBrowser.Core.Services.Eureka;

namespace CloudAppBrowser.ViewModels.Services.Eureka
{
    public class EurekaApplicationViewModel
    {
        public EurekaApplicationViewModel(EurekaApplication application, EurekaApplicationInstance instance)
        {
            Application = application;
            Instance = instance;
            Update();
        }

        public EurekaApplication Application { get; }
        public EurekaApplicationInstance Instance { get; }

        public string AppName { get; private set; }
        public string InstanceId { get; private set; }
        public string HostName { get; private set; }

        public void Update()
        {
            AppName = Application.Name;
            InstanceId = Instance.InstanceId;
            HostName = Instance.HostName;
        }
    }
}