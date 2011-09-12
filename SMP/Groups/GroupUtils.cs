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

/* Notes to self (Keith)
 * load groups, load group tracks
 * try to promote along a track  //done??
 * if no track is found than try to find a group with inheritance //done??
 * if multiple groups inherit or no inheritance is found then throw a error  //done
 * methods (groups) to add/remove inheritance add/remove permissions, change attributes
 * methods (players) change color, canbuild, suffix/prefix, etc, etc, etc
 */

/*utility class for groups
 * mainly just group handling
 * and all the stuff
 */
namespace SMP
{
	public static class GroupUtils
	{
		#region group methods
		public static bool AddGroupInheritance(Group g, Group addg)
		{
			if (!g.InheritanceList.Contains(addg))
			{
				g.InheritanceList.Add(addg);  //TODO: add inherited permissions
				//UpdateGroupPermissions(g);
				return true;
			}
			return false;
		}
		
		public static bool DelGroupInheritance(Group g, Group delg)
		{
			if (!g.InheritanceList.Contains(delg))
			{
				g.InheritanceList.Remove(delg);
				//UpdateGroupPermissions(g);
				return true;
			}				
			return false;
				
		}
		
		public static bool AddGroup(string name, bool isDefaultGroup, bool canBuild, string prefix, string suffix, char color)
		{
			Group g = new Group();
			
			if(name == "" || name == null)
				return false;
			
			g.Name = name;
			
			if(isDefaultGroup)
				Group.DefaultGroup = g;
			
			g.CanBuild = canBuild;
			
			if(prefix == null)
				prefix = "";
			
			g.Prefix = prefix;
			
			if(suffix == null)
				suffix = "";
			
			g.Suffix = suffix;
			
			if(!Color.IsColorValid(color))
				return false;
			
			g.GroupColor = color.ToString();
			Group tempG = Group.FindGroup(g.Name);
			if (tempG != null)
				return false;
			
			Group.GroupList.Add(g);
			return true;
				
		}
		
		public static bool DelGroup(Group g)
		{
			foreach(Player p in Player.players)
			{
				if (p.group == g)
				{
					return false;
				}
			}
			
			if (Group.GroupList.Contains(g))
			{
				Group.GroupList.Remove(g);
				return true;
			}
			return false;
		}
		
		public static bool AddGroupPermission(Group g, string perm)
		{
			if (!g.PermissionList.Contains(perm))
			{
				g.PermissionList.Add(perm);
				return true;
			}
			return false;
		}
		
		public static bool DelGroupPermission(Group g, string perm)
		{
			if (g.PermissionList.Contains(perm))
			{
				g.PermissionList.Remove(perm);
				return true;
			}
			return false;
		}
		
		public static void ChangeCanBuild(Group g, bool cb)
		{
			g.CanBuild = cb;
		}
		
		public static bool ChangeGroupColor(Group g, char color)
		{
			if (Color.IsColorValid(color))
			{
				g.GroupColor = "§" + color;
				return true;
			}
			return false;		
		}
		
		public static void ChangeGroupPrefix(Group g, string prefix)
		{
			g.Prefix = prefix;		
		}
		
		public static void ChangeGroupSuffix(Group g, string suffix)
		{
			g.Suffix = suffix;
		}
		#endregion
		
		#region Tracks
		//TODO
		#endregion
		
		
		#region player methods
		public static bool AddSubGroup(Player p, Group g)
		{
			if(!p.SubGroups.Contains(g))
			{
				p.SubGroups.Add(g);
				return true;
			}
			else
			{
				return false;
			}
				
		}
		
		public static bool RemoveSubGroup(Player p, Group g)
		{
			if(p.SubGroups.Contains(g))
				{
					p.SubGroups.Remove(g);
					return true;
				}
			else 
				return false;
		}
		
		public static bool SetRank(Player p, Group g)
		{
			if(Group.GroupList.Contains(g))
			{
				p.group = g;
				return true;
			}
			else 
				return false;
		}
		
		/// <summary>
		/// Tries to promote a player along a defined track, if not tries to promote based on inheritance.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Player"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool PromotePlayer(Player p)
		{
			for (int i = 0; i < p.group.Tracks.Count; i++)
			{
				if(Group.TracksDictionary.ContainsKey(p.group.Tracks[i]))
				{
					List<Group> tempList;
					Group.TracksDictionary.TryGetValue(p.group.Tracks[i], out tempList);
					
					if(tempList.Count >= 1)
					{
						for(int ind = 0; i < tempList.Count; i++)
						{
							if(p.group == tempList[ind])
							{
								if(ind + 1 > tempList.Count)
								{
									p.group = tempList[ind + 1];
									return true;
								}
							}
						}
					}
					
				}
			}
			
			//maybe add checks to make sure there isn't multiple inheritance
			foreach(Group g in Group.GroupList)
			{
				if(g.InheritanceList.Contains(p.group))
				{
					p.group = g;
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Tries to demote a player based on track, if not, and inheritance has only one entry uses it.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Player"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool DemotePlayer(Player p)
		{
			for (int i = 0; i < p.group.Tracks.Count; i++)
			{
				if(Group.TracksDictionary.ContainsKey(p.group.Tracks[i]))
				{
					List<Group> tempList;
					Group.TracksDictionary.TryGetValue(p.group.Tracks[i], out tempList);
					
					if(tempList.Count >= 1)
					{
						for(int ind = 0; i < tempList.Count; i++)
						{
							if(p.group == tempList[ind])
							{
								if(ind > 0)
								{
									p.group = tempList[ind - 1];
									return true;
								}
							}
						}
					}
					
				}
			}
			
			if(p.group.InheritanceList.Count == 1)
			{
				p.group = p.group.InheritanceList[0];
				return true;
			}
			return false;
		}
		
		public static bool AddPlayerPermission(Player p, string perm)
		{
			if(!p.AdditionalPermissions.Contains(perm))
			{
				p.AdditionalPermissions.Add(perm);
				return true;	
			}
			return false;
		}
		
		public static bool DelPlayerPermission(Player p, string perm)
		{
			if(p.AdditionalPermissions.Contains(perm))
			{
				p.AdditionalPermissions.Remove(perm);
				return true;	
			}
			return false;
			   
		}
		
		public static bool ChangePlayerColor(Player p, char color)
		{
			if (Color.IsColorValid(color))
			{
				p.color = "§" + color;
				return true;
			}
			return false;
				
		}
		
		public static void ChangePlayerCanBuild(Player p, bool cb)
		{
			p.CanBuild = cb;	
		}
		
		public static void ChangePlayerPrefix(Player p, string prefix)
		{
			p.Prefix = prefix;		
		}
		
		public static void ChangePlayerSuffix(Player p, string suffix)
		{
			p.Suffix = suffix;	
		}
		#endregion
		
		#region Misc
		
		public static void UpdateGroupPermissions(Group g)
		{
			//TODO
			
		}
		
		
		public static void UpdatePlayerPermissions(Player p)
		{
			//TODO
		}
		#endregion
	}
}

