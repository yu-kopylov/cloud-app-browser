﻿using System;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services.Docker;
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

            ViewContext.Instance = new EtoViewContext();

            DockerService service = new DockerService();
            service.Name = "Docker";
            service.MachineName = "default";
            service.Connect();

            AppBrowser appBrowser = new AppBrowser();
            appBrowser.Environments.Add(new AppEnvironment
            {
                Name = "Env1",
                Services = {service}
            });

            MainFormViewModel viewModel = new MainFormViewModel(appBrowser);
            MainForm mainForm = new MainForm(viewModel);

            application.Run(mainForm);
        }
    }

    internal class EtoViewContext : ViewContext
    {
        public override void Invoke(Action action)
        {
            Application.Instance.Invoke(action);
        }

        public override void MessageBox(string message, string caption)
        {
            Eto.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK);
        }

        public override IViewResolver ViewResolver
        {
            get { return Views.ViewResolver.Instance; }
        }
    }
}