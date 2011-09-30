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

/*TODO
 * inheritance semi-done
 * check permissions still has some bugs
 * grouputils
 * Commands
*/

namespace SMP
{
    public class Group
    {
        public static List<Group> GroupList = new List<Group>();
        public static Group DefaultGroup;
		public static Dictionary<string, List<Group>> TracksDictionary = new Dictionary<string, List<Group>>(); //holds the all the tracks
		public List<string> Tracks = new List<string>(); //holds whatever track(s) it is a part of, used to reference Dictionary id
        public string Name;
		public int PermLevel = 0;  //temporary system
        public bool IsDefaultGroup = false;
        public bool CanBuild = false;
        public string Prefix = "";
        public string Suffix = "";
        public string GroupColor = Color.Gray;
        public List<string> PermissionList = new List<string>();
		public List<string> InheritedPermissionList = new List<string>();
        public List<Group> InheritanceList = new List<Group>();
        private List<string> tempInheritanceList = new List<string>();

		/// <summary>
		/// Saves the basics of the group to db. Returns the id of the group.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int Save()
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			
			data.Add("Name", this.Name);
			data.Add("PermLevel", this.PermLevel.ToString());
			
			if (this.IsDefaultGroup)
				data.Add("IsDefault", "'1'");
			else
				data.Add("IsDefault", "'0'");
			
			if (this.CanBuild)
				data.Add("CanBuild", "'1'");
			else
				data.Add("CanBuild", "'0'");
			
			data.Add("Prefix", this.Prefix);
			data.Add("Suffix", this.Suffix);
			data.Add("Color", this.GroupColor);
			
			
			StringBuilder sb = new StringBuilder("");
			foreach(String s in this.PermissionList)
			{
				string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
				if (String.IsNullOrEmpty(id))
				{
					Server.SQLiteDB.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES ('" + s + "');");	
					id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
				}
				
				sb.Append(id + ",");
			}
			sb.Remove(sb.Length - 1, 1);
			data.Add("Permissions", sb.ToString());
			
			sb.Clear();
			
			Server.SQLiteDB.Insert("Groups", data);
			
			return int.Parse(Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Groups WHERE Name = '" + this.Name + "';").ToString());
		}
        /// <summary>
        /// Checks if a player has permission to use a command or whatever.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckPermission(Player p, String perm)
        {
			List<string> nodes = new List<string>();
			nodes.AddRange(GetParentNodes(perm));
			
			foreach(string node in nodes)
			{
				if(p.group.PermissionList.Contains("-" + node))
				   return false;
				else if(p.AdditionalPermissions.Contains("-" + node))
				   return false;
				else if(p.AdditionalPermissions.Contains(node) || p.group.PermissionList.Contains(node))
				   return true;
				else if(p.group.InheritedPermissionList.Contains("-" + node))
				   return false;
				else if(p.group.InheritedPermissionList.Contains(node))
				   return true;
			}
			
			if(p.group.PermissionList.Contains("*") || p.AdditionalPermissions.Contains("*") || p.group.InheritedPermissionList.Contains("*"))
				return true;
			
			return false;
        }

        /// <summary>
        /// Finds a group by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Group FindGroup(string name)
        {
            foreach (Group g in GroupList)
            {
                if (g.Name.ToLower() == name.ToLower())
                    return g;
            }
            return null;
        }
		
		private static List<string> GetParentNodes(string perm)
		{
			string[] nodearray = perm.Split('.');
			List<string> nodeList = new List<string>();
			
			for(int i = 0; i < nodearray.Length; i++)
			{
				StringBuilder sb = new StringBuilder("");
				
				for(int ix = 0; ix <= i; ix ++)
				{
					sb.Append(nodearray[ix] + ".");
				}
				
				sb.Remove(sb.Length - 1, 1);
				nodeList.Add(sb.ToString());
				sb.Append(".*");
				nodeList.Add(sb.ToString());
			}
			
			nodeList.Reverse();
			return nodeList;
			
		}
		
		#region LOADING/SAVING
		public static void LoadGroups()
		{
			System.Data.DataTable dt = new System.Data.DataTable();
			
			try
			{
				dt = Server.SQLiteDB.GetDataTable("SELECT * FROM Groups;");
			}
			catch{Server.Log("Something went wrong loading groups");}
			
			for(int i = 0; i < dt.Rows.Count; i++)
			{
				Group g = new Group();
				
				g.Name = dt.Rows[i]["Name"].ToString();
				
				g.PermLevel = int.Parse(dt.Rows[i]["PermLevel"].ToString());
				
				if (dt.Rows[i]["IsDefault"].ToString() == "1")
				{
					g.IsDefaultGroup = true;
					Group.DefaultGroup = g;
				}
				
				if (dt.Rows[i]["CanBuild"].ToString() == "1")
					g.CanBuild = true;
				
				g.Prefix = dt.Rows[i]["Prefix"].ToString();
				g.Suffix = dt.Rows[i]["Suffix"].ToString();
				
				g.GroupColor = dt.Rows[i]["Color"].ToString();
				
				if (g.GroupColor.Length == 2 && g.GroupColor[0] == '%' || g.GroupColor[0] == '§' || g.GroupColor[0] == '&')
					if (Color.IsColorValid((char)g.GroupColor[1]))
					    g.GroupColor = "§" + g.GroupColor[1];
				else if (g.GroupColor.Length == 1 && Color.IsColorValid((char)g.GroupColor[0]))
				 	g.GroupColor = "§" + g.GroupColor[1];
				
				string[] perms = dt.Rows[i]["Permissions"].ToString().Replace(" ", "").Split(',');
				foreach(string s in perms)
				{
					string perm;
					if (s[0] == '-')
						perm = "-" + Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s.Substring(1) + "';");
					else
						perm = Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s + "';");
					
					if (perm.Substring(0,1) == "-" && !g.PermissionList.Contains(perm.Substring(1)))
						g.PermissionList.Add(perm);
					else if (perm.Substring(0,1) != "-" && !g.PermissionList.Contains("-" + perm))
						g.PermissionList.Add(perm);
				}
				
