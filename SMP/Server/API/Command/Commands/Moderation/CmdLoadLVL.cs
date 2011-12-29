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
    public class CmdLoadLVL : Command
    {
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Load a world!"; } }
        public override string Name { get { return "loadlvl"; } }
        public override string PermissionNode { get { return "core.world.load"; } }
        public override System.Collections.Generic.List<string> Shortcuts { get { return new System.Collections.Generic.List<string> { "load" }; } }


        public override void Use(Player p, params string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    try { if (World.Find(args[0]) == null) { World.LoadLVL(args[0]); p.SendMessage("Loaded " + args[0]); } else p.SendMessage("Level already loaded."); }
                    catch { p.SendMessage("Failed to Load " + args[0]); } 
                    break;
                default:
                    Help(p);
                    break;
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("Loads a level");
            p.SendMessage("/loadlvl [name]");
        }
    }
}