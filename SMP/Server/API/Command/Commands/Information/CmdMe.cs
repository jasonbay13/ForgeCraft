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
using System.Linq;
using System.Text;
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
    public class CmdMe : Command
    {
        public override string Name { get { return "me"; } }
        public override List<String> Shortcuts { get { return new List<string> { }; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Tells everybody what you are doing."; } } //used for displaying what the commands does when using /help
		public override string PermissionNode { get { return "core.info.me"; } }
        
        public override void Use(Player p, params string[] args)
        {
           if (args.Length == 0 || args[0].ToLower() == "help")
            {
                Help(p);
                return;
            }
            
            Player.GlobalMessage(p.username + " " + String.Join(" ", args));
        }

        public override void Help(Player p)
        {
            p.SendMessage("Shows you doing something.");
            p.SendMessage("/me (message)");
        }
    }
}

