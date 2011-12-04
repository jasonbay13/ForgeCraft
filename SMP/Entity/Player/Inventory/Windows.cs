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
using System.Linq;
using SMP.util;

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
	public class Windows : IDisposable
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
        public Player p;
        public Container container;
		public Item[] items; //Hold all the items this window has inside it.
		public Windows(WindowType type, Point3 pos, World world, Player p)
		{
            try
            {
                id = FreeId();
                this.type = (byte)type;
                this.p = p;

                switch (Type)
                {
                    case WindowType.Chest:
                        name = "Chest"; //We change this to "Large Chest" Later if it needs it :3
                        container = world.GetBlockContainer(pos);
                        items = container.Items;
                        container.AddPlayer(p);
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

                if (Type == WindowType.Workbench || Type == WindowType.EnchantmentTable)
                {
                    for (int i = 0; i < InventorySize; i++)
                        items[i] = Item.Nothing;
                }
            }
            catch { Logger.Log("Error making window!"); }
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

        public void HandleClick(Player p, short slot, ClickType click, short ActionID, bool Shift)
		{
            if (slot == -999)
            {
                p.inventory.HandleClick(slot, click, ActionID, Shift);
                return;
            }
            if (slot < 0 || slot > InventorySize + 35) return;
            if (Type == WindowType.Workbench || Type == WindowType.Furnace)
            {
                if (slot == 0)
                {
                    // TODO: Crafting/smelting output handler.
                    return;
                }
            }

            if (Shift)
            {
                if (slot >= InventorySize)
                {
                    if (Type == WindowType.Workbench || Type == WindowType.Furnace || Type == WindowType.EnchantmentTable || Type == WindowType.BrewingStand)
                    {
                        p.inventory.HandleClick((short)((slot - InventorySize) + 9), click, ActionID, Shift);
                        return;
                    }
                }

                Item item;
                bool useEmptySlot = false;
                if (slot < InventorySize)
                {
                    Item clickItem = items[slot];
                    if (clickItem.id != -1)
                    {
                        for (int i = 44; i >= 36; i--)
                        {
                            item = p.inventory.items[i];
                            if (useEmptySlot)
                            {
                                if (item.id == -1)
                                {
                                    p.inventory.items[i] = clickItem;
                                    items[slot] = Item.Nothing;
                                    break;
                                }
                            }
                            else if (item.id == clickItem.id && item.meta == clickItem.meta)
                            {
                                byte stack = Inventory.isStackable(item.id);
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
                            if (i == 36 && !useEmptySlot) { useEmptySlot = true; i = 45; }
                        }
                        if (items[slot].id != -1)
                        {
                            useEmptySlot = false;
                            for (int i = 35; i >= 9; i--)
                            {
                                item = p.inventory.items[i];
                                if (useEmptySlot)
                                {
                                    if (item.id == -1)
                                    {
                                        p.inventory.items[i] = clickItem;
                                        items[slot] = Item.Nothing;
                                        break;
                                    }
                                }
                                else if (item.id == clickItem.id && item.meta == clickItem.meta)
                                {
                                    byte stack = Inventory.isStackable(item.id);
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
                                if (i == 9 && !useEmptySlot) { useEmptySlot = true; i = 36; }
                            }
                        }
                    }
                }
                else
                {
                    slot = (short)((slot - InventorySize) + 9);
                    Item clickItem = p.inventory.items[slot];
                    if (clickItem.id != -1)
                    {
                        for (int i = 0; i < InventorySize; i++)
                        {
                            item = items[i];
                            if (useEmptySlot)
                            {
                                if (item.id == -1)
                                {
                                    items[i] = clickItem;
                                    p.inventory.items[slot] = Item.Nothing;
                                    break;
                                }
                            }
                            else if (item.id == clickItem.id && item.meta == clickItem.meta)
                            {
                                byte stack = Inventory.isStackable(item.id);
                                byte avail = (byte)(stack - item.count);
                                if (avail < 1) continue;

                                if (clickItem.count <= avail)
                                {
                                    item.count += clickItem.count;
                                    p.inventory.items[slot] = Item.Nothing;
                                    break;
                                }
                                else
                                {
                                    item.count = stack;
                                    clickItem.count -= avail;
                                }
                            }
                            if (i == InventorySize - 1 && !useEmptySlot) { useEmptySlot = true; i = -1; }
                        }
                    }
                }
            }
            else
            {
                if (slot >= InventorySize)
                {
                    p.inventory.HandleClick((short)((slot - InventorySize) + 9), click, ActionID, Shift);
                    return;
                }

                Item clickItem = items[slot];
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
                            byte stack = Inventory.isStackable(clickItem.id);
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
                        else
                        {
                            items[slot] = p.OnMouse;
                            p.OnMouse = clickItem;
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

            if (container != null) container.UpdateContents(p);
            List<Item> items2 = new List<Item>(items); items2.AddRange((Item[])p.inventory.items.TruncateStart(9));
            p.SendWindowItems(id, items2.ToArray());
            p.SendItem(255, -1, p.OnMouse);
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

        public void Dispose()
        {
            if (container != null)
            {
                container.RemovePlayer(p);
                container = null;
            }
            items = null;
            name = null;
            p = null;
        }

        private static byte FreeId()
        {
            if (nextId == 0) nextId++;
            return nextId++;
        }
	}
}

