using Loger;
using ProtocolCommunication;
using ProtocolCommunication.MessagePack.SesionMess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Terminal.TerminalWindow;

namespace Terminal.SessionWindow
{
    /// <summary>
    /// Логика взаимодействия для SessionStarter.xaml
    /// </summary>
    public partial class SessionStarter : Window
    {
        public class asd: SocketAsyncEventArgs
        {
            public SessionStarter st;
            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                st.registration(e.ConnectSocket);
            }
        }
        private IPEndPoint IPPDlocal;
        private IPEndPoint IPPDremote;
        private TerminalModel TM;
        private byte[] Pass;
        private byte[] remdevGuid;
        private string remdevName;
        public SessionStarter(IPEndPoint ippdlocal, IPEndPoint ippdremote, byte[] pass, TerminalModel tm)
        {
            InitializeComponent();
            TM = tm;
            IPPDlocal = ippdlocal;
            IPPDremote = ippdremote;
            Pass = pass;
            Thread thr = new Thread(new ThreadStart(ConnectionSes)) { IsBackground = true };
            thr.Start();
        }
        private void ConnectionSes()
        {
            Socket soc = null;
            try
            {
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                soc.Bind(IPPDlocal);

                SendStatus($"Соединение с {IPPDremote}");

                    soc.Connect(IPPDremote);
                    if (registration(soc))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SessionW sw = new SessionW(TM, soc, remdevName, remdevGuid);
                            sw.Show();
                        });
                    }
            }
            catch(Exception e)
            {
                SendStatus(e.Message, ConsoleColor.Red);
                Protocol.ProtectedCloseSocket(soc);
            }
            Dispatcher.Invoke(() => Close());
        }
        private void SendStatus(string text, ConsoleColor color = ConsoleColor.Green)
        {
            LogWriter.SendLog(text, color);
            Dispatcher.Invoke(() =>
            {
                listBox.Items.Add(new TextBlock() { Text = text });
            });
        }
        private bool registration(Socket s)
        {
            try
            {
                SendStatus($"Процесс авторизации");
                s.ReceiveTimeout = 3000;

                byte[] regData = new DataCover128kb(new TerminalRegRemDev(MainWindow.TerminalGuid, Pass, MainWindow.CW.Name).Pack(), DataType.registrationRequist).Pack();
                s.Send(regData, 0, regData.Length, SocketFlags.None);

                byte[] coverBuf = new byte[Protocol.CoverSize];
                Protocol.ReadStream(s, coverBuf, 0, coverBuf.Length);

                Cover cov = Protocol.BufferToObject<Cover>(coverBuf);
                if (cov.BufferType == DataType.registrationComlited)
                {
                    byte[] dataCovBuf = new byte[cov.DataSize];
                    //Array.Copy(coverBuf, dataCovBuf, Protocol.CoverSize);

                    Protocol.ReadStream(s, dataCovBuf, 0, cov.DataSize);
                    TerminalRegRemDev result = Protocol.BufferToObject<TerminalRegRemDev>(dataCovBuf);

                    if (result.Pass.SequenceEqual(Pass))
                    {
                        remdevName = result.SenderName;
                        remdevGuid = result.SenderGuid;

                        SendStatus($"Авторизация произведена");
                        SendStatus($"Соединение с {remdevName} успешно установлено");
                        return true;
                    }
                    else
                        throw new Exception("Регистрационные данные не верны");
                }
                else
                    throw new Exception("Принят не верный ответ от удаленного устройства");
            }
            catch(Exception e)
            {
                SendStatus(e.Message, ConsoleColor.Red);
                return false;
            }         
        }
    }
}
