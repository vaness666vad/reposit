using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal.TerminalWindow
{
    /// <summary>
    /// Представляет объект клиента для отображения в терминале
    /// </summary>
    public class ConnectedClient : NotifyPropertyChangeClass
    {
        public ConnectedClient(int id, string name)
        {
            Id = id;
            Name = name;
        }
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
        public int Id { get; private set; }
        public string Name { get; private set; }
    }
}
