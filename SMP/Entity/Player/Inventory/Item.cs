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

namespace SMP
{
	public class Item
    {
        #region Enchantable Items List
        public static readonly System.Collections.Generic.List<short> ENCHANTABLE_ITEMS = new System.Collections.Generic.List<short> {
            0x15A, //Fishing rod
            0x167, //Shears

            //TOOLS
            //sword, shovel, pickaxe, axe, hoe
            0x10C, 0x10D, 0x10E, 0x10F, 0x122, //WOOD
            0x110, 0x111, 0x112, 0x113, 0x123, //STONE
            0x10B, 0x100, 0x101, 0x102, 0x124, //IRON
            0x114, 0x115, 0x116, 0x117, 0x125, //DIAMOND
            0x11B, 0x11C, 0x11D, 0x11E, 0x126, //GOLD

            //ARMOR
            //helmet, chestplate, leggings, boots
            0x12A, 0x12B, 0x12C, 0x12D, //LEATHER
            0x12E, 0x12F, 0x130, 0x131, //CHAIN
            0x132, 0x133, 0x134, 0x135, //IRON
            0x136, 0x137, 0x138, 0x139, //DIAMOND
            0x13A, 0x13B, 0x13C, 0x14D  //GOLD
		};
        #endregion
        public Entity e;
		public World level { get { return e.level; } set { e.level = value; } }

		public short item = -1;
		public byte count = 1;
		private short Mymeta = 0;
		public short meta
		{
			get
			{
				return Mymeta;
			}
			set
			{
				Mymeta = value;
				CheckDamage();
			}
		}
		public bool OnGround; //This is used to tell the server that this item is on the ground.

		public static Item Nothing = new Item();

		public Point3 pos;
		public byte[] rot;

		private Item() { }
		public Item (short item, World l)
		{
			this.item = item;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(Items item, World l)
		{
			this.item = (short)item;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(short item, byte count, short meta, World l)
		{
			this.item = (short)item;
			this.meta = meta;
			this.count = count;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(short item, byte count, short meta, World l, double[] pos, byte[] rot)
		{
			this.item = item;
			this.count = count;
			this.meta = meta;
			this.level = l;
			this.pos = pos;
			this.rot = rot;
			OnGround = true;
			e = new Entity(this, l);
			e.UpdateChunks(false, false);
		}

		public void CheckDamage()
		{
			/*
			 * SUDO CODE: 
			 * 
			 * if(!tool) return;
			 * if(damage > MaxDamage(id))
			 *	DeleteThisItem();
			*/
		}

        public void Physics()
        {
            // TODO
        }
	}
}

