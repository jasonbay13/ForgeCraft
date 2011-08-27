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

namespace SMP
{
    public class CmdDevs : Command
    {
        List<string> devs = new List<string> { "Silentneeb", "BizarreCake", "GamezGalaxy (hypereddie10)", "Merlin33069", "jakeanator14", "headdetect", "The_Legacy", "Dmitchell" }; //add your names here

        public override string Name { get { return "devs"; } }
        public override List<string> Shortcuts { get { return new List<string> {"developers", "authors"}; } }
        public override string Category { get { return "information"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Shows you who developed ForgeCraft."; } }
		public override string PermissionNode { get { return "core.info.devs"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length > 0 && args[0] == "help")
            {
                Help(p);
                return;
            }

            string devlist = "";
            string temp;
            foreach (string dev in devs)
            {
                temp = dev.Substring(0, 1);
                temp = temp.ToUpper() + dev.Remove(0, 1);
                devlist += temp + ", ";
            }
            devlist = devlist.Remove(devlist.Length - 2);
            p.SendMessage(Color.DarkBlue + "ForgeCraft Development Team: " + Color.DarkRed + devlist, WrapMethod.Chat);  //lol it was ForgetCraft
			short slot = (short)p.inventory.FindEmptySlot();
			if (slot == -1) return;
			p.SendItem(slot, 278, 1, 3);
        }

        public override void Help(Player p)
        {
            p.SendMessage("/devs - Displays the list of ForgeCraft developers.");
        }
    }
}
