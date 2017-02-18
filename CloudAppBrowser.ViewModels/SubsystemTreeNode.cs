using System.Collections.Generic;
using CloudAppBrowser.ViewModels.Services;

namespace CloudAppBrowser.ViewModels
{
    public class SubsystemTreeNode
    {
        public string Name { get; set; }
        public ISubsystemViewModel SubsystemViewModel { get; set; }
        public List<SubsystemTreeNode> Children { get; } = new List<SubsystemTreeNode>();
    }
}