﻿using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Subsystems.Eureka;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class EurekaSubsystemView : Panel
    {
        protected GridView ApplicationsGridView;

        protected TextBoxCell AppNameCell;
        protected TextBoxCell InstanceIdCell;
        protected TextBoxCell HostNameCell;

        public EurekaSubsystemView()
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
            EurekaSubsystemViewModel viewModel = (EurekaSubsystemViewModel) DataContext;
            viewModel.SelectedApplications.Clear();
            foreach (EurekaApplicationViewModel containerModel in containers)
            {
                viewModel.SelectedApplications.Add(containerModel);
            }
        }
    }
}