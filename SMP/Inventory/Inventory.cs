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
	public class Inventory
	{
		public Item[] items;
		public Player p;
		public Item current_item;
		public int current_index;

		bool ActiveWindow;
		Windows window; //The type of window that is currently open

		public Inventory (Player pl)
		{
			p = pl;

			items = new Item[45];
			
			for (int i = 0; i < items.Length; i++)
				items[i] = Item.Nothing;

			current_item = items[36];
			current_index = 36;
		}

		public void Add(short item, int slot)
		{
			Add(item, 1, 0, slot);
		}
		public void Add(Item item)
		{
			Add(item.item, item.count, item.meta);
		}
		public void Add(short item, byte count, short meta)
		{
			//Console.WriteLine("add1");
			if (ActiveWindow)
			{
				//TODO pass action to the window
				//SUDO window.Add(item, count, meta, slot);
				return;
			}
			byte stackable = isStackable(item);
			byte c = count;
			//Console.WriteLine("add2");
			for (int i = 36; i < 45; i++)
			{
				if (c == 0) return;
				if (items[i].item == item)
					if (items[i].count < stackable)
					{
						items[i].count += c;
						c = 0;
						if (items[i].count > stackable)
						{
							c = (byte)(items[i].count - stackable);
							items[i].count -= c;
						}
						p.SendItem((short)i, item, items[i].count, meta);
					}
			}
			//Console.WriteLine("add3");
			for (int i = 9; i <= 35; i++)
			{
				if (c == 0) return;
				if (items[i].item == item)
					if (items[i].count < stackable)
					{
						items[i].count += c;
						c = 0;
						if (items[i].count > stackable)
						{
							c = (byte)(items[i].count - stackable);
							items[i].count -= c;
						}
						p.SendItem((short)i, item, items[i].count, meta);
					}
			}
			//Console.WriteLine("add4");
			Add(item, c, meta, FindEmptySlot());
		}
		public void Add(short item, byte count, short meta, int slot)
		{
			Console.WriteLine("d1");
			if (count == 0) return;
			if (ActiveWindow)
			{
				//TODO pass action to the window
				//SUDO window.Add(item, count, meta, slot);
				return;
			}

			Item I = new Item(item, count, meta, p.level);
			if (slot > 44 || slot < 0) return;
			items[slot] = I;

			p.SendItem((short)slot, item, count, meta);
		}

		public void Remove(int slot)
		{
			items[slot] = Item.Nothing;
		}
		public void Remove(int slot, byte count)
		{
			if (count >= items[slot].count)
			{
				items[slot] = Item.Nothing;
				p.SendItem((short)slot, -1, 0, 0);
				return;
			}

			items[slot].count--;
			p.SendItem((short)slot, items[slot].item, items[slot].count, items[slot].meta);
		}

		public int Right_Click(int slot)
		{
			try
			{
				int temp;
				if (items.Length % 2 == 0)
				{
					temp = items.Length / 2;
					items[slot].count = (byte)(items[slot].count / 2);
				}
				else
				{
					temp = items.Length / 2;
					items[slot].count = (byte)(items[slot].count - temp);
				}
				return temp;
			}
			catch
			{
				return 0;
			}
		}
		
		public int FindEmptySlot()
		{			
			for (int i = 36; i < 45; i++)
				if (items[i].item == (short)Items.Nothing)
				{
					return i;
				}
			
			for (int i = 9; i <= 35; i++)
				if (items[i].item == (short)Items.Nothing)
				{
					return i;
				}
			
			return -1;
		}
		public byte isStackable(short id)
		{
			if(id >= 1 && id <= 96) //all blocks are stackable and there is no missing id's
				return 64;
			
			// may be missing a few
			List<short> stackable64 = new List<short> {
				262, 263, 264, 265, 266, 280, 281, 287,	288, 
				289, 295, 296, 318, 321, 331, 334, 336, 337,
				338, 339, 340, 341, 348, 351, 352, 353, 356
				
			};
			
			if(stackable64.Contains(id))
			{
				return 64;	
			}
			
			List<short> stackable16 = new List<short> {
				332, 344 
			};
			
			if( stackable16.Contains(id))
			{
				return 16;	
			}
			
			if(id == 357) //cookies
			{
				return 8;	
			}
			return 1;
		}
	}
}

