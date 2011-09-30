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

namespace SMP
{
	public class CmdSetRank : Command
	{
		public override string Name { get { return "setrank"; } }
        public override List<string> Shortcuts { get { return new List<string> {}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Set a player's rank."; } }
		public override string PermissionNode { get { return "core.mod.setrank"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length < 2)
			{
				Help(p);
				return;
			}
			
			Player pr = Player.FindPlayer(args[0]);
			Group gr = Group.FindGroup(args[1]);
			
			if (pr == p)
			{
				p.SendMessage(HelpBot + "You can't change your own rank.");
				return;
			}
			
			if (!GroupUtils.IsHigherRank(p.group, gr))
			{
				p.SendMessage(HelpBot + "You can't rank someone higher than your own rank.");
				return;
			}
			if (gr != null && pr != null)
			{
				pr.group = gr;
				p.SendMessage("There have a nice day!");
				pr.SendMessage(HelpBot + p.username + " set your rank to " + gr.Name + ". Congratulations!");
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/setrank [Player] [Group]");
		}
	}
}