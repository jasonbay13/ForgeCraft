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
using SMP.INVENTORY;

namespace SMP.PLAYER.Crafting
{
    public class CraftingManager
    {
        public static List<Recipe> recipes;
        // private Item inHand = Item.Nothing;
        public static void Init()
        {
            recipes = new List<Recipe>();
            loadRecipies("path");
        }
        public static void loadRecipies(string filePath)
        {
            DateTime start = DateTime.Now;
            //TODO Place Recipes here
            CreateDefinedRecipe(new Item(Items.DiamondSword), "***"," * ","***", '*', new Item((short)Blocks.Sand)); //Custom recipe, needs mod to fully work (it works though)
            CreateDefinedRecipe(new Item((short)Blocks.SandStone), "##", "##", '#', new Item((short)Blocks.Sand));

            util.Logger.Log(recipes.Count + " Recipes. Created in " + (DateTime.Now - start).TotalSeconds);
        }
        public static void CreateDefinedRecipe(Item result, params object[] aobj)
        {
            if (result == null || result == Item.Nothing) throw new ArgumentNullException("result", "The recipe result cannot be null or nothing");


            string s = "";
            int i = 0;
            int width = 0;
            int height = 0;
            if (aobj[i] is string[])
            {
                string[] what = (string[])aobj[i++];
                for (int l = 0; l < what.Length; l++)
                {
                    String s2 = what[l];
                    height++;
                    width = s2.Length;
                    s += (s + s2);
                }

            }
            else
            {
                while (aobj[i] is string)
                {
                    string s1 = (string)aobj[i++];
                    height++;
                    width = s1.Length;
                    s += (s + s1);
                }
            }
            Dictionary<char, Item> dict = new Dictionary<char, Item>();
            for (; i < aobj.Length; i += 2)
            {
                char character = (char)aobj[i];
                Item itemstack1 = null;
                if (aobj[i + 1] is Item)
                {
                    itemstack1 = (Item)aobj[i + 1];
                }
                dict.Add(character, itemstack1);
            }

            Item[] items = new Item[width * height];
            for (int i1 = 0; i1 < width * height; i1++)
            {
                char c = s[i1];
                if (dict.ContainsKey(c))
                {
                    items[i1] = ((Item)dict[c]);
                }
                else
                {
                    items[i1] = Item.Nothing;
                }
            }

            recipes.Add(new Recipe((byte)width, (byte)height, items, result));
        }

        public static Item getResult(Player p)
        {
            int i = 0;
            Item item1 = null;
            Item item2 = null;
            for (byte j = 1; j < p.window.InventorySize; j++)
            {
                Item getItem = p.window.items[j];
                if (getItem == null || getItem.id == -1)
                    continue;
                if (i == 0)
                    item1 = getItem;
                if (i == 1)
                    item2 = getItem;
                i++;

            }
            for (int k = 0; k < recipes.Count; k++)
            {
                Recipe r = recipes[k];
                if (r.Equals(p.window))
                    return r.sOutput;
            }
            return Item.Nothing;
        }

        public static void UpdateCrafting(Windows window)
        {
            for (byte i = 0; i < window.InventorySize; i++)
            {
                Item getItem = window.items[i];
                Item setItem = null;
                if (getItem == null || getItem.id == -1)
                    continue;
                setItem = getItem ?? Item.Nothing;
                window.p.SendItem(window.id, i, setItem);
                //window.p.window.p.window.p.window.p.window.p.window.p.window.p.window.p.SendMessage("Redundancy");
            }
        }
    }

}
