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
    public class CmdKill : Command
    {
        public override string Name { get { return "kill"; } }
        public override List<string> Shortcuts { get { return new List<string> { "murder" }; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Tasty Murder!!"; } }
        public override string PermissionNode { get { return "core.mod.devs"; } }

        public override void Use(Player p, params string[] args)
        {
            // CURRENTLY JUST USING FOR DEBUG
            if (args.Length >= 1)
            {
                bool silent = false;
                string text = args[0];
                if (text[0] == '@')
                {
                    text = text.Remove(0, 1);
                    silent = true;
                }

                Player pl = Player.FindPlayer(args[0]);
                if (pl == null) { p.SendMessage("Player not found.", WrapMethod.Chat); return; }

                Use(pl);
                if (!silent) Player.GlobalMessage(pl.username + " was destroyed by " + p.username);
            }
            else p.hurt(9999, true);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/kill (optional: <playername>)");
            p.SendMessage("place an '@'in front of playername to do it silently");
        }
    }
}
