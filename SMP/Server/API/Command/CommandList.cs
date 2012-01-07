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
using System.Linq;
using System.Text;
using SMP.API;

namespace SMP.API.Commands
{
    public sealed class CommandList
    {
        public List<Command> commands = new List<Command>();
        public CommandList() { }
        public void Add(Command cmd) { commands.Add(cmd); }
        public void AddRange(List<Command> listCommands)
        {
            listCommands.ForEach(delegate(Command cmd) { commands.Add(cmd); });
        }
        public List<string> commandNames()
        {
            List<string> tempList = new List<string>();

            commands.ForEach(delegate(Command cmd)
            {
                tempList.Add(cmd.Name);
            });
           
            return tempList;
        }

        public bool Remove(Command cmd) { return commands.Remove(cmd); }
        public bool Contains(Command cmd) { return commands.Contains(cmd); }
		
		//add shortcut support
        public bool Contains(string name)
        {
            name = name.ToLower(); foreach (Command cmd in commands)
            {
                if (cmd.Name == name.ToLower()) { return true; }
            } return false;
        }

		/// <summary>
		/// Finds command, even shortcuts
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Command"/>
		/// </returns> 
        public Command Find(string name)
        {
            name = name.ToLower(); 

            foreach (Command cmd in commands)
            {
                if (cmd.Name == name)
                {
                    return cmd;
                }
                else
                {
                    foreach (string shortcut in cmd.Shortcuts)
                    {
                        if (shortcut.ToLower() == name)
                            return cmd;
                    }
                }
            } 
            
            return null;
        }

        /// <summary>
        /// Finds command based on shortcut value use Find() where at all possible.
        /// </summary>
        /// <param name="shortcut">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string FindShort(string shortcut)
        {
            if (shortcut == "") return "";

            shortcut = shortcut.ToLower();
            foreach (Command cmd in commands)
            {
                foreach (string shortc in cmd.Shortcuts)
                {
                    if (shortc.ToLower() == shortcut)
                        return cmd.Name;
                }
            }
            return "";
        }

        public List<Command> All() { return new List<Command>(commands); }
    }
}


