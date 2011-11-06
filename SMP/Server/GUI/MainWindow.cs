using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SMP.GUI
{
    public partial class MainWindow : Form
    {
        public static MainWindow This = new MainWindow();
        delegate void LogDelegate(string message);
        public MainWindow()
        {
            InitializeComponent();
            Server.ServerLog += new Server.OnLog(Log);
        }

        public static void Log(string message)
        {
            This.log(message);
        }
        public void log(string message)
        {
            
            if (this.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(log);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                txtLog.AppendText(Environment.NewLine + message);
                
            }
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Text = "ForgeCraft v" + Server.version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player.GlobalMessage(textBox1.Text);
        }
    }
}
