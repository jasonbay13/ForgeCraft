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

namespace SMP.Commands
{
    public class CmdFlat : Command
    {
        public override string Name { get { return "flat"; } }
        public override List<string> Shortcuts { get { return new List<string> { "fl" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Creates a flat area to work on"; } }
        public override string PermissionNode { get { return "core.build.flat"; } }

        public override void Use(Player p, params string[] args)
        {
            World w = p.level;
            if (args.Length < 4) { Help(p); return; }
            int width = 0;
            int length = 0;
            int height = 0;
            byte block = 2;
            try { width = Convert.ToInt32(args[0]); length = Convert.ToInt32(args[1]); height = Convert.ToInt32(args[2]); block = Convert.ToByte(args[3]); }
            catch { Help(p); return; }
            width /= 2;
            length /= 2;
            int y = (int)p.pos.y - 1;
            int x = (int)p.pos.x - width;
            int z = (int)p.pos.z - length;
            int oldx = x;
            int oldz = z;
            int endx = (int)p.pos.x + width;
            int endz = (int)p.pos.z + length;
            int endy = (int)p.pos.y + height;
            while (x < endx)
            {
                z = oldz;
                while (z < endz)
                {
                    w.BlockChange(x, y, z, block, 0);
                    z++;
                }
                x++;
            }
            while (y < endy)
            {
                x = oldx;
                z = oldz;
                y++;
                while (x < endx)
                {
                    z = oldz;
                    while (z < endz)
                    {
                        w.BlockChange(x, y, z, 0, 0);
                        z++;
                    }
                    x++;
                }
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/flat <width> <length> <height> <block> - Makes a flat area,");
            p.SendMessage("and makes a air space above it.");
        }
    }
}