using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack
{
    /// <summary>
    /// Предоставляет информацию о событии подключения/отключения клиента
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class СonnectionQuality : ProtocolPackage
    {
        public СonnectionQuality()
        {
        }
        public СonnectionQuality(int clientId, long ping, ClientType type)
        {
            SetData(clientId, ping, type);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 clientId;
        [MarshalAs(UnmanagedType.I8)]
        private Int64 ping;
        [MarshalAs(UnmanagedType.I1)]
        private ClientType type;
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
        public int ClientId { get => clientId; }
        public long Ping { get => ping; }
        public ClientType Cli_ntType { get => type; }
        public void SetData(int ClientId, long Ping, ClientType Type)
        {
            clientId = ClientId;
            ping = Ping;
            type = Type;
        }
    }
}
