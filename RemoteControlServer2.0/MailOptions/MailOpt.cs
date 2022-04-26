using Loger;
using ProtocolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MailOptions
{
    public static class MailOpt
    {
        /// <summary>
        /// Альтернативный способ получить глобальный ip с использованием сайта
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static string GetGlobalIpFromSite(string site)
        {
            WebClient Client = new WebClient();
            string res;
            try
            {
                LogWriter.SendLog($"Запрос глобального ip с {site}");
                string data = Client.DownloadString(site);

                Regex r = new Regex(@"\d+\.\d+\.\d+\.\d+");

                res = r.Match(data).Value;
                if (res == null || res.Length == 0)
                    throw new Exception("Не удалось получить информацию о глобальном Ip");

                LogWriter.SendLog($"Глобальный ip адрес {res}");
            }
            catch(Exception e)
            {
                res = null;
                LogWriter.SendLog(e.Message, ConsoleColor.Red);
            }
            return res;
        }
        /// <summary>
        /// Спрашивает ip у сервера почтовика
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetGlobalIpFromMailServer(string hostname, int port)
        {
            string res = null;
            TcpClient tc = null;
            SslStream ssl = null;
            try
            {
                LogWriter.SendLog($"Запрос глобального ip с {hostname}");

                tc = new TcpClient();
                tc.Connect(hostname, port);

                ssl = new SslStream(tc.GetStream());
                ssl.AuthenticateAsClient(hostname);
                ssl.ReadTimeout = 2000;

                string reply = readData(ssl);

                Regex r = new Regex(@"\d+\.\d+\.\d+\.\d+");

                res = r.Match(reply).Value;
                if (res == null || res.Length == 0)
                    throw new Exception("Не удалось получить информацию о глобальном Ip");

                LogWriter.SendLog($"Глобальный ip адрес {res}");

            }
            catch(Exception e)
            {
                res = null;
                LogWriter.SendLog(e.Message, ConsoleColor.Red);
            }
            finally
            {
                if (ssl != null)
                    ssl.Close();
                Protocol.ProtectedCloseSocket(tc.Client);
            }
            return res;
        }
        /// <summary>
        /// Возвращает ip и port хранящиеся на почте либо null
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static IpPortData ReadServerIpPortFromMail(string hostname, int port, string login, string pass)
        {
            IpPortData res = null;
            LogWriter.SendLog($"Чтение данных о сервере {login}");
            TcpClient tc = null;
            SslStream ssl = null;

            try
            {
                tc = new TcpClient();
                tc.Connect(hostname, port);

                ssl = new SslStream(tc.GetStream());
                ssl.AuthenticateAsClient(hostname);
                ssl.ReadTimeout = 2000;

                ssl.Write(toByte($"A01 LOGIN {login} {pass}\r\nA02 SELECT INBOX\r\n"));
                string reply = readData(ssl);

                if (reply.Contains("[AUTHENTICATIONFAILED]"))
                    throw new Exception("AUTHENTICATIONFAILED");
                string mailcount = string.Empty;
                string countstring = string.Empty;
                try
                {
                    countstring = reply.Split('\n').First(x => x.Contains("EXISTS"));
                }
                catch
                {
                    throw new Exception($"Получен не верный ответ от почтового сервера\n{reply}");
                }
                for (int i = 0; i < countstring.Length; i += 1)
                    if (int.TryParse(countstring[i].ToString(), out _))
                        mailcount += countstring[i];
                if (mailcount.Length > 0 && int.Parse(mailcount) == 0)
                    throw new Exception("Нет сохраненных сообщений");
                ssl.Write(toByte($"A03 FETCH {mailcount} rfc822.text\r\n"));
                byte[] data = readDataBuf(ssl);
                byte[] r = new byte[24];

                bool okr = false;
                for (int i = 0; i < data.Length; i += 1)
                {
                    if(data[i] == 60 && i + 26 < data.Length && data[i + 25] == 62)
                    {
                        Array.Copy(data, i + 1, r, 0, 24);
                        okr = true;
                        for(int ii = 0; ii < 24; ii += 1)
                            if (r[ii] > 58 || r[ii] < 48)
                            {
                                okr = false;
                                break;
                            }
                        if (okr)
                            break;
                    }
                }

                if (!okr)
                    throw new Exception("Последнее сообщение не содержит полезного контента");

                res = Protocol.BufferToObject<IpPortData>(decodeBuf(r));

                LogWriter.SendLog($"Чтение данных выполнено: {res.Ip}:{res.Port}");
            }
            catch (Exception e)
            {
                LogWriter.SendLog(e.Message, ConsoleColor.Red);
            }
            finally
            {
                if (ssl != null)
                    ssl.Close();
                Protocol.ProtectedCloseSocket(tc.Client);
            }
            return res;
        }
        /// <summary>
        /// Почтовый сервер gmail перед 10 добавляет 13, после 13 10. Надо кодировать сообщение в 0-9 байт
        /// </summary>
        /// <param name="buf"></param>
        private static byte[] codeBuf(byte[] buf)
        {
            byte[] res = new byte[26];
            for (int i = 0; i < buf.Length && i < 8; i += 1)
            {
                res[i * 3 + 1] = (byte)(buf[i] / 100);
                res[i * 3 + 2] = (byte)((buf[i] - res[i * 3 + 1] * 100) / 10);
                res[i * 3 + 3] = (byte)(buf[i] - res[i * 3 + 1] * 100 - res[i * 3 + 2] * 10);
                res[i * 3 + 1] += 48;
                res[i * 3 + 2] += 48;
                res[i * 3 + 3] += 48;
            }
            res[0] = 60;
            res[res.Length - 1] = 62;
            return res;
        }
        /// <summary>
        /// Почтовый сервер gmail перед 10 добавляет 13, после 13 10. Надо декодировать сообщение в 0-9 байт
        /// </summary>
        /// <param name="buf"></param>
        private static byte[] decodeBuf(byte[] buf)
        {
            byte[] bufer = new byte[24];
            int count = 0;
            for(int i = 0; i < buf.Length && count < bufer.Length; i += 1)
            {
                if(buf[i] > 47 && buf[i] < 58)
                {
                    bufer[count] = buf[i];
                    count += 1;
                }
            }

            byte[] res = new byte[8];
            for (int i = 0; i < 8; i += 1)
            {
                res[i] = (byte)(((bufer[i * 3 + 0] - 48) * 100) + ((bufer[i * 3 + 1] - 48) * 10) + (bufer[i * 3 + 2] - 48));
            }
            return res;
        }
        /// <summary>
        /// Выполняет отправку ip и port на почтовый сервер
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="login"></param>
        /// <param name="pass"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SendServerIpPortToMail(string hostname, int port, string login, string pass, IpPortData data)
        {         
            bool res = true;
            TcpClient tc = null;
            SslStream ssl = null;
            try
            {
                LogWriter.SendLog($"Передача данных о сервере на {login}");
                tc = new TcpClient();
                tc.Connect(hostname, port);

                ssl = new SslStream(tc.GetStream());
                ssl.AuthenticateAsClient(hostname);
                ssl.ReadTimeout = 2000;

                ssl.Write(toByte($"EHLO A01\r\nAUTH LOGIN {toBase64(login)}\r\n{toBase64(pass)}\r\nMAIL FROM: <{login}>\r\nrcpt to: <{login}>\r\nDATA\r\nSubject: RC\r\n\n"));
                ssl.Write(codeBuf(Protocol.ObjectToBuffer(data)));
                ssl.Write(toByte("\n.\r\n"));

                string reply = readData(ssl);

                if (reply.Contains("Username and Password not accepted"))
                    throw new Exception("Username and Password not accepted");
                LogWriter.SendLog($"Передача данных выполнена: {data.Ip}:{data.Port}");
            }
            catch (Exception e)
            {
                LogWriter.SendLog(e.Message, ConsoleColor.Red);
                res = false;
            }
            finally
            {
                if (ssl != null)
                    ssl.Close();
                Protocol.ProtectedCloseSocket(tc.Client);
            }
            return res;
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
        private static byte[] readDataBuf(SslStream ssl)
        {
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[128];

            int count;
            try
            {
                while (true)
                {
                    count = ssl.Read(buffer, 0, buffer.Length);
                    byte[] buff = new byte[count];
                    Array.Copy(buffer, 0, buff, 0, count);
                    data.AddRange(buff);
                }
            }
            catch { }
            return data.ToArray();
        }
        private static string readData(SslStream ssl)
        {
            return Encoding.ASCII.GetString(readDataBuf(ssl));
        }
    }
}
