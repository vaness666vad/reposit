using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlServer2._0.ClientObject
{
    public class RemoteDevice : ClientObj
    {
        public RemoteDevice(Socket c, string name, byte[] clientGuid) : base(c, name, clientGuid)
        {
            EventAfterCloseClient += ClosingClient;
        }
        public override void AceptMessage(DataCover128kb data)
        {
            base.AceptMessage(data);

            switch (data.BufferType)
            {
                case DataType.ping: PingInfo(); break;
            }
        }
        public void PingInfo()
        {
            long ping = Ping;
            byte[] data = new DataCover128kb(new СonnectionQuality(Id.Value, ping, ClientType.remote_device).Pack(), DataType.connectionQuality).Pack();
            foreach (Terminal t in ClientsControl.GetTerminals())
                t.Write(data);
        }
        protected void ClosingClient()
        {
            ClientsControl.RemoveDevice(this);
        }
    }
}
