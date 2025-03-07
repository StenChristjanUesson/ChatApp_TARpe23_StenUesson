using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp_TARpe23_StenUesson.MVVM.Core
{
    public class RelayCommand : ICommand
    {
        private Action<Object> execute;
        private Func<Object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(Object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            this.execute(parameter);
        }
    }
}
