using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Terminal
{
    public class ConfigView : NotifyPropertyChangeClass
    {
        private Config _conf;
        private MainWindow _mw;
        private void readConfig()
        {
            Name = _conf.Name;
            login = _conf.Login;
            Pass = _conf.Pass;
            _mw.PassBox.Password = _conf.Pass;
            Mail = _conf.Mail;
            MailPort = _conf.MailPort;
            Ip1 = _conf.Ip1;
            Ip2 = _conf.Ip2;
            Ip3 = _conf.Ip3;
            Ip4 = _conf.Ip4;
            Port = _conf.Port;
            IsBind = _conf.IsBind;
            internetControllerName = _conf.InternetControllerName;
            BindPort = _conf.BindPort;

            LocalConnect = _conf.LocalConnect;
            ForwardConnect = _conf.ForwardConnect;
            MailConnect = _conf.MailConnect;

            SaveChange = _conf.Save;
        }
        private void writeConfig()
        {
            _conf.Name = Name;
            _conf.Login = login;
            _conf.Pass = Pass;
            _conf.Mail = Mail;
            _conf.MailPort = MailPort;
            _conf.Ip1 = Ip1;
            _conf.Ip2 = Ip2;
            _conf.Ip3 = Ip3;
            _conf.Ip4 = Ip4;
            _conf.Port = Port;
            _conf.IsBind = IsBind;
            _conf.BindPort = BindPort;
            _conf.InternetControllerName = internetControllerName;

            _conf.LocalConnect = LocalConnect;
            _conf.ForwardConnect = ForwardConnect;
            _conf.MailConnect = MailConnect;

            _conf.Save = SaveChange;
        }
        private void updateButtionEnabled()
        {
            ButtionEnabled = (LocalConnect && !string.IsNullOrEmpty(Name)) || 
                (ForwardConnect && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Pass)) ||
                (MailConnect && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Pass) && !string.IsNullOrEmpty(Mail));
        }
        public void SaveConfig()
        {
            writeConfig();
            Config.SaveParams(_conf);
        }
        public void DeliteConfig()
        {
            Config.RemoveParam();
        }
        public ConfigView(MainWindow mw)
        {
            _mw = mw;
            _mw.PassBox.PasswordChanged += PassChange;
            _conf = Config.LoadParams();
            readConfig();
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                updateButtionEnabled();
                OnPropertyChanged("Name");
                OnPropertyChanged("NameColor");
            }
        }
        private string name;
        public Brush NameColor
        {
            get
            {
                return Name?.Length > 0 ? Brushes.White : Brushes.Red;
            }
        }
        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                login = value;
                updateButtionEnabled();
                OnPropertyChanged("Login");
                OnPropertyChanged("LoginColor");
            }
        }
        private string login;
        public Brush LoginColor
        {
            get
            {
                return LoginEnabled && string.IsNullOrEmpty(Login) ? Brushes.Red : Brushes.White;
            }
        }
        public string Pass
        {
            get
            {
                return pass;
            }
            set
            {
                pass = value;
                updateButtionEnabled();             
                OnPropertyChanged("PassColor");
            }
        }
        private string pass;
        public Brush PassColor
        {
            get
            {              
                return PassEnabled && string.IsNullOrEmpty(Pass)? Brushes.Red : Brushes.White;
            }
        }
        public string Mail
        {
            get
            {
                return mail;
            }
            set
            {
                mail = value;
                updateButtionEnabled();
                OnPropertyChanged("Mail");
                OnPropertyChanged("MailColor");
            }
        }
        private string mail;
        public Brush MailColor
        {
            get
            {
                return MailEnabled && string.IsNullOrEmpty(Mail) ? Brushes.Red : Brushes.White;
            }
        }
        public int MailPort
        {
            get
            {
                return mailPort;
            }
            set
            {
                mailPort = value;
                OnPropertyChanged("MailPort");
            }
        }
        private int mailPort;
        public string GetIpAsString()
        {
            return Ip1.ToString() + '.' + Ip2.ToString() + '.' + Ip3.ToString() + '.' + Ip4.ToString();
        }
        public byte Ip1
        {
            get
            {
                return ip1;
            }
            set
            {
                ip1 = value;
                OnPropertyChanged("Ip1");
            }
        }
        private byte ip1;
        public byte Ip2
        {
            get
            {
                return ip2;
            }
            set
            {
                ip2 = value;
                OnPropertyChanged("Ip2");
            }
        }
        private byte ip2;
        public byte Ip3
        {
            get
            {
                return ip3;
            }
            set
            {
                ip3 = value;
                OnPropertyChanged("Ip3");
            }
        }
        private byte ip3;
        public byte Ip4
        {
            get
            {
                return ip4;
            }
            set
            {
                ip4 = value;
                OnPropertyChanged("Ip1");
            }
        }
        private byte ip4;
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }
        private int port;
        public ObservableCollection<NetworkInterface> InternetControllerList => new ObservableCollection<NetworkInterface>(NetworkInterface.GetAllNetworkInterfaces());
        public NetworkInterface InternetController
        {
            get
            {
                try
                {
                    return InternetControllerList.First(x => x.Description == internetControllerName);
                }
                catch
                {
                    return InternetControllerList.FirstOrDefault();
                }
            }
            set
            {
                internetControllerName = value?.Description;
            }
        }
        private string internetControllerName;
        public bool IsBind
        {
            get
            {
                return isBind;
            }
            set
            {
                isBind = value;
                OnPropertyChanged("IsBind");
            }
        }
        private bool isBind;
        public int BindPort
        {
            get
            {
                return bindPort;
            }
            set
            {
                bindPort = value;
                OnPropertyChanged("BindPort");
            }
        }
        private int bindPort;
        public bool LocalConnect
        {
            get
            {
                return localConnect;
            }
            set
            {
                localConnect = value;
                _conf.LocalConnect = localConnect;

                if (localConnect)
                {
                    LoginEnabled = false;
                    PassEnabled = false;
                    MailEnabled = false;
                    MailPortEnabled = false;
                    IpEnabled = false;
                    PortEnabled = false;
                    updateButtionEnabled();
                }

                OnPropertyChanged("LocalConnect");
            }
        }
        private bool localConnect;
        public bool ForwardConnect
        {
            get
            {
                return forwardConnect;
            }
            set
            {
                forwardConnect = value;
                _conf.ForwardConnect = forwardConnect;

                if (forwardConnect)
                {
                    LoginEnabled = true;
                    PassEnabled = true;
                    MailEnabled = false;
                    MailPortEnabled = false;
                    IpEnabled = true;
                    PortEnabled = true;
                    updateButtionEnabled();
                }

                OnPropertyChanged("ForwardConnect");
            }
        }
        private bool forwardConnect;
        public bool MailConnect
        {
            get
            {
                return mailConnect;
            }
            set
            {
                mailConnect = value;
                _conf.MailConnect = mailConnect;

                if (mailConnect)
                {
                    LoginEnabled = true;
                    PassEnabled = true;
                    MailEnabled = true;
                    MailPortEnabled = true;
                    IpEnabled = false;
                    PortEnabled = false;
                    updateButtionEnabled();
                }

                OnPropertyChanged("MailConnect");
            }
        }
        private bool mailConnect;
        public bool LoginEnabled
        {
            get
            {
                return loginEnabled;
            }
            set
            {
                loginEnabled = value;
                OnPropertyChanged("LoginColor");
                OnPropertyChanged("LoginEnabled");
            }
        }
        private bool loginEnabled;
        public bool PassEnabled
        {
            get
            {
                return passEnabled;
            }
            set
            {
                passEnabled = value;
                OnPropertyChanged("PassColor");
                OnPropertyChanged("PassEnabled");
            }
        }
        private bool passEnabled;
        public bool MailEnabled
        {
            get
            {
                return mailEnabled;
            }
            set
            {
                mailEnabled = value;
                OnPropertyChanged("MailColor");
                OnPropertyChanged("MailEnabled");
            }
        }
        private bool mailEnabled;
        public bool MailPortEnabled
        {
            get
            {
                return mailPortEnabled;
            }
            set
            {
                mailPortEnabled = value;
                OnPropertyChanged("MailPortEnabled");
            }
        }
        private bool mailPortEnabled;
        public bool IpEnabled
        {
            get
            {
                return ipEnabled;
            }
            set
            {
                ipEnabled = value;
                OnPropertyChanged("IpEnabled");
            }
        }
        private bool ipEnabled;
        public bool PortEnabled
        {
            get
            {
                return portEnabled;
            }
            set
            {
                portEnabled = value;
                OnPropertyChanged("PortEnabled");
            }
        }
        private bool portEnabled;
        public bool SaveChange
        {
            get
            {
                return saveChange;
            }
            set
            {
                saveChange = value;
                OnPropertyChanged("SaveChange");
            }
        }
        private bool saveChange;
        public bool ButtionEnabled
        {
            get
            {
                return buttionEnabled;
            }
            set
            {
                buttionEnabled = value;
                OnPropertyChanged("ButtionEnabled");
            }
        }
        private bool buttionEnabled;
        public void PassChange(object sender, RoutedEventArgs e)
        {
            Pass = ((PasswordBox)sender).Password;
        }
    }
}
