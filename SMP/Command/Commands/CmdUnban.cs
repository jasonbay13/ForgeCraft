using System;
using System.Collections.Generic;

namespace SMP
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