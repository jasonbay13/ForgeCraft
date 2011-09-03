using System;
using System.IO;
using System.Collections.Generic;

namespace SMP
{
	public class ItemDB
	{
		private Dictionary<string, short> Items = new Dictionary<string, short>();
        private Dictionary<string, short> Durability = new Dictionary<string, short>();
		
		public ItemDB (string file)
		{
			if (!File.Exists(file))
			{
				File.Create(file);
                return;
			}

            List<string> itemlist = new List<string>(Properties.LoadList(file));
			
			foreach(string s in itemlist)
			{
				//Server.Log(s);
				
				string [] parts = s.Split(' ');
				
				short numeric;
                if (!short.TryParse(parts[1].Replace(",", ""), out numeric))
                    continue;

                short durability;
                if (parts.Length < 3 || !short.TryParse(parts[2].Replace(",", ""), out durability))
                    durability = 0;

                string item = parts[0].Replace(",", "").ToLower();
                Items.Add(item, numeric);
                Durability.Add(item, durability);
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

