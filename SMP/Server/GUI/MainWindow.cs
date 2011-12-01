/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
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
