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
	public class Windows
	{
		public byte type; //This holds type information, used in deciding which kind of window we need to send.
		public string name = "Chest";
		public Point3 pos; //The pos of the block that this window is attached to
		public World level;
		public Item[] items; //Hold all the items this window has inside it.

		public Windows(byte Type, Point3 Pos, World Level)
		{
			Console.WriteLine("Window Creating.");

			type = Type;
			pos = Pos;
			level = Level;

			switch (type)
			{
				case 0:
					name = "Chest"; //We change this to "Large Chest" Later if it needs it :3
					items = new Item[27];
					break;
				case 1:
					name = "Workbench";
					items = new Item[10];
					break;
				case 2:
					name = "Furnace";
					items = new Item[3];
					break;
				case 3:
					name = "Dispenser";
					items = new Item[9];
					break;
				default:
					name = "Chest";
					items = new Item[27];
					break;
			}
			Console.WriteLine("Window adding.");
			level.windows.Add(pos, this);
			Console.WriteLine("Window done.");
		}

		public bool AddItem(Item item)
		{
			byte slot = FindEmptySlot();
			if (slot == 255) return false;

			return AddItem(item, slot);
		}
		public bool AddItem(Item item, byte slot)
		{
			return false;
		}

		public void RemoveItem(short id)
		{
			RemoveItem(id, 1);
		}
		public void RemoveItem(short id, byte count)
		{

		}

		public byte FindEmptySlot()
		{
			return 255;
		}
	}
}

