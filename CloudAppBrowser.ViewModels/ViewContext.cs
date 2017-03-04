using System;

namespace CloudAppBrowser.ViewModels
{
    public abstract class ViewContext
    {
        public abstract void Invoke(Action action);

        public abstract void MessageBox(string message, string caption);

        public abstract bool ShowDialog(object viewModel);

        public abstract object CreatePanel(object viewModel);
    }
}