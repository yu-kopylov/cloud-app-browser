﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using Docker.DotNet.X509;

namespace CloudAppBrowser.Core.Services.Docker
{
    public class DockerService : IService
    {
        private static readonly BasicThreadPool ThreadPool = new BasicThreadPool();

        public string Name { get; set; }
        public string Url { get; set; }
        public string CertificateFile { get; set; }
        public string CertificateKeyFile { get; set; }

        //todo: Dispose?
        private readonly object monitor = new object();
        private DockerClient client;
        private readonly List<DockerContainer> containers = new List<DockerContainer>();
        private readonly Dictionary<string, DockerLogReader> logReaders = new Dictionary<string, DockerLogReader>();

        static DockerService()
        {
            //todo: move to a common place?
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        }

        public delegate void LogChangedEventHandler(string containerId, string log);

        public event Action ContainersChanged;
        public event LogChangedEventHandler LogChanged;

        public void Connect()
        {
            X509Certificate2 cert = CertificateUtils.ReadCertificateWithKey(CertificateFile, CertificateKeyFile, null);
            CertificateCredentials credentials = new CertificateCredentials(cert);

            DockerClientConfiguration config = new DockerClientConfiguration(new Uri(Url), credentials);
            client = config.CreateClient();

            ThreadPool.Start(RefreshContainerListAsync);
        }

        public void RefreshContainerList()
        {
            ThreadPool.Start(RefreshContainerListAsync);
        }

        private async void RefreshContainerListAsync()
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
            ThreadPool.Start(() => StartContainersAsync(containerIds));
        }

        private async void StartContainersAsync(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStartParameters startParameters = new ContainerStartParameters();
                await client.Containers.StartContainerAsync(containerId, startParameters);
            }
            RefreshContainerListAsync();
        }

        public void StopContainers(List<string> containerIds)
        {
            ThreadPool.Start(() => StopContainersAsync(containerIds));
        }

        private async void StopContainersAsync(List<string> containerIds)
        {
            foreach (string containerId in containerIds)
            {
                ContainerStopParameters stopParameters = new ContainerStopParameters();
                await client.Containers.StopContainerAsync(containerId, stopParameters, CancellationToken.None);
            }
            RefreshContainerListAsync();
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
            ThreadPool.Start(() => EnableLogsAsync(containerId));
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

        internal void NotifyLogChanged(string containerId, string log)
        {
            LogChanged?.Invoke(containerId, log);
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