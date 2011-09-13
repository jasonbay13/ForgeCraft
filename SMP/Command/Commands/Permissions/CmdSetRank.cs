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
        public override string Description { get { return "Set a player's rank (debug)."; } }
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
			
			if (gr != null && pr != null)
				pr.group = gr;
			
			p.SendMessage("There have a nice day!");
		}
		
		public override void Help(Player p)
		{
			
		}
	}
}