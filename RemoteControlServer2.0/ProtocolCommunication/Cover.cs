using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ProtocolCommunication.Protocol;

namespace ProtocolCommunication
{
    /// <summary>
    /// Класс описывает оболочку конверта
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class Cover
    {
        /// <summary>
        /// Граница обозначающая начало конверта
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        protected readonly char[] border = Border;
        /// <summary>
        /// Уникальный guid конверта
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        protected byte[] guidCover;
        /// <summary>
        /// Требует ли отправитель от получателя подтверждения о доставке этого конверта
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        protected byte deliveryReport;
        /// <summary>
        /// Тип содержимого конверта
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        protected DataType dataType;
        /// <summary>
        /// Размер конверта
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        protected Int32 dataSize;
        /// <summary>
        /// Возвращает уникальный guid конверта
        /// </summary>
        public string GuidCover { get { return Encoding.ASCII.GetString(guidCover); } }
        /// <summary>
        /// Возвращает уникальный guid конверта
        /// </summary>
        public byte[] GuidCoverBytes { get { return guidCover; } }
        /// <summary>
        /// Возвращает размер содержимого конверта
        /// </summary>
        public int DataSize { get { return dataSize; } }
        /// <summary>
        /// Возвращает тип содержимого конверта
        /// </summary>
        public DataType BufferType { get { return dataType; } }
        /// <summary>
        /// Возвращает запрос очета о доставке
        /// </summary>
        public bool DeliveryReport { get { return deliveryReport > 0; } }
    }
}
