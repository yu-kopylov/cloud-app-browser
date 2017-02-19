using CloudAppBrowser.Core.Services;

namespace CloudAppBrowser.ViewModels.Services
{
    public interface IServiceViewModel : IModuleViewModel
    {
        IService Service { get; }
        ServiceType ServiceType { get; }
        void Update();
    }
}