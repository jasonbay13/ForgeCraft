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
	public class CmdPromote : Command
	{
		public override string Name { get { return "promote"; } }
        public override List<string> Shortcuts { get { return new List<string> {"pr"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Promote a player."; } }
		public override string PermissionNode { get { return "core.mod.promote"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length == 0)
			{
				Help(p);
				return;
			}
			
			Player pr = Player.FindPlayer(args[0]);
			if (pr == null)
			{
				p.SendMessage(HelpBot + "Could not find player.");
				return;
			}
			if (pr == p)
			{
				p.SendMessage(HelpBot + "You can't promote yourself.");
				return;
			}
			if (GroupUtils.PromotePlayer(pr))
			{
				p.SendMessage(HelpBot + "Player promoted.");
				pr.SendMessage(HelpBot + p.username + " promoted you. Congratulations!");
			}
			else
				p.SendMessage(HelpBot + "Could not promote player");
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/promote [Player]");
		}
	}
}