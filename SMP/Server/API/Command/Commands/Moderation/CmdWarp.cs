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
using SMP.API.Commands;
using SMP.PLAYER;

//TODO Maybe add rotation and yaw
namespace SMP.Commands
{
	public class CmdWarp : Command
	{
		public override string Name { get { return "warp"; } }
        public override List<string> Shortcuts { get { return new List<string>(0); } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Warps you to a warp point."; } }
		public override string PermissionNode { get { return "core.general.warp"; } }

        public override void Use(Player p, params string[] args)
		{
			if (args[0].ToLower() == "help")
			{
				Help(p);
				return;
			}
			
			if (args.Length == 0 || args[0].ToLower() == "list")
			{
				System.Data.DataTable dt = Server.SQLiteDB.GetDataTable("SELECT Name FROM Warp;");
				
				StringBuilder sb = new StringBuilder();
				string name = "";
				bool allwarps = Group.CheckPermission(p, PermissionNode);
				
				for(int i = 0; i < dt.Rows.Count; i++)
				{
					name = dt.Rows[i]["Name"].ToString();
					if(allwarps)
					{
						sb.Append(name + ", ");
						continue;
					}
					if(Group.CheckPermission(p, PermissionNode + "." + name))
					   sb.Append(name + ", ");
					
				}
				
				if (sb.Length > 2)
					sb.Remove(sb.Length - 2, 2);
				
				p.SendMessage(sb.ToString(), WrapMethod.Chat);
			}
			else if (args.Length == 1)
			{
				double x = 0;
				double y = 0;
				double z = 0;
				World w = World.Find(Server.SQLiteDB.ExecuteScalar("SELECT World FROM Warp WHERE Name = '" + args[0] + "';"));
				
				if(!double.TryParse(Server.SQLiteDB.ExecuteScalar("SELECT X FROM Warp WHERE Name = '" + args[0] + "';"), out x))
				{
					p.SendMessage(HelpBot + "There is a problem with that warp point.");
					return;
				}
				
				if(!double.TryParse(Server.SQLiteDB.ExecuteScalar("SELECT Y FROM Warp WHERE Name = '"  + args[0] + "';"), out y))
				{
					p.SendMessage(HelpBot + "There is a problem with that warp point.");
					return;	
				}
				
				if(!double.TryParse(Server.SQLiteDB.ExecuteScalar("SELECT Z FROM Warp WHERE Name = '" + args[0] + "';"), out z))
				{
					p.SendMessage(HelpBot + "There is a problem with that warp point.");
					return;	
				}
				
				if(w == null)
				{
					p.SendMessage(HelpBot + "World: " + Server.SQLiteDB.ExecuteScalar("SELECT World FROM Warp WHERE Name = '" + args[0] + "';") + " can not be found. Maybe it isn't loaded.");
					return;
				}
				
				if (Group.CheckPermission(p, PermissionNode + "." + args[0]))
				{
					//TODO CHANGE WORLDS
					p.Teleport_Player(x, y, z);
				}
			}
			else if (args.Length == 2 && args[0].ToLower() == "add" && Group.CheckPermission(p, PermissionNode + ".add"))
			{
				Server.SQLiteDB.ExecuteNonQuery(String.Format("INSERT INTO Warp(Name, X, Y ,Z, World) VALUES('{0}', {1}, {2}, {3}, '{4}');", args[1], p.pos.X, p.pos.Y, p.pos.Z, p.level.name));
				p.SendMessage(HelpBot + "Warp " + args[1] + " created.");
			}
			else if (args.Length == 2 && args[0].ToLower() == "del" && Group.CheckPermission(p, PermissionNode + ".del"))
			{
				Server.SQLiteDB.ExecuteNonQuery("DELETE FROM Warp WHERE Name = '" + args[1] + "';");
				p.SendMessage(HelpBot + "Warp " + args[1] + " deleted.");
			}
			
		}
		
		public override void Help(Player p)
		{
			p.SendMessage(Description);
			p.SendMessage("/warp (list:add:del) [Warpname]");
		}
	}
}

