using System;

namespace SMP.Generator {

    /// <summary>
    /// Default terrain generator.
    /// </summary>
    public class GenStandard : ChunkGen {
        private readonly object Lock = new object();
        
        private java.util.Random random;
        public World worldObj;
        private NoiseOctaves field_705_k;
        private NoiseOctaves field_704_l;
        private NoiseOctaves field_703_m;
        private NoiseOctaves field_702_n;
        public NoiseOctaves field_715_a;
        public NoiseOctaves field_714_b;
        //public NoiseOctaves mobSpawnerNoise;

        private MapGenBase caveGenerator;
        //public MapGenStronghold strongholdGenerator;
        //public MapGenVillage villageGenerator;
        //public MapGenMineshaft mineshaftGenerator;
        private MapGenBase ravineGenerator;
        private BiomeGenBase[] biomesForGeneration;

        private readonly bool mapFeaturesEnabled;
        private double[] field_4224_q;
        private double[] stoneNoise;
        double[] field_4229_d;
        double[] field_4228_e;
        double[] field_4227_f;
        double[] field_4226_g;
        double[] field_4225_h;
        float[] field_35561_l;


        public GenStandard(World w, bool flag)
        {
            worldObj = w;
            random = new java.util.Random(w.seed);
            stoneNoise = new double[256];
            caveGenerator = new MapGenCaves();
            //strongholdGenerator = new MapGenStronghold();
            //villageGenerator = new MapGenVillage();
            //mineshaftGenerator = new MapGenMineshaft();
            ravineGenerator = new MapGenRavine();
            //unusedIntArray32x32 = new int[32][32];
            mapFeaturesEnabled = flag;
            field_705_k = new NoiseOctaves(random, 16);
            field_704_l = new NoiseOctaves(random, 16);
            field_703_m = new NoiseOctaves(random, 8);
            field_702_n = new NoiseOctaves(random, 4);
            field_715_a = new NoiseOctaves(random, 10);
            field_714_b = new NoiseOctaves(random, 16);
            //mobSpawnerNoise = new NoiseOctaves(random, 8);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public override void Generate(Chunk c)
        {
            //DateTime start = DateTime.Now;
            //int cx = c.x << 4, cz = c.z << 4;
            //int waterLevel = 64 + 15 / 2 - 4;
            if (worldObj.seed == 0)
            {
                for (int x = 0; x < 16; ++x)
                    for (int z = 0; z < 16; ++z)
                    {
                        for (int y = 0; y < 1; ++y)
                            c.UNCHECKEDPlaceBlock(x, y, z, 0x07);
                        for (int y = 1; y < 50; ++y)
                            c.UNCHECKEDPlaceBlock(x, y, z, 0x01);
                        for (int y = 50; y < 65; ++y)
                            c.UNCHECKEDPlaceBlock(x, y, z, 0x03);
                        c.UNCHECKEDPlaceBlock(x, 65, z, 0x02);
                    }
            }
            else
            {
                lock (this.Lock) // We have to do this otherwise shit gets fucked.
                {
                    random.setSeed((long)c.x * 0x4f9939f508L + (long)c.z * 0x1ef1565bd5L);
                    generateTerrain(c.x, c.z, c.blocks);
                    biomesForGeneration = worldObj.chunkManager.loadBlockGeneratorData(biomesForGeneration, c.x * 16, c.z * 16, 16, 16);
                    replaceBlocksForBiome(c.x, c.z, c.blocks, biomesForGeneration);
                    caveGenerator.generate(worldObj, c.x, c.z, c.blocks);
                    if (mapFeaturesEnabled)
                    {
                        //mineshaftGenerator.generate(this, worldObj, i, j, abyte0);
                        //villageGenerator.generate(this, worldObj, i, j, abyte0);
                        //strongholdGenerator.generate(this, worldObj, i, j, abyte0);
                    }
                    ravineGenerator.generate(worldObj, c.x, c.z, c.blocks);
                    c.GenerateHeightMap();
                }
            }
            //TimeSpan took = DateTime.Now - start;
            //Console.WriteLine(took.TotalMilliseconds);
        }

        public override void Populate(Chunk c)
        {
            if (worldObj.seed != 0)
            {
                lock (this.Lock)
                {
                    //BlockSand.fallInstantly = true;
                    int k = c.x * 16;
                    int l = c.z * 16;
                    BiomeGenBase biomegenbase = worldObj.chunkManager.getBiomeGenAt(k + 16, l + 16);
                    random.setSeed(worldObj.seed);
                    long l1 = (random.nextLong() / 2L) * 2L + 1L;
                    long l2 = (random.nextLong() / 2L) * 2L + 1L;
                    random.setSeed((long)c.x * l1 + (long)c.z * l2 ^ worldObj.seed);
                    bool flag = false;
                    if (mapFeaturesEnabled)
                    {
                        //mineshaftGenerator.func_35532_a(worldObj, rand, i, j);
                        //flag = villageGenerator.func_35532_a(worldObj, rand, i, j);
                        //strongholdGenerator.func_35532_a(worldObj, rand, i, j);
                    }
                    if (!flag && random.nextInt(4) == 0)
                    {
                        int i1 = k + random.nextInt(16) + 8;
                        int i2 = random.nextInt(worldObj.worldYMax);
                        int i3 = l + random.nextInt(16) + 8;
                        (new WorldGenLakes((byte)Blocks.SWater)).generate(worldObj, random, i1, i2, i3);
                    }
                    if (!flag && random.nextInt(8) == 0)
                    {
                        int j1 = k + random.nextInt(16) + 8;
                        int j2 = random.nextInt(random.nextInt(worldObj.worldYMax - 8) + 8);
                        int j3 = l + random.nextInt(16) + 8;
                        if (j2 < 63 || random.nextInt(10) == 0)
                        {
                            (new WorldGenLakes((byte)Blocks.SLava)).generate(worldObj, random, j1, j2, j3);
                        }
                    }
                    for (int k1 = 0; k1 < 8; k1++)
                    {
                        int k2 = k + random.nextInt(16) + 8;
                        int k3 = random.nextInt(worldObj.worldYMax);
                        int l3 = l + random.nextInt(16) + 8;
                        if (!(new WorldGenDungeons()).generate(worldObj, random, k2, k3, l3)) ;
                        //if ((new WorldGenDungeons()).generate(worldObj, random, k2, k3, l3)) Console.WriteLine("Dungeon @ {0},{1},{2}", k2, k3, l3);
                    }

                    biomegenbase.func_35513_a(worldObj, random, k, l);
                    //SpawnerAnimals.func_35573_a(worldObj, biomegenbase, k + 8, l + 8, 16, 16, rand);
                    k += 8;
                    l += 8;
                    for (int i2 = 0; i2 < 16; i2++)
                    {
                        for (int j3 = 0; j3 < 16; j3++)
                        {
                            int j4 = worldObj.GetTopSolidOrLiquidBlock(k + i2, l + j3);
                            if (worldObj.func_40210_p(i2 + k, j4 - 1, j3 + l))
                            {
                                worldObj.SetBlock(i2 + k, j4 - 1, j3 + l, (byte)Blocks.Ice);
                            }
                            if (worldObj.func_40215_r(i2 + k, j4, j3 + l))
                            {
                                worldObj.SetBlock(i2 + k, j4, j3 + l, (byte)Blocks.Snow);
                            }
                        }

                    }

                    //BlockSand.fallInstantly = false;
                }
            }
        }


        public void generateTerrain(int i, int j, byte[] abyte0)
        {
            byte byte0 = 4;
            int k = 128 / 8;
            byte byte1 = 63;
            int l = byte0 + 1;
            int i1 = 128 / 8 + 1;
            int j1 = byte0 + 1;
            biomesForGeneration = worldObj.chunkManager.func_35142_b(biomesForGeneration, i * 4 - 2, j * 4 - 2, l + 5, j1 + 5);
            field_4224_q = func_4058_a(field_4224_q, i * byte0, 0, j * byte0, l, i1, j1);
            for (int k1 = 0; k1 < byte0; k1++)
            {
                for (int l1 = 0; l1 < byte0; l1++)
                {
                    for (int i2 = 0; i2 < k; i2++)
                    {
                        double d = 0.125D;
                        double d1 = field_4224_q[((k1 + 0) * j1 + (l1 + 0)) * i1 + (i2 + 0)];
                        double d2 = field_4224_q[((k1 + 0) * j1 + (l1 + 1)) * i1 + (i2 + 0)];
                        double d3 = field_4224_q[((k1 + 1) * j1 + (l1 + 0)) * i1 + (i2 + 0)];
                        double d4 = field_4224_q[((k1 + 1) * j1 + (l1 + 1)) * i1 + (i2 + 0)];
                        double d5 = (field_4224_q[((k1 + 0) * j1 + (l1 + 0)) * i1 + (i2 + 1)] - d1) * d;
                        double d6 = (field_4224_q[((k1 + 0) * j1 + (l1 + 1)) * i1 + (i2 + 1)] - d2) * d;
                        double d7 = (field_4224_q[((k1 + 1) * j1 + (l1 + 0)) * i1 + (i2 + 1)] - d3) * d;
                        double d8 = (field_4224_q[((k1 + 1) * j1 + (l1 + 1)) * i1 + (i2 + 1)] - d4) * d;
                        for (int j2 = 0; j2 < 8; j2++)
                        {
                            double d9 = 0.25D;
                            double d10 = d1;
                            double d11 = d2;
                            double d12 = (d3 - d1) * d9;
                            double d13 = (d4 - d2) * d9;
                            for (int k2 = 0; k2 < 4; k2++)
                            {
                                int l2 = k2 + k1 * 4 << 11 | 0 + l1 * 4 << 7 | i2 * 8 + j2;
                                int i3 = 1 << 7;
                                double d14 = 0.25D;
                                double d15 = d10;
                                double d16 = (d11 - d10) * d14;
                                for (int j3 = 0; j3 < 4; j3++)
                                {
                                    int k3 = 0;
                                    if (i2 * 8 + j2 < byte1)
                                    {
                                        k3 = (byte)Blocks.SWater;
                                    }
                                    if (d15 > 0.0D)
                                    {
                                        k3 = (byte)Blocks.Stone;
                                    }
                                    abyte0[l2] = (byte)k3;
                                    l2 += i3;
                                    d15 += d16;
                                }

                                d10 += d12;
                                d11 += d13;
                            }

                            d1 += d5;
                            d2 += d6;
                            d3 += d7;
                            d4 += d8;
                        }

                    }

                }

            }
        }

        public void replaceBlocksForBiome(int i, int j, byte[] abyte0, BiomeGenBase[] abiomegenbase)
        {
            byte byte0 = 63;
            double d = 0.03125D;
            stoneNoise = field_702_n.GetNoise(stoneNoise, i * 16, j * 16, 0, 16, 16, 1, d * 2D, d * 2D, d * 2D);
            for (int k = 0; k < 16; k++)
            {
                for (int l = 0; l < 16; l++)
                {
                    BiomeGenBase biomegenbase = abiomegenbase[l + k * 16];
                    int i1 = (int)(stoneNoise[k + l * 16] / 3D + 3D + random.nextDouble() * 0.25D);
                    int j1 = -1;
                    byte byte1 = biomegenbase.topBlock;
                    byte byte2 = biomegenbase.fillerBlock;
                    for (int k1 = 127; k1 >= 0; k1--)
                    {
                        int l1 = (l * 16 + k) * 128 + k1;
                        if (k1 <= 0 + random.nextInt(5))
                        {
                            abyte0[l1] = (byte)Blocks.Bedrock;
                            continue;
                        }
                        byte byte3 = abyte0[l1];
                        if (byte3 == 0)
                        {
                            j1 = -1;
                            continue;
                        }
                        if (byte3 != (byte)Blocks.Stone)
                        {
                            continue;
                        }
                        if (j1 == -1)
                        {
                            if (i1 <= 0)
                            {
                                byte1 = 0;
                                byte2 = (byte)Blocks.Stone;
                            }
                            else
                                if (k1 >= byte0 - 4 && k1 <= byte0 + 1)
                                {
                                    byte1 = biomegenbase.topBlock;
                                    byte2 = biomegenbase.fillerBlock;
                                }
                            if (k1 < byte0 && byte1 == 0)
                            {
                                byte1 = (byte)Blocks.SWater;
                            }
                            j1 = i1;
                            if (k1 >= byte0 - 1)
                            {
                                abyte0[l1] = byte1;
                            }
                            else
                            {
                                abyte0[l1] = byte2;
                            }
                            continue;
                        }
                        if (j1 <= 0)
                        {
                            continue;
                        }
                        j1--;
                        abyte0[l1] = byte2;
                        if (j1 == 0 && byte2 == (byte)Blocks.Sand)
                        {
                            j1 = random.nextInt(4);
                            byte2 = (byte)Blocks.SandStone;
                        }
                    }

                }

            }
        }

        private double[] func_4058_a(double[] ad, int i, int j, int k, int l, int i1, int j1)
        {
            if(ad == null)
            {
                ad = new double[l * i1 * j1];
            }
            if(field_35561_l == null)
            {
                field_35561_l = new float[25];
                for(int k1 = -2; k1 <= 2; k1++)
                {
                    for(int l1 = -2; l1 <= 2; l1++)
                    {
                        float f = 10F / MathHelper.sqrt_float((float)(k1 * k1 + l1 * l1) + 0.2F);
                        field_35561_l[k1 + 2 + (l1 + 2) * 5] = f;
                    }

                }

            }
            double d = 684.41200000000003D;
            double d1 = 684.41200000000003D;
            field_4226_g = field_715_a.func_4103_a(field_4226_g, i, k, l, j1, 1.121D, 1.121D, 0.5D);
            field_4225_h = field_714_b.func_4103_a(field_4225_h, i, k, l, j1, 200D, 200D, 0.5D);
            field_4229_d = field_703_m.GetNoise(field_4229_d, i, j, k, l, i1, j1, d / 80D, d1 / 160D, d / 80D);
            field_4228_e = field_705_k.GetNoise(field_4228_e, i, j, k, l, i1, j1, d, d1, d);
            field_4227_f = field_704_l.GetNoise(field_4227_f, i, j, k, l, i1, j1, d, d1, d);
            i = k = 0;
            int i2 = 0;
            int j2 = 0;
            for(int k2 = 0; k2 < l; k2++)
            {
                for(int l2 = 0; l2 < j1; l2++)
                {
                    float f1 = 0.0F;
                    float f2 = 0.0F;
                    float f3 = 0.0F;
                    byte byte0 = 2;
                    BiomeGenBase biomegenbase = biomesForGeneration[k2 + 2 + (l2 + 2) * (l + 5)];
                    for(int i3 = -byte0; i3 <= byte0; i3++)
                    {
                        for(int j3 = -byte0; j3 <= byte0; j3++)
                        {
                            BiomeGenBase biomegenbase1 = biomesForGeneration[k2 + i3 + 2 + (l2 + j3 + 2) * (l + 5)];
                            float f4 = field_35561_l[i3 + 2 + (j3 + 2) * 5] / (biomegenbase1.minHeight + 2.0F);
                            if(biomegenbase1.minHeight > biomegenbase.minHeight)
                            {
                                f4 /= 2.0F;
                            }
                            f1 += biomegenbase1.maxHeight * f4;
                            f2 += biomegenbase1.minHeight * f4;
                            f3 += f4;
                        }

                    }

                    f1 /= f3;
                    f2 /= f3;
                    f1 = f1 * 0.9F + 0.1F;
                    f2 = (f2 * 4F - 1.0F) / 8F;
                    double d2 = field_4225_h[j2] / 8000D;
                    if(d2 < 0.0D)
                    {
                        d2 = -d2 * 0.29999999999999999D;
                    }
                    d2 = d2 * 3D - 2D;
                    if(d2 < 0.0D)
                    {
                        d2 /= 2D;
                        if(d2 < -1D)
                        {
                            d2 = -1D;
                        }
                        d2 /= 1.3999999999999999D;
                        d2 /= 2D;
                    } else
                    {
                        if(d2 > 1.0D)
                        {
                            d2 = 1.0D;
                        }
                        d2 /= 8D;
                    }
                    j2++;
                    for(int k3 = 0; k3 < i1; k3++)
                    {
                        double d3 = f2;
                        double d4 = f1;
                        d3 += d2 * 0.20000000000000001D;
                        d3 = (d3 * (double)i1) / 16D;
                        double d5 = (double)i1 / 2D + d3 * 4D;
                        double d6 = 0.0D;
                        double d7 = (((double)k3 - d5) * 12D * 128D) / 128D / d4;
                        if(d7 < 0.0D)
                        {
                            d7 *= 4D;
                        }
                        double d8 = field_4228_e[i2] / 512D;
                        double d9 = field_4227_f[i2] / 512D;
                        double d10 = (field_4229_d[i2] / 10D + 1.0D) / 2D;
                        if(d10 < 0.0D)
                        {
                            d6 = d8;
                        } else
                        if(d10 > 1.0D)
                        {
                            d6 = d9;
                        } else
                        {
                            d6 = d8 + (d9 - d8) * d10;
                        }
                        d6 -= d7;
                        if(k3 > i1 - 4)
                        {
                            double d11 = (float)(k3 - (i1 - 4)) / 3F;
                            d6 = d6 * (1.0D - d11) + -10D * d11;
                        }
                        ad[i2] = d6;
                        i2++;
                    }

                }

            }

            return ad;
        }
    }
}
