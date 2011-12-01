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
    public class CmdGameMode : Command
    {
        public override string Name { get { return "gamemode"; } }
        public override List<string> Shortcuts { get { return new List<string> { "gm" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Toggle the game mode"; } }
        public override string PermissionNode { get { return "core.other.gamemode"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length < 1 || String.IsNullOrEmpty(args[0]))
            {
                Server.mode = Server.mode == 0 ? (byte)1 : (byte)0;
                foreach (Player pl in Player.players)
                    pl.Mode = Server.mode;
                Player.GlobalMessage("The server game mode is now " + (Server.mode == 0 ? Color.DarkRed + "Survival" : Color.DarkGreen + "Creative") + Color.ServerDefaultColor + "!");
            }
            else
            {
                Player pl = Player.FindPlayer(args[0]);
                if (pl == null) { p.SendMessage("Could not find player."); return; }
                pl.Mode = pl.Mode == 0 ? (byte)1 : (byte)0;
                pl.SendMessage("Your game mode is now " + (pl.Mode == 0 ? Color.DarkRed + "Survival" : Color.DarkGreen + "Creative") + Color.ServerDefaultColor + "!");
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/gamemode [player]");
        }
    }
}
