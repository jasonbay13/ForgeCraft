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
using System.Linq;
using System.Text;

namespace SMP
{
    public class CmdReveal : Command
    {
        public override string Name { get { return "reveal"; } }
        public override List<string> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Reload all the visible chunks."; } }
        public override string PermissionNode { get { return "core.general.reveal"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "all")
                {
                    Player.players.ForEach(delegate(Player pl)
                    {
                        Use(pl, "");
                    });
                    p.SendMessage("Chunks reloaded for everyone!");
                }
                else
                {
                    Player pl = Player.FindPlayer(args[0]);
                    if (pl == null) { p.SendMessage("Could not find player!"); return; }
                    Use(pl, "");
                    p.SendMessage("Chunks reloaded for " + pl.GetName() + "!");
                }
            }
            else
            {
                if (p == Server.s.consolePlayer) { p.SendMessage("Console can't be revealed. Try using /reveal <player> or /reveal all", WrapMethod.None); return; }
                p.UpdateChunks(true, true);
                p.SendMessage("Chunks reloaded!");
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage(HelpBot + Description, WrapMethod.Chat);
            p.SendMessage("/reveal");
        }
    }
}
