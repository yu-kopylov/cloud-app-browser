using CloudAppBrowser.Core;

namespace CloudAppBrowser.ViewModels.Subsystems
{
    public class AppEnvironmentSubsystemViewModel : ISubsystemViewModel
    {
        private readonly AppEnvironment environment;

        public AppEnvironmentSubsystemViewModel(AppEnvironment environment)
        {
            this.environment = environment;
            Name = environment.Name;
        }

        public string Name { get; set; }
    }
}