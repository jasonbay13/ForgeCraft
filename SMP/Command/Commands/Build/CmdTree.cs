using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class CmdTree : Command
    {
        public override string Name { get { return "tree"; } }
        public override List<string> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "build"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Tree"; } }
        public override string PermissionNode { get { return "core.build.tree"; } }

        public override void Use(Player p, params string[] args)
        {
            p.ClearBlockChange();
            p.BlockChangeObject = args.Length > 0 ? byte.Parse(args[0]) : (byte)0;
            p.OnBlockChange += Blockchange1;
            p.SendMessage("Place/delete a block where you want the tree.");
            //p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }

        void Blockchange1(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            new GenTrees().Normal(p.level, x, y, z, (byte)p.BlockChangeObject);
        }
    }
}
