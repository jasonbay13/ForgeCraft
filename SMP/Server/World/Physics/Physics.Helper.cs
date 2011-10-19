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
            if (Handlers.handlers.ContainsKey(w.GetBlock(x, y, z))) AddCheck(x, y, z);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x + 1, y, z))) AddCheck(x + 1, y, z);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x - 1, y, z))) AddCheck(x - 1, y, z);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x, y + 1, z))) AddCheck(x, y + 1, z);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x, y - 1, z))) AddCheck(x, y - 1, z);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x, y, z + 1))) AddCheck(x, y, z + 1);
            if (Handlers.handlers.ContainsKey(w.GetBlock(x, y, z - 1))) AddCheck(x, y, z - 1);

            // Extra stuff should be added for specific blocks.
            if (oldBlock == 19)
                SpongeRemoved(x, y, z);
        }

        #region Liquids
        public bool SourceLiquidCheck(int x, int y, int z, byte type, byte meta, bool above = true, bool below = false)
        {
            return ((w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x + 1, y, z) & 0x7) == 0) || (w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x - 1, y, z) & 0x7) == 0) || (w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x, y, z + 1) & 0x7) == 0) || (w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x, y, z - 1) & 0x7) == 0) || (above && w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x, y + 1, z) & 0x7) == 0) || (below && w.GetBlock(x + 1, y, z) == type && (w.GetMeta(x, y, z) & 0x7) == 0));
        }

        public bool HigherLiquidCheck(int x, int y, int z, byte type, byte meta)
        {
            if (w.GetBlock(x, y, z) == type && (w.GetBlock(x, y + 1, z) == type || ((w.GetMeta(x, y, z) & 0x7) == 0 && (w.GetMeta(x, y, z) & 0x8) == 0))) return true;
            if ((w.GetMeta(x, y, z) & 0x8) != 0) return false;
            if (w.GetBlock(x + 1, y, z) == type && ((w.GetMeta(x + 1, y, z) & 0x7) < meta || (w.GetMeta(x + 1, y, z) & 0x8) != 0)) return true;
            if (w.GetBlock(x - 1, y, z) == type && ((w.GetMeta(x - 1, y, z) & 0x7) < meta || (w.GetMeta(x - 1, y, z) & 0x8) != 0)) return true;
            if (w.GetBlock(x, y, z + 1) == type && ((w.GetMeta(x, y, z + 1) & 0x7) < meta || (w.GetMeta(x, y, z + 1) & 0x8) != 0)) return true;
            if (w.GetBlock(x, y, z - 1) == type && ((w.GetMeta(x, y, z - 1) & 0x7) < meta || (w.GetMeta(x, y, z - 1) & 0x8) != 0)) return true;
            if (w.GetBlock(x, y + 1, z) == type && ((w.GetMeta(x, y + 1, z) & 0x7) < meta || (w.GetMeta(x, y + 1, z) & 0x8) != 0)) return true;
            return false;
        }

        public bool AdjacentLiquidCheck(int x, int y, int z, byte type, bool above = true, bool below = false)
        {
            return (w.GetBlock(x + 1, y, z) == type || w.GetBlock(x - 1, y, z) == type || w.GetBlock(x, y, z + 1) == type || w.GetBlock(x, y, z - 1) == type || (above && w.GetBlock(x, y + 1, z) == type) || (below && w.GetBlock(x, y, z) == type));
        }

        public bool WaterFlowCheck(int x, int y, int z)
        {
            byte block = w.GetBlock(x, y, z);
            return block == 0 || block == 51 || block == 8 || block == 9 || BlockData.LiquidDestroy(block);
        }
        public bool LavaFlowCheck(int x, int y, int z)
        {
            byte block = w.GetBlock(x, y, z);
            return block == 0 || block == 51 || block == 10 || block == 11 || BlockData.LiquidDestroy(block);
        }

        public void WaterFlow(int x, int y, int z, byte meta)
        {
            if (SpongeCheck(x, y, z)) return;
            byte block = w.GetBlock(x, y, z);
            byte bMeta = w.GetMeta(x, y, z);
            if (block == 0 || block == 51 || ((block == 8 || block == 9) && (meta & 0x7) < bMeta && (bMeta & 0x8) == 0))
            {
                AddUpdate(x, y, z, 8, meta);
                if (block == 51)
                    Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
            }
            else if (BlockData.LiquidDestroy(block))
            {
                AddUpdate(x, y, z, 8, meta);

                short dropId = Player.BlockDropSwitch(block);
                if (FindBlocks.ValidItem(dropId))
                    w.DropItem(x, y, z, dropId);
            }
            else if (block == 10 || block == 11)
            {
                if ((bMeta & 0x8) != 0) return;
                if ((bMeta & 0x7) == 0)
                    AddUpdate(x, y, z, 49, 0);
                else
                    AddUpdate(x, y, z, 4, 0);
                Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
                //Player.GlobalSoundEffect(x, (byte)y, z, 2000, 4, w);
            }
        }

        public void LavaFlow(int x, int y, int z, byte meta)
        {
            byte block = w.GetBlock(x, y, z);
            byte bMeta = w.GetMeta(x, y, z);
            if (block == 0 || block == 51 || ((block == 10 || block == 11) && (meta & 0x7) < bMeta && (bMeta & 0x8) == 0))
            {
                AddUpdate(x, y, z, 10, meta);
                if (block == 51)
                    Player.GlobalSoundEffect(x, (byte)y, z, 1004, w);
            }
            else if (BlockData.LiquidDestroy(block))
            {
                AddUpdate(x, y, z, 10, meta);
                Player.GlobalSoundEffect(x, (byte)y, z, 2000, 4, w);

                short dropId = Player.BlockDropSwitch(block);
                if (FindBlocks.ValidItem(dropId))
                    w.DropItem(x, y, z, dropId);
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
                        if (block == 8 || block == 9) AddUpdate(x + xx, y + yy, z + zz, 0, 0);
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
