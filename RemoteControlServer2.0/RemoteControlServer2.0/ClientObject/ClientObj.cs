using Loger;
using ProtectedValues;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using ProtocolCommunication.MessagePack.SesionMess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlServer2._0.ClientObject
{
    public abstract class ClientObj : ClientObjPrototype
    {
        public string Name { get; private set; }
        public byte[] ClientGuid { get; private set; }
        public ClientObj(Socket c, string name, byte[] clientGuid) : base (c)
        {
            ClientGuid = clientGuid;

            Name = name;
            Id.Value = nextId;
            nextId += 1;

            ValInt32 vi32 = new ValInt32(Id.Value);
            DataCover128kb dc128 = new DataCover128kb(vi32.Pack(), DataType.assignedID);
            Write(dc128);
        }
        public override void AceptMessage(DataCover128kb data)
        {
            switch (data.BufferType)
            {
                case DataType.messageChat: MessageChat(data); break;
                case DataType.sessionInfo: SessionInf(data); break;
            }
        }
        private void SessionInf(DataCover128kb data)
        {
            TerminalConnectData tcd = Protocol.BufferToObject<TerminalConnectData>(data.Data);

            SessionData sd = ClientsControl.GetSessions().FirstOrDefault(x => x.RemDevGuid.SequenceEqual(tcd.RemDevGuid) && x.TerminalGuid.SequenceEqual(tcd.TerminalGuid));
            if (tcd.IsEnabled)
            {
                if (sd != null)
                    sd.UpdateInfoToClients();
                else
                    ClientsControl.AddSession(new SessionData(tcd.TerminalGuid, tcd.RemDevGuid));
            }
            else
            {
                if(sd != null)
                    ClientsControl.RemoveSession(sd);
            }
        }
        protected void MessageChat(DataCover128kb data)
        {
            MessageChat mc = Protocol.BufferToObject<MessageChat>(data.Data);
            string mess = $"[{DateTime.Now}]:id{Id.Value}:{Name}:{mc.TypeSender}\n{mc.Text}";
            LogWriter.SendLog(mess);
            byte[] buf = data.Pack();
            foreach (Terminal t in ClientsControl.GetTerminals())
                t.Write(buf);
        }
        private static int nextId;
    }
}
