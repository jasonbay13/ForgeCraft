using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.Commands
{
    public class CmdMoonPhase : Command
    {
        public override string Name { get { return "moonphase"; } }
        public override List<string> Shortcuts { get { return new List<string> { "moon" }; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Set the moon phase"; } }
        public override string PermissionNode { get { return "core.mod.moonphase"; } }


        public override void Use(Player p, params string[] args)
        {
            if (args.Length < 1) { Help(p); return; }
            try
            {
                p.level.moonPhase = byte.Parse(args[0]);
            }
            catch { Help(p); return; }
        }
        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("moonphase <phase>");
        }
    }
}
