using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CloudAppBrowser.Core.Configuration
{
    public class AppConfig
    {
        public List<EnvironmentConfig> Environments { get; set; } = new List<EnvironmentConfig>();

        public static void WriteTo(TextWriter writer, AppConfig config)
        {
            Serializer serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            serializer.Serialize(writer, config);
        }

        public static AppConfig ReadFrom(TextReader reader)
        {
            Deserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            return deserializer.Deserialize<AppConfig>(reader);
        }
    }

    public class EnvironmentConfig
    {
        public string Name { get; set; }
        public List<ServiceConfig> Services { get; set; } = new List<ServiceConfig>();
    }

    public class ServiceConfig
    {
        public string Name { get; set; }
        public EurekaServiceConfig Eureka { get; set; }
        public DockerServiceConfig Docker { get; set; }
    }

    public class EurekaServiceConfig
    {
        public string Url { get; set; }
    }

    public class DockerServiceConfig
    {
        public string Url { get; set; }
    }
}