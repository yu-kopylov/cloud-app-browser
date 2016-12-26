namespace CloudAppBrowser.Core.Services
{
    public interface IService
    {
        string Name { get; }
        bool Connected { get; }
        void Connect();
        void Disconnect();
    }
}