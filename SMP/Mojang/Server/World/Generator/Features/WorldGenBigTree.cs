﻿using System;

namespace SMP.Generator
{
    public class WorldGenBigTree : WorldGenerator
    {
        public WorldGenBigTree(bool flag)
            : base(flag)
        {
            random0 = new java.util.Random();
            heightLimit = 0;
            heightAttenuation = 0.61799999999999999D;
            field_753_h = 1.0D;
            field_752_i = 0.38100000000000001D;
            field_751_j = 1.0D;
            field_750_k = 1.0D;
            trunkSize = 1;
            heightLimitLimit = 12;
            leafDistanceLimit = 4;
        }

        void generateLeafNodeList()
        {
            height = (int)((double)heightLimit * heightAttenuation);
            if(height >= heightLimit)
            {
                height = heightLimit - 1;
            }
            int i = (int)(1.3819999999999999D + java.lang.Math.pow((field_750_k * (double)heightLimit) / 13D, 2D));
            if(i < 1)
            {
                i = 1;
            }
            int[][] ai = new int[i * heightLimit][];
            for (int c = 0; c < ai.Length; c++) ai[c] = new int[4];
            int j = (basePos[1] + heightLimit) - leafDistanceLimit;
            int k = 1;
            int l = basePos[1] + height;
            int i1 = j - basePos[1];
            ai[0][0] = basePos[0];
            ai[0][1] = j;
            ai[0][2] = basePos[2];
            ai[0][3] = l;
            j--;
            while(i1 >= 0) 
            {
                int j1 = 0;
                float f = func_431_a(i1);
                if(f < 0.0F)
                {
                    j--;
                    i1--;
                } else
                {
                    double d = 0.5D;
                    for(; j1 < i; j1++)
                    {
                        double d1 = field_751_j * ((double)f * ((double)random0.nextFloat() + 0.32800000000000001D));
                        double d2 = (double)random0.nextFloat() * 2D * 3.1415899999999999D;
                        int k1 = MathHelper.floor_double(d1 * java.lang.Math.sin(d2) + (double)basePos[0] + d);
                        int l1 = MathHelper.floor_double(d1 * java.lang.Math.cos(d2) + (double)basePos[2] + d);
                        int[] ai1 = {
                            k1, j, l1
                        };
                        int[] ai2 = {
                            k1, j + leafDistanceLimit, l1
                        };
                        if(checkBlockLine(ai1, ai2) != -1)
                        {
                            continue;
                        }
                        int[] ai3 = {
                            basePos[0], basePos[1], basePos[2]
                        };
                        double d3 = java.lang.Math.sqrt(java.lang.Math.pow(java.lang.Math.abs(basePos[0] - ai1[0]), 2D) + java.lang.Math.pow(java.lang.Math.abs(basePos[2] - ai1[2]), 2D));
                        double d4 = d3 * field_752_i;
                        if((double)ai1[1] - d4 > (double)l)
                        {
                            ai3[1] = l;
                        } else
                        {
                            ai3[1] = (int)((double)ai1[1] - d4);
                        }
                        if(checkBlockLine(ai3, ai1) == -1)
                        {
                            ai[k][0] = k1;
                            ai[k][1] = j;
                            ai[k][2] = l1;
                            ai[k][3] = ai3[1];
                            k++;
                        }
                    }

                    j--;
                    i1--;
                }
            }
            leafNodes = new int[k][];
            Array.Copy(ai, 0, leafNodes, 0, k);
        }

        void func_426_a(int i, int j, int k, float f, byte byte0, int l)
        {
            int i1 = (int)((double)f + 0.61799999999999999D);
            byte byte1 = otherCoordPairs[byte0];
            byte byte2 = otherCoordPairs[byte0 + 3];
            int[] ai = {
                i, j, k
            };
            int[] ai1 = {
                0, 0, 0
            };
            int j1 = -i1;
            int k1 = -i1;
            ai1[byte0] = ai[byte0];
            for(; j1 <= i1; j1++)
            {
                ai1[byte1] = ai[byte1] + j1;
                for(int l1 = -i1; l1 <= i1;)
                {
                    double d = java.lang.Math.sqrt(java.lang.Math.pow((double)java.lang.Math.abs(j1) + 0.5D, 2D) + java.lang.Math.pow((double)java.lang.Math.abs(l1) + 0.5D, 2D));
                    if(d > (double)f)
                    {
                        l1++;
                    } else
                    {
                        ai1[byte2] = ai[byte2] + l1;
                        int i2 = worldObj.GetBlock(ai1[0], ai1[1], ai1[2]);
                        if(i2 != 0 && i2 != 18)
                        {
                            l1++;
                        } else
                        {
                            func_41043_a(worldObj, ai1[0], ai1[1], ai1[2], (byte)l, 0);
                            l1++;
                        }
                    }
                }

            }

        }

