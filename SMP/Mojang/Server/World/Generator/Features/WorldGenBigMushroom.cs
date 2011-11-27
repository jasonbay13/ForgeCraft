using System;

namespace SMP.Generator
{
    public class WorldGenBigMushroom : WorldGenerator
    {
        public WorldGenBigMushroom(int i)
        {
            mushroomType = -1;
            mushroomType = i;
        }

        public WorldGenBigMushroom()
        {
            mushroomType = -1;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            int l;
            int i1;
            bool flag;
            label0:
            {
                l = random.nextInt(2);
                if(mushroomType >= 0)
                {
                    l = mushroomType;
                }
                i1 = random.nextInt(3) + 4;
                flag = true;
                bool label0 = false;
                if(j >= 1)
                {
                    if (j + i1 + 1 <= world.worldYMax)
                    {
                        label0 = true;
                    }
                }
                if (!label0) return false;
            }
            for(int j1 = j; j1 <= j + 1 + i1; j1++)
            {
                byte byte0 = 3;
                if(j1 == j)
                {
                    byte0 = 0;
                }
                for(int j2 = i - byte0; j2 <= i + byte0 && flag; j2++)
                {
                    for(int i3 = k - byte0; i3 <= k + byte0 && flag; i3++)
                    {
                        if(j1 >= 0)
                        {
                            if(j1 < 128)
                            {
                                int k3 = world.GetBlock(j2, j1, i3);
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
            int k1 = world.GetBlock(i, j - 1, k);
            if (k1 != (byte)Blocks.Dirt && k1 != (byte)Blocks.Grass && k1 != (byte)Blocks.Mycelium)
            {
                return false;
            }
            if (!world.CanPlaceAt((byte)Blocks.MushroomBrown, i, j, k))
            {
                return false;
            }
            world.SetBlock(i, j - 1, k, (byte)Blocks.Dirt);
            int l1 = j + i1;
            if (l == 1)
            {
                l1 = (j + i1) - 3;
            }
            for (int j2 = l1; j2 <= j + i1; j2++)
            {
                int i3 = 1;
                if (j2 < j + i1)
                {
                    i3++;
                }
                if (l == 0)
                {
                    i3 = 3;
                }
                for (int l3 = i - i3; l3 <= i + i3; l3++)
                {
                    for (int i4 = k - i3; i4 <= k + i3; i4++)
                    {
                        int j4 = 5;
                        if (l3 == i - i3)
                        {
                            j4--;
                        }
                        if (l3 == i + i3)
                        {
                            j4++;
                        }
                        if (i4 == k - i3)
                        {
                            j4 -= 3;
                        }
                        if (i4 == k + i3)
                        {
                            j4 += 3;
                        }
                        if (l == 0 || j2 < j + i1)
                        {
                            if ((l3 == i - i3 || l3 == i + i3) && (i4 == k - i3 || i4 == k + i3))
                            {
                                continue;
                            }
                            if (l3 == i - (i3 - 1) && i4 == k - i3)
                            {
                                j4 = 1;
                            }
                            if (l3 == i - i3 && i4 == k - (i3 - 1))
                            {
                                j4 = 1;
                            }
                            if (l3 == i + (i3 - 1) && i4 == k - i3)
                            {
                                j4 = 3;
                            }
                            if (l3 == i + i3 && i4 == k - (i3 - 1))
                            {
                                j4 = 3;
                            }
                            if (l3 == i - (i3 - 1) && i4 == k + i3)
                            {
                                j4 = 7;
                            }
                            if (l3 == i - i3 && i4 == k + (i3 - 1))
                            {
                                j4 = 7;
                            }
                            if (l3 == i + (i3 - 1) && i4 == k + i3)
                            {
                                j4 = 9;
                            }
                            if (l3 == i + i3 && i4 == k + (i3 - 1))
                            {
                                j4 = 9;
                            }
                        }
                        if (j4 == 5 && j2 < j + i1)
                        {
                            j4 = 0;
                        }
                        if ((j4 != 0 || j >= (j + i1) - 1) && Chunk.LightOpacity[world.GetBlock(l3, j2, i4)] < 0xf)
                        {
                            world.SetBlock(l3, j2, i4, (byte)((byte)Blocks.HugeBrownMushroom + l), (byte)j4);
                        }
                    }

                }

            }

            for (int k2 = 0; k2 < i1; k2++)
            {
                int j3 = world.GetBlock(i, j + k2, k);
                if (Chunk.LightOpacity[j3] < 0xf)
                {
                    world.SetBlock(i, j + k2, k, (byte)((byte)Blocks.HugeBrownMushroom + l), 10);
                }
            }

            return true;
        }

        private int mushroomType;
    }
}
