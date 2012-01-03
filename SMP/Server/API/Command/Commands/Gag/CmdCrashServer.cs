/*
	Copyright 2010 MCLawl (Modified for use with MCForge)
	
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
using SMP.API;

namespace SMP.Commands
{
    public class CmdCrashServer : Command
    {
        public override string Name { get { return "crashserver"; } }
        public override List<string> Shortcuts { get { return new List<string> { "crash" }; } }
        public override string Category { get { return "cheats"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Crash the server like a boss."; } }
        public override string PermissionNode { get { return "core.cheat.crashserver"; } }

        public override void Use(Player p, params string[] args)
        {
			p.Kick(HelpBot + Color.DarkRed + "Your IP is being backtraced and forwarded to the FBI!"); 
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/crashserv or /crash");
		}
	}
}

