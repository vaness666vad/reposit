using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.SesionMess
{
    /// <summary>
    /// Предоставляет информацию терминалу и удаленному устройству для создания сессии
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class ConnectedInfo : ProtocolPackage
    {
        public ConnectedInfo()
        {
        }
        public ConnectedInfo(byte[] ip, int port, byte[] pass)
        {
            SetData(ip, port, pass);
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] ip =new byte[4];
        [MarshalAs(UnmanagedType.I4)]
        private Int32 port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] pass = new byte[16];
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
        public byte[] Ip
        {
            get
            {
                byte[] res = new byte[4];
                Array.Copy(ip, res, res.Length);
                return res;
            }
        }
        public int Port => port;
        public byte[] Pass
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(pass, res, res.Length);
                return res;
            }
        }
        public void SetData(byte[] IP, int Port, byte[] Pass)
        {
            Array.Copy(IP, ip, 4);
            port = Port;
            Array.Copy(Pass, pass, 16);
        }
    }
}
