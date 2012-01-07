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
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
    public class CmdNewLVL : Command
    {
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Create a new world!"; } }
        public override string Name { get { return "newlvl"; } }
        public override string PermissionNode { get { return "core.world.create"; } }
        public override System.Collections.Generic.List<string> Shortcuts { get { return new System.Collections.Generic.List<string> { }; } }


        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 0) { Help(p); return; }
            else if (args.Length == 1)
            {
                Random rand = new Random();
                int seed = new Random().Next();
                p.SendMessage("Creating world with seed: " + seed);
                double x = 0; double y = 127; double z = 0;
                World temp = new World(x, y, z, args[0], seed);
                //while (Chunk.GetChunk((int)x, (int)z, temp).GetBlock((int)x, (int)(y - 1), (int)z) == 0)
                //	y--;
                temp.SpawnY = y;
                World.worlds.Add(temp);
                p.SendMessage("World " + args[0] + " MADE!");
            }
            else if (args.Length == 2 || args.Length == 3)
            {
                int seed = Convert.ToInt32(args[1]);
                p.SendMessage("Creating world with seed: " + seed);
                double x = 0; double y = 127; double z = 0;
                World temp = new World(x, y, z, args[0], seed);
                if (args.Length == 3)
                {
                    int limit = Convert.ToInt32(args[2]);
                    if (limit > 2)
                        temp.ChunkLimit = limit;
                    else { p.SendMessage("maxchunks cannot be less than 3. creating with maxchunks 3."); temp.ChunkLimit = 3; }
                }
                World.worlds.Add(temp);
                p.SendMessage("World " + args[0] + " MADE!");
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("Create a new level");
            p.SendMessage("/newlvl [name] (seed (maxchunks))");
            p.SendMessage("maxchunks of 4 is a 128x128x128 map. Minimum is 3");
        }
    }
}

