using System;

namespace SMP.Generator
{
    public class WorldGenSwamp : WorldGenerator
    {
        public WorldGenSwamp()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            int l;
            bool flag;
            label0:
            {
                l = random.nextInt(4) + 5;
                for(; BlockData.BlockMaterial(world.GetBlock(i, j - 1, k)) == Material.Water; j--) { }
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
                        byte0 = 3;
                    }
                    for(int j2 = i - byte0; j2 <= i + byte0 && flag; j2++)
                    {
                        for(int j3 = k - byte0; j3 <= k + byte0 && flag; j3++)
                        {
                            if(i1 >= 0)
                            {
                                if(i1 < 128)
                                {
                                    int i4 = world.GetBlock(j2, i1, j3);
                                    if(i4 == 0 || i4 == (byte)Blocks.Leaves)
                                    {
                                        continue;
                                    }
                                    if(i4 == (byte)Blocks.SWater || i4 == (byte)Blocks.AWater)
                                    {
                                        if(i1 > j)
                                        {
                                            flag = false;
                                        }
                                    } else
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
                if(j1 == (byte)Blocks.Grass || j1 == (byte)Blocks.Dirt)
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
                int k2 = k1 - (j + l);
                int k3 = 2 - k2 / 2;
                for(int j4 = i - k3; j4 <= i + k3; j4++)
                {
                    int l4 = j4 - i;
                    for(int j5 = k - k3; j5 <= k + k3; j5++)
                    {
                        int k5 = j5 - k;
                        if ((java.lang.Math.abs(l4) != k3 || java.lang.Math.abs(k5) != k3 || random.nextInt(2) != 0 && k2 != 0) && Chunk.LightOpacity[world.GetBlock(j4, k1, j5)] < 0xf)
                        {
                            world.SetBlock(j4, k1, j5, (byte)Blocks.Leaves);
                        }
                    }

                }

            }

            for(int l1 = 0; l1 < l; l1++)
            {
                int l2 = world.GetBlock(i, j + l1, k);
                if(l2 == 0 || l2 == (byte)Blocks.Leaves || l2 == (byte)Blocks.AWater || l2 == (byte)Blocks.SWater)
                {
                    world.SetBlock(i, j + l1, k, (byte)Blocks.Wood);
                }
            }

            for(int i2 = (j - 3) + l; i2 <= j + l; i2++)
            {
                int i3 = i2 - (j + l);
                int l3 = 2 - i3 / 2;
                for(int k4 = i - l3; k4 <= i + l3; k4++)
                {
                    for(int i5 = k - l3; i5 <= k + l3; i5++)
                    {
                        if(world.GetBlock(k4, i2, i5) != (byte)Blocks.Leaves)
                        {
                            continue;
                        }
                        if(random.nextInt(4) == 0 && world.GetBlock(k4 - 1, i2, i5) == 0)
                        {
                            func_35292_a(world, k4 - 1, i2, i5, 8);
                        }
                        if(random.nextInt(4) == 0 && world.GetBlock(k4 + 1, i2, i5) == 0)
                        {
                            func_35292_a(world, k4 + 1, i2, i5, 2);
                        }
                        if(random.nextInt(4) == 0 && world.GetBlock(k4, i2, i5 - 1) == 0)
                        {
                            func_35292_a(world, k4, i2, i5 - 1, 1);
                        }
                        if(random.nextInt(4) == 0 && world.GetBlock(k4, i2, i5 + 1) == 0)
                        {
                            func_35292_a(world, k4, i2, i5 + 1, 4);
                        }
                    }

                }

            }

            return true;
        }

        private void func_35292_a(World world, int i, int j, int k, int l)
        {
            world.SetBlock(i, j, k, (byte)Blocks.Vines, (byte)l);
            for (int i1 = 4; world.GetBlock(i, --j, k) == 0 && i1 > 0; i1--)
            {
                world.SetBlock(i, j, k, (byte)Blocks.Vines, (byte)l);
            }

        }
    }
}
