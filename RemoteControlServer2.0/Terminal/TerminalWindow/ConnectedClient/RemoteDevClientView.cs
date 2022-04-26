using ProtocolCommunication;
using ProtocolCommunication.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Terminal.TerminalWindow.ConnectedClient
{
    public class RemoteDevClientView : ClientView
    {
        public RemoteDevClientView(int id, string name, TerminalClient tc__t, TerminalModel tm) : base(id, tc__t, tm)
        {
            Name = name;
            Con__Menu.Add(new MyContextItem("Подключиться", CreateSession));
        }
        public long Ping
        {
            get
            {
                return ping;
            }
            set
            {
                long maxrange = 400;
                byte green = value < maxrange / 2 ? (byte)255 : (byte)(255 * (maxrange - (value > maxrange ? maxrange : value)) / (maxrange / 2));
                byte red = value > maxrange / 2 ? (byte)255 : (byte)(255 * value / (maxrange / 2));
                PingColor = new SolidColorBrush(Color.FromArgb(255, red, green, 0));
                ping = value;
                OnPropertyChanged("Ping");
                OnPropertyChanged("PingColor");
            }
        }
        private long ping;
        public Brush PingColor { get; set; }
        public string Name { get; private set; }
        public void CreateSession()
        {
            TerminalClie__t.CreateSession(this);
        }
    }
}
