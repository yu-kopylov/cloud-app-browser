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

        private readonly List<SubsystemTreeNode> treeNodes = new List<SubsystemTreeNode>();
        private readonly Dictionary<IService, SubsystemTreeNode> treeNodesByService = new Dictionary<IService, SubsystemTreeNode>();
        private SubsystemTreeNode selectedTreeNode;
        private string subsystemName;

        public MainFormViewModel(AppBrowser appBrowser)
        {
            this.appBrowser = appBrowser;
            UpdateSubsystems();
        }

        private void UpdateSubsystems()
        {
            treeNodes.Clear();
            treeNodesByService.Clear();
            foreach (AppEnvironment environment in appBrowser.Environments)
            {
                var envNode = CreateTreeNode(environment);
                treeNodes.Add(envNode);
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
                treeNodesByService.Add(service, serviceNode);
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

        public List<SubsystemTreeNode> TreeNodes
        {
            get { return treeNodes; }
        }

        public SubsystemTreeNode SelectedTreeNode
        {
            get { return selectedTreeNode; }
            set
            {
                selectedTreeNode = value;
                OnPropertyChanged();
                SubsystemName = selectedTreeNode?.SubsystemViewModel?.Name;
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

            treeNodes.Add(envNode);
            treeNodes.Sort((node1, node2) => string.Compare(node1.Name, node2.Name, StringComparison.Ordinal));
            SubsystemsChanged?.Invoke();

            SelectedTreeNode = envNode;
        }

        public void SelectService(IService service)
        {
            SubsystemTreeNode node;
            if (!treeNodesByService.TryGetValue(service, out node))
            {
                SelectedTreeNode = null;
            }
            SelectedTreeNode = node;
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