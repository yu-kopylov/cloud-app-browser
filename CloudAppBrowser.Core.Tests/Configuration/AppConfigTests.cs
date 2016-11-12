using System.IO;
using CloudAppBrowser.Core.Configuration;
using NUnit.Framework;

namespace CloudAppBrowser.Core.Tests.Configuration
{
    [TestFixture]
    public class AppConfigTests
    {
        [Test]
        public void TestSmoke()
        {
            AppConfig appConfig = new AppConfig
            {
                Environments =
                {
                    new EnvironmentConfig
                    {
                        Name = "Env1",
                        Services =
                        {
                            new ServiceConfig
                            {
                                Name = "Eureka1",
                                Eureka = new EurekaServiceConfig {Url = "http://xxx.yyy:123"}
                            },
                            new ServiceConfig
                            {
                                Name = "Eureka2",
                                Eureka = new EurekaServiceConfig {Url = "http://xxx.yyy:123"}
                            },
                            new ServiceConfig
                            {
                                Name = "Docker",
                                Docker = new DockerServiceConfig {Url = "http://xxx.yyy:123"}
                            }
                        }
                    },
                    new EnvironmentConfig
                    {
                        Name = "Env2",
                        Services =
                        {
                            new ServiceConfig
                            {
                                Name = "Docker",
                                Docker = new DockerServiceConfig {Url = "http://xxx.yyy:123"}
                            }
                        }
                    }
                }
            };

            string config;
            using (StringWriter writer = new StringWriter())
            {
                AppConfig.WriteTo(writer, appConfig);
                config = writer.ToString();
            }

            System.Console.WriteLine(config);

            AppConfig appConfig2;
            using (StringReader reader = new StringReader(config))
            {
                appConfig2 = AppConfig.ReadFrom(reader);
            }

            Assert.That(appConfig2.Environments.Count, Is.EqualTo(appConfig.Environments.Count));
            Assert.That(appConfig2.Environments[0].Services.Count, Is.EqualTo(appConfig.Environments[0].Services.Count));
            Assert.That(appConfig2.Environments[0].Services[0].Name, Is.EqualTo(appConfig.Environments[0].Services[0].Name));
        }
    }
}