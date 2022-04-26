using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication
{
    /// <summary>
    /// Класс наследует оболочку конверта и описывает функции содержимого конверта
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DataCover128kb : Cover, ProtocolPackage
    {
        /// <summary>
        /// максимальный размер сообщения
        /// </summary>
        public const int MaxDataSize = 131072;
        /// <summary>
        /// содержимое сообщения (не может превышать MaxDataSize максимальный размер сообщения)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDataSize)]
        private byte[] data;
        /// <summary>
        /// Возвращает содержимое конверта
        /// </summary>
        public byte[] Data
        {
            get
            {
                byte[] buffer = new byte[dataSize];
                Array.Copy(data, buffer, dataSize);
                return buffer;
            }
        }
        /// <summary>
        /// Задает содержимое конверту
        /// </summary>
        public void SetData(byte[] buffer, DataType type, bool report = false)
        {
            dataSize = buffer.Length > MaxDataSize ? MaxDataSize : buffer.Length;            
            Array.Copy(buffer, data, dataSize);
            deliveryReport = report ? (byte)1 : (byte)0;
            dataType = type;
        }
        /// <summary>
        /// Упаковывает сообщение
        /// </summary>
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, dataSize + Protocol.CoverSize);
        }
        public DataCover128kb()
        {
            data = new byte[MaxDataSize];
            guidCover = Guid.NewGuid().ToByteArray();
        }
        public DataCover128kb(byte[] buffer, DataType type, bool report = false)
        {
            data = new byte[MaxDataSize];
            SetData(buffer, type, report);
            guidCover = Guid.NewGuid().ToByteArray();
        }
    }
}
