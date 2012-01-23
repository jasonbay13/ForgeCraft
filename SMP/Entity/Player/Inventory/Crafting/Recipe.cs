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
namespace SMP.PLAYER.Crafting
{
    public class Recipe
    {
        /////////////////////////////////////////////////
        //                    SHAPED                   //
        /////////////////////////////////////////////////
        private byte sHeight;
        private byte sWidth;
        public Item[] sStack { get; private set; }
        public Item sOutput { get; private set; }

        ////////////////////////////////////////////////
        //


        /*
         * 
         * Shaped Recipe
         * 
         */
        public Recipe(byte width, byte height, Item[] stack, Item output)
        {
            sHeight = height;
            sWidth = width;
            sStack = stack;
            sOutput = output;
        }
        public bool Equals(Windows w)
        {
            for (int i = 0; i <= 3 - sWidth; i++)
            {
                for (int j = 0; j <= 3 - sHeight; j++)
                {
                    if (doItemCheck(w, i, j, true))
                        return true;
                    if (doItemCheck(w, i, j, false))
                        return true;
                }
            }
            return false;
        }
        private bool doItemCheck(Windows p, int i, int j, bool shift)
        {
            for (int k = 0; k <3; k++)
            {
                for (int l = 0; l <3; l++)   //ITS A HEART <3
                {
                    int x = k - i;
                    int y = l - j;
                    Item stack = null;
                    if (x >= 0 && y >= 0 && x < sWidth && y < sHeight)
                        if (shift)
                            stack = sStack[(sWidth - x - 1) + y * sWidth];
                        else
                            stack = sStack[(x + y * sWidth)];
                    Item oStack = p.getItem((byte)x, (byte)y);
                    if (stack == null && oStack == null)
                        continue;
                    if (oStack == null && stack != null || oStack != null && stack == null) 
                        return false;
                    if (oStack.id != stack.id)
                        return false;
                    //if(stack.AttackDamage > -1 && stack.AttackDamage != oStack.AttackDamage)
                    //return false;

                }
            }
            return true;

        }

    }


}