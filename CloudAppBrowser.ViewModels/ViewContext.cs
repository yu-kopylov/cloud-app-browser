using System;

namespace CloudAppBrowser.ViewModels
{
    public abstract class ViewContext
    {
        public static ViewContext Instance { get; set; }

        public abstract void Invoke(Action action);

        public abstract void MessageBox(string message, string caption);

        public abstract IViewResolver ViewResolver { get; }

        public bool ShowDialog(object viewModel)
        {
            return ViewResolver.ShowDialog(viewModel);
        }
    }
}