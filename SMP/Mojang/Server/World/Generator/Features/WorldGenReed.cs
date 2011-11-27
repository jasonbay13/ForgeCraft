using System;

namespace SMP.Generator
{
    public class WorldGenReed : WorldGenerator
    {
        public WorldGenReed()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; l < 20; l++)
            {
                int i1 = (i + random.nextInt(4)) - random.nextInt(4);
                int j1 = j;
                int k1 = (k + random.nextInt(4)) - random.nextInt(4);
                if (!world.IsAirBlock(i1, j1, k1) || BlockData.BlockMaterial(world.GetBlock(i1 - 1, j1 - 1, k1)) != Material.Water && BlockData.BlockMaterial(world.GetBlock(i1 + 1, j1 - 1, k1)) != Material.Water && BlockData.BlockMaterial(world.GetBlock(i1, j1 - 1, k1 - 1)) != Material.Water && BlockData.BlockMaterial(world.GetBlock(i1, j1 - 1, k1 + 1)) != Material.Water)
                {
                    continue;
                }
                int l1 = 2 + random.nextInt(random.nextInt(3) + 1);
                for (int i2 = 0; i2 < l1; i2++)
                {
                    if (world.CanBlockStay((byte)Blocks.SugarCane, i1, j1 + i2, k1))
                    {
                        world.SetBlock(i1, j1 + i2, k1, (byte)Blocks.SugarCane);
                    }
                }

            }

            return true;
        }
    }
}
