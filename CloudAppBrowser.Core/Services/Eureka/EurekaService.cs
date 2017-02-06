using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

        public List<EurekaApplication> Applications { get; } = new List<EurekaApplication>();

        public event Action StateChanged;

        private bool connected;

        public bool Connected
        {
            get { return connected; }
        }

        public void Connect()
        {
            connected = true;
            ThreadPool.Execute(RefreshApplicationsAsync);
            StateChanged?.Invoke();
        }

        public void Disconnect()
        {
            connected = false;
            Applications.Clear();
            StateChanged?.Invoke();
        }

        public void RefreshApplications()
        {
            ThreadPool.Execute(RefreshApplicationsAsync);
        }

        private async void RefreshApplicationsAsync()
        {
            using (HttpClientHandler clientHandler = new HttpClientHandler())
            {
                clientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    HttpResponseMessage response = await client.SendAsync(request);
                    string responseText = await response.Content.ReadAsStringAsync();
                    JsonSerializer ser = new JsonSerializer();
                    EurekaApplicationsJson apps = ser.Deserialize<EurekaApplicationsJson>(new JsonTextReader(new StringReader(responseText)));
                    Applications.Clear();
                    foreach (EurekaApplicationJson appJson in apps.ApplicationList.Applications)
                    {
                        EurekaApplication app = new EurekaApplication();
                        Applications.Add(app);

                        app.Name = appJson.Name;

                        foreach (EurekaApplicationInstanceJson instanceJson in appJson.Instances)
                        {
                            EurekaApplicationInstance instance = new EurekaApplicationInstance();
                            app.Instances.Add(instance);

                            instance.InstanceId = instanceJson.InstanceId;
                            instance.HostName = instanceJson.HostName;
                        }
                    }
                    StateChanged?.Invoke();
                }
            }
        }

        public void Deregister(string appId, string instanceId)
        {
            ThreadPool.Execute(() => DeregisterAsync(appId, instanceId));
        }

        private async void DeregisterAsync(string appId, string instanceId)
        {
            using (HttpClient client = new HttpClient())
            {
                //todo: use more robust concatenation
                Uri uri = new Uri(Url + "/" + appId + "/" + instanceId, UriKind.RelativeOrAbsolute);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
                HttpResponseMessage response = await client.SendAsync(request);
            }
            RefreshApplicationsAsync();
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