﻿using System;

namespace SMP.Generator
{
    public class WorldGenCactus : WorldGenerator
    {
        public WorldGenCactus()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; l < 10; l++)
            {
                int i1 = (i + random.nextInt(8)) - random.nextInt(8);
                int j1 = (j + random.nextInt(4)) - random.nextInt(4);
                int k1 = (k + random.nextInt(8)) - random.nextInt(8);
                if (!world.IsAirBlock(i1, j1, k1))
                {
                    continue;
                }
                int l1 = 1 + random.nextInt(random.nextInt(3) + 1);
                for (int i2 = 0; i2 < l1; i2++)
                {
                    if (world.CanBlockStay((byte)Blocks.Cactus, i1, j1 + i2, k1))
                    {
                        world.SetBlock(i1, j1 + i2, k1, (byte)Blocks.Cactus);
                    }
                }

            }

            return true;
        }
    }
}