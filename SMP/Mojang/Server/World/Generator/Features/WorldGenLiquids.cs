using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class WorldGenLiquids : WorldGenerator
    {
        public WorldGenLiquids(int i)
        {
            liquidBlockId = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            if (world.GetBlock(i, j + 1, k) != (byte)Blocks.Stone)
            {
                return false;
            }
            if (world.GetBlock(i, j - 1, k) != (byte)Blocks.Stone)
            {
                return false;
            }
            if (world.GetBlock(i, j, k) != 0 && world.GetBlock(i, j, k) != (byte)Blocks.Stone)
            {
                return false;
            }
            int l = 0;
            if (world.GetBlock(i - 1, j, k) == (byte)Blocks.Stone)
            {
                l++;
            }
            if (world.GetBlock(i + 1, j, k) == (byte)Blocks.Stone)
            {
                l++;
            }
            if (world.GetBlock(i, j, k - 1) == (byte)Blocks.Stone)
            {
                l++;
            }
            if (world.GetBlock(i, j, k + 1) == (byte)Blocks.Stone)
            {
                l++;
            }
            int i1 = 0;
            if (world.IsAirBlock(i - 1, j, k))
            {
                i1++;
            }
            if (world.IsAirBlock(i + 1, j, k))
            {
                i1++;
            }
            if (world.IsAirBlock(i, j, k - 1))
            {
                i1++;
            }
            if (world.IsAirBlock(i, j, k + 1))
            {
                i1++;
            }
            if (l == 3 && i1 == 1)
            {
                world.SetBlock(i, j, k, (byte)liquidBlockId);
                //world.physics.AddCheck(i, j, k);
                //world.scheduledUpdatesAreImmediate = true;
                //Block.blocksList[liquidBlockId].updateTick(world, i, j, k, random);
                //world.scheduledUpdatesAreImmediate = false;
            }
            return true;
        }

        private int liquidBlockId;
    }
}
