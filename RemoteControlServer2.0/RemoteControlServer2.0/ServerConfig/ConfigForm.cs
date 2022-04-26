using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControlServer2._0.ServerConfig
{
    public partial class ConfigForm : Form
    {
        public Config ConfigData { get; private set; }
        public bool IsStart { get; private set; }
        public ConfigForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            ConfigData = Config.LoadParams();

            NetworkInterface selected = ConfigData.InternetController;
            comboBoxInterfaces.DataSource = Config.InternetControllerList;
            comboBoxInterfaces.DisplayMember = "Description";
            comboBoxInterfaces.SelectedItem = selected;

            textBoxImapH.Text = ConfigData.IMAPhost;
            textBoxIpSite.Text = ConfigData.IpSite;
            textBoxImapP.Text = ConfigData.IMAPport.ToString();
            textBoxSmtpH.Text = ConfigData.SMTPhost;
            textBoxSmtpP.Text = ConfigData.SMTPport.ToString();
            textBoxLogin.Text = ConfigData.Login;
            textBoxPass.Text = ConfigData.Pass;
            textBoxPort.Text = ConfigData.Port.ToString();
            checkBox1.Checked = ConfigData.Save;

            ChangeController(); ImapH(); ImapP(); SmtpH(); SmtpP(); LoginBox(); PassBox(); Port(); IpSiteChange();

            textBoxPass.PasswordChar = '*';
            float longest = 0;
            Graphics g = comboBoxInterfaces.CreateGraphics();
            foreach (NetworkInterface n in Config.InternetControllerList)
            {
                SizeF textLength = g.MeasureString(n.Description, comboBoxInterfaces.Font);

                if (textLength.Width > longest)
                    longest = textLength.Width;
            }
            if (longest > 0)
                comboBoxInterfaces.DropDownWidth = (int)longest;
        }
        private void testReady()
        {
            if (controller && imapH && imapP && smtpH && smtpP && login && pass && port && ipSite)
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            IsStart = true;
            if (ConfigData.Save)
                Config.SaveParams(ConfigData);
            else
                Config.RemoveParam();
            Close();
        }
        private bool controller;
        private void ChangeController(object sender = null, EventArgs e = null)
        {
            ConfigData.InternetController = (NetworkInterface)comboBoxInterfaces.SelectedItem;
            controller = ConfigData.InternetController != null;
            comboBoxInterfaces.BackColor = controller ? Color.White : Color.Red;

            testReady();
        }

        private void ImapH(object sender = null, EventArgs e = null)
        {
            ConfigData.IMAPhost = textBoxImapH.Text;
            imapH = ConfigData.IMAPhost.Length > 0;
            textBoxImapH.BackColor = imapH ? Color.White : Color.Red;

            testReady();
        }
        private bool imapH;
        private void ImapP(object sender = null, EventArgs e = null)
        {
            int res;
            imapP = int.TryParse(textBoxImapP.Text, out res);
            ConfigData.IMAPport = res;
            textBoxImapP.BackColor = imapP ? Color.White : Color.Red;

            testReady();
        }
        private bool imapP;
        private void SmtpH(object sender = null, EventArgs e = null)
        {
            ConfigData.SMTPhost = textBoxSmtpH.Text;
            smtpH = ConfigData.SMTPhost.Length > 0;
            textBoxSmtpH.BackColor = smtpH ? Color.White : Color.Red;

            testReady();
        }
        private bool smtpH;
        private void SmtpP(object sender = null, EventArgs e = null)
        {
            int res;
            smtpP = int.TryParse(textBoxSmtpP.Text, out res);
            ConfigData.SMTPport = res;
            textBoxSmtpP.BackColor = smtpP ? Color.White : Color.Red;

            testReady();
        }
        private bool smtpP;
        private void LoginBox(object sender = null, EventArgs e = null)
        {
            ConfigData.Login = textBoxLogin.Text;
            login = ConfigData.Login.Length > 0;
            textBoxLogin.BackColor = login ? Color.White : Color.Red;

            testReady();
        }
        private bool login;
        private void PassBox(object sender = null, EventArgs e = null)
        {
            ConfigData.Pass = textBoxPass.Text;
            pass = ConfigData.Pass.Length > 0;
            textBoxPass.BackColor = pass ? Color.White : Color.Red;

            testReady();
        }
        private bool pass;

        private void Port(object sender = null, EventArgs e = null)
        {
            int res;
            port = int.TryParse(textBoxPort.Text, out res);
            ConfigData.Port = res;
            textBoxPort.BackColor = port ? Color.White : Color.Red;

            testReady();
        }
        private bool port;
        private void SaveChage(object sender = null, EventArgs e = null)
        {
            ConfigData.Save = checkBox1.Checked;
        }
        private bool ipSite;
        private void IpSiteChange(object sender = null, EventArgs e = null)
        {
            ConfigData.IpSite = textBoxIpSite.Text;
            ipSite = ConfigData.IpSite.Length > 0;
            textBoxIpSite.BackColor = ipSite ? Color.White : Color.Red;

            testReady();
        }
    }
}
