using System;
using System.ComponentModel;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Subsystems;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views
{
    public class MainForm : Form
    {
        private static readonly ObservableCollectionMapper<SubsystemTreeNode, ITreeItem> TreeMapper = new ObservableCollectionMapper<SubsystemTreeNode, ITreeItem>(
            subsystem => CreateTreeNode(subsystem),
            treeNode => (SubsystemTreeNode) ((TreeItem) treeNode).Tag,
            (subsystem, treeNode) => UpdateTreeNode(subsystem, (TreeItem) treeNode),
            null);

        private readonly MainFormViewModel viewModel;

        protected TreeView SubsystemTree;
        protected GroupBox SubsystemPanel;

        private readonly TreeItemCollection treeNodes = new TreeItemCollection();

        public MainForm(MainFormViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            XamlReader.Load(this);

            SubsystemTree.SelectionChanged += SubsystemTreeOnSelectionChanged;

            viewModel.SubsystemsChanged += UpdateTree;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;

            SubsystemTree.DataStore = treeNodes;

            UpdateTree();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "SelectedTreeNode")
            {
                ITreeItem treeItem = FindTreeItem(treeNodes, viewModel.SelectedTreeNode);
                if (SubsystemTree.SelectedItem != treeItem)
                {
                    SubsystemTree.SelectedItem = treeItem;
                    if (treeItem != null)
                    {
                        ITreeItem topParent = treeItem;
                        ITreeItem parent = treeItem.Parent;
                        while (parent != null)
                        {
                            topParent = parent;
                            parent.Expanded = true;
                            parent = parent.Parent;
                        }
                        SubsystemTree.RefreshItem(topParent);
                    }
                }

                SubsystemPanel.RemoveAll();
                ISubsystemViewModel selectedSubsystem = viewModel.SelectedTreeNode?.SubsystemViewModel;
                if (selectedSubsystem != null)
                {
                    Panel panel = ViewResolver.Instance.CreatePanel(selectedSubsystem);
                    SubsystemPanel.Content = panel;
                }
            }
        }

        private ITreeItem FindTreeItem(TreeItemCollection nodes, SubsystemTreeNode targetNode)
        {
            foreach (ITreeItem treeNode in nodes)
            {
                TreeItem node = (TreeItem) treeNode;
                if (node == null)
                {
                    continue;
                }
                if (node.Tag == targetNode)
                {
                    return node;
                }
                ITreeItem matchingChild = FindTreeItem(node.Children, targetNode);
                if (matchingChild != null)
                {
                    return matchingChild;
                }
            }
            return null;
        }

        private void SubsystemTreeOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            TreeItem selectedItem = SubsystemTree.SelectedItem as TreeItem;
            viewModel.SelectedTreeNode = selectedItem?.Tag as SubsystemTreeNode;
        }

        private void UpdateTree()
        {
            TreeMapper.UpdateCollection(viewModel.TreeNodes, treeNodes);
        }

        private static TreeItem CreateTreeNode(SubsystemTreeNode subsystemTreeNode)
        {
            TreeItem node = new TreeItem {Text = subsystemTreeNode.Name, Tag = subsystemTreeNode};
            UpdateTreeNode(subsystemTreeNode, node);
            return node;
        }

        private static void UpdateTreeNode(SubsystemTreeNode subsystemTreeNode, TreeItem treeNode)
        {
            treeNode.Text = subsystemTreeNode.Name;
            treeNode.Tag = subsystemTreeNode;
            TreeMapper.UpdateCollection(subsystemTreeNode.Children, treeNode.Children);
        }

        public void AddEnvironment(object sender, EventArgs args)
        {
            viewModel.AddEnvironment();
        }
    }
}