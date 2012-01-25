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
    public class CmdGotoLVL : Command
    {
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Go to a world!"; } }
        public override string Name { get { return "goto"; } }
        public override string PermissionNode { get { return "core.world.goto"; } }
        public override System.Collections.Generic.List<string> Shortcuts { get { return new System.Collections.Generic.List<string> { "g" }; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length != 1) { Help(p); return; }

            World w = World.Find(args[0]);
            if (w == null) { p.SendMessage("Could not find specified level"); return; }
            if (p.level.name == w.name) { p.SendMessage("Already in " + p.level.name); return; }

            Player.players.ForEach(p1 => { if (p1.level == p.level) { p1.SendDespawn(p.id); p.SendDespawn(p1.id); } }); //dont want to be seen on 2 maps at once do we?
            //p.VisibleChunks.ForEach(pt => p.SendPreChunk(pt.x, pt.z, 0));  //apparently not needed since respawn packet is used.
            p.VisibleChunks.Clear();

            p.SaveLoc(); //when we go back to that level we end up at the same place.
            p.level = w;
            p.SendRespawn(); //loading screen and map settings information changes.
            p.pos = p.Saved_Pos();
            p.UpdateChunks(true, true);
            p.Teleport_Saved_Pos(); //send to saved position
            return;
        }
        public override void Help(Player p)
        {
            p.SendMessage("Goto a new level");
        }
    }
}

