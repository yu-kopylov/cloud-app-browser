using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string Name { get; set; }
        public string MachineName { get; set; }

        //todo: Dispose?
        private readonly object monitor = new object();
        private DockerClient client;
        private readonly Dictionary<string, DockerImage> images = new Dictionary<string, DockerImage>();
        private readonly Dictionary<string, DockerContainer> containers = new Dictionary<string, DockerContainer>();
        private readonly Dictionary<string, DockerLogReader> logReaders = new Dictionary<string, DockerLogReader>();

        private bool connected;

        static DockerService()
        {
            //todo: move to a common place?
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        }

        public delegate void LogChangedEventHandler(string containerId);

        public event Action StageChanged;
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

            await Refresh();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Disconnect()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            connected = false;

            lock (monitor)
            {
                foreach (DockerLogReader logReader in logReaders.Values)
                {
                    logReader.Stop();
                }
                logReaders.Clear();
                images.Clear();
                containers.Clear();
            }

            StageChanged?.Invoke();
        }

        public async Task Refresh()
        {
            await RefreshImagesList();
            await RefreshContainerList();
            StageChanged?.Invoke();
        }

        private async Task RefreshImagesList()
        {
            ImagesListParameters listParameters = new ImagesListParameters();
            listParameters.All = true;
            IList<ImagesListResponse> responseImages = await client.Images.ListImagesAsync(listParameters);

            lock (monitor)
            {
                HashSet<string> removedImageIds = new HashSet<string>(images.Keys);
                foreach (ImagesListResponse responseImage in responseImages)
                {
                    removedImageIds.Remove(responseImage.ID);
                    DockerImage dockerImage;
                    if (!images.TryGetValue(responseImage.ID, out dockerImage))
                    {
                        dockerImage = new DockerImage();
                        images.Add(responseImage.ID, dockerImage);
                    }
                    dockerImage.Id = responseImage.ID;
                    dockerImage.Created = responseImage.Created;
                    dockerImage.RepoTags = new List<string>(responseImage.RepoTags ?? Enumerable.Empty<string>());
                    dockerImage.RepoDigests = new List<string>(responseImage.RepoDigests ?? Enumerable.Empty<string>());
                    dockerImage.Size = responseImage.Size;
                    dockerImage.VirtualSize = responseImage.VirtualSize;
                }
                foreach (string imageId in removedImageIds)
                {
                    images.Remove(imageId);
                }
            }
        }

        private async Task RefreshContainerList()
        {
            ContainersListParameters listParameters = new ContainersListParameters();
            listParameters.All = true;
            IList<ContainerListResponse> responseContainers = await client.Containers.ListContainersAsync(listParameters);

            lock (monitor)
            {
                HashSet<string> removedContainerIds = new HashSet<string>(containers.Keys);
                foreach (ContainerListResponse responseContainer in responseContainers)
                {
                    removedContainerIds.Remove(responseContainer.ID);
                    DockerContainer dockerContainer;
                    if (!containers.TryGetValue(responseContainer.ID, out dockerContainer))
                    {
                        dockerContainer = new DockerContainer();
                        containers.Add(responseContainer.ID, dockerContainer);
                    }

                    dockerContainer.Id = responseContainer.ID;
                    dockerContainer.Image = responseContainer.Image;
                    dockerContainer.ImageId = responseContainer.ImageID;
                    dockerContainer.Created = responseContainer.Created;
                    dockerContainer.State = responseContainer.State;
                    dockerContainer.Ports = responseContainer.Ports.Select(p => new DockerContainerPort(p.Type, p.PrivatePort, p.PublicPort)).ToArray();
                }
                foreach (string containerId in removedContainerIds)
                {
                    containers.Remove(containerId);
                    DockerLogReader logReader;
                    if (logReaders.TryGetValue(containerId, out logReader))
                    {
                        logReader.Stop();
                        logReaders.Remove(containerId);
                    }
                }
            }
        }

        public async Task StartContainers(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStartParameters startParameters = new ContainerStartParameters();
                await client.Containers.StartContainerAsync(containerId, startParameters);
            }
            await Refresh();
        }

        public async Task StopContainers(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStopParameters stopParameters = new ContainerStopParameters();
                await client.Containers.StopContainerAsync(containerId, stopParameters, CancellationToken.None);
            }
            await Refresh();
        }

        public IEnumerable<DockerImage> GetImages()
        {
            lock (monitor)
            {
                return new List<DockerImage>(images.Values);
            }
        }

        public IEnumerable<DockerContainer> GetContainers()
        {
            lock (monitor)
            {
                return new List<DockerContainer>(containers.Values);
            }
        }

        public async Task EnableLogs(string containerId)
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