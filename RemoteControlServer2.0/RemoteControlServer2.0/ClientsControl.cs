using Loger;
using ProtectedValues;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using ProtocolCommunication.MessagePack.SesionMess;
using RemoteControlServer2._0.ClientObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlServer2._0
{
    public static class ClientsControl
    {
        private static List<Terminal> Terminals;
        private static List<RemoteDevice> Devices;
        private static List<SessionData> Sessions;
        private static AutoResetEvent are;
        static ClientsControl()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClientsControlDestructor);
            are = new AutoResetEvent(true);
            Terminals = new List<Terminal>();
            Devices = new List<RemoteDevice>();
            Sessions = new List<SessionData>();
        }
        public static void ClientsControlDestructor(object sender, EventArgs e)
        {
            try { are.Close(); } catch { }
            try { are.Dispose(); } catch { }
            LogWriter.SendLog("Завершение работы сервера");
        }
        #region Terminal
        public static List<Terminal> GetTerminals()
        {
            List<Terminal> res;
            are.WaitOne();
            res = new List<Terminal>(Terminals);
            are.Set();
            return res;
        }
        public static void AddTerminal(Terminal t)
        {
            are.WaitOne();
            addTerminal(t);
            are.Set();
        }
        private static void addTerminal(Terminal t)
        {
            if (t.IsConnect)
            {
                byte[] bufer = new DataCover128kb(new ConnectedClientsInfo(t.Name, t.Id.Value, ClientInfoConnect.ConnectNow, ClientType.terminal).Pack(), DataType.connectedClientsInfo).Pack();
                Terminals.Add(t);
                foreach (Terminal term in Terminals)
                {
                    term.Write(bufer);
                    byte[] buf = new DataCover128kb(new ConnectedClientsInfo(term.Name, term.Id.Value, ClientInfoConnect.ClientInf, ClientType.terminal).Pack(), DataType.connectedClientsInfo).Pack();
                    if(term.Id.Value != t.Id.Value)
                        t.Write(buf);
                }
                foreach (RemoteDevice rd in Devices)
                {
                    byte[] buf = new DataCover128kb(new ConnectedClientsInfo(rd.Name, rd.Id.Value, ClientInfoConnect.ClientInf, ClientType.remote_device).Pack(), DataType.connectedClientsInfo).Pack();
                    t.Write(buf);
                }
                foreach(SessionData sd in Sessions)
                {
                    byte[] buf = new DataCover128kb(new SessionInfo(sd.Id, sd.GetTerminalId(Terminals), sd.GetRemoteDevId(Devices), SessionMessageType.SesInformation).Pack(), DataType.sessionInfo).Pack();
                    t.Write(buf);
                }
                LogWriter.SendLog($"Подключен терминал {t.Name}, присвоен id {t.Id.Value}");
            }
        }
        public static void RemoveTerminal(Terminal t)
        {
            are.WaitOne();
            removeTerminal(t);
            are.Set();
        }
        public static void removeTerminal(Terminal t)
        {
            t.CloseClient();

            if (Terminals.Remove(t))
            {
                byte[] bufer = new DataCover128kb(new ConnectedClientsInfo(t.Name, t.Id.Value, ClientInfoConnect.DisconectNow, ClientType.terminal).Pack(), DataType.connectedClientsInfo).Pack();
                foreach (Terminal term in Terminals)
                    term.Write(bufer);
                LogWriter.SendLog($"Терминал {t.Name} отключен, id { t.Id.Value}");

                SessionData sd = Sessions.FirstOrDefault(x => x.TerminalGuid.SequenceEqual(t.ClientGuid));
                if (sd != null)
                    tryLostSession(sd);
            }
        }
        #endregion
        #region Device
        public static List<RemoteDevice> GetDevices()
        {
            List<RemoteDevice> res;
            are.WaitOne();
            res = new List<RemoteDevice>(Devices);
            are.Set();
            return res;
        }
        public static void AddDevice(RemoteDevice t)
        {
            are.WaitOne();
            addDevice(t);
            are.Set();
        }
        private static void addDevice(RemoteDevice t)
        {
            if (t.IsConnect)
            {
                Devices.Add(t);
                byte[] bufer = new DataCover128kb(new ConnectedClientsInfo(t.Name, t.Id.Value, ClientInfoConnect.ConnectNow, ClientType.remote_device).Pack(), DataType.connectedClientsInfo).Pack();
                foreach (Terminal term in Terminals)
                    term.Write(bufer);

                LogWriter.SendLog($"Подключено удаленное устройство {t.Name}, присвоен id {t.Id.Value}");
            }
        }
        public static void RemoveDevice(RemoteDevice t)
        {
            are.WaitOne();
            removeDevice(t);
            are.Set();
        }
        private static void removeDevice(RemoteDevice t)
        {
            t.CloseClient();

            if (Devices.Remove(t))
            {
                byte[] bufer = new DataCover128kb(new ConnectedClientsInfo(t.Name, t.Id.Value, ClientInfoConnect.DisconectNow, ClientType.remote_device).Pack(), DataType.connectedClientsInfo).Pack();
                foreach (Terminal term in Terminals)
                    term.Write(bufer);
                LogWriter.SendLog($"Удаленное устройство {t.Name} отключено, id {t.Id.Value}");

                SessionData sd = Sessions.FirstOrDefault(x => x.RemDevGuid.SequenceEqual(t.ClientGuid));
                if (sd != null)
                    tryLostSession(sd);
            }
        }
        #endregion
        #region Sessions
        public static List<SessionData> GetSessions()
        {
            List<SessionData> res;
            are.WaitOne();
            res = new List<SessionData>(Sessions);
            are.Set();
            return res;
        }
        public static void AddSession(SessionData t)
        {
            are.WaitOne();
            addSession(t);
            are.Set();
        }
        private static void addSession(SessionData t)
        {
            Sessions.Add(t);

            int idt = t.GetTerminalId(Terminals);
            int idr = t.GetRemoteDevId(Devices);
            byte[] dc128 = new DataCover128kb(new SessionInfo(t.Id, idt, idr, SessionMessageType.SesCreateNow).Pack(), DataType.sessionInfo).Pack();
            foreach (Terminal te in Terminals)
                te.Write(dc128);

            LogWriter.SendLog($"Создана новая сессия {t.Id}:{idt}:{idr}");
        }
        public static void RemoveSession(SessionData t)
        {
            are.WaitOne();
            removeSession(t);
            are.Set();
        }
        public static void TryLostSession(SessionData t)
        {
            are.WaitOne();
            tryLostSession(t);
            are.Set();
        }
        private static void removeSession(SessionData t)
        {
            if (Sessions.Remove(t))
            {
                int idt = t.GetTerminalId(Terminals);
                int idr = t.GetRemoteDevId(Devices);
                byte[] dc128 = new DataCover128kb(new SessionInfo(t.Id, idt, idr, SessionMessageType.SesShutdownNow).Pack(), DataType.sessionInfo).Pack();
                foreach (Terminal te in Terminals)
                    te.Write(dc128);

                LogWriter.SendLog($"Cессия более не зарегестрирована {t.Id}:{idt}:{idr}");
            }
        }
        private static void tryLostSession(SessionData t)
        {
            int idt = t.GetTerminalId(Terminals);
            int idr = t.GetRemoteDevId(Devices);
            if (idt == -1 && idr == -1 && Sessions.Remove(t))
            {
                byte[] dc128 = new DataCover128kb(new SessionInfo(t.Id, idt, idr, SessionMessageType.SesLost).Pack(), DataType.sessionInfo).Pack();
                foreach (Terminal te in Terminals)
                    te.Write(dc128);

                LogWriter.SendLog($"Cессия вышла из зоны видимости {t.Id}:{idt}:{idr}");
            }
        }
        #endregion
        public static void SendGlobalChat(string mess)
        {
            byte[] buf = new DataCover128kb(new MessageChat(-1, SendrType.server, mess).Pack(), DataType.messageChat).Pack();
            foreach (Terminal t in GetTerminals())
                t.Write(buf);
        }
    }
}
