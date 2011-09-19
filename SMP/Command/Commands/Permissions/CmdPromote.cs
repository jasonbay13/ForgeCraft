using System;
using System.Collections.Generic;

namespace SMP
{
	public class CmdPromote : Command
	{
		public override string Name { get { return "promote"; } }
        public override List<string> Shortcuts { get { return new List<string> {"pr"}; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Promote a player. (Debug)"; } }
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
			
			if (GroupUtils.PromotePlayer(p))
			{
				p.SendMessage(HelpBot + "Player promoted.");
			}
			else
				p.SendMessage(HelpBot + "Could not promote player");
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(":P");
		}
	}
}