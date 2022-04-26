using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Terminal
{
    public class MyContextItem : NotifyPropertyChangeClass
    {
        public MyContextItem(string title, Action act)
        {
            Title = title;
            ACT = new CustomCommand(act);
        }
        public string Title { get; private set; }
        public ICommand ACT { get; private set; }
    }
}
