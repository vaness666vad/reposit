using ProtocolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Terminal.TerminalWindow
{
    public abstract class BaseModel : NotifyPropertyChangeClass
    {
        protected ClientObjPrototype baseClient { get; set; }
        public long ServerPing
        {
            get
            {
                long res = baseClient.Ping;
                long maxrange = 400;
                byte green = res < maxrange / 2 ? (byte)255 : (byte)(255 * (maxrange - (res > maxrange ? maxrange : res)) / (maxrange / 2));
                byte red = res > maxrange / 2 ? (byte)255 : (byte)(255 * res / (maxrange / 2));
                ServerPingColor = new SolidColorBrush(Color.FromArgb(255, red, green, 0));
                OnPropertyChanged("ServerPingColor");
                return res;
            }
        }
        public Brush ServerPingColor { get; set; }
    }
}
