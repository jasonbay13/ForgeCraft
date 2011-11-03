using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    class CmdAbout : Command
    {
        public override string Name { get { return "about"; } }
        public override List<string> Shortcuts { get { return new List<string> { "b" }; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "View block info."; } }
        public override string PermissionNode { get { return "core.info.about"; } }

        public override void Use(Player p, params string[] args)
        {
            p.ClearBlockChange();
            p.OnBlockChange += Blockchange1;
            p.SendMessage("Place/delete a block to get info.");
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }

        void Blockchange1(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), p.level.GetMeta(x, y, z));

            p.SendMessage("Position: " + x + "," + y + "," + z);
            p.SendMessage("Type: " + p.level.GetBlock(x, y, z));
            p.SendMessage("Meta: " + p.level.GetMeta(x, y, z));
            p.SendMessage("Extra: " + p.level.GetExtra(x, y, z));
        }
    }
}
