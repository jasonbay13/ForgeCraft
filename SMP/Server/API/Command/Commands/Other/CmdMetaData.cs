using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
    class CmdMetaData : Command
    {
        public override string Name { get { return "metadata"; } }
        public override List<string> Shortcuts { get { return new List<string> { "meta" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Change block metadata."; } }
        public override string PermissionNode { get { return "core.other.metadata"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length < 1) { Help(p); return; }

            byte meta = 0;
            try { meta = byte.Parse(args[0]); }
            catch { p.SendMessage("Invalid input."); return; }

            if (meta < 0)
                meta = 0;
            else if (meta > 15)
                meta = 15;

            p.ClearBlockChange();
            p.BlockChangeObject = meta;
            p.OnBlockChange += Blockchange1;
            p.SendMessage("Place/delete a block to change it's meta data.");
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }

        void Blockchange1(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            p.level.BlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), (byte)p.BlockChangeObject);
            p.SendMessage("Metadata set!");
        }
    }
}
