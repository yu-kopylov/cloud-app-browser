using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CloudAppBrowser.Core;
using CloudAppBrowser.ViewModels.Annotations;

namespace CloudAppBrowser.ViewModels
{
    public class ApplicationLog : INotifyPropertyChanged
    {
        private const int MaxLength = 64 * 1024;

        public static ApplicationLog Instance { get; } = new ApplicationLog();

        private string logText = "";
        private readonly object monitor = new object();

        public string LogText
        {
            get
            {
                lock (monitor)
                {
                    return logText;
                }
            }
        }

        public void LogInfo(string text)
        {
            lock (monitor)
            {
                string newText = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) + ": " + text + "\n" + logText;
                logText = newText.Abbreviate(MaxLength);
            }

            ViewContext.Instance.Invoke(() => OnPropertyChanged("LogText"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}