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
					Server.Log("SHIT!!");
					continue;	
				}
				
				Items.Add(alias, id);
				Durability.Add(alias, meta);
				
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