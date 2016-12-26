using System.Collections.Generic;

namespace CloudAppBrowser.Core.Services.Eureka
{
    public class EurekaApplication
    {
        public string Name { get; set; }
        public List<EurekaApplicationInstance> Instances { get; } = new List<EurekaApplicationInstance>();
    }
}
