using Loger;
using MailOptions;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Terminal.TerminalWindow;

namespace Terminal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static byte[] TerminalGuid
        {
            get
            {
                byte[] guid = new byte[16];
                Array.Copy(guidclient, guid, guid.Length);
                return guid;
            }
        }
        private static byte[] guidclient = Guid.NewGuid().ToByteArray();

        public MainWindow()
        {      
            InitializeComponent();
            CW = new ConfigView(this);
            DataContext = CW;
        }
        public static ConfigView CW { get; private set; }
        public static IpPortData IPPD { get; private set; }
        private void ConnectToServer(object sender, RoutedEventArgs e)
        {
            Socket client = null;
            try
            {
                IPPD = GetIpPortData(CW);
                if (IPPD != null)
                {
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    if (CW.IsBind)
                        client.Bind(new IPEndPoint(CW.InternetController.GetIPProperties().UnicastAddresses.First(x => x.Address.AddressFamily == AddressFamily.InterNetwork).Address, CW.BindPort));
                    client.Connect(IPPD.Ip, IPPD.Port);

                    if (registrationProcess(client, CW))
                    {
                        LogWriter.SendLog("Процесс регистрации на сервере успешно пройден, подключение установлено.");
                        if (CW.SaveChange)
                            CW.SaveConfig();
                        else
                            CW.DeliteConfig();
                        Visibility = Visibility.Collapsed;
                        TerminalWindowView twv = new TerminalWindowView(client, this);
                        twv.Show();
                    }
                    else
                    {
                        throw new Exception("Получен не верный ответ от сервера, регистрация не подтверждена.");
                    }
                }
            }
            catch(Exception ex)
            {
                Protocol.ProtectedCloseSocket(client);
                LogWriter.SendLog(ex.Message, ConsoleColor.Red);
            }
        }
        private bool registrationProcess(Socket client, ConfigView conf)
        {
            client.ReceiveTimeout = 1000;
            RegistrationData rd = new RegistrationData(conf.Name, conf.Login, conf.Pass, TerminalGuid, ClientType.terminal);
            DataCover128kb dc = new DataCover128kb(rd.Pack(), DataType.registrationRequist);
            byte[] rdbytes = dc.Pack();
            client.Send(rdbytes, 0, rdbytes.Length, SocketFlags.None);

            byte[] bufer = new byte[Protocol.CoverSize];
            Protocol.ReadStream(client, bufer, 0, bufer.Length);

            Cover report = Protocol.BufferToObject<Cover>(bufer);
            return report.BufferType == DataType.registrationComlited;
        }
        private IpPortData GetIpPortData(ConfigView conf)
        {
            if (conf.LocalConnect)
                return new IpPortData() { Ip = "127.0.0.1", Port = 31688 };
            else if (conf.ForwardConnect)
                return new IpPortData() { Ip = conf.GetIpAsString(), Port = conf.Port };
            else
                return MailOpt.ReadServerIpPortFromMail(conf.Mail, conf.MailPort, conf.Login, conf.Pass);
        }
    }
}
