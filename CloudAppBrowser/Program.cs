using System;
using System.Collections.Generic;
using CloudAppBrowser.Core;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Services;
using CloudAppBrowser.ViewModels.Services.Docker;
using CloudAppBrowser.ViewModels.Services.Eureka;
using CloudAppBrowser.ViewModels.Settings;
using CloudAppBrowser.Views;
using CloudAppBrowser.Views.Services;
using CloudAppBrowser.Views.Services.Docker;
using CloudAppBrowser.Views.Services.Eureka;
using CloudAppBrowser.Views.Settings;
using Eto;
using Eto.Forms;

namespace CloudAppBrowser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application application = new Application(Platform.Detect);
            AppBrowser appBrowser = new AppBrowser();
            EtoViewContext viewContext = new EtoViewContext();
            AppBrowserViewModel appBrowserViewModel = new AppBrowserViewModel(appBrowser, viewContext);

            MainForm mainForm = new MainForm(appBrowserViewModel);
            viewContext.MainForm = mainForm;

            application.Run(mainForm);
        }
    }

    internal class EtoViewContext : ViewContext
    {
        private readonly Dictionary<Type, Func<Panel>> constructors = new Dictionary<Type, Func<Panel>>();

        public EtoViewContext()
        {
            constructors.Add(typeof(AppEnvironmentViewModel), () => new AppEnvironmentView());
            constructors.Add(typeof(DockerServiceViewModel), () => new DockerServiceView());
            constructors.Add(typeof(DockerImageListViewModel), () => new DockerImageListView());
            constructors.Add(typeof(DockerContainerListViewModel), () => new DockerContainerListView());
            constructors.Add(typeof(EurekaServiceViewModel), () => new EurekaServiceView());
            constructors.Add(typeof(EurekaSettingsViewModel), () => new EurekaSettingsDialog());
            constructors.Add(typeof(DockerSettingsViewModel), () => new DockerSettingsDialog());
        }

        public MainForm MainForm { get; set; }

        public override void Invoke(Action action)
        {
            Application.Instance.Invoke(action);
        }

        public override void MessageBox(string message, string caption)
        {
            Eto.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK);
        }

        public override bool ShowDialog(object viewModel)
        {
            Dialog<bool> dialog = (Dialog<bool>) CreatePanel(viewModel);
            dialog.DataContext = viewModel;
            return dialog.ShowModal(MainForm);
        }

        public override object CreatePanel(object viewModel)
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
    }
}