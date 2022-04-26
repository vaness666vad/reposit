using Loger;
using ProtocolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Terminal.TerminalWindow;
using Terminal.TerminalWindow.ConnectedClient;

namespace Terminal.SessionWindow
{
    /// <summary>
    /// Логика взаимодействия для SessionW.xaml
    /// </summary>
    public partial class SessionW : Window
    {
        public SessionModel _SessionModel { get; private set; }
        public TerminalModel _TerminalModel { get; private set; }
        public SessionW(TerminalModel tm, Socket s, string remoteName, byte[] remotelGuid)
        {
            _TerminalModel = tm;
            _SessionModel = new SessionModel(this, s, remoteName, remotelGuid);
            InitializeComponent();
            DataContext = _SessionModel;
        }

        private void ClosedControlPanel(object sender, EventArgs e)
        {
            _SessionModel._SessionClient.CloseClient();
        }
    }
}
