using Loger;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using ProtocolCommunication.MessagePack.SesionMess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terminal.TerminalWindow;
using Terminal.TerminalWindow.ConnectedClient;

namespace Terminal.SessionWindow
{
    public class SessionClient : ClientObjPrototype
    {
        private SessionModel _SessionModel;
        public byte[] RemotelGuid { get; private set; }
        public SessionClient(Socket c, SessionModel sm, byte[] remotelGuid) : base(c)
        {
            RemotelGuid = remotelGuid;

            _SessionModel = sm;

            if (_SessionModel._SessionW._TerminalModel.TerminalClie__t != null && _SessionModel._SessionW._TerminalModel.TerminalClie__t.IsConnect)
            {
                byte[] data = new DataCover128kb(new TerminalConnectData(MainWindow.TerminalGuid, RemotelGuid, true).Pack(), DataType.sessionInfo).Pack();
                _SessionModel._SessionW._TerminalModel.TerminalClie__t.Write(data);
            }

            EventAfterCloseClient += ClosingClient;
        }
        public override void AceptMessage(DataCover128kb data)
        {
            switch(data.BufferType)
            {
                case DataType.ping:UpdateServerPing(); break;
                case DataType.assignedID: aceptRemdevId(data); break;
                default: break;
            }
        }
        public void UpdateServerPing()
        {
            _SessionModel?.OnPropertyChanged("ServerPing");
        }
        private void aceptRemdevId(DataCover128kb data)
        {
            int id = Protocol.BufferToObject<ValInt32>(data.Data).Value;
            Id.Value = id;
        }
        private void ClosingClient()
        {
            SessionModel.RemoveSessions(_SessionModel);
            if (_SessionModel._SessionW._TerminalModel.TerminalClie__t != null && _SessionModel._SessionW._TerminalModel.TerminalClie__t.IsConnect)
            {
                byte[] data = new DataCover128kb(new TerminalConnectData(MainWindow.TerminalGuid, RemotelGuid, false).Pack(), DataType.sessionInfo).Pack();
                _SessionModel._SessionW._TerminalModel.TerminalClie__t.Write(data);
            }

            LogWriter.SendLog($"Соединение с удаленным устройством {_SessionModel.Title} прервано.", ConsoleColor.Red, true);
            _SessionModel.DispatcherInvokeSessionW((x) => x.Close());
        }
    }
}
