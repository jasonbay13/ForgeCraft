using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public partial class Physics
    {
        public void BlockUpdate(int x, int y, int z, byte oldBlock, byte oldMeta)
        {
            if (BlockData.HasPhysics(w.GetBlock(x, y, z))) AddCheck(x, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x + 1, y, z))) AddCheck(x + 1, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x - 1, y, z))) AddCheck(x - 1, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y + 1, z))) AddCheck(x, y + 1, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y - 1, z))) AddCheck(x, y - 1, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y, z + 1))) AddCheck(x, y, z + 1);
            if (BlockData.HasPhysics(w.GetBlock(x, y, z - 1))) AddCheck(x, y, z - 1);

            // Extra stuff should be added for specific blocks.
            if (oldBlock == 19)
                SpongeRemoved(x, y, z);
        }

        #region Liquids
        public bool LiquidSourceCheck()
        {
            return false;
        }

        public bool AdjacentLiquidCheck(int x, int y, int z, byte type, bool above = true, bool below = false)
        {
            return (w.GetBlock(x + 1, y, z) == type || w.GetBlock(x - 1, y, z) == type || w.GetBlock(x, y, z + 1) == type || w.GetBlock(x, y, z - 1) == type || (above && w.GetBlock(x, y + 1, z) == type) || (below && w.GetBlock(x, y, z) == type));
        }

        public bool WaterFlowCheck(int x, int y, int z)
        {
            byte block = w.GetBlock(x, y, z);
            return block == 0 || block == 8 || block == 9 || BlockData.LiquidDestroy(block);
        }
        public bool LavaFlowCheck(int x, int y, int z)
        {
            byte block = w.GetBlock(x, y, z);
            return block == 0 || block == 10 || block == 11 || BlockData.LiquidDestroy(block);
        }

        public void WaterFlow(int x, int y, int z, byte meta)
        {
            if (SpongeCheck(x, y, z)) return;
            byte block = w.GetBlock(x, y, z);
            byte bMeta = w.GetMeta(x, y, z);
            if (block == 0 || block == 51 || ((block == 8 || block == 9) && (meta.GetBits(3, 1) != 0 || meta.GetBits(0, 3) < bMeta)))
            {
                w.BlockChange(x, y, z, 8, meta);
                if (block == 51)
                    Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
            }
            else if (BlockData.LiquidDestroy(block))
            {
                short dropId = Player.BlockDropSwitch(block);
                if (dropId != 0)
                {
                    Item item = new Item(dropId, w) { count = 1, meta = w.GetMeta(x, y, z), pos = new double[3] { x + .5, y + .5, z + .5 }, rot = new byte[3] { 1, 1, 1 }, OnGround = true };
                    item.e.UpdateChunks(false, false);
                }

                w.BlockChange(x, y, z, 8, meta);
            }
            else if (block == 10 || block == 11)
            {
                if (bMeta.GetBits(0, 3) == 0)
                    w.BlockChange(x, y, z, 49, 0);
                else
                    w.BlockChange(x, y, z, 4, 0);
                Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
                Player.GlobalSoundEffect(x, (byte)y, z, 2000, 4, w);
            }
        }

        public void LavaFlow(int x, int y, int z, byte meta)
        {
            byte block = w.GetBlock(x, y, z);
            byte bMeta = w.GetMeta(x, y, z);
            if (block == 0 || block == 51 || ((block == 10 || block == 11) && (meta.GetBits(3, 1) != 0 || meta.GetBits(0, 3) < bMeta)))
            {
                w.BlockChange(x, y, z, 10, meta);
                if (block == 51)
                    Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
            }
            else if (BlockData.LiquidDestroy(block))
            {
                short dropId = Player.BlockDropSwitch(block);
                if (dropId != 0)
                {
                    Item item = new Item(dropId, w) { count = 1, meta = w.GetMeta(x, y, z), pos = new double[3] { x + .5, y + .5, z + .5 }, rot = new byte[3] { 1, 1, 1 }, OnGround = true };
                    item.e.UpdateChunks(false, false);
                }

                w.BlockChange(x, y, z, 10, meta);
            }
        }
        #endregion
        #region Sponge
        public void SpongePlaced(int x, int y, int z)
        {
            byte block;
            for (int xx = -2; xx <= 2; ++xx)
                for (int yy = -2; yy <= 2; ++yy)
                    for (int zz = -2; zz <= 2; ++zz)
                    {
                        block = w.GetBlock(x + xx, y + yy, z + zz);
                        if (block == 8 || block == 9) w.BlockChange(x + xx, y + yy, z + zz, 0, 0);
                    }
        }

        public void SpongeRemoved(int x, int y, int z)
        {
            byte block;
            for (int xx = -3; xx <= 3; ++xx)
                for (int yy = -3; yy <= 3; ++yy)
                    for (int zz = -3; zz <= 3; ++zz)
                    {
                        block = w.GetBlock(x + xx, y + yy, z + zz);
                        if (block == 8 || block == 9) AddCheck(x + xx, y + yy, z + zz);
                    }
        }

        public bool SpongeCheck(int x, int y, int z)
        {
            for (int xx = -2; xx <= 2; ++xx)
                for (int yy = -2; yy <= 2; ++yy)
                    for (int zz = -2; zz <= 2; ++zz)
                        if (w.GetBlock(x + xx, y + yy, z + zz) == 19) return true;
            return false;
        }
        #endregion
    }
}
