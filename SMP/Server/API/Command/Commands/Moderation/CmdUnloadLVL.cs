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
using SMP.API;

namespace SMP.Commands
{
    public class CmdUnloadLVL : Command
    {
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Unload a world!"; } }
        public override string Name { get { return "unloadlvl"; } }
        public override string PermissionNode { get { return "core.world.unload"; } }
        public override System.Collections.Generic.List<string> Shortcuts { get { return new System.Collections.Generic.List<string> { "unload" }; } }


        public override void Use(Player p, params string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    try
                    {
                            World w = World.Find(args[0]);
                        if (w == null) p.SendMessage("Level not found.");
                        else if (w == Server.mainlevel) p.SendMessage("Cannot unload main level.");
                        else
                        {
                            for (int i = 0; i < Player.players.Count; i++)
                            {
                                if (Player.players[i].level == w) Command.core.Find("goto").Use(Player.players[i], Server.mainlevel.name);
                            }
                            w.SaveLVL();
                            w.chunkData = null;
                            //w.generator = null;
                            w.lightningTimer.Dispose();
                            w.timeupdate.Dispose();
                            w.ToGenerate.Clear();
                            w.weatherTimer.Dispose();
                            //w.windows.Clear();
                            World.worlds.Remove(w);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            p.SendMessage("Unloaded " + args[0]);
                        }
                    }
                    catch { p.SendMessage("Failed to unload " + args[0]); }
                    break;
                default:
                    Help(p);
                    break;
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("Unload a level");
            p.SendMessage("/unloadlvl [name]");
        }
    }
}