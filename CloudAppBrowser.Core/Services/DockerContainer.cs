using System;

namespace CloudAppBrowser.Core.Services
{
    public class DockerContainer
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string ImageId { get; set; }
        public DateTime Created { get; set; }
        public string State { get; set; }
    }
}