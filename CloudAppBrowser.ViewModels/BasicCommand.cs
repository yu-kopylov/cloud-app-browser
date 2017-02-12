using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CloudAppBrowser.ViewModels
{
    public class BasicCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Func<object, Task> execute;

        private bool canExecuteValue;

        public BasicCommand(Func<bool> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = obj => Task.Run(() => execute(obj));
            canExecuteValue = canExecute();
        }

        public BasicCommand(Func<bool> canExecute, Func<object, Task> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
            canExecuteValue = canExecute();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public async void Execute(object parameter)
        {
            try
            {
                await execute(parameter);
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"---- {e.GetType().Name}: {e.Message}");
                sb.AppendLine($"---- StackTrace:");
                sb.AppendLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    sb.AppendLine($"---- caused by {e.GetType().Name}: {e.Message}");
                    sb.AppendLine($"---- StackTrace:");
                    sb.AppendLine(e.InnerException.StackTrace);
                }
                ApplicationLog.Instance.LogInfo(sb.ToString());
            }
        }

        public void UpdateState()
        {
            bool canExecuteNow = canExecute();
            if (canExecuteNow != canExecuteValue)
            {
                canExecuteValue = canExecuteNow;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}