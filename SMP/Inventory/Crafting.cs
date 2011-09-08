using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	static class Crafting
	{
		public static void CheckCrafting(Player p)
		{
			if (p.OpenWindow)
			{
				if (p.window.type == 1) //Workbench is open
				{

				}
				else return; //Player has a different window open.
			}
			else
			{
				//Crafting recipes will always use a 9slot window for searching, so we dont need doubles, for 4square checking, use 0,1,3,4 and use the 9spot checker.
				
			}
		}
	}
}
