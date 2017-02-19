using System.Collections.ObjectModel;
using System.Linq;
using CloudAppBrowser.Core;
using CloudAppBrowser.ViewModels.Services;

namespace CloudAppBrowser.ViewModels
{
    public class AppBrowserViewModel
    {
        private readonly ObservableCollectionMapper<AppEnvironment, AppEnvironmentViewModel> modulesMapper;

        public AppBrowserViewModel(AppBrowser appBrowser, ViewContext viewContext)
        {
            AppBrowser = appBrowser;
            ViewContext = viewContext;
            MainForm = new MainFormViewModel(this);

            modulesMapper = new ObservableCollectionMapper<AppEnvironment, AppEnvironmentViewModel>(
                env => new AppEnvironmentViewModel(this, env),
                viewModel => viewModel.Environment,
                (env, viewModel) => viewModel.Update(),
                (viewModel1, viewModel2) => string.CompareOrdinal(viewModel1.ModuleName, viewModel2.ModuleName)
            );

            AppBrowser.EnvironmentsChanged += Update;
            Update();
        }

        public AppBrowser AppBrowser { get; }

        public ViewContext ViewContext { get; }

        public MainFormViewModel MainForm { get; }

        public ObservableCollection<AppEnvironmentViewModel> Environments { get; } = new ObservableCollection<AppEnvironmentViewModel>();

        private void Update()
        {
            modulesMapper.UpdateCollection(AppBrowser.Environments, Environments);
        }

        public AppEnvironmentViewModel AddEnvironment(string name)
        {
            AppEnvironment environment = AppBrowser.AddEnvironment(name);
            return Environments.FirstOrDefault(e => e.Environment == environment);
        }
    }
}