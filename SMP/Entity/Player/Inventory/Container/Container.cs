using System;
using Substrate.Nbt;

namespace SMP
{
    public abstract class Container
    {
        public abstract ContainerType Type { get; }
        public abstract int Size { get; }
        public abstract Point3 Pos { get; }
        internal abstract Item[] Items { get; }
        public abstract Item GetSlot(int slot);
        public abstract void SetSlot(int slot, Item item);
        public abstract TagNodeList GetNBTData();
        public abstract void LoadNBTData(TagNodeList list);


        public Container()
        {
        }

        public Container(Point3 point)
        {
        }

        public Container(Point3 point, Item[] items)
        {
        }

        public static Container CreateInstance(ContainerType type, Point3 point)
        {
            switch (type)
            {
                case ContainerType.Chest:
                    return new ContainerChest(point);
                case ContainerType.Furnace:
                    // TODO
                    break;
                case ContainerType.Dispenser:
                    // TODO
                    break;
                case ContainerType.BrewingStand:
                    // TODO
                    break;
            }
            return null;
        }

        public void SetSlot(int slot, short id)
        {
            SetSlot(slot, id, 1, 0);
        }

        public void SetSlot(int slot, short id, byte count)
        {
            SetSlot(slot, id, count, 0);
        }

        public void SetSlot(int slot, short id, byte count, short meta)
        {
            SetSlot(slot, new Item(id, count, meta));
        }
    }

    public enum ContainerType : byte
    {
        Chest = 0,
        Furnace = 1,
        Dispenser = 2,
        BrewingStand = 3
    }
}
