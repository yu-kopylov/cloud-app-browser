using System;
using System.Collections.Concurrent;
using System.Threading;
using NLog;

namespace CloudAppBrowser.Core
{
    public class BasicThreadPool
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlockingCollection<Action> workItems = new BlockingCollection<Action>();

        public BasicThreadPool()
        {
            Thread thread = new Thread(MainLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        // ReSharper disable once FunctionNeverReturns
        private void MainLoop()
        {
            while (true)
            {
                Action action = workItems.Take();
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception in a workitem.");
                }
            }
        }

        public void Start(Action action)
        {
            workItems.Add(action);
        }
    }
}