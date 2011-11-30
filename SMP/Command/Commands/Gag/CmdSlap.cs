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
    public class CmdSlap : Command
    {
        public override string Name { get { return "slap"; } }
        public override List<string> Shortcuts { get { return new List<string> { "slap", "slaps" }; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Slap a player senseless, or give them fair warning."; } }
        public override string PermissionNode { get { return "core.general.slap"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length <= 1 && !p.IsConsole)
            {
                p.hurt(2);
                p.SendMessage(Color.DarkRed + "You slapped yourself in confusion.");
                p.SendMessage(Color.Yellow + "Who were you trying to slap again?");
                Help(p);
                return;
            }
            else
            {
                Player q = Player.FindPlayer(args[1]);
                if (q == null)
                {
                    if (!p.IsConsole) { Help(p); } else { return; }
                }
                else
                {
                    if (!p.IsConsole)
                    {
                        bool slapSelf = new Random().Next(0, 100) % 2 == 1;
                        if (slapSelf == true)
                        {
                            p.hurt(2);
                            p.SendMessage(Color.DarkRed + "You miss " + q.username + "'s face, and fall to the ground.");
                            p.SendMessage(Color.Yellow + "Try harder next time.");
                            return;
                        }
                        else
                        {
                            q.hurt(2);
                            q.SendMessage(Color.Teal + p.username + " slapped you, how rude.");
                            return;
                        }
                    }
                    else
                    {
                        q.hurt(2);
                        q.SendMessage(Color.Teal + "You were slapped by the console, but you probably deserved it.");
                        return;
                    }
                }
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/slap <player>");
            return;
        }
    }
}