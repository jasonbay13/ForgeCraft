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
            CreateRecipe("XXX#X##X#", new Item((short)Items.Diamond, 3),
                Parse(new char[]{'#', 'X'}, new Item[]{new Item((short)-1), new Item((short)Blocks.Sand)}));
        }
        public void CreateRecipe(string format, Item output, params object[] obj )
        {
            /*
             * XXX
             * #X#     ===  Pick axe
             * #x#
             */
            if (String.IsNullOrWhiteSpace(format)) throw new NullReferenceException("Recipe format is null");
            if (output == null) throw new NullReferenceException("Item output is null");
            if ((obj[0].GetType() != typeof(char[]) || obj[obj.Length - 1].GetType() != typeof(Item[])) && obj.Length % 2 != 0) throw new ArgumentException("Incorrect recipe format");
            Item[] iArray = new Item[size];

            for (int i = 1; i < obj.Length; i+=2)
            {
                if (obj[i - 1].GetType() == typeof(char[]) && obj[i].GetType() == typeof(Item[]))
                {
                    char[] c = (char[])obj[i - 1];
                    Item[] e = (Item[])obj[i];
                    for(int j = 0; j < size; j++)
                    {
                        for (int holyWtf = 0; holyWtf < c.Length; holyWtf++)
                        {
                            if (c[holyWtf] == format[j])
                                iArray[j] = e[holyWtf];
                        }
                    }
                }
            }
            TableData data = new TableData(size);
            data.addData(iArray);
            recipes.Add(data);
        }

        public void CheckCrafting(short slot, Item[] items)
        {
            //if (slot == 0) return;
            if (size == 4)
            {
                if (items[1].id == (short)Blocks.Sand)
                {
                    p.SendItem(p.window.id, 0, new Item((short)Items.Diamond));
                    loadRecipies("lol");
                }
            }
            else if (size == 2) //furnace?
            {

            }
            else if (size == 9)
            {
                Item[] it = new Item[9];
                for (int k = 1; k < 10; k++)
                    it[k - 1] = items[k];
                Table.addData(it);
                for(int i = 0; i < recipes.Count; i++)          //Need better way
                    if (Table.equals(recipes[i])) Player.GlobalMessage("MATCH");
                
            }
            else { util.Logger.Log("Incorrect inventory size"); }
        }
        public static object[] Parse(char[] c, Item[] item)
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
            _size = size;
            items = new Item[size];
        }
        public void addData(Item[] inOrder)
        {
            if (inOrder.Length > _size) throw new ArgumentException("Size of array cant be bigger than initialized");
            items = inOrder;
        }
        public bool equals(TableData compare)
        {
            for (int i = 0; i < _size; i++)
            {
                if ((compare.items[i].id != this.items[i].id) || (compare.items[i].count != this.items[i].count) || (compare.items[i].meta != this.items[i].meta))
                    return false;
            }
            return true;
        }

    }

}
