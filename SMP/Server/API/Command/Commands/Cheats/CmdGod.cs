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
using SMP.API;

namespace SMP.Commands
{
	public class CmdGod : Command
	{
		public override string Name { get { return "god"; } }
        public override List<string> Shortcuts { get { return new List<string> {"invincible"}; } }
        public override string Category { get { return "cheats"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Mine like a coward!"; } }
		public override string PermissionNode { get { return "core.cheat.god"; } }

        public override void Use(Player p, params string[] args)
		{
			//maybe add a silent option
			if (args.Length >= 1)
			{
				Help(p);
			}
			
			//p.SendMessage(Color.DarkTeal + "Currently doesn't do anything. :(");
			if (!p.GodMode)
			{
				p.GodMode = true;
				p.SendMessage("You are now invincible. Type /god again to be a mortal", WrapMethod.Chat);
				Player.GlobalMessage(p.username + " is now being cheap and immortal, kill them!");
			}
			else if (p.GodMode)
			{
				p.GodMode = false;
				p.SendMessage("You are no longer invincible.", WrapMethod.Chat);
				Player.GlobalMessage(p.username + " is no longer being a wuss, don't kill them.");
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/god");
		}
	}
}


