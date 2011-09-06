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
    public class Group
    {
        public static List<Group> GroupList = new List<Group>();
        public static Group DefaultGroup;
		public static Dictionary<string, List<Group>> TracksDictionary = new Dictionary<string, List<Group>>(); //holds the all the tracks
		public List<string> Tracks = new List<string>(); //holds whatever track(s) it is a part of, used to reference Dictionary id
        public string Name;
        public bool IsDefaultGroup = false;
        public bool CanBuild = false;
        public string Prefix = "";
        public string Suffix = "";
        public string GroupColor = Color.Gray;
        public List<string> PermissionList = new List<string>();
        public List<Group> InheritanceList = new List<Group>();
        public List<string> tempInheritanceList = new List<string>();

        /// <summary>
        /// Checks if a player has permission to use a command
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckPermission(Player p, String perm)
        {
            if (p.AdditionalPermissions.Contains(perm))
            {
                return true;
            }
            else if (p.group.PermissionList.Contains(perm))
            {
                return true;
            }
            else
            {
                return false;
            }
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
				
				//Server.Log("damn");
				string[] perms = dt.Rows[i]["Permissions"].ToString().Replace(" ", "").Split(',');
				foreach(string s in perms)
				{
					//Server.Log("??");
					g.PermissionList.Add(Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s + "';"));
				}
				
				/*Server.Log("balls");
				string[] inheritance = dt.Rows[i]["Inheritance"].ToString().Split(',');
				if (inheritance.Length >= 1)
				foreach(string s in inheritance)
				{
					Server.Log("??: " + s);
					if (s != "" || s != null)
						g.tempInheritanceList.Add(Server.SQLiteDB.ExecuteScalar("SELECT Name FROM Group WHERE ID = '" + Convert.ToInt32(s) + "';"));	
				}*/
				
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
		}
		
		#endregion
    }
}
