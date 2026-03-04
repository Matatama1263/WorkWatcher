using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkWatcher.Bases
{
    public class Command : ICommand
    {
        Action<object?> ExecuteAction;
        Func<object?, bool> CanExecuteFunc;

        public Command(Action<object?> executeAction, Func<object?, bool> canExecuteFunc)
        {
            ExecuteAction = executeAction;
            CanExecuteFunc = canExecuteFunc;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteAction(parameter);
        }
    }
}
