using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.CaptureInfo
{
    /// <summary>
    /// служит для приема сообщения о существующей камере
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class SupportedFormat : ProtocolPackage
    {
        public SupportedFormat()
        {
            data = new byte[256];
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 idLenght;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 indexfmt;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 nameLenght;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private byte[] data;
        public string Name
        {
            get
            {
                return Encoding.UTF8.GetString(data, idLenght, nameLenght);
            }
        }
        public byte[] Id
        {
            get
            {
                byte[] res = new byte[idLenght];
                Array.Copy(data, res, idLenght);
                return res;
            }
        }
        public int IndexFMT { get => indexfmt; }
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 12 + nameLenght + idLenght);
        }
    }
}
