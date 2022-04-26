using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Terminal
{
    public class CustomCommand : ICommand
    {
        public CustomCommand(Action act)
        {
            ACT = act;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ACT?.Invoke();
        }
        private Action ACT;
    }
}
