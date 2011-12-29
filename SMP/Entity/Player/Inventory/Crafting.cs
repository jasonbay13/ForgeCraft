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

namespace SMP
{
    public class Crafting
    {
        private short size;
        private Player p;
        private List<TableData> recipes;
        private TableData Table { get; set; }
        private Item inHand = Item.Nothing;
        public Crafting(Player p, short size)
        {
            this.p = p;
            this.size = size;
            recipes = new List<TableData>();
            Table = new TableData(size);
            loadRecipies("LOL");
        }
        public void loadRecipies(string filePath)
        {
            /*
             * XXX
             * #X#     ===  Pick axe format
             * #x#
             */
            CreateRecipe("XXX#X##X#", new Item((short)Items.Diamond, 3),
                ParseForCrafting(new char[] { '#', 'X' }, new Item[] { new Item((short)-1), new Item((short)Blocks.Sand) }));
        }
        public void CreateRecipe(string format, Item output, params object[] obj)
        {

            if (String.IsNullOrWhiteSpace(format)) throw new NullReferenceException("Recipe format is null");
            if (output == null) throw new NullReferenceException("Item output is null");
            if ((obj[0].GetType() != typeof(char[]) || obj[obj.Length - 1].GetType() != typeof(Item[])) && obj.Length % 2 != 0)
                throw new ArgumentException("Incorrect recipe format");
            Item[] iArray = new Item[size + 1];

            for (int i = 1; i < obj.Length; i += 2)
            {
                if (obj[i - 1].GetType() == typeof(char[]) && obj[i].GetType() == typeof(Item[]))
                {
                    char[] c = (char[])obj[i - 1];
                    Item[] e = (Item[])obj[i];
                    for (int j = 1; j <= size; j++)
                    {
                        for (int holyWtf = 0; holyWtf < c.Length; holyWtf++)
                        {
                            if (c[holyWtf] == format[j - 1])
                                iArray[j] = e[holyWtf];
                        }
                    }
                    iArray[0] = output;
                }
            }
            TableData data = new TableData(size);
            data.addData(iArray);
            recipes.Add(data);
        }

        public void CheckCrafting(short slot, Item[] items)
        {
            if (size == 4)
            {
                // if (items[1].id == (short)Blocks.Sand)
                // {
                ////    p.SendItem(p.window.id, 0, new Item((short)Items.Diamond));
                //loadRecipies("lol");
                //    }
            }
            else if (size == 2) //furnace?
            {

            }
            else if (size == 9)
            {
                p.SendMessage(slot + " - slot");
                Table.addData(items);
                for (int i = 0; i < recipes.Count; i++)          //Need better way
                    if (Table.equals(recipes[i]))
                    {
                        SendItem(recipes[i].items[0]);
                        items[0] = recipes[i].items[0];
                        if (slot == 0)
                        {
                            if (p.OnMouse != null)
                            {
                                p.OnMouse = inHand;
                                inHand = p.OnMouse;
                                if ((p.OnMouse.id == items[0].id) && (p.OnMouse.meta == items[0].meta))
                                {
                                    p.OnMouse.count += items[0].count;
                                }
                                items[0] = Item.Nothing;
                            }

                            for (int id = 1; id < 9; id++)
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
                            return;

                        }
                    }

            }
            else util.Logger.Log("Incorrect inventory size"); 
        }

        private void SendItem(Item item)
        {
            if (!p.HasWindowOpen) return;
            p.SendItem(p.window.id, 0, item);
            item = Item.Nothing;
        }
        public static object[] ParseForCrafting(char[] c, Item[] item)
        {
            return new object[] { c, item };
        }


    }

    public class TableData
    {
        private short _size;
        public Item[] items;
        public TableData(short size)
        {
            _size = (short)(size + 1);
        }
        public void addData(Item[] inOrder)
        {
            if (inOrder.Length > _size) throw new ArgumentException("Size of array cant be bigger than initialized");
            items = inOrder;
        }
        public bool equals(TableData compare)
        {
            if (items == null) throw new NullReferenceException("You must add data using \"addData(Item[])\" before comparing");
            for (int i = 1; i < _size; i++)
            {
                if ((compare.items[i].id != this.items[i].id) || (compare.items[i].meta != this.items[i].meta))
                    return false;
            }
            return true;
        }

    }

}
