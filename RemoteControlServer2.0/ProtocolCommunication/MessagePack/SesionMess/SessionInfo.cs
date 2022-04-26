using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.SesionMess
{
    public enum SessionMessageType : byte
    {   
        SesCreateNow = 0,//Сообщение несет информацию о о только что созданной сессии с задаными параметрами. 
        SesShutdownNow = 1,//Сообщение несет информацию о завершенной сессии с задаными параметрами.
        SesInformation = 2,//Сообщение несет информацию о существующей сессии с задаными параметрами.
        SesLost = 3//Сообщение несет информацию о сессии вышедшей из диапазона видимости.
    }
    /// <summary>
    /// Предоставляет информацию о текущей сессии для терминалов
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class SessionInfo : ProtocolPackage
    {
        public SessionInfo()
        {
        }
        public SessionInfo(int sessionId, int terminalId, int remDevId, SessionMessageType infotype)
        {
            SetData(sessionId, terminalId, remDevId, infotype);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 sessionId;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 terminalId;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 remDevId;
        [MarshalAs(UnmanagedType.I1)]
        private SessionMessageType infotype;
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
        public int SessionId { get => sessionId; }
        public int TerminalId { get => terminalId; }
        public int RemDevId { get => remDevId; }
        public SessionMessageType Infotype { get => infotype; }
        public void SetData(int SessionId, int TerminalId, int RemDevId, SessionMessageType Infotype)
        {
            sessionId = SessionId;
            terminalId = TerminalId;
            remDevId = RemDevId;
            infotype = Infotype;
        }
    }
}
