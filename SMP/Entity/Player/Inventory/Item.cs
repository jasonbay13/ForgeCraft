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
using System.IO;
using System.Collections.Generic;
using Substrate.Nbt;

namespace SMP
{
	public class Item
    {
        #region Enchantable Items List
        public static readonly System.Collections.Generic.List<short> ENCHANTABLE_ITEMS = new System.Collections.Generic.List<short> {
            //TOOLS
            //sword, shovel, pickaxe, axe
            0x10C, 0x10D, 0x10E, 0x10F, //WOOD
            0x110, 0x111, 0x112, 0x113, //STONE
            0x10B, 0x100, 0x101, 0x102, //IRON
            0x114, 0x115, 0x116, 0x117, //DIAMOND
            0x11B, 0x11C, 0x11D, 0x11E, //GOLD

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
        public List<Enchantment> enchantments = new List<Enchantment>();
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


        public bool IsDamageable()
        {
            return BlockData.IsItemDamageable(item);
        }

        public static int GetDataLength(byte[] buffer, int index)
        {
            int length = 2;
            if (util.EndianBitConverter.Big.ToInt16(buffer, index) != -1)
            {
                length += 3;
                if (BlockData.IsItemDamageable(util.EndianBitConverter.Big.ToInt16(buffer, index)))
                {
                    length += 2;
                    if (util.EndianBitConverter.Big.ToInt16(buffer, index + 5) != -1)
                        length += util.EndianBitConverter.Big.ToInt16(buffer, index + 5);
                }
            }
            return length;
        }

        public void ReadData(byte[] buffer, int index)
        {
            item = util.EndianBitConverter.Big.ToInt16(buffer, index);
            if (item != -1)
            {
                count = buffer[index + 2];
                meta = util.EndianBitConverter.Big.ToInt16(buffer, index + 3);
                if (IsDamageable()) ReadEnchantmentNBTData(buffer, index + 5);
            }
        }

        public void ReadEnchantmentNBTData(byte[] buffer, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("Index cannot be less than 0.");

            int length = util.EndianBitConverter.Big.ToInt16(buffer, index);
            byte[] data = new byte[length];
            Array.Copy(buffer, index + 2, data, 0, length);

            ReadEnchantmentNBTData(data);
        }

        public void ReadEnchantmentNBTData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data.Decompress(CompressionType.GZip)))
            {
                try
                {
                    NbtTree nbt = new NbtTree(ms);
                    TagNodeList list = nbt.Root["ench"].ToTagList();

                    TagNodeCompound compound;
                    foreach (TagNode tag in list)
                    {
                        compound = tag.ToTagCompound();
                        enchantments.Add(new Enchantment(compound["id"].ToTagShort().Data, compound["lvl"].ToTagShort().Data));
                    }
                }
                catch (InvalidCastException) { Server.ServerLogger.Log("NBT data is invalid."); }
            }
        }

        public byte[] GetEnchantmentNBTData()
        {
            return GetEnchantmentNBTData(enchantments);
        }

        public static byte[] GetEnchantmentNBTData(List<Enchantment> enchantments)
        {
            if (enchantments.Count < 1) return new byte[0];

            NbtTree nbt = new NbtTree();
            TagNodeList list = new TagNodeList(TagType.TAG_COMPOUND);

            TagNodeCompound compound;
            foreach (Enchantment ench in enchantments.ToArray())
            {
                compound = new TagNodeCompound();
                compound.Add("id", new TagNodeShort(ench.id));
                compound.Add("lvl", new TagNodeShort(ench.level));
                list.Add(compound);
            }

            nbt.Root.Add("ench", list);

            using (MemoryStream ms = new MemoryStream())
            {
                nbt.WriteTo(ms);
                return ms.ToArray().Compress(Ionic.Zlib.CompressionLevel.BestCompression, CompressionType.GZip);
            }
        }
	}
}

