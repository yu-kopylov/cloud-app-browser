using System.IO;
using System.Text;
using System.Threading;

namespace CloudAppBrowser.Core.Services.Docker
{
    public class DockerLogReader
    {
        private const int BufferSize = 16*1024;
        private const int MaxLogLength = 256*1024;

        private readonly DockerService service;
        private readonly string containerId;
        private readonly Stream stream;

        private readonly Thread thread;

        private readonly object monitor = new object();
        private volatile bool running;
        private string log;

        public DockerLogReader(DockerService service, string containerId, Stream stream)
        {
            this.service = service;
            this.containerId = containerId;
            this.stream = stream;

            running = true;
            log = "";

            thread = new Thread(MainLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        public string Log
        {
            get
            {
                lock (monitor)
                {
                    return log;
                }
            }
        }

        private void MainLoop()
        {
            byte[] buffer = new byte[BufferSize];
            while (running)
            {
                int bytesRead = stream.Read(buffer, 0, BufferSize);
                if (bytesRead > 0)
                {
                    string text = ParseMessage(buffer, 0, bytesRead);
                    lock (monitor)
                    {
                        log += text;
                        if (log.Length > MaxLogLength)
                        {
                            log = log.Substring(log.Length - MaxLogLength);
                        }
                    }
                    service.NotifyLogChanged(containerId, log);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private string ParseMessage(byte[] buffer, int offset, int length)
        {
            bool hasHeader = length >= 8 && (buffer[offset + 1] == 0) && (buffer[offset + 2] == 0) && (buffer[offset + 3] == 0);
            if (hasHeader)
            {
                return Encoding.ASCII.GetString(buffer, offset + 8, length - 8);
            }
            return Encoding.ASCII.GetString(buffer, offset, length);
        }

        public void Stop()
        {
            running = false;
        }
    }
}