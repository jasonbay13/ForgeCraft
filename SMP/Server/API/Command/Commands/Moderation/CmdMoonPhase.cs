using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.API;

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
                byte phase = byte.Parse(args[0]);
                if (phase < 0 || phase > 7) { p.SendMessage("Phase must be between 0 and 7 inclusive.", WrapMethod.Chat); return; }
                p.level.moonPhase = phase;
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
