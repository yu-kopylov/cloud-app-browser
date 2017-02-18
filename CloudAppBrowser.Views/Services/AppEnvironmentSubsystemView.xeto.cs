using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Services;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services
{
    public class AppEnvironmentSubsystemView : Panel
    {
        protected GridView ServicesGridView;
        protected TextBoxCell ServiceTypeCell;
        protected TextBoxCell ServiceNameCell;

        public AppEnvironmentSubsystemView()
        {
            XamlReader.Load(this);
            ServiceTypeCell.Binding = Binding.Delegate<ServiceViewModel, string>(c => c.ServiceType.ToString());
            ServiceNameCell.Binding = Binding.Delegate<ServiceViewModel, string>(c => c.ServiceName);
        }

        public void AddDockerService(object sender, EventArgs e)
        {
            AppEnvironmentViewModel model = (AppEnvironmentViewModel) DataContext;
            model.AddDockerService();
        }

        public void AddEurekaService(object sender, EventArgs e)
        {
            AppEnvironmentViewModel model = (AppEnvironmentViewModel) DataContext;
            model.AddEurekaService();
        }

        public void RemoveService(object sender, EventArgs e)
        {
            List<ServiceViewModel> services = ServicesGridView
                .SelectedItems
                .Cast<ServiceViewModel>()
                .ToList();

            if (!services.Any())
            {
                MessageBox.Show(this.Parent, "No services are selected.");
                return;
            }

            AppEnvironmentViewModel model = (AppEnvironmentViewModel) DataContext;
            foreach (ServiceViewModel service in services)
            {
                model.RemoveService(service);
            }
        }
    }
}