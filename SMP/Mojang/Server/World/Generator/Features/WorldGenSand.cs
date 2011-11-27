using System;

namespace SMP.Generator
{
    public class WorldGenSand : WorldGenerator
    {
        public WorldGenSand(int i, int j)
        {
            field_35291_a = j;
            field_35290_b = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            if (BlockData.BlockMaterial(world.GetBlock(i, j, k)) != Material.Water)
            {
                return false;
            }
            int l = random.nextInt(field_35290_b - 2) + 2;
            byte byte0 = 2;
            for (int i1 = i - l; i1 <= i + l; i1++)
            {
                for (int j1 = k - l; j1 <= k + l; j1++)
                {
                    int k1 = i1 - i;
                    int l1 = j1 - k;
                    if (k1 * k1 + l1 * l1 > l * l)
                    {
                        continue;
                    }
                    for (int i2 = j - byte0; i2 <= j + byte0; i2++)
                    {
                        int j2 = world.GetBlock(i1, i2, j1);
                        if (j2 == (byte)Blocks.Dirt || j2 == (byte)Blocks.Grass)
                        {
                            world.SetBlock(i1, i2, j1, (byte)field_35291_a);
                        }
                    }

                }

            }

            return true;
        }

        private int field_35291_a;
        private int field_35290_b;
    }
}
