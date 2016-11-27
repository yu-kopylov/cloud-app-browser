using System.Collections.Generic;

namespace CloudAppBrowser.Core
{
    public class AppBrowser
    {
        public List<AppEnvironment> Environments { get; } = new List<AppEnvironment>();

        public AppEnvironment AddEnvironment(string name)
        {
            AppEnvironment environment = new AppEnvironment {Name = name};
            Environments.Add(environment);
            return environment;
        }
    }
}