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

namespace SMP
{
    public abstract class Command
    {
        /// <summary>
        /// Declarations
        /// Note the variance between this and Mclawl's command structure to save yourself some headaches.
        /// </summary>
        public abstract string Name { get; }
        public abstract List<String> Shortcuts { get; }
        public abstract string Category { get; }
        public abstract bool ConsoleUseable { get; }
        public abstract string Description { get; } //used for displaying what the commands does when using /help
		public abstract string PermissionNode { get; }
        public abstract void Use(Player p, params string[] args);
        public abstract void Help(Player p);
        public static CommandList all = new CommandList();
        public static CommandList core = new CommandList();
        public static List<List<string>> CommandCategories = new List<List<string>>();
        public static string HelpBot = Color.Purple + "HelpBot V12: "; //we can totally change it, or keep it for lawls. hint, hint

		/// <summary>
		/// Initializes core commands
		/// </summary>
        public static void InitCore()
        {
			#region please put in alphabetical order and use core.add now not all.add
            core.Add(new CmdAFK());
            core.Add(new CmdCheckPort());
            core.Add(new CmdClearInventory());
            core.Add(new CmdCuboid());
			core.Add(new CmdBan());
            core.Add(new CmdDevs());
			core.Add(new CmdDND());
			core.Add(new CmdFire());
            core.Add(new CmdFly());
            core.Add(new CmdGameMode());
            core.Add(new CmdGive());
			core.Add(new CmdGod());
			core.Add(new CmdGotoLVL());
            core.Add(new CmdHelp());
			core.Add(new CmdHackz());
            core.Add(new CmdKick());
			core.Add(new CmdKill());  
            core.Add(new CmdList());
            core.Add(new CmdLoadLVL());
            //core.Add(new CmdMBan());
            core.Add(new CmdMe());
			core.Add(new CmdMotd());
			core.Add(new CmdMsg());
			core.Add(new CmdNewLVL());
			core.Add(new CmdPromote());
            core.Add(new CmdRain());
            core.Add(new CmdSay());
			core.Add(new CmdSetRank());
			core.Add(new CmdSpawn());
            core.Add(new CmdStrike());
			core.Add(new CmdTeleport());
            core.Add(new CmdSaveLVL());
			core.Add(new CmdSetTime());
            core.Add(new CmdUnban());
            core.Add(new CmdUnloadLVL());
			core.Add(new CmdViewDistance());
			core.Add(new CmdVIPList());
            core.Add(new CmdWhiteList());
            all.commands = new List<Command>(core.commands);
            InitCommandTypes();
			#endregion
		}

		/// <summary>
		/// Init core types
		/// </summary>
        public static void InitCommandTypes()
        {
            CommandCategories.Add(new List<string> { "Build", " for Building Commands" });
            CommandCategories.Add(new List<string> { "Mod", " for Moderation Commands" });
            CommandCategories.Add(new List<string> { "Information", " for Informative Commands" });
			CommandCategories.Add(new List<string> { "General", " for General Commands"});
			CommandCategories.Add(new List<string> { "Cheats", " Commands for Wusses" });
            CommandCategories.Add(new List<string> { "Core", " for Non-plugin Commands Commands" });
            CommandCategories.Add(new List<string> { "Short", " for Command Shortcuts" });
            CommandCategories.Add(new List<string> { "Other", " for Uncategorized Commands" });

            //TODO: add types when plugins are added
                //might just have it done in the plugin initialization
        }
    }
}
