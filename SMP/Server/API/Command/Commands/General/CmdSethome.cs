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
using System.IO;

namespace SMP.Commands
{
    public class CmdSethome : Command
    {
        public override string Name { get { return "sethome"; } }
        public override List<string> Shortcuts { get { return new List<string> { "sh" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Sets your home location"; } }
        public override string PermissionNode { get { return "core.other.guest"; } }

        public override void Use(Player p, params string[] args)
        {
            if (p == null) { p.SendMessage("You can't execute this command as console."); return; }
            try
            {
                foreach (string line in Server.homedata.ToArray())
                {
                    if (line.Contains(p.username.ToLower()))
                    {
                        Server.homedata.Remove(line);
                    }
                }
                Server.homedata.Add(p.username.ToLower() + "|" + p.pos.X.ToString() + "|" + p.pos.Y.ToString() + "|" + p.pos.Z.ToString());
                File.WriteAllLines("text/homedata.dat", Server.homedata.ToArray());
                p.SendMessage("Your home location has been saved!");
            }
            catch
            {
                p.SendMessage("HOME SAVING ERROR OCCURRED");
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/sethome - Sets your home location to your current location");
        }
    }
}
