using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core;
using CloudAppBrowser.Core.Services;
using CloudAppBrowser.Core.Services.Docker;
using CloudAppBrowser.ViewModels.Subsystems;
using CloudAppBrowser.ViewModels.Subsystems.Docker;

namespace CloudAppBrowser.ViewModels
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        private readonly AppBrowser appBrowser;

        private readonly List<SubsystemTreeNode> subsystems = new List<SubsystemTreeNode>();
        private readonly Dictionary<IService, SubsystemTreeNode> subsystemsByService = new Dictionary<IService, SubsystemTreeNode>();
        private SubsystemTreeNode selectedNode;
        private string subsystemName;

        public MainFormViewModel(AppBrowser appBrowser)
        {
            this.appBrowser = appBrowser;
            UpdateSubsystems();
        }

        private void UpdateSubsystems()
        {
            subsystems.Clear();
            subsystemsByService.Clear();
            foreach (AppEnvironment environment in appBrowser.Environments)
            {
                var envNode = CreateTreeNode(environment);
                subsystems.Add(envNode);
            }
            SubsystemsChanged?.Invoke();
        }

        private SubsystemTreeNode CreateTreeNode(AppEnvironment environment)
        {
            AppEnvironmentSubsystemViewModel envViewModel = new AppEnvironmentSubsystemViewModel(this, environment);
            SubsystemTreeNode envNode = CreateNode(envViewModel);

            foreach (IService service in environment.Services)
            {
                ISubsystemViewModel serviceViewModel = CreateSubsystemViewModel(service);
                SubsystemTreeNode serviceNode = CreateNode(serviceViewModel);
                envNode.Children.Add(serviceNode);
                subsystemsByService.Add(service, serviceNode);
            }

            //todo: there is a memory leak here
            environment.ServiceListChanged += UpdateSubsystems;

            return envNode;
        }

        private SubsystemTreeNode CreateNode(ISubsystemViewModel viewModel)
        {
            SubsystemTreeNode node = new SubsystemTreeNode();
            node.Name = viewModel?.Name;
            node.SubsystemViewModel = viewModel;
            return node;
        }

        private ISubsystemViewModel CreateSubsystemViewModel(IService service)
        {
            if (service is DockerService)
            {
                return new DockerSubsystemViewModel((DockerService) service);
            }
            return null;
        }

        public List<SubsystemTreeNode> Subsystems
        {
            get { return subsystems; }
        }

        public SubsystemTreeNode SelectedNode
        {
            get { return selectedNode; }
            set
            {
                selectedNode = value;
                OnPropertyChanged();
                SubsystemName = selectedNode?.SubsystemViewModel?.Name;
            }
        }

        public string SubsystemName
        {
            get { return subsystemName; }
            private set
            {
                subsystemName = value;
                OnPropertyChanged();
            }
        }

        public void AddEnvironment()
        {
            AppEnvironment env = appBrowser.AddEnvironment("New Environment");
            SubsystemTreeNode envNode = CreateTreeNode(env);

            subsystems.Add(envNode);
            subsystems.Sort((node1, node2) => string.Compare(node1.Name, node2.Name, StringComparison.Ordinal));
            SubsystemsChanged?.Invoke();

            SelectedNode = envNode;
        }

        public void SelectService(IService service)
        {
            SubsystemTreeNode node;
            if (!subsystemsByService.TryGetValue(service, out node))
            {
                SelectedNode = null;
            }
            SelectedNode = node;
        }

        public delegate void SubsystemsChangedEventHandler();

        public event SubsystemsChangedEventHandler SubsystemsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}