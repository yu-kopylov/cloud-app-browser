using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Docker.DotNet;
using Docker.DotNet.Models;
using Docker.DotNet.X509;

namespace CloudAppBrowser.Core.Services
{
    public class DockerService : IService
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string CertificateFile { get; set; }
        public string CertificateKeyFile { get; set; }
        public List<DockerContainer> Containers { get; } = new List<DockerContainer>();

        //todo: Dispose?
        private DockerClient client;

        static DockerService()
        {
            //todo: move to a common place?
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
        }

        public void Connect()
        {
            X509Certificate2 cert = CertificateUtils.ReadCertificateWithKey(CertificateFile, CertificateKeyFile, null);
            CertificateCredentials credentials = new CertificateCredentials(cert);

            DockerClientConfiguration config = new DockerClientConfiguration(new Uri(Url), credentials);
            client = config.CreateClient();

            RefreshContainerList();
        }

        private void RefreshContainerList()
        {
            ContainersListParameters listParameters = new ContainersListParameters();
            listParameters.All = true;

            IList<ContainerListResponse> responseContainers = client.Containers.ListContainersAsync(listParameters).Result;

            Containers.Clear();
            foreach (ContainerListResponse responseContainer in responseContainers)
            {
                DockerContainer dockerContainer = new DockerContainer();
                Containers.Add(dockerContainer);

                dockerContainer.Id = responseContainer.ID;
                dockerContainer.Image = responseContainer.Image;
                dockerContainer.ImageId = responseContainer.ImageID;
                dockerContainer.Created = responseContainer.Created;
                dockerContainer.State = responseContainer.State;
            }
        }
    }
}