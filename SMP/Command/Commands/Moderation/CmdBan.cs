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
	public class CmdBan : Command
	{
		public override string Name { get { return "ban"; } }
        public override List<string> Shortcuts { get { return new List<string> (); } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Bans a player."; } }
		public override string PermissionNode { get { return "core.mod.ban"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length == 0)
			{
				Help(p);
				return;
			}
			
			Player banplayer = Player.FindPlayer(args[0]);
			
			if (banplayer != null)
			{
				if (args.Length >= 2)
				{
					banplayer.Kick("You were banned: " + String.Join(" ", args, 1, args.Length - 1));
					Server.BanList.Add(banplayer.username.ToLower());
				}
				else
				{
					banplayer.Kick("You were banned by " + p.username);
					Server.BanList.Add(banplayer.username.ToLower());
				}
				Player.GlobalMessage(Color.Announce + banplayer.username + " has been banned!");
				
			}
			else
			{
				Server.BanList.Add(args[0]);
				p.SendMessage(HelpBot + args[0] + " has been banned");
			}
			
			foreach(string s in Server.BanList)
				Server.Log(s);
				
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/ban (Player) [Message]");
		}
	}
}
