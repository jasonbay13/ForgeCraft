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
	public class CmdUnban : Command
	{
		public override string Name { get { return "unban"; } }
        public override List<string> Shortcuts { get { return new List<string> {"uban"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Unban a player"; } }
		public override string PermissionNode { get { return "core.mod.unban"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length == 0)
			{
				Help(p);
				return;
			}
			
			if (Server.BanList.Contains(args[0].ToLower()))
			{
				Server.BanList.Remove(args[0].ToLower());
				p.SendMessage(HelpBot + args[0] + " was unbanned.");
			}
			else p.SendMessage(HelpBot + "Couldn't find that banned player.");
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/unban (Player)");
		}
	}
}