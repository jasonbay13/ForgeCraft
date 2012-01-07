/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.API.Commands;
using SMP.PLAYER;
using SMP.ENTITY;

namespace SMP.Commands
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
            p.level.GrowTree(x, y, z, (byte)p.BlockChangeObject, Entity.randomJava);
        }
    }
}
