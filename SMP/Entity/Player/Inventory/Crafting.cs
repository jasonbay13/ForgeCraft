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
 * 
 * 
 * 
 * THiS IS IN "BETA" (Not finished)
 * 
 * 
*/




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMP
{
    public class Crafting
    {
        public static List<TableData> recipes;
        public static TableData Table { get; set; }
        // private Item inHand = Item.Nothing;
        public static void Init()
        {
            recipes = new List<TableData>();
            Table = new TableData();
            loadRecipies("path");
        }
        public static void loadRecipies(string filePath)
        {
            DateTime start = DateTime.Now;
            /*
             * XXX
             * #X#     ===  Pick axe format
             * #x#
             */
            CreateDefinedRecipe("XXX###XXX", new Item((short)Items.Diamond, 3), ParseForCrafting("X#".ToArray(), new Item[] { new Item((short)Blocks.Sand), new Item((short)-1) }), 9);
            util.Logger.Log(recipes.Count + " Recipes. Created in " + (DateTime.Now - start).Milliseconds);
        }
        public static void CreateDefinedRecipe(string format, Item output, CraftStack[] stack, short tableSize)
        {

            if (String.IsNullOrWhiteSpace(format)) throw new NullReferenceException("Recipe format is null");
            if (output == null) throw new NullReferenceException("Item output is null");
            Item[] iArray = new Item[tableSize + 1];
            string[] lines = format.Split(',');

            for (int i = 0; i < stack.Length; i++)
            {
                for (int j = 1; j <= tableSize; j++)
                    if (stack[i].CharId == format[j - 1])
                        iArray[j] = stack[i].Item;
            }
            iArray[0] = output;
            TableData data = new TableData();
            data.addData(iArray);
            recipes.Add(data);
        }

        public static Item[] CheckCrafting(Player p, short slot, Item[] items)
        {

            short tableSize = p.HasWindowOpen ? (p.window.Type == WindowType.Workbench ? (short)9 : (short)2) : (short)4;
            if (slot > tableSize) return items;
            Item[] array = new Item[items.Length];
            if (tableSize == 4) //personal crafting
            {
                items[0] = new Item((short)Items.Arrow);
                p.inventory.Add((short)Items.Arrow, 0);
                p.SendMessage(slot + " - slot from nothing");
                return items;
                // if (items[1].id == (short)Blocks.Sand)
                // {
                ////    p.SendItem(p.window.id, 0, new Item((short)Items.Diamond));
                //loadRecipies("lol");
                //    }
            }
            else if (tableSize == 2) //furnace?
            {

            }
            else if (tableSize == 9)
            {
                p.SendMessage(slot + " - slot");
                Table.addData(items);
                for (int i = 0; i < recipes.Count; i++)          //Need better way
                    if (Table.equals(recipes[i]))
                    {
                        items[0] = recipes[i].items[0];
                        p.SendItem(p.window.id, 0, recipes[i].items[0]);
                        if (slot == 0)
                        {
                            if (p.OnMouse.id != -1)
                            {
                                //p.OnMouse = inHand;
                                //inHand = p.OnMouse;
                                if ((p.OnMouse.id == items[0].id) && (p.OnMouse.meta == items[0].meta))
                                {
                                    //p.OnMouse.count += items[0].count;
                                    //items[0] = Item.Nothing;
                                }

                            }

                            for (int id = 1; id <= 9; id++)
                            {
                                Item toTake = items[id];
                                if (toTake != Item.Nothing)
                                {
                                    if (toTake.count > 0)
                                    {
                                        toTake.count -= 1;
                                        if (toTake.count == 0)
                                            toTake = Item.Nothing;
                                    }
                                    else toTake = Item.Nothing;
                                    items[id] = toTake;
                                }

                            }


                        }
                    }
            }
            else { util.Logger.Log("Incorrect inventory size"); } return items;
        }

        private static void SendItem(Player p, Item item, byte id)
        {
            if (!p.HasWindowOpen && id > 0) return;  //HAXORS
            p.SendItem(id, 0, item.id, item.count, item.meta);
            item = Item.Nothing;
        }
        public static CraftStack[] ParseForCrafting(char[] c, Item[] item)
        {
            if (c.Length != item.Length) throw new ArgumentException("Size difference");
            CraftStack[] stack = new CraftStack[c.Length];
            for (int i = 0; i < c.Length; i++)
                stack[i] = new CraftStack(c[i], item[i]);
            return stack;
        }


    }

    public class TableData
    {
        public Item[] items;
        public void addData(Item[] inOrder)
        {
            items = inOrder;
        }
        public bool equals(TableData compare)
        {
            if (items == null) throw new NullReferenceException("You must add data using \"addData(Item[])\" before comparing");
            for (int i = 1; i < items.Count(); i++)
            {
                if ((compare.items[i].id != this.items[i].id) || (compare.items[i].meta != this.items[i].meta))
                    return false;
            }
            return true;
        }

    }
    public class CraftStack
    {
        public char CharId { get; set; }
        public Item Item { get; set; }
        public CraftStack(char c, Item it)
        {
            CharId = c;
            Item = it;
        }
    }

}
