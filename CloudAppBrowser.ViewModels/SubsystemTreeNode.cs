using System.Collections.Generic;
using CloudAppBrowser.ViewModels.Subsystems;

namespace CloudAppBrowser.ViewModels
{
    public class SubsystemTreeNode
    {
        public string Name { get; set; }
        public ISubsystemViewModel SubsystemViewModel { get; set; }
        public List<SubsystemTreeNode> Children { get; } = new List<SubsystemTreeNode>();
    }
}