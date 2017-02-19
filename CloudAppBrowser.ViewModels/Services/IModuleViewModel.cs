using System;
using System.Collections.Generic;

namespace CloudAppBrowser.ViewModels.Services
{
    public interface IModuleViewModel
    {
        string ModuleName { get; }
        IEnumerable<IModuleViewModel> GetSubModules();
        event Action SubModulesChanged;
    }
}