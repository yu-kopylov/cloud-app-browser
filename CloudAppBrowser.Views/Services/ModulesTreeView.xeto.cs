using System;
using System.ComponentModel;
using CloudAppBrowser.ViewModels;
using CloudAppBrowser.ViewModels.Services;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Services
{
    public class ModulesTreeView : Panel
    {
        private readonly ObservableCollectionMapper<IModuleViewModel, ITreeItem> nodeMapper;

        private ModulesTreeViewModel viewModel;

        private readonly TreeItemCollection treeNodes = new TreeItemCollection();

        protected TreeView Tree;

        public ModulesTreeView()
        {
            XamlReader.Load(this);
            DataContextChanged += OnDataContextChanged;

            nodeMapper = new ObservableCollectionMapper<IModuleViewModel, ITreeItem>(
                CreateNode,
                node => ((TreeItem) node).Tag as IModuleViewModel,
                UpdateNode,
                (node1, node2) => string.CompareOrdinal(node1.Text, node2.Text)
            );

            Tree.DataStore = treeNodes;
            Tree.SelectionChanged += TreeOnSelectionChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs eventArgs)
        {
            viewModel = DataContext as ModulesTreeViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += ViewModelOnPropertyChanged;
                viewModel.Modules.CollectionChanged += (o, args) => nodeMapper.UpdateCollection(viewModel.Modules, treeNodes);
                nodeMapper.UpdateCollection(viewModel.Modules, treeNodes);
            }
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == nameof(ModulesTreeViewModel.SelectedModule))
            {
                SelectNode(viewModel.SelectedModule);
            }
        }

        private void TreeOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            TreeItem selectedItem = Tree.SelectedItem as TreeItem;
            viewModel.SelectedModule = selectedItem?.Tag as IModuleViewModel;
        }

        private TreeItem CreateNode(IModuleViewModel model)
        {
            TreeItem node = new TreeItem();
            model.SubModulesChanged += () => UpdateNode(model, node);
            UpdateNode(model, node);
            return node;
        }

        private void UpdateNode(IModuleViewModel model, ITreeItem node)
        {
            TreeItem treeItem = (TreeItem) node;
            treeItem.Text = model.ModuleName;
            treeItem.Tag = model;
            nodeMapper.UpdateCollection(model.GetSubModules(), treeItem.Children);
            Tree.RefreshItem(treeItem);
        }

        private void SelectNode(IModuleViewModel module)
        {
            ITreeItem treeItem = FindTreeItem(treeNodes, module);
            if (Tree.SelectedItem != treeItem)
            {
                Tree.RefreshData();
                Tree.SelectedItem = treeItem;
                Tree.RefreshData();
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
                    Tree.RefreshItem(topParent);
                }
            }
        }

        private ITreeItem FindTreeItem(TreeItemCollection nodes, IModuleViewModel module)
        {
            foreach (ITreeItem treeNode in nodes)
            {
                TreeItem node = (TreeItem) treeNode;
                if (node == null)
                {
                    continue;
                }
                if (node.Tag == module)
                {
                    return node;
                }
                ITreeItem matchingChild = FindTreeItem(node.Children, module);
                if (matchingChild != null)
                {
                    return matchingChild;
                }
            }
            return null;
        }
    }
}