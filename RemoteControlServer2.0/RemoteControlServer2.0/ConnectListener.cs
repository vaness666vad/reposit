using Loger;
using ProtectedValues;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using RemoteControlServer2._0.ClientObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlServer2._0
{
    /// <summary>
    /// Клас реализует регистрационные действия новых подключений
    /// </summary>
    public class ConnectListener
    {
        private Socket listener;
        private Thread thread;
        public IPAddress CurrentAdress { get => currentAdress.Value; private set => currentAdress.Value = value; }
        private ProtectedVal<IPAddress> currentAdress;
        public int CurrentPort { get => currentPort.Value; private set => currentPort.Value = value; }
        private ProtectedVal<int> currentPort;
        public bool IsOpen { get => isOpen.Value; private set => isOpen.Value = value; }
        private ProtectedVal<bool> isOpen;
        public string Login { get => login.Value; private set => login.Value = value; }
        private ProtectedVal<string> login;
        public string Pass { get => pass.Value; private set => pass.Value = value; }
        private ProtectedVal<string> pass;
        public ConnectListener(IPAddress address, int port, string login = null, string pass = null)
        {
            Login = login;
            Pass = pass;
            CurrentAdress = address;
            CurrentPort = port;
        }
        ~ConnectListener()
        {
            CloseListener();
        }
        /// <summary>
        /// Завершает процесс регистрации новых клиентов
        /// </summary>
        public void CloseListener()
        {
            if(IsOpen)
            {
                IsOpen = false;
                listener.Disconnect(false);
                listener.Dispose();
                thread.Abort();
                LogWriter.SendLog("Процесс регистрации новых клиентов по " + CurrentAdress.ToString() + ':' + CurrentPort.ToString() + " остановлен.");
            }
        }
        public void Listen()
        {
            if (!IsOpen)
                try
                {
                    listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    listener.Bind(new IPEndPoint(CurrentAdress, CurrentPort));
                    listener.Listen(1);
                    thread = new Thread(new ThreadStart(listen)) { IsBackground = true };
                    thread.Start();
                    IsOpen = true;
                    LogWriter.SendLog("Процесс регистрации новых клиентов запущен " + CurrentAdress.ToString() + ':' + CurrentPort.ToString() + " Ожидание подключений...");
                }
                catch (Exception e)
                {
                    LogWriter.SendLog(e.Message, ConsoleColor.Red);
                }
        }
        private void listen()
        {
            while(IsOpen)
            {
                Socket client = listener.Accept();
                IPEndPoint ipep = (IPEndPoint)client.RemoteEndPoint;
                LogWriter.SendLog($"Попытка подключения нового клиента {ipep.Address}:{ipep.Port}");

                Thread t = new Thread(new ParameterizedThreadStart(registration)) { IsBackground = true };
                t.Start(client);
            }
        }
        private void registration(object o)
        {
            Socket client = (Socket)o;
            try
            {
                client.ReceiveTimeout = 1000;

                byte[] bufer = new byte[Protocol.CoverSize];
                Protocol.ReadStream(client, bufer, 0, bufer.Length);

                Cover cov = Protocol.BufferToObject<Cover>(bufer);
                if (cov.BufferType == DataType.registrationRequist && cov.DataSize <= DataCover128kb.MaxDataSize)
                {
                    byte[] data = new byte[cov.DataSize + bufer.Length];
                    Array.Copy(bufer, data, bufer.Length);
                    Protocol.ReadStream(client, data, bufer.Length, cov.DataSize);

                    DataCover128kb dc128 = Protocol.BufferToObject<DataCover128kb>(data);
                    RegistrationData regdata = Protocol.BufferToObject<RegistrationData>(dc128.Data);

                    if((Login == regdata.Login || Login == null) && (Pass == regdata.Pass || Pass == null) && (regdata.TypeClient == ClientType.remote_device || regdata.TypeClient == ClientType.terminal))
                    {
                        LogWriter.SendLog("Подключение установлено с " + regdata.Name + ':' + regdata.TypeClient.ToString());
                        DataCover128kb dcComp = new DataCover128kb(new byte[0], DataType.registrationComlited);
                        byte[] acept = dcComp.Pack();
                        client.Send(acept, 0, acept.Length, SocketFlags.None);

                        if (regdata.TypeClient == ClientType.terminal)
                            ClientsControl.AddTerminal(new Terminal(client, regdata.Name, regdata.ClientGuid));
                        else
                            ClientsControl.AddDevice(new RemoteDevice(client, regdata.Name, regdata.ClientGuid));
                    }
                    else
                        throw new Exception("Регистрационные данные не верны.");
                }
                else
                    throw new Exception("Полученные данные не являются регистрационным сообщением.");
            }
            catch (Exception e)
            {
                Protocol.ProtectedCloseSocket(client);
                LogWriter.SendLog(e.Message, ConsoleColor.Red);
            }
        }
    }
}
