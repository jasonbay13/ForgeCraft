﻿/*
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
    public class CmdHome : Command
    {
        public override string Name { get { return "home"; } }
        public override List<string> Shortcuts { get { return new List<string> { "" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Teleports you to your home location"; } }
        public override string PermissionNode { get { return "core.other.guest"; } }

        public override void Use(Player p, params string[] args)
        {
            if (p == null) { p.SendMessage("You can't execute this command as console!"); return; }
            string line = "NONE";
            foreach (string line2 in Server.homedata.ToArray())
            {
                if (line2.Contains(p.username.ToLower()))
                {
                    line = line2;
                }
            }
            if (line == "NONE") { p.SendMessage("You haven't set a home yet! Use /sethome"); return; }
            p.Teleport_Player(Convert.ToDouble(line.Split('|')[1]), Convert.ToDouble(line.Split('|')[2]), Convert.ToDouble(line.Split('|')[3])); 
            p.SendMessage("You have been teleported to your home!");
        }

        public override void Help(Player p)
        {
            p.SendMessage("/home - teleports you to your home location");
        }
    }
}