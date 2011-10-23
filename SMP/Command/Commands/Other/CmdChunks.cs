﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    class CmdChunks : Command
    {
        public override string Name { get { return "chunks"; } }
        public override List<string> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "View chunk count."; } }
        public override string PermissionNode { get { return "core.other.chunks"; } }

        public override void Use(Player p, params string[] args)
        {
            World.worlds.ForEach(delegate(World w)
            {
                p.SendMessage(w.name);
                p.SendMessage("Chunks: " + w.chunkData.Count);
                p.SendMessage("Physics: " + w.physics.CheckCount);
                p.SendMessage("Updates: " + w.physics.UpdateCount);
            });
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }
    }
}
