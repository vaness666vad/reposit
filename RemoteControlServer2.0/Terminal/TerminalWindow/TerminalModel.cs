using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using Terminal.SessionWindow;
using Terminal.TerminalWindow.ConnectedClient;

namespace Terminal.TerminalWindow
{
    public class TerminalModel : BaseModel
    {
        public TerminalClient TerminalClie__t { get; private set; }
        public TerminalWindowView TWMV { get; private set; }
        public string Title  => $"Id{baseClient.Id.Value}:{TWMV.TitleName}";
        public TerminalModel(Socket client, TerminalWindowView twv)
        {
            TWMV = twv;
            ConnectedTermenals = new ObservableCollection<TerminalClientView>();
            ConnectedDevices = new ObservableCollection<RemoteDevClientView>();
            ConnectedSessions = new ObservableCollection<SessionView>();
            TerminalClie__t = new TerminalClient(client, this);
            baseClient = TerminalClie__t;
        }
        public ObservableCollection<TerminalClientView> ConnectedTermenals { get; private set; }
        public ObservableCollection<RemoteDevClientView> ConnectedDevices { get; private set; }
        public ObservableCollection<SessionView> ConnectedSessions { get; private set; }
        public void DispatcherInvokeModel(Action<TerminalModel> act)
        {
            TWMV.Dispatcher.Invoke(act, this);
        }
        public void DispatcherInvokeWindowView(Action<TerminalWindowView> act)
        {
            TWMV.Dispatcher.Invoke(act, TWMV);
        }
        public void AddChatMessage(string mess)
        {
            DispatcherInvokeWindowView((x) => x.TextBoxChat.AppendText($"[{DateTime.Now}]:{mess}\n"));
        }
    }
}
