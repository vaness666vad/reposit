using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Terminal.TerminalWindow.ConnectedClient
{
    public abstract class ClientView : NotifyPropertyChangeClass
    {
        public ClientView(int id, TerminalClient tc__t, TerminalModel tm)
        {
            Con__Menu = new ObservableCollection<MyContextItem>();
            Id = id;
            TerminalClie__t = tc__t;
            TM = tm;
        }
        public ObservableCollection<MyContextItem> Con__Menu { get; private set; }
        public TerminalClient TerminalClie__t { get; private set; }
        public TerminalModel TM { get; private set; }
        public int Id { get; private set; }
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        private bool isSelected;
    }
}
