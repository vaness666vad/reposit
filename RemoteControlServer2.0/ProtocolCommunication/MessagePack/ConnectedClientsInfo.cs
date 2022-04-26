using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack
{
    public enum ClientInfoConnect : byte
    {
        ConnectNow = 1,
        DisconectNow = 2,
        ClientInf = 3
    }
    /// <summary>
    /// Предоставляет информацию о событии подключения/отключения клиента
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class ConnectedClientsInfo : ProtocolPackage
    {
        public ConnectedClientsInfo()
        {
        }
        public ConnectedClientsInfo(string name, int clientid, ClientInfoConnect connect, ClientType type)
        {

            SetData(name, clientid, connect, type);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 nameLenght;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 id;
        [MarshalAs(UnmanagedType.I1)]
        private ClientInfoConnect connected;
        [MarshalAs(UnmanagedType.I1)]
        private ClientType clientType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private byte[] data = new byte[256];
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 10 + nameLenght);
        }
        public string Name
        {
            get
            {
                byte[] ndata = new byte[nameLenght];
                Array.Copy(data, 0, ndata, 0, nameLenght);
                return Encoding.UTF8.GetString(ndata);
            }
        }
        public int Id { get => id; }
        public ClientInfoConnect Connect { get => connected; }
        public ClientType TypeClient
        {
            get
            {
                return clientType;
            }
        }
        public void SetData(string name, int clientid, ClientInfoConnect connect, ClientType type)
        {
            id = clientid;
            connected = connect;

            name = name == null ? "" : name;

            byte[] namedata = Encoding.UTF8.GetBytes(name);
            nameLenght = namedata.Length;
            Array.Copy(namedata, 0, data, 0, nameLenght);

            clientType = type;
        }
    }
}
