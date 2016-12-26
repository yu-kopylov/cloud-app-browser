using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CloudAppBrowser.Core.Services.Eureka
{
    public class EurekaService : IService
    {
        private static readonly BasicThreadPool ThreadPool = new BasicThreadPool();

        public string Name { get; set; }

        public string Url { get; set; }

        private bool connected;

        public bool Connected
        {
            get { return connected; }
        }

        public void Connect()
        {
            connected = true;
            ThreadPool.Execute(RefreshApplicationsAsync);
        }

        public void Disconnect()
        {
            connected = false;
        }

        public void RefreshApplications()
        {
            ThreadPool.Execute(RefreshApplicationsAsync);
        }

        public async void RefreshApplicationsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                HttpResponseMessage response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();
                JsonSerializer ser = new JsonSerializer();
                EurekaApplicationsJson apps = ser.Deserialize<EurekaApplicationsJson>(new JsonTextReader(new StringReader(responseText)));
            }
        }
    }

    public class EurekaApplicationsJson
    {
        [JsonProperty("applications")]
        public EurekaApplicationListJson ApplicationList { get; set; }
    }

    public class EurekaApplicationListJson
    {
        [JsonProperty("versions__delta")]
        public string VersionDelta { get; set; }

        [JsonProperty("application")]
        public List<EurekaApplicationJson> Applications { get; set; }
    }

    public class EurekaApplicationJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("instance")]
        public List<EurekaApplicationInstanceJson> Instances { get; set; }
    }

    public class EurekaApplicationInstanceJson
    {
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("hostName")]
        public string HostName { get; set; }

        [JsonProperty("lastUpdatedTimestamp")]
        public long LastUpdatedTimestamp { get; set; }
    }
}