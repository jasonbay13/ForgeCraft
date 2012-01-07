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
	public class CmdDND : Command
	{
		public override string Name { get { return "donotdisturb"; } }
        public override List<string> Shortcuts { get { return new List<string> {"dnd", "mineinpeace"}; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Mine in peace and quiet."; } }
		public override string PermissionNode { get { return "core.other.donotdisturb"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args.Length >= 1)
			{
				Help(p);
			}
			
			if (!p.DoNotDisturb)
			{
				p.DoNotDisturb = true;
				p.SendMessage("You will not be able to recieve or send any global chat. Type /dnd again to recieve chat again", WrapMethod.Chat);
			}
			else if (p.DoNotDisturb)
			{
				p.DoNotDisturb = false;
				p.SendMessage("You will now be able to recieve and send global chat again. I don't why you'd want to though.", WrapMethod.Chat);
			}
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/dnd");
		}
	}
}