        float func_431_a(int i)
        {
            if((double)i < (double)(float)heightLimit * 0.29999999999999999D)
            {
                return -1.618F;
            }
            float f = (float)heightLimit / 2.0F;
            float f1 = (float)heightLimit / 2.0F - (float)i;
            float f2;
            if(f1 == 0.0F)
            {
                f2 = f;
            } else
            if (java.lang.Math.abs(f1) >= f)
            {
                f2 = 0.0F;
            } else
            {
                f2 = (float)java.lang.Math.sqrt(java.lang.Math.pow(java.lang.Math.abs(f), 2D) - java.lang.Math.pow(java.lang.Math.abs(f1), 2D));
            }
            f2 *= 0.5F;
            return f2;
        }

        float func_429_b(int i)
        {
            if(i < 0 || i >= leafDistanceLimit)
            {
                return -1F;
            }
            return i != 0 && i != leafDistanceLimit - 1 ? 3F : 2.0F;
        }

        void generateLeafNode(int i, int j, int k)
        {
            int l = j;
            for(int i1 = j + leafDistanceLimit; l < i1; l++)
            {
                float f = func_429_b(l - j);
                func_426_a(i, l, k, f, (byte)1, 18);
            }

        }

        void placeBlockLine(int[] ai, int[] ai1, int i)
        {
            int[] ai2 = {
                0, 0, 0
            };
            byte byte0 = 0;
            int j = 0;
            for(; byte0 < 3; byte0++)
            {
                ai2[byte0] = ai1[byte0] - ai[byte0];
                if (java.lang.Math.abs(ai2[byte0]) > java.lang.Math.abs(ai2[j]))
                {
                    j = byte0;
                }
            }

            if(ai2[j] == 0)
            {
                return;
            }
            byte byte1 = otherCoordPairs[j];
            byte byte2 = otherCoordPairs[j + 3];
            sbyte byte3;
            if(ai2[j] > 0)
            {
                byte3 = 1;
            } else
            {
                byte3 = -1;
            }
            double d = (double)ai2[byte1] / (double)ai2[j];
            double d1 = (double)ai2[byte2] / (double)ai2[j];
            int[] ai3 = {
                0, 0, 0
            };
            int k = 0;
            for(int l = ai2[j] + byte3; k != l; k += byte3)
            {
                ai3[j] = MathHelper.floor_double((double)(ai[j] + k) + 0.5D);
                ai3[byte1] = MathHelper.floor_double((double)ai[byte1] + (double)k * d + 0.5D);
                ai3[byte2] = MathHelper.floor_double((double)ai[byte2] + (double)k * d1 + 0.5D);
                func_41043_a(worldObj, ai3[0], ai3[1], ai3[2], (byte)i, 0);
            }

        }

        void generateLeaves()
        {
            int i = 0;
            for(int j = leafNodes.Length; i < j; i++)
            {
                int k = leafNodes[i][0];
                int l = leafNodes[i][1];
                int i1 = leafNodes[i][2];
                generateLeafNode(k, l, i1);
            }

        }

        bool leafNodeNeedsBase(int i)
        {
            return (double)i >= (double)heightLimit * 0.20000000000000001D;
        }

        void generateTrunk()
        {
            int i = basePos[0];
            int j = basePos[1];
            int k = basePos[1] + height;
            int l = basePos[2];
            int[] ai = {
                i, j, l
            };
            int[] ai1 = {
                i, k, l
            };
            placeBlockLine(ai, ai1, 17);
            if(trunkSize == 2)
            {
                ai[0]++;
                ai1[0]++;
                placeBlockLine(ai, ai1, 17);
                ai[2]++;
                ai1[2]++;
                placeBlockLine(ai, ai1, 17);
                ai[0]--;
                ai1[0]--;
                placeBlockLine(ai, ai1, 17);
            }
        }

