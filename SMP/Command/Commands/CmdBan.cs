using System;
using System.Collections.Generic;

namespace SMP
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