				string temp = dt.Rows[i]["Inheritance"].ToString().Replace(" ", "");
				string[] inheritance = temp.Split(',');
				if (inheritance.Length >= 1)
				{
					foreach(string s in inheritance)
					{
						if (!String.IsNullOrEmpty(s))
							g.tempInheritanceList.Add(Server.SQLiteDB.ExecuteScalar("SELECT Name FROM Groups WHERE ID = '" + s + "';"));	
					}
				}
				
				Group.GroupList.Add(g);
			}
			
			foreach(Group g in Group.GroupList)
			{
				foreach(string s in g.tempInheritanceList)
				{
					Group gr = Group.FindGroup(s);
					if (gr != null)
					{
						GroupUtils.AddGroupInheritance(g, gr);
					}
				}
			}
			
			LoadTracks();
		}
		
		public static void SaveGroups()
		{
			foreach(Group g in GroupList)
			{
				Dictionary<string, string> data = new Dictionary<string, string>();
				
				data.Add("Name", g.Name);
				data.Add("PermLevel", g.PermLevel.ToString());
				
				if (g.IsDefaultGroup)
					data.Add("IsDefault", "1");
				else
					data.Add("IsDefault", "0");
				
				if (g.CanBuild)
					data.Add("CanBuild", "1");
				else
					data.Add("CanBuild", "0");
				
				data.Add("Prefix", g.Prefix);
				data.Add("Suffix", g.Suffix);
				data.Add("Color", g.GroupColor);
				
				
				StringBuilder sb = new StringBuilder("");
				foreach(String s in g.PermissionList)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
					if (String.IsNullOrEmpty(id))
					{
						Server.SQLiteDB.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES ('" + s + "');");	
						id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
					}
					
					sb.Append(id + ",");
				}
				
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("Permissions", sb.ToString());				
				sb.Clear();
				
				foreach(Group gr in g.InheritanceList)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Groups WHERE Name = '" + gr.Name + "';");
					if(String.IsNullOrEmpty(id))
					{
						id = gr.Save().ToString();
					}
					
					sb.Append(id + ",");
				}
				
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("Inheritance", sb.ToString());
				sb.Clear();
				
				//TODO SAVE TRACKS
				foreach(string s in g.Tracks)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Track WHERE Name = '" + s + "';");
					if(String.IsNullOrEmpty(id))
					{
						Server.SQLiteDB.ExecuteNonQuery("INSERT INTO Track(Name) VALUES ('" + s + "');");
						id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Track WHERE Name = '" + s + "';");
					}
					sb.Append(id + ",");
				}
				
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("Tracks", sb.ToString());
				sb.Clear();
				
				Server.SQLiteDB.Update("Groups", data, "Name = '" + g.Name + "';");
			}
			
			SaveTracks();
		}
		
		private static void LoadTracks()
		{
			System.Data.DataTable tracksdt = new System.Data.DataTable();
			try
			{
				tracksdt = Server.SQLiteDB.GetDataTable("SELECT * FROM Track;");
			}
			catch{Server.Log("Something went wrong loading tracks");}
						
			for (int i = 0; i < tracksdt.Rows.Count; i++)
			{
				string name = tracksdt.Rows[i]["Name"].ToString();
				string[] groups = tracksdt.Rows[i]["Groups"].ToString().Replace(" ", "").Split(',');
				List<Group> grouplist = new List<Group>();
				
				foreach(string s in groups)
				{
					Group gr = Group.FindGroup(Server.SQLiteDB.ExecuteScalar("SELECT Name FROM Groups WHERE ID = '" + s + "';"));
					
					if (gr != null)
					{
						grouplist.Add(gr);
						gr.Tracks.Add(name);
					}
				}
				if (grouplist.Count > 0)
					TracksDictionary.Add(name, grouplist);				
			}	
		}
		
		private static void SaveTracks()
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			
			foreach(KeyValuePair<string, List<Group>> kvp in TracksDictionary)
			{
				StringBuilder sb = new StringBuilder("");
				data.Add("Name", kvp.Key);
				
				foreach(Group g in kvp.Value)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Groups WHERE Name = '" + g.Name + "';");
					if(String.IsNullOrEmpty(id))
					{
						id = g.Save().ToString();
					}
					
					sb.Append(id + ",");
				}
				
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("Groups", sb.ToString());
				sb.Clear();
				Server.SQLiteDB.Update("Track", data, "Name = '" + kvp.Key + "';");
			}
		}
		#endregion
    }
}
