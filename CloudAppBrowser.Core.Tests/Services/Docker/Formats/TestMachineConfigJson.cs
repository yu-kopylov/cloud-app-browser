using System.IO;
using CloudAppBrowser.Core.Services.Docker.Formats;
using NUnit.Framework;

namespace CloudAppBrowser.Core.Tests.Services.Docker.Formats
{
    [TestFixture]
    public class TestMachineConfigJson
    {
        [Test]
        public void Test()
        {
            byte[] text = ReadConfigText();
            MachineConfigJson conf = MachineConfigJson.Read(text);

            Assert.That(conf.Driver.IPAddress, Is.EqualTo("192.168.99.100"));
            Assert.That(conf.HostOptions.AuthOptions.ClientCertPath, Is.EqualTo("C:\\Users\\User1\\.docker\\machine\\certs\\cert.pem"));
            Assert.That(conf.HostOptions.AuthOptions.ClientKeyPath, Is.EqualTo("C:\\Users\\User1\\.docker\\machine\\certs\\key.pem"));
        }

        private byte[] ReadConfigText()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (Stream stream = typeof(TestMachineConfigJson).Assembly.GetManifestResourceStream(typeof(TestMachineConfigJson), "config.json"))
                {
                    stream.CopyTo(mem);
                }
                return mem.ToArray();
            }
        }
    }
}