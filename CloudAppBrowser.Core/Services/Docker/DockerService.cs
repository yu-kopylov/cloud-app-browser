using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CloudAppBrowser.Core.Services.Docker.Formats;
using Docker.DotNet;
using Docker.DotNet.Models;
using Docker.DotNet.X509;

namespace CloudAppBrowser.Core.Services.Docker
{
    public class DockerService : IService
    {
        private static readonly BasicThreadPool ThreadPool = new BasicThreadPool();

        public string Name { get; set; }
        public string MachineName { get; set; }

        //todo: Dispose?
        private readonly object monitor = new object();
        private DockerClient client;
        private readonly List<DockerContainer> containers = new List<DockerContainer>();
        private readonly Dictionary<string, DockerLogReader> logReaders = new Dictionary<string, DockerLogReader>();

        private bool connected;

        static DockerService()
        {
            //todo: move to a common place?
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        }

        public delegate void LogChangedEventHandler(string containerId);

        public event Action ContainersChanged;
        public event LogChangedEventHandler LogChanged;

        public bool Connected
        {
            get { return connected; }
        }

        public async Task Connect()
        {
            connected = true;

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string configPath = Path.Combine(userProfile, @".docker\machine\machines", MachineName, "config.json");
            MachineConfigJson machineConfig = MachineConfigJson.Read(File.ReadAllBytes(configPath));

            string clientCertPath = machineConfig.HostOptions.AuthOptions.ClientCertPath;
            string clientKeyPath = machineConfig.HostOptions.AuthOptions.ClientKeyPath;
            Uri uri = new Uri($"http://{machineConfig.Driver.IPAddress}:2376");

            X509Certificate2 cert = CertificateUtils.ReadCertificateWithKey(clientCertPath, clientKeyPath, null);
            CertificateCredentials credentials = new CertificateCredentials(cert);

            DockerClientConfiguration config = new DockerClientConfiguration(uri, credentials);
            client = config.CreateClient();

            await RefreshContainerList();
        }

        public async Task Disconnect()
        {
            connected = false;

            foreach (DockerLogReader logReader in logReaders.Values)
            {
                logReader.Stop();
            }
            logReaders.Clear();
            containers.Clear();

            ContainersChanged?.Invoke();
        }

        public async Task RefreshContainerList()
        {
            ContainersListParameters listParameters = new ContainersListParameters();
            listParameters.All = true;
            IList<ContainerListResponse> responseContainers = await client.Containers.ListContainersAsync(listParameters);

            lock (monitor)
            {
                containers.Clear();
                foreach (ContainerListResponse responseContainer in responseContainers)
                {
                    DockerContainer dockerContainer = new DockerContainer();
                    containers.Add(dockerContainer);

                    dockerContainer.Id = responseContainer.ID;
                    dockerContainer.Image = responseContainer.Image;
                    dockerContainer.ImageId = responseContainer.ImageID;
                    dockerContainer.Created = responseContainer.Created;
                    dockerContainer.State = responseContainer.State;
                }
            }

            ContainersChanged?.Invoke();
        }

        public void StartContainers(List<string> containerIds)
        {
            ThreadPool.Execute(() => StartContainersAsync(containerIds));
        }

        private async void StartContainersAsync(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStartParameters startParameters = new ContainerStartParameters();
                await client.Containers.StartContainerAsync(containerId, startParameters);
            }
            RefreshContainerList();
        }

        public void StopContainers(List<string> containerIds)
        {
            ThreadPool.Execute(() => StopContainersAsync(containerIds));
        }

        private async void StopContainersAsync(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStopParameters stopParameters = new ContainerStopParameters();
                await client.Containers.StopContainerAsync(containerId, stopParameters, CancellationToken.None);
            }
            RefreshContainerList();
        }

        public IEnumerable<DockerContainer> GetContainers()
        {
            lock (monitor)
            {
                return new List<DockerContainer>(containers);
            }
        }

        public void EnableLogs(string containerId)
        {
            ThreadPool.Execute(() => EnableLogsAsync(containerId));
        }

        private async void EnableLogsAsync(string containerId)
        {
            ContainerLogsParameters logsParameters = new ContainerLogsParameters();
            logsParameters.Follow = true;
            logsParameters.ShowStdout = true;
            logsParameters.ShowStderr = true;
            logsParameters.Tail = "1000";
            logsParameters.Timestamps = true;
            Stream stream = await client.Containers.GetContainerLogsAsync(containerId, logsParameters, CancellationToken.None);

            lock (monitor)
            {
                DockerLogReader reader;
                if (logReaders.TryGetValue(containerId, out reader))
                {
                    reader.Stop();
                }
                logReaders[containerId] = new DockerLogReader(this, containerId, stream);
            }
        }

        internal void NotifyLogChanged(string containerId)
        {
            LogChanged?.Invoke(containerId);
        }

        public string GetLog(string containerId)
        {
            DockerLogReader reader;
            lock (monitor)
            {
                if (!logReaders.TryGetValue(containerId, out reader))
                {
                    return null;
                }
            }
            return reader.Log;
        }
    }
}