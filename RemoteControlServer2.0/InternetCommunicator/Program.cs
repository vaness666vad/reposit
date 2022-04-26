using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InternetCommunicator
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buffff = Encoding.ASCII.GetBytes("<>");

            TcpClient tc = new TcpClient();
            tc.Connect("imap.gmail.com", 993);

            SslStream ssl = new SslStream(tc.GetStream());
            ssl.AuthenticateAsClient("imap.gmail.com");
            ssl.ReadTimeout = 3000;
            while (true)
            ssl.Read(new byte[16],0,16);

            byte[] buffer = new byte[128];
            int bytes = 0;
            while (true)
            {
                do
                {

                    try { bytes = ssl.Read(buffer, 0, buffer.Length); } catch { bytes = 0; }
                    if (bytes > 0)
                        Console.Write(Encoding.ASCII.GetString(buffer, 0, bytes));

                } while (bytes != 0);

                string command = Console.ReadLine();
                ssl.Write(toByte(command + "\r\n"));
            }


         //   ssl.Write(SendData("communicationmodule2@gmail.com", "Vopabu39", new byte[] { 1,2,3,4}));
        }

        private static string toBase64(string value)
        {
            byte[] data = toByte(value);
            return Convert.ToBase64String(data);
        }
        private static byte[] toByte(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }
    }
}
