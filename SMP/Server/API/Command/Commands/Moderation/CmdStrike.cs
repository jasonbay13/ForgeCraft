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

namespace SMP.Commands
{
    public class CmdStrike : Command
    {
        public override string Name { get { return "strike"; } }
        public override List<string> Shortcuts { get { return new List<string> {  }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Strike other players using lightning"; } }
        public override string PermissionNode { get { return "core.weather.strike"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 0 || args.Length > 1) { Help(p); return; }

            Player q = Player.FindPlayer(args[0]);
            
            int x = (int)Math.Round(q.pos.X, 0, MidpointRounding.AwayFromZero);
            int y = (int)Math.Round(q.pos.Y, 0, MidpointRounding.AwayFromZero);
            int z = (int)Math.Round(q.pos.Z, 0, MidpointRounding.AwayFromZero);
            World w = World.Find(p.level.name);
            w.Lightning(x, y, z);

            q.hurt(6);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/strike <victim>");
        }
    }
}