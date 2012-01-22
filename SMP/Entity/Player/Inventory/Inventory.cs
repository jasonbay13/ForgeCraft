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
using SMP.PLAYER;
using SMP.PLAYER.Crafting;
using SMP.ENTITY;

namespace SMP.INVENTORY
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

		public bool Add(short item, int slot)
		{
			return Add(item, 1, 0, slot);
		}
		public bool Add(Item item)
		{
			return Add(item.id, item.count, item.meta);
		}
		public bool Add(short item, byte count, short meta)
		{
			//Console.WriteLine("add1");
			byte stackable = isStackable(item);
			byte c = count;
			//Console.WriteLine("add2");
			for (int i = 36; i < 45; i++)
			{
				if (c == 0) return true;
				if (items[i].id == item)
				{
					if (items[i].meta != meta) continue;
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
				if (c == 0) return true;
				if (items[i].id == item)
				{
					if (items[i].meta != meta) continue;
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
			return Add(item, c, meta, FindEmptySlot());
		}
		public bool Add(short id, byte count, short meta, int slot)
		{
            if (count < 1) return Add(Item.Nothing, slot);
            else return Add(new Item(id, count, meta), slot);
		}
        public bool Add(Item item, int slot)
        {
            if (slot > 44 || slot < 0) return false;
            items[slot] = item;
            p.SendItem((short)slot, item.id, item.count, item.meta);
            if (slot == current_index) UpdateVisibleItemInHand(item.id, item.meta);
            return true;
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

        public void HandleClick(short slot, ClickType click, short ActionID, bool Shift)
		{
			if (slot == -999)
			{
				//TODO throw item
                if (p.OnMouse.id != -1)
                {
                    if (click == ClickType.RightClick && p.OnMouse.count > 1)
                        p.OnMouse.count--;
                    else
                        p.OnMouse = Item.Nothing;
                }
				return;
			}
            if (slot < 0 || slot > 44) return;
            Item item, clickItem = items[slot];

            //if(slot < 5)items = Crafting.CheckCrafting(p, slot, items);

            if (Shift)
            {
                if (clickItem.id != -1)
                {
                    bool useEmptySlot = false;
                    if (slot >= 36 || slot <= 8)
                    {
                        for (int i = 9; i < 36; i++)
                        {
                            item = items[i];
                            if (useEmptySlot)
                            {
                                if (item.id == -1)
                                {
                                    items[i] = clickItem;
                                    items[slot] = Item.Nothing;
                                    break;
                                }
                            }
                            else if (item.id == clickItem.id && item.meta == clickItem.meta)
                            {
                                byte stack = isStackable(item.id);
                                byte avail = (byte)(stack - item.count);
                                if (avail < 1) continue;

                                if (clickItem.count <= avail)
                                {
                                    item.count += clickItem.count;
                                    items[slot] = Item.Nothing;
                                    break;
                                }
                                else
                                {
                                    item.count = stack;
                                    clickItem.count -= avail;
                                }
                            }
                            if (i == 35 && !useEmptySlot) { useEmptySlot = true; i = 8; }
                        }
                    }
                    else
                    {
                        for (int i = 36; i < 45; i++)
                        {
                            item = items[i];
                            if (useEmptySlot)
                            {
                                if (item.id == -1)
                                {
                                    items[i] = clickItem;
                                    items[slot] = Item.Nothing;
                                    break;
                                }
                            }
                            else if (item.id == clickItem.id && item.meta == clickItem.meta)
                            {
                                byte stack = isStackable(item.id);
                                byte avail = (byte)(stack - item.count);
                                if (avail < 1) continue;

                                if (clickItem.count <= avail)
                                {
                                    item.count += clickItem.count;
                                    items[slot] = Item.Nothing;
                                    break;
                                }
                                else
                                {
                                    item.count = stack;
                                    clickItem.count -= avail;
                                }
                            }
                            if (i == 44 && !useEmptySlot) { useEmptySlot = true; i = 35; }
                        }
                    }
                }
            }
            else
            {
                if (p.OnMouse.id == -1)
                {
                    if (clickItem.id != -1)
                    {
                        if (click == ClickType.RightClick && clickItem.count > 1)
                        {
                            p.OnMouse = new Item(clickItem);
                            p.OnMouse.count = (byte)Math.Ceiling((float)p.OnMouse.count / 2F);
                            clickItem.count /= 2;
                        }
                        else
                        {
                            items[slot] = Item.Nothing;
                            p.OnMouse = clickItem;
                        }
                    }
                }
                else
                {
                    if (clickItem.id != -1)
                    {
                        if (p.OnMouse.id == clickItem.id && p.OnMouse.meta == clickItem.meta)
                        {
                            if (slot < 5 || slot > 8)
                            {
                                byte stack = isStackable(clickItem.id);
                                if (click == ClickType.RightClick && p.OnMouse.count > 1)
                                {
                                    if (clickItem.count < stack)
                                    {
                                        p.OnMouse.count--;
                                        clickItem.count++;
                                    }
                                }
                                else
                                {
                                    if (clickItem.count < stack)
                                    {
                                        byte avail = (byte)(stack - clickItem.count);
                                        if (p.OnMouse.count <= avail)
                                        {
                                            clickItem.count += p.OnMouse.count;
                                            p.OnMouse = Item.Nothing;
                                        }
                                        else
                                        {
                                            clickItem.count = stack;
                                            p.OnMouse.count -= avail;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (slot >= 5 && slot <= 8)
                            {
                                if (ArmorSlotCheck(slot, p.OnMouse.id))
                                {
                                    items[slot] = p.OnMouse;
                                    p.OnMouse = clickItem;
                                }
                            }
                            else
                            {
                                items[slot] = p.OnMouse;
                                p.OnMouse = clickItem;
                            }
                        }
                    }
                    else
                    {
                        if (slot >= 5 && slot <= 8)
                        {
                            if (ArmorSlotCheck(slot, p.OnMouse.id))
                            {
                                items[slot] = new Item(p.OnMouse);
                                items[slot].count = 1;
                                if (p.OnMouse.count > 1) p.OnMouse.count--;
                                else p.OnMouse = Item.Nothing;
                            }
                        }
                        else
                        {
                            if (click == ClickType.RightClick && p.OnMouse.count > 1)
                            {
                                items[slot] = new Item(p.OnMouse);
                                items[slot].count = 1;
                                p.OnMouse.count--;
                            }
                            else
                            {
                                items[slot] = p.OnMouse;
                                p.OnMouse = Item.Nothing;
                            }
                        }
                    }
                }
            }

            p.SendWindowItems(0, items);
            p.SendItem(255, 255, p.OnMouse);
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
				if (items[i].id == -1)
					return i;
			
			for (int i = 9; i <= 35; i++)
				if (items[i].id == -1)
					return i;
			
			return -1;
		}
		public static byte isStackable(short id)
		{
			if(id >= 1 && id <= 122) //all blocks are stackable and there is no missing id's
				return 64;
			
			// may be missing a few
			List<short> stackable64 = new List<short> {
				260, 262, 263, 264, 265, 266, 280, 281, 287,
                288, 289, 295, 296, 297, 318, 319, 320, 321,
                322, 331, 334, 336, 337, 338, 339, 340, 341,
                345, 347, 348, 349, 350, 351, 352, 353, 356,
                360, 361, 362, 363, 364, 365, 366, 367, 369,
                370, 371, 372, 374, 375, 376, 377, 378, 381,
                382
			};
			
			if(stackable64.Contains(id))
			{
				return 64;
			}
			
			List<short> stackable16 = new List<short> {
				332, 344, 368
			};
			
			if(stackable16.Contains(id))
			{
				return 16;
			}
			
			if(id == 357) //cookies
			{
				return 8;
			}
			return 1;
		}

        public static bool IsArmorPiece(short id, ArmorType type)
        {
            switch (type)
            {
                case ArmorType.Head:
                    switch (id)
                    {
                        case 86:
                        case 298:
                        case 302:
                        case 306:
                        case 310:
                        case 314:
                            return true;
                    }
                    break;
                case ArmorType.Chest:
                    switch (id)
                    {
                        case 299:
                        case 303:
                        case 307:
                        case 311:
                        case 315:
                            return true;
                    }
                    break;
                case ArmorType.Legs:
                    switch (id)
                    {
                        case 300:
                        case 304:
                        case 308:
                        case 312:
                        case 316:
                            return true;
                    }
                    break;
                case ArmorType.Shoes:
                    switch (id)
                    {
                        case 301:
                        case 305:
                        case 309:
                        case 313:
                        case 317:
                            return true;
                    }
                    break;
            }
            return false;
        }

        public static bool ArmorSlotCheck(int slot, short id)
        {
            switch (slot)
            {
                case 5: return IsArmorPiece(id, ArmorType.Head);
                case 6: return IsArmorPiece(id, ArmorType.Chest);
                case 7: return IsArmorPiece(id, ArmorType.Legs);
                case 8: return IsArmorPiece(id, ArmorType.Shoes);
            }
            return false;
        }
	}

    public enum ArmorType
    {
        Head,
        Chest,
        Legs,
        Shoes
    }
}

