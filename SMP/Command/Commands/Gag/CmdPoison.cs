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
    public class CmdPoison : Command
    {
        public override string Name { get { return "poison"; } }
        public override List<string> Shortcuts { get { return new List<string> { "" }; } }
        public override string Category { get { return "gag"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "poisons the set player"; } }
        public override string PermissionNode { get { return "core.gag.poison"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length < 1)
            {
                Help(p);
                return;
            }
            Player pl = Player.FindPlayer(args[0]);
            if (pl == null) { p.SendMessage("Player " + args[0] + " not found!"); return; }

            if (pl.Mode == 0)
            {
                pl.SlowlyDie(10, 1000, 2);
                if (pl != p) { Player.GlobalMessage(pl.GetName() + " got poisonned by " + p.GetName() + "! D:"); }
                else { Player.GlobalMessage(p.GetName() + " poisonned him/herself! D:"); }
            }
            else
            {
                if (pl != p) { p.SendMessage(pl.GetName() + " is in " + Color.DarkGreen + "Creative" + Color.White + " mode!"); }
                else { p.SendMessage("You are in " + Color.DarkGreen + "Creative" + Color.White + " mode!"); }
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/poison <player> - Does damage to player slowly, till the player's health is halfway.");
        }
    }
}