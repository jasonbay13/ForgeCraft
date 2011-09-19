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
        public MainWindow()
        {
            InitializeComponent();
            Server.ServerLog += new Server.OnLog(Server_ServerLog);
        }

        void Server_ServerLog(string message)
        {
            log.Text += message;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player.GlobalMessage(textBox1.Text);
        }
    }
}
