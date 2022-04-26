using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.CaptureInfo
{
    /// <summary>
    /// служит для приема сообщения о поддерживаемых разрешениях формата сжатия камеры
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DevFormatSize : ProtocolPackage
    {
        public DevFormatSize()
        {
            data = new byte[256];
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 idLenght;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 indexfmt;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 widht;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 height;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private byte[] data;
        public int IndexFMT { get => indexfmt; }
        public int Widht { get => widht; }
        public int Height { get => height; }
        public byte[] Id
        {
            get
            {
                byte[] res = new byte[idLenght];
                Array.Copy(data, res, idLenght);
                return res;
            }
        }
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 16 + idLenght);
        }
    }
}
