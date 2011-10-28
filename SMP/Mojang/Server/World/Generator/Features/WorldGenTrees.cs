using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class WorldGenTrees : WorldGenerator
    {
        public WorldGenTrees()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            int l;
            bool flag;
            label0:
            {
                l = random.nextInt(3) + 4;
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
                for(int i1 = j; i1 <= j + 1 + l; i1++)
                {
                    byte byte0 = 1;
                    if(i1 == j)
                    {
                        byte0 = 0;
                    }
                    if(i1 >= (j + 1 + l) - 2)
                    {
                        byte0 = 2;
                    }
                    for(int i2 = i - byte0; i2 <= i + byte0 && flag; i2++)
                    {
                        for(int l2 = k - byte0; l2 <= k + byte0 && flag; l2++)
                        {
                            if(i1 >= 0)
                            {
                                if(i1 < 128)
                                {
                                    int j3 = world.GetBlock(i2, i1, l2);
                                    if (j3 != 0 && j3 != (byte)Blocks.Leaves)
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
                int j1 = world.GetBlock(i, j - 1, k);
                bool label1 = false;
                if (j1 == (byte)Blocks.Grass || j1 == (byte)Blocks.Dirt)
                {
                    if(j < 128 - l - 1)
                    {
                        label1 = true;
                    }
                }
                if (!label1) return false;
            }
            world.SetBlock(i, j - 1, k, (byte)Blocks.Dirt);
            for(int k1 = (j - 3) + l; k1 <= j + l; k1++)
            {
                int j2 = k1 - (j + l);
                int i3 = 1 - j2 / 2;
                for(int k3 = i - i3; k3 <= i + i3; k3++)
                {
                    int l3 = k3 - i;
                    for(int i4 = k - i3; i4 <= k + i3; i4++)
                    {
                        int j4 = i4 - k;
                        if ((java.lang.Math.abs(l3) != i3 || java.lang.Math.abs(j4) != i3 || random.nextInt(2) != 0 && j2 != 0) && Chunk.LightOpacity[world.GetBlock(k3, k1, i4)] < 0xf)
                        {
                            world.SetBlock(k3, k1, i4, (byte)Blocks.Leaves);
                        }
                    }

                }

            }

            for(int l1 = 0; l1 < l; l1++)
            {
                int k2 = world.GetBlock(i, j + l1, k);
                if (k2 == 0 || k2 == (byte)Blocks.Leaves)
                {
                    world.SetBlock(i, j + l1, k, (byte)Blocks.Wood);
                }
            }

            return true;
        }
    }
}
