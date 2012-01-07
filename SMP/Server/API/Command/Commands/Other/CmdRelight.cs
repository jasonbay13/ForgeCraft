using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
    public class CmdRelight : Command
    {
        public override string Name { get { return "relight"; } }
        public override List<String> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "Other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Relights all loaded chunks."; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.other.relight"; } }

        public override void Use(Player p, params string[] args)
        {
            p.SendMessage("Relighting all chunks! Please wait...", WrapMethod.Chat);
            foreach (Chunk c in p.level.chunkData.Values.ToArray())
                c.RecalculateLight();
            p.SendMessage("Done!");
            Command.all.Find("reveal").Use(p, "all");
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/relight");
        }
    }
}
