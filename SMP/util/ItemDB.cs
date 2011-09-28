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
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace SMP
{
	public class ItemDB
	{
		private Dictionary<string, short> Items = new Dictionary<string, short>();
        private Dictionary<string, short> Durability = new Dictionary<string, short>();

		public ItemDB ()
		{
			DataTable dt = new DataTable();
			
			dt = Server.SQLiteDB.GetDataTable("SELECT Value, Meta, Alias FROM Item");
			
			for(int i = 0; i < dt.Rows.Count; i++)
			{
				string alias = "";
				short id;
				short meta;
				
				alias = dt.Rows[i]["Alias"].ToString();
				try
				{
					id = Convert.ToInt16(dt.Rows[i]["Value"]);
					meta = Convert.ToInt16(dt.Rows[i]["Meta"]);
				}
				catch
				{
					continue;	
				}
				
				Server.Log(alias + ": " + id + ": " + meta);
				try{
				Items.Add(alias, id);
				Durability.Add(alias, meta);
				}
				catch (ArgumentException)
				{
					Server.Log(alias + "already exsists in the item table, please rename " + alias + ".");
				}
				
			}
		}
		
		public short[] FindItem(string name)
		{
		short id;
		if (!Items.TryGetValue(name.ToLower(), out id))
		return null;
		
		short durability;
		if (!Items.TryGetValue(name.ToLower(), out durability))
		return null;
		
		return new short[] {id, durability};
		}
	}
}