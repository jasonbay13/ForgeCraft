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
using SMP.API.Commands;
using SMP.PLAYER;
using SMP.util;

namespace SMP.Commands
{
    public class CmdMapInfo : Command
    {
        public override string Name { get { return "mapinfo"; } }
        public override List<string> Shortcuts { get { return new List<string> { "" }; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Displays info for current map if none specified."; } }
        public override string PermissionNode { get { return "core.other.mapinfo"; } }

        public override void Use(Player p, params string[] args)
        {
            World w;
            if (p == null)
            {
                if (args.Length != 1) Logger.Log("Must specify a map.");
                w = World.Find(args[0]);
                if (w == null) { Logger.Log("Could not find level"); return; }
                Logger.Log("Map Info For: " + w.name);
                Logger.Log("Seed: " + w.seed);
                Logger.Log(String.Format("Spawn: {0} {1} {2}", w.SpawnX, w.SpawnY, w.SpawnZ));
                Logger.Log(w.ChunkLimit != 1875000 ? "Chumk Limit: " + w.ChunkLimit : "Chumk Limit: None");
                Logger.Log("Level-Type: " + w.leveltype);
                Logger.Log("Time: " + w.time);
                return;
            }

            switch (args.Length)
            {
                case 1: w = World.Find(args[0]); break;
                case 0: w = p.level; break;
                default: Help(p); return;
            }
            if (w == null) { p.SendMessage("Could not find level"); return; }
            p.SendMessage("Map Info For: " + w.name);
            p.SendMessage("Seed: " + w.seed);
            p.SendMessage(String.Format("Spawn: {0} {1} {2}", w.SpawnX, w.SpawnY, w.SpawnZ));
            p.SendMessage(w.ChunkLimit != 1875000 ? "Chumk Limit: " + w.ChunkLimit : "Chumk Limit: None");
            p.SendMessage("Level-Type: " + w.leveltype);
            p.SendMessage("Time: " + w.time);
        }

        public override void Help(Player p)
        {
            if (p == null) { Logger.Log("/mapinfo (mapname)"); Logger.Log(Description); return; }
            p.SendMessage("/mapinfo (mapname)");
            p.SendMessage(Description);
        }
    }
}