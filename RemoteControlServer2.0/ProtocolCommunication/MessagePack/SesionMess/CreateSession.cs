using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.SesionMess
{
    /// <summary>
    /// Служит для передачи запроса на создание новой сессии
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class CreateSession : ProtocolPackage
    {
        public CreateSession()
        {
        }
        public CreateSession(int remDevId, int customPort)
        {
            SetData(remDevId, customPort);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 remDevId;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 customPort;
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
        public int RemDevId { get => remDevId; }
        public int CustomPort { get => customPort; }
        public void SetData(int RemDevId, int CustomPort)
        {
            customPort = CustomPort;
            remDevId = RemDevId;
        }
    }
}
