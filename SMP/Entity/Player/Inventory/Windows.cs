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
    public enum WindowType : byte
    {
        Chest = 0,
        Workbench = 1,
        Furnace = 2,
        Dispenser = 3,
        EnchantmentTable = 4,
        BrewingStand = 5
    }
	public class Windows
    {
        #region Events
        //-----------Events-----------
        public delegate Item OnRightClick(Player p, int slot);
        public event OnRightClick RightClick;
        public delegate void OnClick(Player p, WindowType window, short slot, ClickType click, short ActionID, bool PressingShift, short itemID, byte count, short MetaData);
        public event OnClick OnWindowClick;
        internal bool cancelclick = false;
        internal bool cancelright = false;
        //-----------Events-----------
        #endregion
        private static byte nextId = 1;
        byte type; //This holds type information, used in deciding which kind of window we need to send.
        public WindowType Type { get { return (WindowType)type; } }
        public int InventorySize { get { return items.Length; } }
        public byte id;
		public string name = "Chest";
        public Container container;
		public Item[] items; //Hold all the items this window has inside it.
		public Windows(WindowType type, Point3 pos, World world)
		{
            try
            {
                id = FreeId();
                this.type = (byte)Type;

                switch (Type)
                {
                    case WindowType.Chest:
                        name = "Chest"; //We change this to "Large Chest" Later if it needs it :3
                        container = world.GetBlockContainer(pos);
                        items = container.Items;
                        break;
                    case WindowType.Dispenser:
                        name = "Workbench";
                        //container = world.GetBlockContainer(pos); // We don't have a container for this yet.
                        items = new Item[9];
                        break;
                    case WindowType.Furnace:
                        name = "Furnace";
                        //container = world.GetBlockContainer(pos); // We don't have a container for this yet.
                        items = new Item[3];
                        break;
                    case WindowType.Workbench:
                        name = "Dispenser";
                        items = new Item[10];
                        break;
                    case WindowType.EnchantmentTable:
                        name = "Enchant";
                        items = new Item[1];
                        break;
                    case WindowType.BrewingStand:
                        name = "Brewing Stand";
                        //container = world.GetBlockContainer(pos); // We don't have a container for this yet.
                        items = new Item[4];
                        break;
                }
            }
            catch { Server.Log("Error making window!"); }
		}

		//public bool AddItem(Item item)
		//{
		//    byte slot = FindEmptySlot();
		//    if (slot == 255) return false;

		//    return AddItem(item, slot);
		//}
		public bool AddItem(Item item, byte slot)
		{
			return false;
		}

		public void Remove(Player p, int slot)
		{
			if (slot < items.Length) items[slot] = Item.Nothing;
			else p.inventory.Remove((slot - items.Length) + 9);
				
		}
		public void Remove(Player p, int slot, byte count)
		{
			if (slot < items.Length) items[slot].count -= count;
			else p.inventory.Remove((slot - items.Length) + 9, count);

		}

		public Item Right_Click(Player p, int slot)
		{
            return Item.Nothing; // TODO
            
            /*Item temp11 = Item.Nothing;
            if (RightClick != null)
                temp11 = RightClick(p, slot);
            if (cancelright)
            {
                cancelright = false;
                return temp11;
            }
			if (slot > items.Length)
			{
				return p.inventory.Right_Click((slot - items.Length) + 9);
			}
			else
			{
				try
				{
                    Item temp = new Item(items[slot].id, 0, items[slot].meta);
					if (items[slot].count == 1)
					{
						temp = items[slot];
						items[slot] = Item.Nothing;
						return temp;
					}
					if (items[slot].count % 2 == 0)
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
			}*/
		}
		
		public void HandleClick(Player p, byte[] message)
		{
            return; // TODO
			/*byte id = message[0];
			short slot = util.EndianBitConverter.Big.ToInt16(message, 1);
			ClickType click = (ClickType)message[3];
			short ActionID = util.EndianBitConverter.Big.ToInt16(message, 4);
			bool Shift = (message[6] == 1);
			short ItemID = util.EndianBitConverter.Big.ToInt16(message, 7);
			byte Count = 1;
			short Meta = 0;
            if (OnWindowClick != null)
                OnWindowClick(p, Type, slot, click, ActionID, Shift, ItemID, Count, Meta);
			if (ItemID != -1)
			{
				Count = message[9];
				Meta = util.EndianBitConverter.Big.ToInt16(message, 10);
			}

			if (slot == -999)
			{
				//TODO throw item
				p.OnMouse = Item.Nothing;
				return;
			}

			if (slot > items.Length && !Shift)
			{
				p.inventory.ClickHandler((short)((slot - items.Length) + 9), click, ActionID, Shift, ItemID, Count, Meta);
				return;
			}
			if (Shift)
			{
				if (type == 3) return;
				else if (type == 0)
				{
					if (slot > items.Length)
					{
						//TODO Inventory to Chest
					}
					else
					{
						//TODO Chest to Inventory
					}
				}
				else if (type == 1)
				{
					if (slot > items.Length)
					{
						p.inventory.ClickHandler((short)((slot - items.Length) + 9), click, ActionID, Shift, ItemID, Count, Meta);
						return;
					}
					else
					{
						//TODO Workbench to Inventory
					}
				}
				else if (type == 2)
				{
					if (slot > items.Length)
					{
						p.inventory.ClickHandler((short)((slot - items.Length) + 9), click, ActionID, Shift, ItemID, Count, Meta);
						return;
					}
					else
					{
						//TODO Furnace to Inventory
					}
				}
			}
			else
			{
				if (p.OnMouse != Item.Nothing)
				{
					if (items[slot] != Item.Nothing)
					{
						#region Crafting/Furnace Output
						if ((type == 1 && slot == 0) || (type == 2 && slot == 0))
						{
							if (p.OnMouse.id == items[slot].id)
							{
								if (p.OnMouse.id < 255)
								{
									if (p.OnMouse.meta == items[slot].meta)
									{
										byte stacking = Inventory.isStackable(p.OnMouse.id);
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
									byte stacking = Inventory.isStackable(p.OnMouse.id);
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
											byte stacking = Inventory.isStackable(p.OnMouse.id);
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
										byte stacking = Inventory.isStackable(p.OnMouse.id);
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
											byte stacking = Inventory.isStackable(p.OnMouse.id);
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
										byte stacking = Inventory.isStackable(p.OnMouse.id);
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
                                    items[slot] = new Item(p.OnMouse.id, 1, p.OnMouse.meta);
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
							p.OnMouse = Right_Click(p, slot);
						}
						else //Player left-clicked
						{
							p.OnMouse = items[slot];
							Remove(p, slot);
						}
					}
					else
					{
						return;
					}
				}
				#endregion
			}*/
		}

		public int GetEmptyWindowSlot()
		{
			for (byte i = 0; i < items.Length; i++)
                if (items[i].id == (short)Items.Nothing)
					return i;
            return -1;
		}

		public int GetEmptyHotbarSlot(Player p)
		{
			for (byte i = 36; i < 45; i++)
				if (p.inventory.items[i].id == (short)Items.Nothing)
					return i;
            return -1;
		}
		public int GetEmptyHotbarSlotReversed(Player p)
		{
			for (byte i = 44; i >= 36; i--)
				if (p.inventory.items[i].id == (short)Items.Nothing)
					return i;
            return -1;
		}

		public int GetEmptyInventorySlot(Player p)
		{
			for (byte i = 9; i <= 35; i++)
				if (p.inventory.items[i].id == (short)Items.Nothing)
					return i;
            return -1 - 1;
		}
		public int GetEmptyInvetorySlotReversed(Player p)
		{
			for (byte i = 35; i >= 9; i--)
				if (items[i].id == (short)Items.Nothing)
					return i;
			return -1;
		}

        private static byte FreeId()
        {
            if (nextId == 0) nextId++;
            return nextId++;
        }
	}
}

