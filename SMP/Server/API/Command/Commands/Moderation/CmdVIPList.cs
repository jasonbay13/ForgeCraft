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
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
	public class CmdVIPList : Command
	{
		public override string Name { get { return "vip"; } }
        public override List<string> Shortcuts { get { return new List<string> {"viplist"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Add/remove player(s) from the vip list"; } }
		public override string PermissionNode { get { return "core.mod.vip"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length == 0)
			{
				Help(p);
				return;
			}
			
			if (args.Length == 1)
			{
				string name;
				Player pl = Player.FindPlayer(args[0].ToLower());
				
				if (pl != null)
					name = pl.username.ToLower();
				else
					name = args[0].ToLower();
				
				if(Server.VIPList.Contains(name))
				{
					Server.VIPList.Remove(name);
					p.SendMessage(HelpBot + name + " removed from VIP List.");
				}
				else
				{
					Server.VIPList.Add(name);							
					p.SendMessage(HelpBot + name + " added to VIP List.");
				}
			}
			else
			{
				foreach(string s in args)
				{
					string name;
					Player pl = Player.FindPlayer(s.ToLower());
					
					if (pl != null)
						name = pl.username.ToLower();
					else
						name = s.ToLower();
					
					if(Server.VIPList.Contains(name))
					{
						Server.VIPList.Remove(name);
					}
					else
					{
						Server.VIPList.Add(name);							
					}
				}
				p.SendMessage(HelpBot + "VIP list modified.");
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/vip (Player) [Player] [Player] [etc]");
		}
	}
}