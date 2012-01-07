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
    public class CmdRain : Command
    {
        public override string Name { get { return "rain"; } }
        public override List<string> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Makes it rain?"; } }
        public override string PermissionNode { get { return "core.weather.rain"; } }

        public override void Use(Player p, params string[] args)
        {
            try
            {
                if (args[0] == "status") { p.SendMessage(Color.Purple + "Rain is: " + p.level.IsRaining); return; }
            }
            catch { }

			if (p.level.IsRaining)
            {
				p.SendMessage(Color.Red + "Stopping rain..");
                p.level.Rain(false);

            }
            else
            {
				p.SendMessage(Color.Red + "Starting rain..");
				p.level.Rain(true);
            }
        }

        public override void Help(Player p)
        {

            p.SendMessage(Color.Blue + "/rain....was that so hard?");
        }
    }
}