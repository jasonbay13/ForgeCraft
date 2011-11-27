using System;

namespace SMP.Generator
{
    public class WorldGenHellLava : WorldGenerator
    {
        private int field_4250_a;

        public WorldGenHellLava(int i)
        {
            field_4250_a = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            if (world.GetBlock(i, j + 1, k) != (byte)Blocks.Netherrack)
            {
                return false;
            }
            if (world.GetBlock(i, j, k) != 0 && world.GetBlock(i, j, k) != (byte)Blocks.Netherrack)
            {
                return false;
            }
            int l = 0;
            if (world.GetBlock(i - 1, j, k) == (byte)Blocks.Netherrack)
            {
                l++;
            }
            if (world.GetBlock(i + 1, j, k) == (byte)Blocks.Netherrack)
            {
                l++;
            }
            if (world.GetBlock(i, j, k - 1) == (byte)Blocks.Netherrack)
            {
                l++;
            }
            if (world.GetBlock(i, j, k + 1) == (byte)Blocks.Netherrack)
            {
                l++;
            }
            if (world.GetBlock(i, j - 1, k) == (byte)Blocks.Netherrack)
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
            if (world.IsAirBlock(i, j - 1, k))
            {
                i1++;
            }
            if (l == 4 && i1 == 1)
            {
                world.SetBlock(i, j, k, (byte)field_4250_a);
                world.physics.AddCheck(i, j, k);
                //Block.blocksList[field_4250_a].updateTick(world, i, j, k, random);
            }
            return true;
        }
    }
}
