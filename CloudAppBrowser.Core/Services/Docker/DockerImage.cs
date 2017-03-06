using System;
using System.Collections.Generic;

namespace CloudAppBrowser.Core.Services.Docker
{
    public class DockerImage
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public List<string> RepoTags { get; set; }
        public List<string> RepoDigests { get; set; }
        public long Size { get; set; }
        public long VirtualSize { get; set; }
    }
}