using System;

namespace SMP
{
    public class WorldGenClay : WorldGenerator
    {
        public WorldGenClay(int i)
        {
            clayBlockId = (byte)Blocks.ClayBlock;
            numberOfBlocks = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            if (BlockData.BlockMaterial(world.GetBlock(i, j, k)) != Material.Water)
            {
                return false;
            }
            int l = random.nextInt(numberOfBlocks - 2) + 2;
            int i1 = 1;
            for (int j1 = i - l; j1 <= i + l; j1++)
            {
                for (int k1 = k - l; k1 <= k + l; k1++)
                {
                    int l1 = j1 - i;
                    int i2 = k1 - k;
                    if (l1 * l1 + i2 * i2 > l * l)
                    {
                        continue;
                    }
                    for (int j2 = j - i1; j2 <= j + i1; j2++)
                    {
                        int k2 = world.GetBlock(j1, j2, k1);
                        if (k2 == (byte)Blocks.Dirt || k2 == (byte)Blocks.ClayBlock)
                        {
                            world.SetBlock(j1, j2, k1, (byte)clayBlockId);
                        }
                    }

                }

            }

            return true;
        }

        private int clayBlockId;
        private int numberOfBlocks;
    }
}
