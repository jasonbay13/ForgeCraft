using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public partial class Physics
    {
        public void BlockUpdate(int x, int y, int z)
        {
            if (BlockData.HasPhysics(w.GetBlock(x, y, z))) AddCheck(x, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x + 1, y, z))) AddCheck(x + 1, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x - 1, y, z))) AddCheck(x - 1, y, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y + 1, z))) AddCheck(x, y + 1, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y - 1, z))) AddCheck(x, y - 1, z);
            if (BlockData.HasPhysics(w.GetBlock(x, y, z + 1))) AddCheck(x, y, z + 1);
            if (BlockData.HasPhysics(w.GetBlock(x, y, z - 1))) AddCheck(x, y, z - 1);
            // Extra stuff should be added for specific blocks.
        }

        #region Liquids
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
            byte block = w.GetBlock(x, y, z);
            if (block == 0 || ((block == 8 || block == 9) && GetHalf(1, meta) == 0x8))
            {
                w.BlockChange(x, y, z, 8, meta);
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
        }

        public void LavaFlow(int x, int y, int z, byte meta)
        {
            byte block = w.GetBlock(x, y, z);
            if (block == 0 || ((block == 10 || block == 11) && GetHalf(1, meta) == 0x8))
            {
                w.BlockChange(x, y, z, 10, meta);
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
    }
}
