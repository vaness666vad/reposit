using Loger;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using ProtocolCommunication.MessagePack.SesionMess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Terminal.SessionWindow;
using Terminal.TerminalWindow.ConnectedClient;

namespace Terminal.TerminalWindow
{
    public class TerminalClient : ClientObjPrototype
    {
        private TerminalModel _TerminalModel;
        public TerminalClient(Socket c, TerminalModel tm) : base(c)
        {
            _TerminalModel = tm;
            EventAfterCloseClient += ClosingClient;

            foreach(SessionModel sc in SessionModel.GetSessions())
            {
                if (sc._SessionClient.IsConnect)
                {
                    byte[] data = new DataCover128kb(new TerminalConnectData(MainWindow.TerminalGuid, sc._SessionClient.RemotelGuid, true).Pack(), DataType.sessionInfo).Pack();
                    Write(data);
                }
            }
        }

        private void ClosingClient()
        {
            LogWriter.SendLog("Соединение с сервером прервано.", ConsoleColor.Red, true);
            _TerminalModel.DispatcherInvokeWindowView((x) => x.Close());
        }
        public override void AceptMessage(DataCover128kb data)
        {
            switch(data.BufferType)
            {
                case DataType.ping: UpdateServerPing(); break;
                case DataType.connectionQuality: ConnectQuality(data); break;
                case DataType.assignedID: AssignedID(data); break;
                case DataType.connectedClientsInfo: ConnectedClientInfo(data); break;
                case DataType.messageChat: MessageChat(data); break;
                case DataType.sessionInfo: SessionInf(data); break;
                case DataType.createSessionIpPort: TryConnectToRemDev(data); break;
            }
        }
        #region Intake_Messages
        public void UpdateServerPing()
        {
            _TerminalModel.OnPropertyChanged("ServerPing");
        }
        public void ConnectQuality(DataCover128kb data)
        {
            СonnectionQuality cq = Protocol.BufferToObject<СonnectionQuality>(data.Data);
            if (cq.Cli_ntType == ClientType.remote_device)
                _TerminalModel.DispatcherInvokeModel(x =>
                {
                    RemoteDevClientView rd = null;
                    rd = x.ConnectedDevices.FirstOrDefault(xx => xx.Id == cq.ClientId);
                    if (rd != null)
                        rd.Ping = cq.Ping;
                });
        }
        public void SessionInf(DataCover128kb data)
        {
            SessionInfo si = Protocol.BufferToObject<SessionInfo>(data.Data);
            string mess = string.Empty;
            _TerminalModel.DispatcherInvokeModel(x =>
            {
                if (si.Infotype == SessionMessageType.SesInformation || si.Infotype == SessionMessageType.SesCreateNow)
                {
                    SessionView sv = x.ConnectedSessions.FirstOrDefault(xx => xx.Id == si.SessionId);
                    if (sv == null)
                        x.ConnectedSessions.Add(new SessionView(si.SessionId, si.TerminalId, si.RemDevId, this, _TerminalModel));
                    else
                        sv.UpdateIds(si.TerminalId, si.RemDevId);
                    mess = $"Сессия id{si.SessionId} создана, terminal:id{si.TerminalId}, remote_device:id{si.RemDevId}";
                }
                else if(si.Infotype == SessionMessageType.SesShutdownNow || si.Infotype == SessionMessageType.SesLost)
                {
                    x.ConnectedSessions.Remove(x.ConnectedSessions.FirstOrDefault(xx => xx.Id == si.SessionId));
                    mess = $"Сессия id{si.SessionId} прервана, terminal:id{si.TerminalId}, remote_device:id{si.RemDevId}";
                }              
            });
            if (si.Infotype == SessionMessageType.SesCreateNow || si.Infotype == SessionMessageType.SesShutdownNow)
            {
                LogWriter.SendLog(mess);
                _TerminalModel.AddChatMessage(mess);
            }
        }
        public void AssignedID(DataCover128kb data)
        {
            int id = Protocol.BufferToObject<ValInt32>(data.Data).Value;
            Id.Value = id;
            LogWriter.SendLog("Присвоен id " + Id.ToString());
            _TerminalModel.OnPropertyChanged("Title");
        }
        public void ConnectedClientInfo(DataCover128kb data)
        {
            ConnectedClientsInfo cci = Protocol.BufferToObject<ConnectedClientsInfo>(data.Data);
            string mess = string.Empty;
            _TerminalModel.DispatcherInvokeModel((x) => 
            {
                if (cci.TypeClient == ClientType.terminal)
                {
                    if (cci.Connect == ClientInfoConnect.ConnectNow || cci.Connect == ClientInfoConnect.ClientInf)
                    {
                        _TerminalModel.ConnectedTermenals.Add(new TerminalClientView(cci.Id, cci.Name, this, _TerminalModel));
                        mess = $"Подключен терминал {cci.Name}, присвоен id {cci.Id}";
                    }
                    else
                    {
                        foreach (TerminalClientView c in new List<TerminalClientView>(_TerminalModel.ConnectedTermenals.Where(xx => xx.Id == cci.Id)))
                            _TerminalModel.ConnectedTermenals.Remove(c);
                        mess = $"Терминал {cci.Name} отключен, id {cci.Id}";
                    }
                }
                else if (cci.TypeClient == ClientType.remote_device)
                {
                    if (cci.Connect == ClientInfoConnect.ConnectNow || cci.Connect == ClientInfoConnect.ClientInf)
                    {
                        _TerminalModel.ConnectedDevices.Add(new RemoteDevClientView(cci.Id, cci.Name, this, _TerminalModel));
                        mess = $"Подключено удаленное устройство {cci.Name}, присвоен id {cci.Id}";
                    }
                    else
                    {
                        foreach (RemoteDevClientView c in new List<RemoteDevClientView>(_TerminalModel.ConnectedDevices.Where(xx => xx.Id == cci.Id)))
                            _TerminalModel.ConnectedDevices.Remove(c);
                        mess = $"Удаленное устройство {cci.Name} отключено, id {cci.Id}";
                    }
                }
            });
            if (cci.Connect != ClientInfoConnect.ClientInf)
            {
                LogWriter.SendLog(mess);
                _TerminalModel.AddChatMessage(mess);
            }
        }
        public void MessageChat(DataCover128kb data)
        {
            MessageChat mc = Protocol.BufferToObject<MessageChat>(data.Data);
            string mess;
            if (mc.TypeSender == SendrType.server)
                mess = "SERVER\n";
            else
            {
                string name = "unknown";
                if (mc.TypeSender == SendrType.terminal)
                    _TerminalModel.DispatcherInvokeModel((x) => name = x.ConnectedTermenals.FirstOrDefault(xx => xx.Id == mc.Id)?.Name);
                else if (mc.TypeSender == SendrType.remote_device)
                    _TerminalModel.DispatcherInvokeModel((x) => name = x.ConnectedDevices.FirstOrDefault(xx => xx.Id == mc.Id)?.Name);

                mess = $"id{Id.Value}:{name}:{mc.TypeSender}\n";
            }
            mess += mc.Text;
            LogWriter.SendLog(mess);
            _TerminalModel.AddChatMessage(mess);
        }
        public void TryConnectToRemDev(DataCover128kb data)
        {
            ConnectedInfo ci = Protocol.BufferToObject<ConnectedInfo>(data.Data);
            _TerminalModel.TWMV.Dispatcher.Invoke(() =>
            {
                SessionStarter st = new SessionStarter((IPEndPoint)Client.LocalEndPoint, new IPEndPoint(new IPAddress(ci.Ip), ci.Port), ci.Pass, _TerminalModel);
                st.Show();
            });
        }
        #endregion
        #region Exhoust_Messages
        public void SendChatMessage(string text)
        {
            byte[] bufer = new DataCover128kb(new MessageChat(Id.Value, SendrType.terminal, text).Pack(), DataType.messageChat).Pack();
            Write(bufer);
        }
        public void CreateSession(RemoteDevClientView remote)
        {
            DataCover128kb dc128 = new DataCover128kb(new CreateSession(remote.Id, MainWindow.CW.IsBind? MainWindow.CW.BindPort : -1).Pack(), DataType.requistCreateSession);
            Write(dc128.Pack());
        }
        #endregion
    }
}
