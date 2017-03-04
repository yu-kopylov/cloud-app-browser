using System.Collections.Generic;
using Newtonsoft.Json;

namespace CloudAppBrowser.Core.Services.Eureka.Formats
{
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

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("lastUpdatedTimestamp")]
        public long LastUpdatedTimestamp { get; set; }
    }
}