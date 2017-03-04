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

        private ISet<Tuple<string, string>> deregisteredInstances = new HashSet<Tuple<string, string>>();

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
                            instance.Status = instanceJson.Status;
                        }
                    }

                    await RefreshDeregisteredInstances();

                    StateChanged?.Invoke();
                }
            }
        }

        private async Task RefreshDeregisteredInstances()
        {
            ISet<Tuple<string, string>> listedInstances = new HashSet<Tuple<string, string>>();

            using (HttpClient client = new HttpClient())
            {
                foreach (EurekaApplication app in Applications)
                {
                    foreach (EurekaApplicationInstance instance in app.Instances)
                    {
                        Tuple<string, string> instanceKey = Tuple.Create(app.Name, instance.InstanceId);
                        listedInstances.Add(instanceKey);

                        if (deregisteredInstances.Contains(instanceKey))
                        {
                            //todo: use more robust concatenation
                            Uri uri = new Uri(Url + "/" + app.Name + "/" + instance.InstanceId, UriKind.RelativeOrAbsolute);
                            HttpResponseMessage instanceResponse = await client.GetAsync(uri);
                            if (instanceResponse.IsSuccessStatusCode)
                            {
                                deregisteredInstances.Remove(instanceKey);
                                listedInstances.Add(instanceKey);
                            }
                            else if (instanceResponse.StatusCode == HttpStatusCode.NotFound)
                            {
                                instance.Status += " (REMOVED)";
                            }
                        }
                    }
                }
            }

            deregisteredInstances.IntersectWith(listedInstances);
        }

        public async Task Deregister(string appId, string instanceId)
        {
            deregisteredInstances.Add(Tuple.Create(appId, instanceId));

            using (HttpClient client = new HttpClient())
            {
                //todo: use more robust concatenation
                Uri uri = new Uri(Url + "/" + appId + "/" + instanceId, UriKind.RelativeOrAbsolute);
                HttpResponseMessage response = await client.DeleteAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new IOException($"Server responded with HTTP code '{(int) response.StatusCode}'.");
                }
            }

            await RefreshApplications();
        }
    }
}