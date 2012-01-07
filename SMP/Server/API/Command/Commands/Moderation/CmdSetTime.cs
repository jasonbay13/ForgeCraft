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
using SMP.API.Commands;
using SMP.PLAYER;

namespace SMP.Commands
{
	public class CmdSetTime : Command
	{
		public override string Category {
			get {
				return "Mod";
			}
		}
		public override bool ConsoleUseable {
			get {
				return true;
			}
		}
		public override string Description {
			get {
				return "Change a world time";
			}
		}
		public override string Name {
			get {
				return "settime";
			}
		}
		public override string PermissionNode {
			get {
				return "core.world.time.set";
			}
		}
		public override System.Collections.Generic.List<string> Shortcuts {
			get {
				return new System.Collections.Generic.List<string>{ "time" };
			}
		}
		public override void Use (Player p, params string[] args1)
		{
			string args = "";
			for (int i = 0; i < args1.Length; i++) { if (args == "") args = args1[i]; else args = args + " " + args1[i]; }
			if (args.Split(' ').Length == 1 && p == null)
			{
			retry:
				try
				{
				//CONSOLE / PLAYER SETTING GLOBAL TIME
				World.worlds.ForEach(delegate(World w) { w.time = Convert.ToInt32(args); });
				}
				catch 
				{
					//CONSOLE / PLAYER SETTING GLOBAL TIME (DAY, NIGHT)
					if (args.ToLower() == "day") { args = "0"; goto retry; }
					else if (args.ToLower() == "night") { args = "12000"; goto retry; }
					else if (args.ToLower() == "noon") { args = "6000"; goto retry; }
					else if (args.ToLower() == "midnight") { args = "18000"; goto retry; }
					else { p.SendMessage("Invalid Time!"); return; }
				}
			}
			else if (args.Split(' ').Length == 1 && p != null)
			{
			retry:
				try
				{
				//PLAYER SETTING CURRENT LEVEL TIME
				p.level.time = Convert.ToInt32(args);
				}
				catch 
				{
					//PLAYER SETTING CURRENT LEVEL TIME (DAY, NIGHT)
					if (args.ToLower() == "day") { args = "0"; goto retry; }
					else if (args.ToLower() == "night") { args = "12000"; goto retry; }
					else if (args.ToLower() == "noon") { args = "6000"; goto retry; }
					else if (args.ToLower() == "midnight") { args = "18000"; goto retry; }
					else { p.SendMessage("Invalid Time!"); return; }
				}
			}
			else if (args.Split(' ').Length != 1)
			{
				if (args.Split(' ')[0].ToLower() == "global") { 
					args = args.Split(' ')[1]; 
				retry:
					try
					{
						//CONSOLE / PLAYER SETTING GLOBAL TIME
						World.worlds.ForEach(delegate(World w) { w.time = Convert.ToInt32(args); });
					}
					catch 
					{
						//CONSOLE / PLAYER SETTING GLOBAL TIME (DAY, NIGHT)
						if (args.ToLower() == "day") { args = "0"; goto retry; }
						else if (args.ToLower() == "night") { args = "12000"; goto retry; }
						else if (args.ToLower() == "noon") { args = "6000"; goto retry; }
						else if (args.ToLower() == "midnight") { args = "18000"; goto retry; }
						else { p.SendMessage("Invalid Time!"); return; }
					}
				}
				else
				{
					World w = World.Find(args.Split(' ')[0]);
					if (w != null)
					{
					retry2:
						try
						{
							w.time = Convert.ToInt32(args.Split(' ')[1]);
						}
						catch 
							{ 
							if (args.ToLower() == "day") { args = "0"; goto retry2; }
							else if (args.ToLower() == "night") { args = "12000"; goto retry2; }
							else if (args.ToLower() == "noon") { args = "6000"; goto retry2; }
							else if (args.ToLower() == "midnight") { args = "18000"; goto retry2; }
							else { p.SendMessage("Invalid Time!"); return; }
						}
					}
				}
			}
		}
		public override void Help (Player p)
		{
			p.SendMessage("/settime - Set Time!");
			p.SendMessage("/settime <level> <time> - Set <level> time!");
			p.SendMessage("/settime global <time> - Set all level time!");
		}
	}
}

