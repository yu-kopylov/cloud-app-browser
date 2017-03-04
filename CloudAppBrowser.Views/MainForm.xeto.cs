using System;
using System.ComponentModel;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Services;
using CloudAppBrowser.Views.Services;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views
{
    public class MainForm : Form
    {
        private readonly AppBrowserViewModel appBrowserViewModel;
        private readonly MainFormViewModel viewModel;

        protected ModulesTreeView ModulesTree;

        protected GroupBox ModulePanel;

        public MainForm(AppBrowserViewModel appBrowserViewModel)
        {
            this.appBrowserViewModel = appBrowserViewModel;
            this.viewModel = appBrowserViewModel.MainForm;

            DataContext = viewModel;

            XamlReader.Load(this);

            ModulesTree.DataContext = viewModel.ModulesTree;
            viewModel.ModulesTree.PropertyChanged += ModulesTreeOnPropertyChanged;
        }

        private void ModulesTreeOnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(ModulesTreeViewModel.SelectedModule))
            {
                ModulePanel.RemoveAll();
                if (viewModel.ModulesTree.SelectedModule != null)
                {
                    Panel panel = (Panel) appBrowserViewModel.ViewContext.CreatePanel(viewModel.ModulesTree.SelectedModule);
                    ModulePanel.Content = panel;
                    ModulePanel.DataContext = viewModel.ModulesTree.SelectedModule;
                }
            }
        }

        public void AddEnvironment(object sender, EventArgs args)
        {
            viewModel.AddEnvironment();
        }
    }
}