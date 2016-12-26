using System;
using System.Collections.Generic;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Settings;
using CloudAppBrowser.ViewModels.Subsystems;
using CloudAppBrowser.ViewModels.Subsystems.Docker;
using CloudAppBrowser.ViewModels.Subsystems.Eureka;
using CloudAppBrowser.Views.Settings;
using CloudAppBrowser.Views.Subsystems;
using Eto.Forms;

namespace CloudAppBrowser.Views
{
    public class ViewResolver : IViewResolver
    {
        private readonly Dictionary<Type, Func<Panel>> constructors = new Dictionary<Type, Func<Panel>>();

        public static ViewResolver Instance { get; } = new ViewResolver();

        public ViewResolver()
        {
            constructors.Add(typeof(AppEnvironmentSubsystemViewModel), () => new AppEnvironmentSubsystemView());
            constructors.Add(typeof(DockerSubsystemViewModel), () => new DockerSubsystemView());
            constructors.Add(typeof(EurekaSubsystemViewModel), () => new EurekaSubsystemView());
            constructors.Add(typeof(EurekaSettingsViewModel), () => new EurekaSettingsDialog());
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