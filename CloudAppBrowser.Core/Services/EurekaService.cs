namespace CloudAppBrowser.Core.Services
{
    public class EurekaService : IService
    {
        public string Name { get; set; }

        private bool connected;

        public bool Connected
        {
            get { return connected; }
        }

        public void Connect()
        {
            connected = true;
        }

        public void Disconnect()
        {
            connected = false;
        }
    }
}