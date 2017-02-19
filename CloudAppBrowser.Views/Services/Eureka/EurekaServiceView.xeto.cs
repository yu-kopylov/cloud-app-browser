using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Services.Eureka;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services.Eureka
{
    public class EurekaServiceView : Panel
    {
        protected GridView ApplicationsGridView;

        protected TextBoxCell AppNameCell;
        protected TextBoxCell InstanceIdCell;
        protected TextBoxCell HostNameCell;

        public EurekaServiceView()
        {
            XamlReader.Load(this);

            AppNameCell.Binding = Binding.Delegate<EurekaApplicationViewModel, string>(c => c.AppName);
            InstanceIdCell.Binding = Binding.Delegate<EurekaApplicationViewModel, string>(c => c.InstanceId);
            HostNameCell.Binding = Binding.Delegate<EurekaApplicationViewModel, string>(c => c.HostName);

            ApplicationsGridView.SelectionChanged += ApplicationsGridViewOnSelectionChanged;
        }

        private void ApplicationsGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            List<EurekaApplicationViewModel> containers = ApplicationsGridView
                .SelectedItems
                .Cast<EurekaApplicationViewModel>()
                .ToList();
            EurekaServiceViewModel viewModel = (EurekaServiceViewModel) DataContext;
            viewModel.SelectedApplications.Clear();
            foreach (EurekaApplicationViewModel containerModel in containers)
            {
                viewModel.SelectedApplications.Add(containerModel);
            }
        }
    }
}