using ProtocolCommunication;
using System;
using System.Runtime.InteropServices;

namespace MailOptions
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class IpPortData: ProtocolPackage
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] ip = new byte[4];
        [MarshalAs(UnmanagedType.I4)]
        public Int32 Port;
        public byte[] Ipbytes
        {
            get
            {
                byte[] res = new byte[4];
                Array.Copy(ip, res, 4);
                return res;
            }
            set
            {
                if (value.Length == 4)
                    Array.Copy(value, ip, 4);
            }
        }
        public string Ip
        {
            get
            {
                string res = string.Empty;
                for(int i = 0; i < ip.Length; i+=1)
                {
                    res += ip[i].ToString();
                    if (i < ip.Length - 1)
                        res += '.';
                }
                return res;
            }
            set
            {
                string[] bytesSTR = value.Split('.');
                for (int i = 0; i < ip.Length; i += 1)
                    ip[i] = byte.Parse(bytesSTR[i]);
            }
        }
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
    }
}
