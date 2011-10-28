using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class WorldGenTaiga2 : WorldGenerator
    {
        public WorldGenTaiga2()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            int l;
            int i1;
            int j1;
            int k1;
            bool flag;
            label0:
            {
                l = random.nextInt(4) + 6;
                i1 = 1 + random.nextInt(2);
                j1 = l - i1;
                k1 = 2 + random.nextInt(2);
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
                        for(int j3 = k - j2; j3 <= k + j2 && flag; j3++)
                        {
                            if(l1 >= 0)
                            {
                                if(l1 < 128)
                                {
                                    int k3 = world.GetBlock(l2, l1, j3);
                                    if (k3 != 0 && k3 != (byte)Blocks.Leaves)
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
            int k2 = random.nextInt(2);
            int i3 = 1;
            bool flag1 = false;
            for(int l3 = 0; l3 <= j1; l3++)
            {
                int j4 = (j + l) - l3;
                for(int l4 = i - k2; l4 <= i + k2; l4++)
                {
                    int j5 = l4 - i;
                    for(int k5 = k - k2; k5 <= k + k2; k5++)
                    {
                        int l5 = k5 - k;
                        if ((java.lang.Math.abs(j5) != k2 || java.lang.Math.abs(l5) != k2 || k2 <= 0) && Chunk.LightOpacity[world.GetBlock(l4, j4, k5)] < 0xf)
                        {
                            world.SetBlock(l4, j4, k5, (byte)Blocks.Leaves, 1);
                        }
                    }

                }

                if(k2 >= i3)
                {
                    k2 = ((flag1) ? 1 : 0);
                    flag1 = true;
                    if(++i3 > k1)
                    {
                        i3 = k1;
                    }
                } else
                {
                    k2++;
                }
            }

            int i4 = random.nextInt(3);
            for(int k4 = 0; k4 < l - i4; k4++)
            {
                int i5 = world.GetBlock(i, j + k4, k);
                if (i5 == 0 || i5 == (byte)Blocks.Leaves)
                {
                    world.SetBlock(i, j + k4, k, (byte)Blocks.Wood, 1);
                }
            }

            return true;
        }
    }
}
