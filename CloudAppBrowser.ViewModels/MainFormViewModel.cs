using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudAppBrowser.ViewModels.Subsystems;

namespace CloudAppBrowser.ViewModels
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        private readonly List<SubsystemTreeNode> subsystems = new List<SubsystemTreeNode>();
        private ISubsystemViewModel subsystem;
        private string subsystemName;

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

        public void AddEnvironment(EnvironmentSubsystemViewModel environment)
        {
            subsystems.Add(new SubsystemTreeNode {Name = environment.Name, SubsystemViewModel = environment});
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