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
using System.Threading.Tasks;

namespace RemoteControlServer2._0.ClientObject
{
    public class Terminal : ClientObj
    {
        public Terminal(Socket c, string name, byte[] clientGuid) : base(c, name, clientGuid)
        {
            EventAfterCloseClient += ClosingClient;
        }
        public override void AceptMessage(DataCover128kb data)
        {
            base.AceptMessage(data);

            switch (data.BufferType)
            {
                case DataType.requistCreateSession: Session(data); break;
            }
        }
        public void Session(DataCover128kb data)
        {
            CreateSession si = Protocol.BufferToObject<CreateSession>(data.Data);
            RemoteDevice rd = ClientsControl.GetDevices().FirstOrDefault(x => x.Id.Value == si.RemDevId);
            if (rd != null)
            {
                string text = $"Запрос создания сессии id{Id.Value}:{Name} id{rd.Id.Value}:{rd.Name}";
                LogWriter.SendLog(text);
                ClientsControl.SendGlobalChat(text);

                byte[] ip = new byte[4];
                int port = si.CustomPort;
                string[] ipstrar = null;
                if (port > -1)
                    ipstrar = Program.GLOBALIP.Split('.');
                else
                {
                    IPEndPoint ipep = (IPEndPoint)Client.RemoteEndPoint;
                    ipstrar = ipep.Address.ToString().Split('.');
                    port = ipep.Port;
                }
                for (int i = 0; i < ip.Length; i += 1)
                    ip[i] = byte.Parse(ipstrar[i]);

                byte[] pass = Guid.NewGuid().ToByteArray();

                rd.Write(new DataCover128kb(new ConnectedInfo(ip, port, pass).Pack(), DataType.createSessionIpPort).Pack());
                Write(new DataCover128kb(new ConnectedInfo(((IPEndPoint)rd.Client.RemoteEndPoint).Address.GetAddressBytes(), ((IPEndPoint)rd.Client.RemoteEndPoint).Port, pass).Pack(), DataType.createSessionIpPort).Pack());
            }
            else
            {
                string text = $"В запросе создания сессии для {Id.Value}:{Name} отказано, удаленное устройство id:{si.RemDevId} не подключено";
                LogWriter.SendLog(text);
                ClientsControl.SendGlobalChat(text);
            }
        }
        protected void ClosingClient()
        {
            ClientsControl.RemoveTerminal(this);
        }      
    }
}
