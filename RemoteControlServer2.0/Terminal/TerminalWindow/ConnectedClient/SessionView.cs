using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal.TerminalWindow.ConnectedClient
{
    public class SessionView : ClientView
    {
        public SessionView(int id, int tid, int rid, TerminalClient tc__t, TerminalModel tm) : base(id, tc__t, tm)
        {
            IdTerminal = tid;
            IdRemDev = rid;
            //Con__Menu.Add(new MyContextItem("Завершить сессию", CloseSession));
        }
        public int IdTerminal { get; private set; }
        public int IdRemDev { get; private set; }
        public string NameTerminal
        {
            get
            {
                string res = null;
                TM.DispatcherInvokeModel((x) => res = x.ConnectedTermenals.FirstOrDefault(xx => xx.Id == IdTerminal)?.Name);
                res = res ?? "unknown";
                return res;
            }
        }
        public string NameRemDev
        {
            get
            {
                string res = null;
                TM.DispatcherInvokeModel((x) => res = x.ConnectedDevices.FirstOrDefault(xx => xx.Id == IdRemDev)?.Name);
                res = res ?? "unknown";
                return res;
            }
        }
        public void UpdateIds(int terid, int remid)
        {
            IdTerminal = terid;
            IdRemDev = remid;
            OnPropertyChanged("IdTerminal");
            OnPropertyChanged("IdRemDev");
            OnPropertyChanged("NameTerminal");
            OnPropertyChanged("NameRemDev");
        }
    }
}
