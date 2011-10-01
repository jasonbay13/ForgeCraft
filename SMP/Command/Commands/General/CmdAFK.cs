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
	public class CmdAFK : Command
	{
		public override string Name { get { return "afk"; } }
        public override List<string> Shortcuts { get { return new List<string> {"away"}; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Sets your status to away."; } }
		public override string PermissionNode { get { return "core.general.afk"; } }

        public override void Use(Player p, params string[] args)
		{
			if (p.AFK)
			{
				p.AFK = false;
				Player.GlobalMessage(p.username + "is back.");
				return;
			}
			if (args.Length == 0)
			{
				p.AFK = true;
				Player.GlobalMessage(p.username + " is AFK");
				return;
			}
			else
			{
				p.AFK = true;
				Player.GlobalMessage(p.username + " is away, " + String.Join(" ", args));
				return;
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(HelpBot + Description, WrapMethod.Chat);
			p.SendMessage("/afk (message)");
		}
	}
}