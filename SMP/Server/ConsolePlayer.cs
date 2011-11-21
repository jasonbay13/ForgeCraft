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
using System.Text;
using System.Net;
using System.Net.Sockets;

//for console commands mainly and removes color codes from text

namespace SMP {

    /// <summary>
    /// Pseudo-player for handling console commands.
    /// </summary>
    public class ConsolePlayer : Player
    {

        public ConsolePlayer(Server server)
            : base()
        {
            this.group = new ConsoleGroup();
            username = Server.ConsoleName;
            ip = "127.0.0.1";
        }

        protected override void SendMessageInternal(string message)
        {
            Server.ServerLogger.Log(LogLevel.Info, ParseColors(message) );
        }

        /// <summary>
        /// Removes color codes from a string.
        /// </summary>
        private static string ParseColors(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§' && text.Length > i + 1 && ((text[i + 1] >= 'a' && text[i + 1] <= 'f') ||
                    (text[i + 1] >= 'A' && text[i + 1] <= 'F') || (text[i + 1] >= '0' && text[i + 1] <= '9')))
                {
                    i++;
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }
		
		public void SetUsername(string name)
		{
			 username = name;
		}
    }
}
