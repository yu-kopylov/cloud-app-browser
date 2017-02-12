namespace CloudAppBrowser.Core.Services.Docker
{
    public class DockerContainerPort
    {
        public DockerContainerPort(string portType, long privatePort, long publicPort)
        {
            PortType = portType;
            PrivatePort = privatePort;
            PublicPort = publicPort;
        }

        public string PortType { get; }
        public long PrivatePort { get; set; }
        public long PublicPort { get; set; }
    }
}