        void generateLeafNodeBases()
        {
            int i = 0;
            int j = leafNodes.Length;
            int[] ai = {
                basePos[0], basePos[1], basePos[2]
            };
            for(; i < j; i++)
            {
                int[] ai1 = leafNodes[i];
                int[] ai2 = {
                    ai1[0], ai1[1], ai1[2]
                };
                ai[1] = ai1[3];
                int k = ai[1] - basePos[1];
                if(leafNodeNeedsBase(k))
                {
                    placeBlockLine(ai, ai2, 17);
                }
            }

        }

        int checkBlockLine(int[] ai, int[] ai1)
        {
            int[] ai2 = {
                0, 0, 0
            };
            byte byte0 = 0;
            int i = 0;
            for(; byte0 < 3; byte0++)
            {
                ai2[byte0] = ai1[byte0] - ai[byte0];
                if (java.lang.Math.abs(ai2[byte0]) > java.lang.Math.abs(ai2[i]))
                {
                    i = byte0;
                }
            }

            if(ai2[i] == 0)
            {
                return -1;
            }
            byte byte1 = otherCoordPairs[i];
            byte byte2 = otherCoordPairs[i + 3];
            sbyte byte3;
            if(ai2[i] > 0)
            {
                byte3 = 1;
            } else
            {
                byte3 = -1;
            }
            double d = (double)ai2[byte1] / (double)ai2[i];
            double d1 = (double)ai2[byte2] / (double)ai2[i];
            int[] ai3 = {
                0, 0, 0
            };
            int j = 0;
            int k = ai2[i] + byte3;
            do
            {
                if(j == k)
                {
                    break;
                }
                ai3[i] = ai[i] + j;
                ai3[byte1] = MathHelper.floor_double((double)ai[byte1] + (double)j * d);
                ai3[byte2] = MathHelper.floor_double((double)ai[byte2] + (double)j * d1);
                int l = worldObj.GetBlock(ai3[0], ai3[1], ai3[2]);
                if(l != 0 && l != 18)
                {
                    break;
                }
                j += byte3;
            } while(true);
            if(j == k)
            {
                return -1;
            } else
            {
                return java.lang.Math.abs(j);
            }
        }

        bool validTreeLocation()
        {
            int[] ai = {
                basePos[0], basePos[1], basePos[2]
            };
            int[] ai1 = {
                basePos[0], (basePos[1] + heightLimit) - 1, basePos[2]
            };
            int i = worldObj.GetBlock(basePos[0], basePos[1] - 1, basePos[2]);
            if(i != 2 && i != 3)
            {
                return false;
            }
            int j = checkBlockLine(ai, ai1);
            if(j == -1)
            {
                return true;
            }
            if(j < 6)
            {
                return false;
            } else
            {
                heightLimit = j;
                return true;
            }
        }

        public void func_420_a(double d, double d1, double d2)
        {
            heightLimitLimit = (int)(d * 12D);
            if(d > 0.5D)
            {
                leafDistanceLimit = 5;
            }
            field_751_j = d1;
            field_750_k = d2;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            worldObj = world;
            long l = random.nextLong();
            random0.setSeed(l);
            basePos[0] = i;
            basePos[1] = j;
            basePos[2] = k;
            if(heightLimit == 0)
            {
                heightLimit = 5 + random0.nextInt(heightLimitLimit);
            }
            if(!validTreeLocation())
            {
                return false;
            } else
            {
                generateLeafNodeList();
                generateLeaves();
                generateTrunk();
                generateLeafNodeBases();
                return true;
            }
        }

        static byte[] otherCoordPairs = {
            2, 0, 0, 1, 2, 1
        };
        java.util.Random random0;
        World worldObj;
        int[] basePos = {
            0, 0, 0
        };
        int heightLimit;
        int height;
        double heightAttenuation;
        double field_753_h;
        double field_752_i;
        double field_751_j;
        double field_750_k;
        int trunkSize;
        int heightLimitLimit;
        int leafDistanceLimit;
        int[][] leafNodes;
    }
}
