using System;
using System.Collections.ObjectModel;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Subsystems;
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

            MainFormViewModel viewModel = new MainFormViewModel();
            viewModel.AddEnvironment(new EnvironmentSubsystemViewModel { Name = "Env1" } );

            MainForm mainForm = new MainForm(viewModel);

            application.Run(mainForm);
        }
    }
}