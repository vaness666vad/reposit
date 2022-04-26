using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack.SesionMess
{
    /// <summary>
    /// Предоставляет информацию для сервера о созданной/текущей сессии 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class TerminalConnectData : ProtocolPackage
    {
        public TerminalConnectData()
        {

        }
        public TerminalConnectData(byte[] terminalG, byte[] remdevG, bool isenabled)
        {
            SetData(terminalG, remdevG, isenabled);
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] terminalGuid = new byte[16];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] remdevGuid = new byte[16];
        [MarshalAs(UnmanagedType.I1)]
        private byte isEnable;
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this);
        }
        public byte[] TerminalGuid
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(terminalGuid, res, res.Length);
                return res;
            }
        }
        public byte[] RemDevGuid
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(remdevGuid, res, res.Length);
                return res;
            }
        }
        public bool IsEnabled => isEnable == 1 ? true : false;
        public void SetData(byte[] TerminalGuid, byte[] RemDevGuid, bool Isenabled)
        {
            isEnable = Isenabled ? (byte)1 : (byte)0;

            Array.Copy(TerminalGuid, terminalGuid, 16);
            Array.Copy(RemDevGuid, remdevGuid, 16);
        }
    }
}
