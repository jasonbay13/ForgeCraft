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
using SMP.API;

namespace SMP.Commands
{
    public class CmdSaveLVL : Command
    {
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Save a world!"; } }
        public override string Name { get { return "savelvl"; } }
        public override string PermissionNode { get { return "core.world.save"; } }
        public override System.Collections.Generic.List<string> Shortcuts { get { return new System.Collections.Generic.List<string> { "save" }; } }


        public override void Use(Player p, params string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    World.SaveLVL(p.level); p.SendMessage("Saved " + p.level.name); break;
                case 1:
                    World w = World.Find(args[0]);
                    if (w == null) { p.SendMessage("Spcified world does not exist: " + p.level.name); return; }
                    World.SaveLVL(w); p.SendMessage("Saved " + w.name);
                    break;
                default:
                    Help(p);
                    break;
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("Create a new level");
            p.SendMessage("/savelvl (name)");
        }
    }
}