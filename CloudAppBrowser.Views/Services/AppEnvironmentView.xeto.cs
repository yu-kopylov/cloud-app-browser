using System;
using System.Collections.Generic;
using System.Linq;
using CloudAppBrowser.ViewModels.Services;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services
{
    public class AppEnvironmentView : Panel
    {
        protected GridView ServicesGridView;
        protected TextBoxCell ServiceTypeCell;
        protected TextBoxCell ServiceNameCell;

        public AppEnvironmentView()
        {
            XamlReader.Load(this);
            ServiceTypeCell.Binding = Binding.Delegate<IServiceViewModel, string>(c => c.ServiceType.ToString());
            ServiceNameCell.Binding = Binding.Delegate<IServiceViewModel, string>(c => c.ModuleName);
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
            List<IServiceViewModel> services = ServicesGridView
                .SelectedItems
                .Cast<IServiceViewModel>()
                .ToList();

            if (!services.Any())
            {
                MessageBox.Show(this.Parent, "No services are selected.");
                return;
            }

            AppEnvironmentViewModel model = (AppEnvironmentViewModel) DataContext;
            foreach (IServiceViewModel service in services)
            {
                model.RemoveService(service);
            }
        }
    }
}