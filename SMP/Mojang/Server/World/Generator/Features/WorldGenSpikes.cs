﻿using System;

namespace SMP.Generator
{
    public class WorldGenSpikes : WorldGenerator
    {
        private int field_40207_a;

        public WorldGenSpikes(int i)
        {
            field_40207_a = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            if (!world.IsAirBlock(i, j, k) || world.GetBlock(i, j - 1, k) != field_40207_a)
            {
                return false;
            }
            int l = random.nextInt(32) + 6;
            int i1 = random.nextInt(4) + 1;
            for (int j1 = i - i1; j1 <= i + i1; j1++)
            {
                for (int l1 = k - i1; l1 <= k + i1; l1++)
                {
                    int j2 = j1 - i;
                    int l2 = l1 - k;
                    if (j2 * j2 + l2 * l2 <= i1 * i1 + 1 && world.GetBlock(j1, j - 1, l1) != field_40207_a)
                    {
                        return false;
                    }
                }

            }

            for (int k1 = j; k1 < j + l && k1 < world.worldYMax; k1++)
            {
                for (int i2 = i - i1; i2 <= i + i1; i2++)
                {
                    for (int k2 = k - i1; k2 <= k + i1; k2++)
                    {
                        int i3 = i2 - i;
                        int j3 = k2 - k;
                        if (i3 * i3 + j3 * j3 <= i1 * i1 + 1)
                        {
                            world.SetBlock(i2, k1, k2, (byte)Blocks.Obsidian);
                        }
                    }

                }

            }

            // TODO
            /*EntityEnderCrystal entityendercrystal = new EntityEnderCrystal(world);
            entityendercrystal.setLocationAndAngles((float)i + 0.5F, j + l, (float)k + 0.5F, random.nextFloat() * 360F, 0.0F);
            world.entityJoinedWorld(entityendercrystal);*/
            world.SetBlock(i, j + l, k, (byte)Blocks.Bedrock);
            return true;
        }
    }
}
