using System;
using System.ComponentModel;
using CloudAppBrowser.ViewModels;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views
{
    public class MainForm : Form
    {
        private readonly MainFormViewModel viewModel;

        protected TreeView SubsystemTree;
        protected GroupBox SubsystemPanel;

        public MainForm(MainFormViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            XamlReader.Load(this);

            SubsystemTree.SelectionChanged += SubsystemTreeOnSelectionChanged;

            viewModel.SubsystemsChanged += UpdateTree;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            UpdateTree();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "Subsystem")
            {
                SubsystemPanel.RemoveAll();
                Panel panel = ViewResolver.Instance.CreatePanel(viewModel.Subsystem);
                SubsystemPanel.Content = panel;
            }
        }

        private void SubsystemTreeOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            TreeItem selectedItem = SubsystemTree.SelectedItem as TreeItem;
            SubsystemTreeNode node = selectedItem?.Tag as SubsystemTreeNode;
            viewModel.Subsystem = node?.SubsystemViewModel;
        }

        private void UpdateTree()
        {
            TreeItemCollection treeNodes = new TreeItemCollection();

            foreach (SubsystemTreeNode subsystem in viewModel.Subsystems)
            {
                AddTreeNode(treeNodes, subsystem);
            }

            SubsystemTree.DataStore = treeNodes;
        }

        private void AddTreeNode(TreeItemCollection treeNodes, SubsystemTreeNode subsystem)
        {
            TreeItem node = new TreeItem { Text = subsystem.Name, Tag = subsystem };
            treeNodes.Add(node);
            foreach (SubsystemTreeNode childSubsystem in subsystem.Children)
            {
                AddTreeNode(node.Children, childSubsystem);
            }
        }

        public void AddEnvironment(object sender, EventArgs args)
        {
            viewModel.AddEnvironment();
        }
    }
}