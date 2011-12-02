using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.Commands
{
    public class CmdPauseTime : Command
    {
        public override string Name { get { return "pausetime"; } }
        public override List<string> Shortcuts { get { return new List<string>() { "pt" }; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Pauses time. Duh!"; } }
        public override string PermissionNode { get { return "core.mod.pausetime"; } }

        public override void Use(Player p, params string[] args)
        {
            if (p.IsConsole && args.Length < 1) { Help(p); return; }

            World world = p.level;
            if (args.Length > 0) {
                world = World.Find(args[0]);
                if (world == null) { p.SendMessage("World not found!", WrapMethod.Chat); return; }
            }

            world.pauseTime = !world.pauseTime;
            Player.GlobalMessage("Time is now " + (world.pauseTime ? "paused" : "unpaused") + "!");
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/pausetime [world]");
        }
    }
}
