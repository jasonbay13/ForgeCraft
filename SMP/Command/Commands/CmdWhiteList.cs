using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdWhiteList : Command
	{
		public override string Name { get { return "whitelist"; } }
        public override List<string> Shortcuts { get { return new List<string> {"wl", "wlist"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Add/remove player(s) from the whitelist"; } }
		public override string PermissionNode { get { return "core.mod.whitelist"; } }

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
				
				if(Server.WhiteList.Contains(name))
				{
					Server.WhiteList.Remove(name);
					p.SendMessage(HelpBot + name + " removed from whitelist.");
				}
				else
				{
					Server.WhiteList.Add(name);							
					p.SendMessage(HelpBot + name + " added to whitelist.");
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
					
					if(Server.WhiteList.Contains(name))
					{
						Server.WhiteList.Remove(name);
					}
					else
					{
						Server.WhiteList.Add(name);							
					}
				}
				p.SendMessage(HelpBot + "whitelist modified.");
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/whitelist (Player) [Player] [Player] [etc]");
		}
	}
}