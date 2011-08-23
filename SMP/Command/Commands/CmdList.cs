using System;
using System.Text;
using System.Collections.Generic;

//TODO: group players by rank

namespace SMP
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
