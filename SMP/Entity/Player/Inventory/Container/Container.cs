using System;
using System.Collections.Generic;
using Substrate.Nbt;
using SMP.PLAYER.Crafting;
using SMP.PLAYER;

namespace SMP.INVENTORY
{
    public abstract class Container
    {
        public abstract ContainerType Type { get; }
        public abstract int Size { get; }
        public abstract Point3 Pos { get; }
        public virtual bool Open { get { return false; } }
        internal abstract Item[] Items { get; }
        public abstract Item GetSlot(int slot);
        public abstract void SetSlot(int slot, Item item);
        public abstract TagNodeList GetNBTData();
        public abstract void LoadNBTData(TagNodeList list);

        private static int nextId = 0;
        public int id; // Used to uniquely identify a container.
        public World level;


        public Container(World level)
        {
            id = FreeId();
            this.level = level;
        }

        public static Container CreateInstance(ContainerType type, World level, Point3 point)
        {
            switch (type)
            {
                case ContainerType.Chest:
                    return new ContainerChest(level, point);
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

        public virtual void AddPlayer(Player p)
        {
        }

        public virtual void RemovePlayer(Player p)
        {
        }

        public virtual void UpdateState()
        {
        }

        public virtual void UpdateContents(Player exclude = null)
        {
        }

        private static int FreeId()
        {
            return nextId++;
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
