using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudAppBrowser.Core.Services.Eureka.Formats;
using Newtonsoft.Json;

namespace CloudAppBrowser.Core.Services.Eureka
{
    public class EurekaService : IService
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<EurekaApplication> Applications { get; } = new List<EurekaApplication>();

        public event Action StateChanged;

        private bool connected;

        public bool Connected
        {
            get { return connected; }
        }

        public async Task Connect()
        {
            connected = true;
            await RefreshApplications();
            StateChanged?.Invoke();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Disconnect()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            connected = false;
            Applications.Clear();
            StateChanged?.Invoke();
        }

        public async Task RefreshApplications()
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

        public async Task Deregister(string appId, string instanceId)
        {
            using (HttpClient client = new HttpClient())
            {
                //todo: use more robust concatenation
                Uri uri = new Uri(Url + "/" + appId + "/" + instanceId, UriKind.RelativeOrAbsolute);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
                HttpResponseMessage response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new IOException($"Server responded with HTTP code '{(int) response.StatusCode}'.");
                }
            }
            await RefreshApplications();
        }
    }
}