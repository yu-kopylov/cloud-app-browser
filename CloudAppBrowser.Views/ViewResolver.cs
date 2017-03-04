using System;
using System.Collections.Generic;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Services;
using CloudAppBrowser.ViewModels.Services.Docker;
using CloudAppBrowser.ViewModels.Services.Eureka;
using CloudAppBrowser.ViewModels.Settings;
using CloudAppBrowser.Views.Services;
using CloudAppBrowser.Views.Services.Docker;
using CloudAppBrowser.Views.Services.Eureka;
using CloudAppBrowser.Views.Settings;
using Eto.Forms;

namespace CloudAppBrowser.Views
{
    public class ViewResolver : IViewResolver
    {
        private readonly Dictionary<Type, Func<Panel>> constructors = new Dictionary<Type, Func<Panel>>();

        public static ViewResolver Instance { get; } = new ViewResolver();

        public ViewResolver()
        {
            constructors.Add(typeof(AppEnvironmentViewModel), () => new AppEnvironmentView());
            constructors.Add(typeof(DockerServiceViewModel), () => new DockerServiceView());
            constructors.Add(typeof(DockerImageListViewModel), () => new DockerImageListView());
            constructors.Add(typeof(DockerContainerListViewModel), () => new DockerContainerListView());
            constructors.Add(typeof(EurekaServiceViewModel), () => new EurekaServiceView());
            constructors.Add(typeof(EurekaSettingsViewModel), () => new EurekaSettingsDialog());
            constructors.Add(typeof(DockerSettingsViewModel), () => new DockerSettingsDialog());
        }

        public Panel CreatePanel(object viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }
            Func<Panel> constructor;
            if (!constructors.TryGetValue(viewModel.GetType(), out constructor))
            {
                return null;
            }
            var panel = constructor();
            panel.DataContext = viewModel;
            return panel;
        }

        public bool ShowDialog(object viewModel)
        {
            Dialog<bool> dialog = (Dialog<bool>) CreatePanel(viewModel);
            dialog.DataContext = viewModel;
            return dialog.ShowModal();
        }
    }
}