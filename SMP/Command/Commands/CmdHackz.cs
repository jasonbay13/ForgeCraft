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

// for LULZ
//Epic Win, we should have the MCLawl /hacks message for teh lulz. 
namespace SMP
{
	public class CmdHackz : Command
	{
		public override string Name { get { return "hackz"; } }
        public override List<string> Shortcuts { get { return new List<string> {"hacks", "hack", "hax"}; } }
        public override string Category { get { return "cheats"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Hack the server like a pro."; } }
		public override string PermissionNode { get { return "core.cheat.hackz"; } }

        public override void Use(Player p, params string[] args)
		{
			p.Kick(HelpBot + Color.DarkRed + "YOU FAIL!"); 
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/hackz");
		}
	}
}
