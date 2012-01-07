using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMP.PLAYER.Crafting;
namespace SMP.API
{
    public enum Size : short
    {
        CraftingBench = 9,
        PersonalInventory = 4,
        furnace = 2
    }
    public abstract class Custom_Recipes
    {
        public abstract Item[] line1 { get; }
        public abstract Item[] line2 { get; }
        public abstract Item[] line3 { get; }
        public abstract Item output { get; }
        public abstract Size size { get; }
        public abstract string name { get; }
        public static void Load(Custom_Recipes c)
        {
            Item[] stack = c.GetStack();
            Crafting.CreateDefinedRecipe(c.output, stack, (short)c.size);
        }
        public Item[] GetStack()
        {
            Item[] stack = new Item[9];
            for (int i = 0; i < line1.Length; i++)
            {
                if (line1[i] == null)
                    stack[i] = new Item(-1);
                else
                    stack[i] = line1[i];
            }
            for (int i = 0; i < line2.Length; i++)
            {
                if (line2[i] == null)
                    stack[i + 3] = new Item(-1);
                else
                    stack[i + 3] = line2[i];
            }
            for (int i = 0; i < line3.Length; i++)
            {
                if (line3[i] == null)
                    stack[i + 6] = new Item(-1);
                else
                    stack[i + 6] = line3[i];
            }
            return stack;
        }
    }
}
