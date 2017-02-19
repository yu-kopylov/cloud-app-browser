using System.Collections.Generic;
using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.Core
{
    public class AppEnvironment
    {
        public string Name { get; set; }
        public List<IService> Services { get; } = new List<IService>();

        public delegate void ServiceListChangedEventHandler();

        public event ServiceListChangedEventHandler ServiceListChanged;

        public void AddService(IService service)
        {
            Services.Add(service);
            ServiceListChanged?.Invoke();
        }

        public void RemoveService(IService service)
        {
            Services.Remove(service);
            ServiceListChanged?.Invoke();
        }
    }
}