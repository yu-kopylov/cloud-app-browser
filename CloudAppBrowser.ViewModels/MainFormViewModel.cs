using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudAppBrowser.ViewModels.Services;

namespace CloudAppBrowser.ViewModels
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        private readonly AppBrowserViewModel appBrowserViewModel;

        private string applicationLogText;

        public MainFormViewModel(AppBrowserViewModel appBrowserViewModel)
        {
            this.appBrowserViewModel = appBrowserViewModel;

            ModulesTree = new ModulesTreeViewModel(appBrowserViewModel);

            ApplicationLog.Instance.PropertyChanged += (sender, args) =>
            {
                string text = ApplicationLog.Instance.LogText;
                appBrowserViewModel.ViewContext.Invoke(() => ApplicationLogText = text);
            };
            applicationLogText = ApplicationLog.Instance.LogText;
        }

        public ModulesTreeViewModel ModulesTree { get; }

        public string ApplicationLogText
        {
            get { return applicationLogText; }
            private set
            {
                applicationLogText = value;
                OnPropertyChanged();
            }
        }

        public void AddEnvironment()
        {
            AppEnvironmentViewModel environmentViewModel = appBrowserViewModel.AddEnvironment("New Environment");
            ModulesTree.SelectedModule = environmentViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}