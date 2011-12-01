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
using System.Text;

namespace SMP.Commands
{
    public class CmdSoftware : Command
    {
        public override string Name { get { return "software"; } }
        public override List<string> Shortcuts { get { return new List<string> { "softwareinfo" }; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Shows you some general information on the ForgeCraft server software."; } }
        public override string PermissionNode { get { return "core.info.software"; } }

        public override void Use(Player p, params string[] args)
        {
            StringBuilder devlist = new StringBuilder();
            foreach (string dev in Server.devs)
                devlist.Append(dev.Capitalize()).Append(',').Append(' ');
            devlist.Remove(devlist.Length - 2, 2);
            p.SendMessage(Color.DarkBlue + "ForgeCraft Development Team: " + Color.DarkRed + devlist.ToString(), WrapMethod.Chat);  //lol it was ForgetCraft

            if (!p.IsConsole)
            {
                short slot = (short)p.inventory.FindEmptySlot();
                if (slot == -1) return;
                if (Server.devs.Contains(p.username.ToLower()))
                    p.inventory.Add(278, 1, 0);
            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/software - " + Description);
        }
    }
}
