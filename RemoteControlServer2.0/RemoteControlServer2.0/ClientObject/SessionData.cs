using ProtocolCommunication;
using ProtocolCommunication.MessagePack.SesionMess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlServer2._0.ClientObject
{
    public class SessionData
    {
        private static int id;
        public int Id { get; private set; }
        public SessionData(byte[] termGuid, byte[] remdevGuid)
        {
            Id = id;
            id += 1;
            TerminalGuid = termGuid;
            RemDevGuid = remdevGuid;
        }
        public byte[] TerminalGuid { get; private set; }
        public byte[] RemDevGuid { get; private set; }
        public int GetTerminalId(IEnumerable<Terminal> terms)
        {
            Terminal t = terms.FirstOrDefault(x => x.ClientGuid.SequenceEqual(TerminalGuid));
            return t is null ? -1 : t.Id.Value;
        }
        public int GetRemoteDevId(IEnumerable<RemoteDevice> devs)
        {
            RemoteDevice t = devs.FirstOrDefault(x => x.ClientGuid.SequenceEqual(RemDevGuid));
            return t is null ? -1 : t.Id.Value;
        }
        public void UpdateInfoToClients()
        {
            int idt = GetTerminalId(ClientsControl.GetTerminals());
            int idr = GetRemoteDevId(ClientsControl.GetDevices());

            byte[] data = new DataCover128kb(new SessionInfo(Id, idt, idr, SessionMessageType.SesInformation).Pack(), DataType.sessionInfo).Pack();
            foreach (Terminal term in ClientsControl.GetTerminals())
                term.Write(data);
        }
    }
}
