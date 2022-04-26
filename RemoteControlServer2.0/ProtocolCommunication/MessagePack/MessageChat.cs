using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack
{
    public enum SendrType : byte
    {
        terminal = 0,//Отправитель терминал.
        remote_device = 1,//Отправитель удаленное устройство.
        server = 2//Отправитель сервер.
    }
    /// <summary>
    /// Передает сообщение в чат
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MessageChat : ProtocolPackage
    {
        public MessageChat()
        {
        }
        public MessageChat(int senderId, SendrType sender, string text)
        {
            SetData(senderId, sender, text);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 senderId;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 dataLenght;
        [MarshalAs(UnmanagedType.I1)]
        private SendrType sendrType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        private byte[] data = new byte[1024];
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 9 + dataLenght);
        }
        public string Text
        {
            get
            {
                byte[] ndata = new byte[dataLenght];
                Array.Copy(data, ndata, dataLenght);
                return Encoding.UTF8.GetString(ndata);
            }
        }
        public SendrType TypeSender
        {
            get
            {
                return sendrType;
            }
        }
        public int Id
        {
            get
            {
                return senderId;
            }
        }
        public void SetData(int senderId, SendrType sender, string text)
        {
            byte[] textdata = Encoding.UTF8.GetBytes(text);
            dataLenght = textdata.Length;
            Array.Copy(textdata, data, dataLenght);
            this.senderId = senderId;
            sendrType = sender;
        }
    }
}
