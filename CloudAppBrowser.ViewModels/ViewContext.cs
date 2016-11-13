using System;

namespace CloudAppBrowser.ViewModels
{
    public abstract class ViewContext
    {
        public static ViewContext Instance { get; set; }

        public abstract void Invoke(Action action);
    }
}