using System;
using System.Windows.Input;

namespace CloudAppBrowser.ViewModels.Subsystems.Docker
{
    public class BasicCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action<object> execute;

        private bool canExecuteValue;

        public BasicCommand(Func<bool> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
            canExecuteValue = canExecute();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute(parameter);
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