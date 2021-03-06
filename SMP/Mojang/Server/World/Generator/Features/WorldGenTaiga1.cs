﻿using System;

namespace SMP.Generator
{
    public class WorldGenTaiga1 : WorldGenerator
    {
        public WorldGenTaiga1()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            int l;
            int i1;
            int k1;
            bool flag;
            label0:
            {
                l = random.nextInt(5) + 7;
                i1 = l - random.nextInt(2) - 3;
                int j1 = l - i1;
                k1 = 1 + random.nextInt(j1 + 1);
                flag = true;
                bool label0 = false;
                if(j >= 1)
                {
                    if(j + l + 1 <= 128)
                    {
                        label0 = true;
                    }
                }
                if (!label0) return false;
            }
            label1:
            {
                for(int l1 = j; l1 <= j + 1 + l && flag; l1++)
                {
                    int j2 = 1;
                    if(l1 - j < i1)
                    {
                        j2 = 0;
                    } else
                    {
                        j2 = k1;
                    }
                    for(int l2 = i - j2; l2 <= i + j2 && flag; l2++)
                    {
                        for(int k3 = k - j2; k3 <= k + j2 && flag; k3++)
                        {
                            if(l1 >= 0)
                            {
                                if(l1 < 128)
                                {
                                    int j4 = world.GetBlock(l2, l1, k3);
                                    if (j4 != 0 && j4 != (byte)Blocks.Leaves)
                                    {
                                        flag = false;
                                    }
                                    continue;
                                }
                            }
                            flag = false;
                        }

                    }

                }

                if(!flag)
                {
                    return false;
                }
                int i2 = world.GetBlock(i, j - 1, k);
                bool label1 = false;
                if (i2 == (byte)Blocks.Grass || i2 == (byte)Blocks.Dirt)
                {
                    if(j < 128 - l - 1)
                    {
                        label1 = true;
                    }
                }
                if (!label1) return false;
            }
            world.SetBlock(i, j - 1, k, (byte)Blocks.Dirt);
            int k2 = 0;
            for(int i3 = j + l; i3 >= j + i1; i3--)
            {
                for(int l3 = i - k2; l3 <= i + k2; l3++)
                {
                    int k4 = l3 - i;
                    for(int l4 = k - k2; l4 <= k + k2; l4++)
                    {
                        int i5 = l4 - k;
                        if ((java.lang.Math.abs(k4) != k2 || java.lang.Math.abs(i5) != k2 || k2 <= 0) && Chunk.LightOpacity[world.GetBlock(l3, i3, l4)] < 0xf)
                        {
                            world.SetBlock(l3, i3, l4, (byte)Blocks.Leaves, 1);
                        }
                    }

                }

                if(k2 >= 1 && i3 == j + i1 + 1)
                {
                    k2--;
                    continue;
                }
                if(k2 < k1)
                {
                    k2++;
                }
            }

            for(int j3 = 0; j3 < l - 1; j3++)
            {
                int i4 = world.GetBlock(i, j + j3, k);
                if (i4 == 0 || i4 == (byte)Blocks.Leaves)
                {
                    world.SetBlock(i, j + j3, k, (byte)Blocks.Wood, 1);
                }
            }

            return true;
        }
    }
}
