using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCommunication.MessagePack
{
    public enum ClientType : byte
    {
        terminal = 0,//запрос на регистрацию от терминала
        remote_device = 1//запрос на регистрацию от удаленного устройства
    }
    /// <summary>
    /// служит для передачи регистрационного сообщения
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class RegistrationData : ProtocolPackage
    {
        public RegistrationData()
        {
        }
        public RegistrationData(string name, string login, string pass, byte[] clientGuid, ClientType type)
        {
            SetData(name, login, pass, clientGuid, type);
        }
        [MarshalAs(UnmanagedType.I4)]
        private Int32 nameLenght;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 loginLenght;
        [MarshalAs(UnmanagedType.I4)]
        private Int32 passLenght;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private byte[] clientGuid = new byte[16];
        [MarshalAs(UnmanagedType.I1)]
        private ClientType clientType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private byte[] data = new byte[256];
        public byte[] Pack()
        {
            return Protocol.ObjectToBuffer(this, 29 + nameLenght + loginLenght + passLenght);
        }
        public string Name
        {
            get
            {
                byte[] ndata = new byte[nameLenght];
                Array.Copy(data, 0, ndata, 0, nameLenght);
                return Encoding.UTF8.GetString(ndata);
            }
        }
        public string Login
        {
            get
            {
                byte[] ndata = new byte[loginLenght];
                Array.Copy(data, nameLenght, ndata, 0, loginLenght);
                return Encoding.UTF8.GetString(ndata);
            }
        }
        public string Pass
        {
            get
            {
                byte[] ndata = new byte[passLenght];
                Array.Copy(data, nameLenght + loginLenght, ndata, 0, passLenght);
                return Encoding.UTF8.GetString(ndata);
            }
        }
        public byte[] ClientGuid
        {
            get
            {
                byte[] res = new byte[16];
                Array.Copy(clientGuid, res, res.Length);
                return res;
            }
        }
        public ClientType TypeClient
        {
            get
            {
                return clientType;
            }
        }
        public void SetData(string name, string login, string pass, byte[] ClientGuid, ClientType type)
        {
            name = name == null ? "" : name;
            login = login == null ? "" : login;
            pass = pass == null ? "" : pass;

            Array.Copy(ClientGuid, clientGuid, 16);

            byte[] namedata = Encoding.UTF8.GetBytes(name);
            nameLenght = namedata.Length;
            Array.Copy(namedata, 0, data, 0, nameLenght);

            byte[] logindata = Encoding.UTF8.GetBytes(login);
            loginLenght = logindata.Length;
            Array.Copy(logindata, 0, data, nameLenght, loginLenght);

            byte[] passdata = Encoding.UTF8.GetBytes(pass);
            passLenght = passdata.Length;
            Array.Copy(passdata, 0, data, nameLenght + loginLenght, passLenght);

            clientType = type;
        }
    }
}
