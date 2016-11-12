using System;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.Views;
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
            appBrowser.Environments.Add(new AppEnvironment
            {
                Name = "Env1",
                Services = {new DockerService {Name = "Docker1"}}
            });

            MainFormViewModel viewModel = new MainFormViewModel(appBrowser);
            MainForm mainForm = new MainForm(viewModel);

            application.Run(mainForm);
        }
    }
}