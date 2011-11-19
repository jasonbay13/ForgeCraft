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
using System.Text;

namespace SMP.Commands
{
    public class CmdSay : Command
    {
        public override string Name { get { return "say"; } }
        public override List<String> Shortcuts { get { return new List<string> {"broadcast"}; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Announces a message to the server"; } } //used for displaying what the commands does when using /help
		public override string PermissionNode { get { return "core.info.say"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 0 || (args.Length == 1 && args[0] == "help"))
            {
                Help(p);
                return;
            }

            Player.GlobalMessage(String.Join(" ", args));
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/say (message)");
        }
    }
}