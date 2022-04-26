using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.SesionMess
{
    /// <summary>
    /// Предоставляет регистрационные данные для терминала к удаленному устройству и обратно
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class TerminalRegRemDev : ProtocolPackage
    {
        public TerminalRegRemDev()
        {

        }
        public TerminalRegRemDev(byte[] senderG, byte[] pass, string name)
        {
            SetData(senderG, pass, name);
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] senderGuid = new byte[16];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] pass = new byte[16];
        [MarshalAs(UnmanagedType.I4)]
        private Int32 nameLeng;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        private byte[] senderName = new byte[128];
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 36 + nameLeng);
        }
        public byte[] SenderGuid
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(senderGuid, res, res.Length);
                return res;
            }
        }
        public byte[] Pass
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(pass, res, res.Length);
                return res;
            }
        }
        public string SenderName
        {
            get
            {
                return Encoding.UTF8.GetString(senderName, 0, nameLeng);
            }
        }
        public void SetData(byte[] SenderG, byte[] Pass, string Name)
        {
            Array.Copy(SenderG, senderGuid, 16);
            Array.Copy(Pass, pass, 16);

            byte[] namebuf = Encoding.UTF8.GetBytes(Name);
            nameLeng = namebuf.Length;
            Array.Copy(namebuf, senderName, nameLeng);
        }
    }
}
