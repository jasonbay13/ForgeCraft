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

namespace SMP.Commands
{
	public class CmdMsg : Command
	{
		public override string Name { get { return "msg"; } }
        public override List<string> Shortcuts { get { return new List<string> {"m", "tell"}; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Send a player a message"; } }
		public override string PermissionNode { get { return "core.general.message"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length <= 1)
			{
				Help(p);
				return;
			}
			
			Player targetP = Player.FindPlayer(args[0]);
			if (targetP != null)
			{
				targetP.SendMessage(Color.DarkRed + "[" + p.username + ">>> Me]" + Color.White + String.Join(" ", args, 1, args.Length - 1));
				p.SendMessage(HelpBot + "Message Sent.");
			}
			else
			{
				p.SendMessage(HelpBot + "Could not find specified player.");	
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description);
			p.SendMessage("/msg <player> <message>");
		}
	}
}