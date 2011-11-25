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
		public Item current_item { get { return items[current_index]; } }
        private short Mycurrent_index;
        public short current_index
        {
            get { return Mycurrent_index; }
            set
            {
                Mycurrent_index = value;
                UpdateVisibleItemInHand(current_item.id, current_item.meta);
            }
        }

		public Inventory (Player pl)
		{
			p = pl;

			items = new Item[45];
			
			for (int i = 0; i < items.Length; i++)
				items[i] = Item.Nothing;

			current_index = 36;
		}

		public void Add(short item, int slot)
		{
			Add(item, 1, 0, slot);
		}
		public void Add(Item item)
		{
			Add(item.id, item.count, item.meta);
		}
		public void Add(short item, byte count, short meta)
		{
			//Console.WriteLine("add1");
			byte stackable = isStackable(item);
			byte c = count;
			//Console.WriteLine("add2");
			for (int i = 36; i < 45; i++)
			{
				if (c == 0) return;
				if (items[i].id == item)
				{
					if (item < 255)
					{
						if (items[i].meta != meta) continue;
					}
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
			}
			//Console.WriteLine("add3");
			for (int i = 9; i <= 35; i++)
			{
				if (c == 0) return;
				if (items[i].id == item)
				{
					if (item < 255)
					{
						if (items[i].meta != meta) continue;
					}
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
			}
			//Console.WriteLine("add4");
			Add(item, c, meta, FindEmptySlot());
		}
		public void Add(short item, byte count, short meta, int slot)
		{
            if (slot > 44 || slot < 0) return;
			if (count == 0) return;

			Item I = new Item(item, count, meta, p.level, true);
			items[slot] = I;
            //if (slot == current_index) p.current_block_holding = I;

			p.SendItem((short)slot, item, count, meta);
            if (slot == current_index) UpdateVisibleItemInHand(item, meta);
		}

		public void Remove(int slot)
		{
            Remove(slot, items[slot].count);
		}
		public void Remove(int slot, byte count)
		{
			if (count >= items[slot].count)
			{
				items[slot] = Item.Nothing;
				p.SendItem((short)slot, -1, 0, 0);
                if (slot == current_index) UpdateVisibleItemInHand(-1, 0);
				return;
			}

			items[slot].count -= count;
			p.SendItem((short)slot, items[slot].id, items[slot].count, items[slot].meta);
		}

        public void Clear()
        {
            Clear(false);
        }
        public void Clear(bool drop)
        {
            for (int i = 0; i < 45; i++)
            {
                //TODO: Spawn item drops (later fling them in random directions) when drop is true
                Remove(i);
            }
        }

		public void HandleClick(byte[] message)
		{
			byte id = message[0];
			short slot = util.EndianBitConverter.Big.ToInt16(message, 1);
			ClickType click = (ClickType)message[3];
			short ActionID = util.EndianBitConverter.Big.ToInt16(message, 4);
			bool Shift = (message[6] == 1);
			short ItemID = util.EndianBitConverter.Big.ToInt16(message, 7);
			byte Count = 1;
			short Meta = 0;
			if (ItemID != -1)
			{
				Count = message[9];
				Meta = util.EndianBitConverter.Big.ToInt16(message, 10);
			}

			ClickHandler(slot, click, ActionID, Shift, ItemID, Count, Meta);
		}
		public void ClickHandler(short slot, ClickType click, short ActionID, bool Shift, short ItemID, byte Count, short Meta)
		{
			if (slot == -999)
			{
				//TODO throw item
				p.OnMouse = Item.Nothing;
				return;
			}
            
			if (Shift)
			{
				if (slot >= 36)
				{
					for (int i = 9; i <= 35; i++)
					{
						Item item = items[i];
						if (item.id == items[slot].id)
						{
							if (item.id < 255)
							{
								if (item.meta != items[slot].meta)
								{
									continue;
								}
								else
								{
									byte stacking = isStackable(item.id);
									byte available = (byte)(stacking - item.count);
									if (available == 0) return;
									if (items[slot].count <= available)
									{
										item.count += items[slot].count;
										items[slot] = Item.Nothing;
										return;
									}
									else
									{
										item.count = stacking;
										items[slot].count -= available;
									}
								}
							}
							else
							{
								byte stacking = isStackable(item.id);
								byte available = (byte)(stacking - item.count);
								if (available == 0) return;
								if (items[slot].count <= available)
								{
									item.count += items[slot].count;
									items[slot] = Item.Nothing;
									return;
								}
								else
								{
									item.count = stacking;
									items[slot].count -= available;
								}
							}
						}
					}
					for (int i = 9; i <= 35; i++)
						if (items[i].id == (short)Items.Nothing)
						{
							items[i] = items[slot];
							items[slot] = Item.Nothing;
						}
					return;
				}
				if (slot < 36)
				{
					for (int i = 36; i < 45; i++)
					{
						Item item = items[i];
						if (item.id == items[slot].id)
						{
							if (item.id < 255)
							{
								if (item.meta != items[slot].meta) continue;
								else
								{
									byte stacking = isStackable(item.id);
									byte available = (byte)(stacking - item.count);
									if (available == 0) return;
									if (items[slot].count <= available)
									{
										item.count += items[slot].count;
										items[slot] = Item.Nothing;
										return;
									}
									else
									{
										item.count = stacking;
										items[slot].count -= available;
									}
								}
							}
							else
							{
								byte stacking = isStackable(item.id);
								byte available = (byte)(stacking - item.count);
								if (available == 0) return;
								if (items[slot].count <= available)
								{
									item.count += items[slot].count;
									items[slot] = Item.Nothing;
									return;
								}
								else
								{
									item.count = stacking;
									items[slot].count -= available;
								}
							}
						}
					}
					for (int i = 36; i < 45; i++)
						if (items[i].id == (short)Items.Nothing)
						{
							items[i] = items[slot];
							items[slot] = Item.Nothing;
						}
					return;
				}
				return;
			}

			if (p.OnMouse != Item.Nothing)
			{
				if (items[slot] != Item.Nothing)
				{
					#region Crafting Slot Done
					if (slot == 0)
					{
						if (p.OnMouse.id == items[slot].id)
						{
							if (p.OnMouse.id < 255)
							{
								if (p.OnMouse.meta == items[slot].meta)
								{
									byte stacking = isStackable(p.OnMouse.id);
									byte availible = (byte)(stacking - p.OnMouse.count);
									if (items[slot].count <= availible)
									{
										p.OnMouse.count += items[slot].count;
									}
								}
								else
								{
									Item temp = items[slot];
									items[slot] = p.OnMouse;
									p.OnMouse = temp;
								}
							}
							else
							{
								byte stacking = isStackable(p.OnMouse.id);
								byte availible = (byte)(stacking - p.OnMouse.count);
								if (items[slot].count <= availible)
								{
									p.OnMouse.count += items[slot].count;
								}
							}
						}
						else
						{
							return;
						}
					}
					#endregion
					#region Armor Slots Done
					else if (slot == 5 || slot == 6 || slot == 7 || slot == 8)
					{
						if (items[slot].id == p.OnMouse.id) return;
						switch (slot)
						{
							case 5:
								if (p.OnMouse.id == 298 || p.OnMouse.id == 302 || p.OnMouse.id == 306 || p.OnMouse.id == 310)
								{
									Item temp = items[slot];
									items[slot] = p.OnMouse;
									p.OnMouse = temp;
								}
								break;
							case 6:
								if (p.OnMouse.id == 299 || p.OnMouse.id == 303 || p.OnMouse.id == 307 || p.OnMouse.id == 311)
								{
									Item temp = items[slot];
									items[slot] = p.OnMouse;
									p.OnMouse = temp;
								}
								break;
							case 7:
								if (p.OnMouse.id == 300 || p.OnMouse.id == 304 || p.OnMouse.id == 308 || p.OnMouse.id == 312)
								{
									Item temp = items[slot];
									items[slot] = p.OnMouse;
									p.OnMouse = temp;
								}
								break;
							case 8:
								if (p.OnMouse.id == 301 || p.OnMouse.id == 305 || p.OnMouse.id == 309 || p.OnMouse.id == 313)
								{
									Item temp = items[slot];
									items[slot] = p.OnMouse;
									p.OnMouse = temp;
								}
								break;
						}
					}
					#endregion
					else
					{
						if (click == ClickType.RightClick)
						{
							if (p.OnMouse.id == items[slot].id)
							{
								if (p.OnMouse.id < 255)
								{
									if (p.OnMouse.meta == items[slot].meta)
									{
										byte stacking = isStackable(p.OnMouse.id);
										if (items[slot].count < stacking)
										{
											items[slot].count += 1;
											if (p.OnMouse.count == 1)
											{
												p.OnMouse = Item.Nothing;
											}
											else
											{
												p.OnMouse.count -= 1;
											}
										}
									}
									else
									{
										Item temp = items[slot];
										items[slot] = p.OnMouse;
										p.OnMouse = temp;
									}
								}
								else
								{
									byte stacking = isStackable(p.OnMouse.id);
									if (items[slot].count < stacking)
									{
										items[slot].count += 1;
										if (p.OnMouse.count == 1)
										{
											p.OnMouse = Item.Nothing;
										}
										else
										{
											p.OnMouse.count -= 1;
										}
									}
								}
							}
						}
						else
						{
							if (p.OnMouse.id == items[slot].id)
							{
								if (p.OnMouse.id < 255)
								{
									if (p.OnMouse.meta == items[slot].meta)
									{
										byte stacking = isStackable(p.OnMouse.id);
										byte available = (byte)(stacking - items[slot].count);
										if (available == 0) return;
										if (p.OnMouse.count <= available)
										{
											items[slot].count += p.OnMouse.count;
											p.OnMouse = Item.Nothing;
										}
										else
										{
											items[slot].count = stacking;
											p.OnMouse.count -= available;
										}
									}
									else
									{
										Item temp = items[slot];
										items[slot] = p.OnMouse;
										p.OnMouse = temp;
									}
								}
								else
								{
									byte stacking = isStackable(p.OnMouse.id);
									byte available = (byte)(stacking - items[slot].count);
									if (available == 0) return;
									if (p.OnMouse.count <= available)
									{
										items[slot].count += p.OnMouse.count;
										p.OnMouse = Item.Nothing;
									}
									else
									{
										items[slot].count = stacking;
										p.OnMouse.count -= available;
									}
								}
							}
							else
							{
								Item temp = items[slot];
								items[slot] = p.OnMouse;
								p.OnMouse = temp;
							}
						}
					}
				}
				#region Empty Slot Done
				else
				{
					if (slot == 0) return; //Crafting output slot
					#region Armor slots Done
					if (slot == 5 || slot == 6 || slot == 7 || slot == 8)
					{
						switch (slot)
						{
							case 5:
								if (p.OnMouse.id == 298 || p.OnMouse.id == 302 || p.OnMouse.id == 306 || p.OnMouse.id == 310)
								{
									items[slot] = p.OnMouse;
									p.OnMouse = Item.Nothing;
								}
								break;
							case 6:
								if (p.OnMouse.id == 299 || p.OnMouse.id == 303 || p.OnMouse.id == 307 || p.OnMouse.id == 311)
								{
									items[slot] = p.OnMouse;
									p.OnMouse = Item.Nothing;
								}
								break;
							case 7:
								if (p.OnMouse.id == 300 || p.OnMouse.id == 304 || p.OnMouse.id == 308 || p.OnMouse.id == 312)
								{
									items[slot] = p.OnMouse;
									p.OnMouse = Item.Nothing;
								}
								break;
							case 8:
								if (p.OnMouse.id == 301 || p.OnMouse.id == 305 || p.OnMouse.id == 309 || p.OnMouse.id == 313)
								{
									items[slot] = p.OnMouse;
									p.OnMouse = Item.Nothing;
								}
								break;
						}
					}
					#endregion
					else
					{
						if (click == ClickType.RightClick)
						{
							if (p.OnMouse.count == 1)
							{
								items[slot] = p.OnMouse;
								p.OnMouse = Item.Nothing;
							}
							else
							{
								p.OnMouse.count -= 1;
                                items[slot] = new Item(p.OnMouse.id, 1, p.OnMouse.meta, p.level, true);
							}
						}
						else
						{
							items[slot] = p.OnMouse;
							p.OnMouse = Item.Nothing;
						}
					}
				}
				#endregion
			}
			#region Empty Mouse done
			else //Player has NOTHING on the mouse
			{
				if (items[slot] != Item.Nothing)
				{
					if (click == ClickType.RightClick)
					{
						p.OnMouse = Right_Click(slot);
					}
					else //Player left-clicked
					{
						p.OnMouse = items[slot];
						Remove(slot);
					}
				}
				else
				{
					return;
				}
			}
			#endregion
		}

		public Item Right_Click(int slot)
		{
			try
			{
                Item temp = new Item(items[slot].id, 0, items[slot].meta, p.level, true);
				if (items[slot].count == 1)
				{
					temp = items[slot];
					items[slot] = Item.Nothing;
					return temp;
				}
				if (items[slot].count % 2 == 0) //this makes no FUCKING SENSE
				{
					temp.count = (byte)(items[slot].count / 2);
					items[slot].count = (byte)(items[slot].count / 2);
				}
				else
				{
					byte a = items[slot].count;
					items[slot].count = (byte)(a / 2);
					temp.count = (byte)(a - items[slot].count);
				}
				return temp;
			}
			catch
			{
				return Item.Nothing;
			}
		}
		
		public void UpdateVisibleItemInHand(short id, short damage)
		{
			foreach (int i in p.VisibleEntities.ToArray())
			{
				Entity e = Entity.Entities[i];
				if (!e.isPlayer || !e.p.VisibleEntities.Contains(p.id)) continue;
				e.p.SendEntityEquipment(p.id, 0, id, damage);
			}
		}
        public void UpdateVisibleItemInHand(short id)
        {
            UpdateVisibleItemInHand(id, 0);
        }

		public int FindEmptySlot()
		{			
			for (int i = 36; i < 45; i++)
				if (items[i].id == (short)Items.Nothing)
				{
					return i;
				}
			
			for (int i = 9; i <= 35; i++)
				if (items[i].id == (short)Items.Nothing)
				{
					return i;
				}
			
			return -1;
		}
		public static byte isStackable(short id)
		{
			if(id >= 1 && id <= 122) //all blocks are stackable and there is no missing id's
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

