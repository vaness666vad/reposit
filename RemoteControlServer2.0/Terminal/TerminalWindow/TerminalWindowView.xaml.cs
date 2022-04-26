using ProtocolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using Terminal.SessionWindow;

namespace Terminal.TerminalWindow
{
    /// <summary>
    /// Логика взаимодействия для TerminalWindowView.xaml
    /// </summary>
    public partial class TerminalWindowView : Window
    {
        private TerminalModel tm;
        public string TitleName { get; private set; }
        private MainWindow connectPanel;
        public TerminalWindowView(Socket client, MainWindow conpanel)
        {
            connectPanel = conpanel;
            InitializeComponent();
            TitleName = MainWindow.CW.Name;
            tm = new TerminalModel(client, this);
            DataContext = tm;
        }
        private void closeWindow(object sender, EventArgs e)
        {
            tm.TerminalClie__t.CloseClient();
            connectPanel.Visibility = Visibility.Visible;
        }
        private bool isShift;
        private void TextBoxChatKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift)
                isShift = true;
            else if (e.Key == Key.Enter && isShift)
                Button_Click(null, null);
            else if (e.Key == Key.Enter)
            {
                TextBoxText.Text += '\n';
                TextBoxText.SelectionStart = TextBoxText.Text.Length;
            }
        }

        private void TextBoxChatKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift)
                isShift = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxText.Text.Length > 0)
            {
                tm.TerminalClie__t.SendChatMessage(TextBoxText.Text);
                TextBoxText.Text = string.Empty;
            }
        }

        private void ChatTextChange(object sender, TextChangedEventArgs e)
        {
            TextBoxChat.ScrollToEnd();
        }
    }
}
