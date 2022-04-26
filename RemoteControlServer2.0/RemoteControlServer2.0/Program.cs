using ProtocolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RemoteControlServer2._0.ServerConfig;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using MailOptions;
using ProtocolCommunication.MessagePack;
using Loger;
using ProtectedValues;

namespace RemoteControlServer2._0
{
    class Program
    {
        public static Config CONF { get; private set; }
        private static ProtectedVal<string> globalip;
        public static string GLOBALIP { get => globalip.Value; }
        static void Main(string[] args)
        {
            LogWriter.SendLog("Сервер запускается...");

            ConfigForm confform = new ConfigForm();
            confform.ShowDialog();
            if (!confform.IsStart)
                return;
            CONF = confform.ConfigData;

            ConnectListener LocalHostListener = new ConnectListener(new IPAddress(new byte[4] { 127, 0, 0, 1 }), 31688);
            LocalHostListener.Listen();

            bool mess = false;
            ConnectListener globalListener = null;
            while (true)
            {
                IPAddress LocalAdress = CONF.InternetController.GetIPProperties().UnicastAddresses.First(x => x.Address.AddressFamily == AddressFamily.InterNetwork).Address;
                LogWriter.SendLog($"Локальный адрес {LocalAdress}:{CONF.Port}");

                if (globalListener != null && (LocalAdress.ToString() != globalListener.CurrentAdress?.ToString() || CONF.Port != globalListener.CurrentPort ||
                    CONF.Login != globalListener.Login || CONF.Pass != globalListener.Pass))
                    globalListener.CloseListener();
                if (globalListener == null)
                    globalListener = new ConnectListener(LocalAdress, CONF.Port, CONF.Login, CONF.Pass);
                if (!globalListener.IsOpen)
                    globalListener.Listen();

                try
                {
                    string globalIp = MailOpt.GetGlobalIpFromMailServer(CONF.IMAPhost, CONF.IMAPport);
                    if (globalIp == null)
                        globalIp = MailOpt.GetGlobalIpFromSite("https://2ip.ua/ru/");
                    if (globalIp == null)
                        throw new Exception("Не удалось установить глобальный Ip арес, нет данных для отправки на почтовый сервер.");
                    globalip.Value = globalIp;
                    if (globalIp != LocalAdress.ToString() && !mess)
                    {
                        mess = true;
                        LogWriter.SendLog($"Внимание! Ip адрес {LocalAdress} сетевого контроллера {CONF.InternetController.Description} не соответствует глобальному Ip " +
                            $"{globalIp}. Убедитесь что этот компьютер зарегестрирован в вашем маршрутизаторе под статическим ip и для него задана переадресация " +
                            $"порта {CONF.Port} по протоколу TCP/IP", ConsoleColor.DarkYellow);
                    }

                    IpPortData ippd = MailOpt.ReadServerIpPortFromMail(CONF.IMAPhost, CONF.IMAPport, CONF.Login, CONF.Pass);
                    if (ippd?.Ip != globalIp || CONF?.Port != ippd.Port)
                        MailOpt.SendServerIpPortToMail(CONF.SMTPhost, CONF.SMTPport, CONF.Login, CONF.Pass, new IpPortData() { Ip = globalIp, Port = CONF.Port });
                }
                catch (Exception e)
                {
                    LogWriter.SendLog(e.Message, ConsoleColor.Red);
                }
                finally
                {
                    Thread.Sleep(60000);
                }
            }
        }
    }
}
