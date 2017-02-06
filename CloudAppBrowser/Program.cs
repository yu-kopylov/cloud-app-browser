using System;
using CloudAppBrowser.Core;
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

            AppBrowser appBrowser = new AppBrowser();

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