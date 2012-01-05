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
using SMP.API;

namespace SMP.Commands
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
