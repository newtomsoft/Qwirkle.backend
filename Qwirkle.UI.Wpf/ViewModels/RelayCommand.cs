using System;
using System.Windows.Input;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _action;

        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action action, Predicate<object> canExecute = null)
        {
            _action = action ?? throw new ArgumentNullException("action is not defined");
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

}
