using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal.TerminalWindow.ConnectedClient
{
    public class TerminalClientView : ClientView
    {
        public TerminalClientView(int id, string name, TerminalClient tc__t, TerminalModel tm) : base(id, tc__t, tm)
        {
            Name = name;
        }
        public string Name { get; private set; }
    }
}
