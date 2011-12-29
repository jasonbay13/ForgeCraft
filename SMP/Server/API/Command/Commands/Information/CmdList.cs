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
using System.Text;
using System.Collections.Generic;
using SMP.API;

//TODO: group players by rank

namespace SMP.Commands
{
    public class CmdList : Command
    {
        public override string Name { get { return "list"; } }
        public override List<string> Shortcuts { get { return new List<string> { "players", "who", "online" }; } }
        public override string Category { get { return "information"; } }
        public override string Description { get { return "Shows who is online."; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string PermissionNode { get { return "core.info.list"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "help")
            {
                Help(p);
                return;
            }
			
			if (Player.players.Count == 0)
			{
				p.SendMessage("Nobody is mincrafting right now. :(");
				return;
			}
			
			p.SendMessage(Color.Yellow + "There is currently " + Color.DarkRed + Player.players.Count + Color.Yellow + "/" + Color.DarkRed + Server.MaxPlayers + Color.Yellow + " players on line.");
			
			if (args.Length == 0)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < Player.players.Count; i++)
				{
					if(!Player.players[i].AFK)
					{
						sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.White);
						//sb.Append(Player.players[i].username);
					}
					else
					{
						sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.Gray + "[AFK]" + Color.White);
						//sb.Append(Player.players[i].username + Color.Gray + "[AFK]");
					}
					
					if (i != Player.players.Count - 1)
                		sb.Append(", ");
				}
				p.SendMessage(sb.ToString(), WrapMethod.Chat);
			}
			else
			{
				if (args[0].ToLower() == "world")
				{
					foreach(World w in World.worlds)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(w.name + ": ");
						for (int i = 0; i < Player.players.Count; i++)
						{
							if (Player.players[i].level == w)
							{
								if(!Player.players[i].AFK)
								{
									sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.White);
									//sb.Append(Player.players[i].username);
								}
								else
								{
									sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.Gray + "[AFK]" + Color.White);
									//sb.Append(Player.players[i].username + Color.Gray + "[AFK]");
								}
								
								if (i != Player.players.Count - 1)
	                        		sb.Append(", ");
							}
						}
						
						p.SendMessage(sb.ToString(), WrapMethod.Chat);
						
					}
				}
				
				if (args[0].ToLower() == "group")
				{
					foreach(Group g in Group.GroupList)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append(g.GroupColor + g.Name + Color.ServerDefaultColor + ": ");
						for(int i = 0; i < Player.players.Count; i++)
						{
							if (Player.players[i].group == g)
							{
								if (!Player.players[i].AFK)
								{
									sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.White);
								}
								else
								{
									sb.Append(Player.players[i].GetColor() + Player.players[i].GetName() + Color.White);	
								}
								
								if (i != Player.players.Count -1)
									sb.Append(", ");
							}
						}
						
						p.SendMessage(sb.ToString(), WrapMethod.Chat);
						
					}
					
				}
			}
			
        }

        public override void Help(Player p)
        {
            p.SendMessage("/list - Displays a list of who is online");
        }
    }
}
