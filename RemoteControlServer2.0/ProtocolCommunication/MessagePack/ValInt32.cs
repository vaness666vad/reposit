using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack
{
    /// <summary>
    /// служит для передачи целого числа
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class ValInt32 : ProtocolPackage
    {
        public ValInt32()
        {

        }
        public ValInt32(int value)
        {
            SetValue(value);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 val;
        public int Value
        {
            get
            {
                return val;
            }
        }
        public void SetValue(int value)
        {
            val = value;
        }
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 4);
        }
    }
}
