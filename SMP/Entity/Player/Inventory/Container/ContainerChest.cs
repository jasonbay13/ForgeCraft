using System;
using Substrate.Nbt;

namespace SMP
{
    public class ContainerChest : Container
    {
        public override ContainerType Type { get { return ContainerType.Chest; } }
        public override int Size { get { return 27; } }
        public override Point3 Pos { get { return point; } }
        protected Point3 point;
        internal Item[] items;


        public ContainerChest(Point3 point)
        {
            this.point = point;
            items = new Item[Size];
            items.Initialize();
        }

        public ContainerChest(Point3 point, Item[] items)
        {
            this.point = point;
            this.items = items;
        }

        public override Item GetSlot(int slot)
        {
            if (slot < 0 || slot > Size - 1) return null;
            return (items[slot] == null ? Item.Nothing : items[slot]);
        }

        public override void SetSlot(int slot, Item item)
        {
            if (slot < 0 || slot > Size - 1) return;
            if (item == null) items[slot] = Item.Nothing;
            else items[slot] = item;
        }

        public override TagNodeList GetNBTData()
        {
            TagNodeCompound comp; Item item;
            TagNodeList list = new TagNodeList(TagType.TAG_COMPOUND);
            for (int i = 0; i < Size; i++)
            {
                item = items[i];
                if (item != null && item.id != -1)
                {
                    comp = item.GetNBTData();
                    comp.Add("Slot", new TagNodeByte((byte)i));
                    list.Add(comp);
                }
            }
            return list;
        }

        public override void LoadNBTData(TagNodeList list)
        {
            try
            {
                TagNodeCompound comp; byte slot;
                for (int i = 0; i < list.Count; i++)
                {
                    comp = list[i].ToTagCompound();
                    slot = comp["Slot"].ToTagByte();
                    if (slot < 0 || slot > Size - 1) continue;
                    if (items[slot] == null) items[slot] = Item.Nothing;
                    items[slot].LoadNBTData(comp);
                }
            }
            catch { Server.ServerLogger.Log("NBT data is invalid."); }
        }
    }
}
