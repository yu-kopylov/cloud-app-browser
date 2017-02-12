using System.Threading.Tasks;

namespace CloudAppBrowser.Core.Services
{
    public interface IService
    {
        string Name { get; }
        bool Connected { get; }
        Task Connect();
        Task Disconnect();
    }
}