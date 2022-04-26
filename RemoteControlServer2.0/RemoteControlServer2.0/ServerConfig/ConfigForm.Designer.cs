
namespace RemoteControlServer2._0.ServerConfig
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxInterfaces = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxImapH = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxImapP = new System.Windows.Forms.TextBox();
            this.textBoxSmtpH = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSmtpP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBoxIpSite = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Сетевая карта";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 265);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(299, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Запуск сервера";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxInterfaces
            // 
            this.comboBoxInterfaces.FormattingEnabled = true;
            this.comboBoxInterfaces.Location = new System.Drawing.Point(163, 12);
            this.comboBoxInterfaces.Name = "comboBoxInterfaces";
            this.comboBoxInterfaces.Size = new System.Drawing.Size(151, 21);
            this.comboBoxInterfaces.TabIndex = 2;
            this.comboBoxInterfaces.SelectedIndexChanged += new System.EventHandler(this.ChangeController);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Сервер входящей почты";
            // 
            // textBoxImapH
            // 
            this.textBoxImapH.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxImapH.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxImapH.Location = new System.Drawing.Point(163, 91);
            this.textBoxImapH.Name = "textBoxImapH";
            this.textBoxImapH.Size = new System.Drawing.Size(151, 20);
            this.textBoxImapH.TabIndex = 4;
            this.textBoxImapH.TextChanged += new System.EventHandler(this.ImapH);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Порт входящей почты";
            // 
            // textBoxImapP
            // 
            this.textBoxImapP.Location = new System.Drawing.Point(163, 117);
            this.textBoxImapP.Name = "textBoxImapP";
            this.textBoxImapP.Size = new System.Drawing.Size(151, 20);
            this.textBoxImapP.TabIndex = 6;
            this.textBoxImapP.TextChanged += new System.EventHandler(this.ImapP);
            // 
            // textBoxSmtpH
            // 
            this.textBoxSmtpH.Location = new System.Drawing.Point(163, 143);
            this.textBoxSmtpH.Name = "textBoxSmtpH";
            this.textBoxSmtpH.Size = new System.Drawing.Size(151, 20);
            this.textBoxSmtpH.TabIndex = 7;
            this.textBoxSmtpH.TextChanged += new System.EventHandler(this.SmtpH);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Сервер исходящей почты";
            // 
            // textBoxSmtpP
            // 
            this.textBoxSmtpP.Location = new System.Drawing.Point(163, 170);
            this.textBoxSmtpP.Name = "textBoxSmtpP";
            this.textBoxSmtpP.Size = new System.Drawing.Size(151, 20);
            this.textBoxSmtpP.TabIndex = 9;
            this.textBoxSmtpP.TextChanged += new System.EventHandler(this.SmtpP);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Порт исходящей почты";
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(163, 196);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(151, 20);
            this.textBoxLogin.TabIndex = 11;
            this.textBoxLogin.TextChanged += new System.EventHandler(this.LoginBox);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Логин";
            // 
            // textBoxPass
            // 
            this.textBoxPass.Location = new System.Drawing.Point(163, 222);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.Size = new System.Drawing.Size(151, 20);
            this.textBoxPass.TabIndex = 13;
            this.textBoxPass.TextChanged += new System.EventHandler(this.PassBox);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 225);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Пароль";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(163, 65);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(151, 20);
            this.textBoxPort.TabIndex = 15;
            this.textBoxPort.TextChanged += new System.EventHandler(this.Port);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Порт сервера";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 242);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(120, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Сохранить данные";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.SaveChage);
            // 
            // textBoxIpSite
            // 
            this.textBoxIpSite.Location = new System.Drawing.Point(163, 39);
            this.textBoxIpSite.Name = "textBoxIpSite";
            this.textBoxIpSite.Size = new System.Drawing.Size(151, 20);
            this.textBoxIpSite.TabIndex = 18;
            this.textBoxIpSite.TextChanged += new System.EventHandler(this.IpSiteChange);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(148, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Сайт выдающий внешний Ip";
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(326, 295);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxIpSite);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPass);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxLogin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxSmtpP);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSmtpH);
            this.Controls.Add(this.textBoxImapP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxImapH);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxInterfaces);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigForm";
            this.Text = "Коммуникационные параметры";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxInterfaces;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxImapH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxImapP;
        private System.Windows.Forms.TextBox textBoxSmtpH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSmtpP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBoxIpSite;
        private System.Windows.Forms.Label label9;
    }
}