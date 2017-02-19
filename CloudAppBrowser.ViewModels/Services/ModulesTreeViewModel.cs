using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels.Services
{
    public class ModulesTreeViewModel : INotifyPropertyChanged
    {
        private static readonly ObservableCollectionMapper<IModuleViewModel, IModuleViewModel> CopyMapper = new ObservableCollectionMapper<IModuleViewModel, IModuleViewModel>(
            module => module,
            module => module,
            (module1, module2) => { },
            null
        );

        private readonly AppBrowserViewModel appBrowserViewModel;

        private IModuleViewModel selectedModule;

        public ModulesTreeViewModel(AppBrowserViewModel appBrowserViewModel)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            appBrowserViewModel.Environments.CollectionChanged += (sender, args) => Update();
            Update();
        }

        public ObservableCollection<IModuleViewModel> Modules { get; } = new ObservableCollection<IModuleViewModel>();

        public IModuleViewModel SelectedModule
        {
            get { return selectedModule; }
            set
            {
                selectedModule = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Update()
        {
            CopyMapper.UpdateCollection(appBrowserViewModel.Environments, Modules);
        }
    }
}