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
            World w = World.Find(p.level.name);






            //if (args.Length == 1)
            //{

            //    if (args[0] == "off")
            //    {
            try
            {
                if (args[0] == "status") { p.SendMessage(Color.Purple + "Rain is: " + w.Israining); return; }
            }
            catch { }

            if (w.Israining)
            {

                w.rain(false);

                p.SendMessage(Color.Red + "Stopping rain..");
                w.Israining = false;

            }

                   // p.SendMessage("rain is: " + w.isRain().ToString());

              //  }
            //if (args[0] == "on")
            //{

            else
            {

                w.SendLightning(1, 1, 100, 2, p);
                w.rain(true);

                w.Israining = true;
                //  p.SendMessage("rain is: " + w.isRain().ToString());

                p.SendMessage(Color.Green + "Starting rain...");



                //  }
            }


            // }
            //  else { Help(p); return; }

        }

        public override void Help(Player p)
        {

            p.SendMessage(Color.Blue + "/rain....was that so hard?");
        }
    }
}