using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication
{
    public static class Protocol
    {
        /// <summary>
        /// Приводит любой буфер к объекту заданного типу данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T BufferToObject<T>(byte[] buffer)
        {
            int size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            for (int i = 0; i < buffer.Length; i += 1)
                Marshal.WriteByte(ptr, i, buffer[i]);
            T res = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return res;
        }
        /// <summary>
        /// Приводит объект ProtocolPackage к буферу данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToBuffer<T>(T obj, int? maxSize = null) where T : ProtocolPackage
        {
            int size = Marshal.SizeOf<T>();
            maxSize = maxSize == null || maxSize > size ? size : maxSize;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            byte[] buffer = new byte[(int)maxSize];
            for (int i = 0; i < maxSize; i += 1)
                buffer[i] = Marshal.ReadByte(ptr, i);
            Marshal.FreeHGlobal(ptr);
            return buffer;
        }
        /// <summary>
        /// Возвращает размер оболочки конверта
        /// </summary>
        public static int CoverSize
        {
            get
            {
                return coversize;
            }
        }
        private static int coversize = Marshal.SizeOf<Cover>();
        /// <summary>
        /// Граница обозначающая начало конверта 
        /// </summary>
        public static char[] Border
        {
            get
            {
                return border;
            }
        }
        private static char[] border = { '\r', '\n', '\r', '\n', '\r', '\n' };
        /// <summary>
        /// Граница обозначающая начало конверта 
        /// </summary>
        public static byte[] BorderBytes
        {
            get
            {
                return borderBytes;
            }
        }
        private static byte[] borderBytes = Encoding.ASCII.GetBytes(border);
        /// <summary>
        /// Обертка читающая байты из стрима
        /// </summary>
        /// <param name="nws">стрим</param>
        /// <param name="bufer">буфер куда писать байты</param>
        /// <param name="offset">индекс буфера откуда нужно начать запись</param>
        /// <param name="size">планируемая длина сообщения</param>
        /// <param name="trycount">колличество попыток прочитать сообщение</param>
        /// <returns>Возвращает колличество прочитанных байтов</returns>
        public static int ReadStream(Socket nws, byte[] bufer, int offset, int size, int trycount = 10)
        {
            int readLenght = 0;
            int bytes;
            int trypos = -1;
            if (size > 0)
            {
                do
                {
                    bytes = nws.Receive(bufer, offset + readLenght, size - readLenght, SocketFlags.None);
                    readLenght += bytes;
                    trypos = bytes == 0 ? trypos + 1 : -1;
                } while (trypos >= trycount && readLenght < size);

                if (size != 0 && trypos >= trycount)
                    throw new Exception($"Принят пустой буфер {trycount} раз подряд. Ошибка чтения данных пакета");
            }
            return readLenght;
        }
        /// <summary>
        /// Безопасное завершение сокета
        /// </summary>
        /// <param name="s"></param>
        public static void ProtectedCloseSocket(Socket s)
        {
            try { s.Shutdown(SocketShutdown.Both); } catch { }
            try { s.Disconnect(false); } catch { }
            try { s.Close(); } catch { }
            try { s.Dispose(); } catch { }
        }
    }
}
