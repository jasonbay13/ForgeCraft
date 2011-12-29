using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.API;

namespace SMP.Commands
{
    public class CmdCredits : Command
    {
        public override string Name { get { return "credits"; } }
        public override List<string> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Shows minecraft credits"; } }
        public override string PermissionNode { get { return "core.other.credits"; } }

        public override void Use(Player p, params string[] args)
        {
            p.SendState(4);
        }

        public override void Help(Player p)
        {
            p.SendMessage(HelpBot + Description, WrapMethod.Chat);
            p.SendMessage("/credits");
        }
    }
}
