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
        private ISubsystemViewModel subsystem;
        private string subsystemName;

        public MainFormViewModel(AppBrowser appBrowser)
        {
            this.appBrowser = appBrowser;
            UpdateSubsystems();
        }

        private void UpdateSubsystems()
        {
            subsystems.Clear();
            foreach (AppEnvironment environment in appBrowser.Environments)
            {
                AppEnvironmentSubsystemViewModel envViewModel = new AppEnvironmentSubsystemViewModel(environment);
                SubsystemTreeNode envNode = CreateNode(envViewModel);
                subsystems.Add(envNode);

                foreach (IService service in environment.Services)
                {
                    ISubsystemViewModel serviceViewModel = CreateSubsystemViewModel(service);
                    SubsystemTreeNode serviceNode = CreateNode(serviceViewModel);
                    envNode.Children.Add(serviceNode);
                }
            }
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

        public ISubsystemViewModel Subsystem
        {
            get { return subsystem; }
            set
            {
                subsystem = value;
                OnPropertyChanged();
                SubsystemName = subsystem?.Name;
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

        public void AddEnvironment(AppEnvironmentSubsystemViewModel appEnvironment)
        {
            subsystems.Add(new SubsystemTreeNode {Name = appEnvironment.Name, SubsystemViewModel = appEnvironment});
            subsystems.Sort((node1, node2) => string.Compare(node1.Name, node2.Name, StringComparison.Ordinal));
            SubsystemsChanged?.Invoke();
            Subsystem = subsystem;
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