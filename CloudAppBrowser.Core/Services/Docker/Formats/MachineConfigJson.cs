using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CloudAppBrowser.Core.Services.Docker.Formats
{
    public class MachineConfigJson
    {
        public DriverConfigJson Driver { get; set; }
        public HostOptionsJson HostOptions { get; set; }

        public static MachineConfigJson Read(byte[] text)
        {
            JsonSerializer ser = new JsonSerializer();
            using (JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(new MemoryStream(text), Encoding.UTF8)))
            {
                return ser.Deserialize<MachineConfigJson>(jsonTextReader);
            }
        }
    }

    public class HostOptionsJson
    {
        public AuthOptionsJson AuthOptions { get; set; }
    }

    public class AuthOptionsJson
    {
        public string ClientCertPath { get; set; }
        public string ClientKeyPath { get; set; }
    }

    public class DriverConfigJson
    {
        public string IPAddress { get; set; }
    }
}