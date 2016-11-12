using System.Collections.Generic;
using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.Core
{
    public class AppEnvironment
    {
        public string Name { get; set; }
        public List<IService> Services { get; } = new List<IService>();
    }
}