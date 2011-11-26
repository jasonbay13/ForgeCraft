using System;
using System.Collections.Generic;

namespace SMP
{
    public class WorldChunkManager
    {
        protected WorldChunkManager()
        {
            biomeCache = new BiomeCache(this);
            biomesToSpawnIn = new List<BiomeGenBase>();
            biomesToSpawnIn.Add(BiomeGenBase.forest);
            biomesToSpawnIn.Add(BiomeGenBase.swampland);
            biomesToSpawnIn.Add(BiomeGenBase.taiga);
        }

        public WorldChunkManager(World world)
            : this()
        {
            GenLayer[] agenlayer = GenLayer.func_35019_a(world.seed);
            field_34907_a = agenlayer[0];
            field_34906_b = agenlayer[1];
            temperatureLayer = agenlayer[2];
            rainfallLayer = agenlayer[3];
        }

        public List<BiomeGenBase> func_35137_a()
        {
            return biomesToSpawnIn;
        }

        public BiomeGenBase getBiomeGenAtChunkCoord(Point chunkcoordintpair)
        {
            return getBiomeGenAt(chunkcoordintpair.x << 4, chunkcoordintpair.z << 4);
        }

        public BiomeGenBase getBiomeGenAt(int i, int j)
        {
            return biomeCache.func_35683_a(i, j);
        }

        public float[] func_4065_a(float[] af, int i, int j, int k, int l)
        {
            IntCache.func_35550_a();
            if(af == null || af.Length < k * l)
            {
                af = new float[k * l];
            }
            int[] ai = rainfallLayer.func_35018_a(i, j, k, l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                float f = (float)ai[i1] / 65536F;
                if(f > 1.0F)
                {
                    f = 1.0F;
                }
                af[i1] = f;
            }

            return af;
        }

        public float func_40579_a(int i, int j, int k)
        {
            return func_40577_a(biomeCache.func_40625_c(i, k), j);
        }

        public float func_40577_a(float f, int i)
        {
            return f;
        }

        public float[] func_40578_a(int i, int j, int k, int l)
        {
            field_40580_a = getTemperatures(field_40580_a, i, j, k, l);
            return field_40580_a;
        }

        public float[] getTemperatures(float[] af, int i, int j, int k, int l)
        {
            IntCache.func_35550_a();
            if(af == null || af.Length < k * l)
            {
                af = new float[k * l];
            }
            int[] ai = temperatureLayer.func_35018_a(i, j, k, l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                float f = (float)ai[i1] / 65536F;
                if(f > 1.0F)
                {
                    f = 1.0F;
                }
                af[i1] = f;
            }

            return af;
        }

        public BiomeGenBase[] func_35142_b(BiomeGenBase[] abiomegenbase, int i, int j, int k, int l)
        {
            IntCache.func_35550_a();
            if(abiomegenbase == null || abiomegenbase.Length < k * l)
            {
                abiomegenbase = new BiomeGenBase[k * l];
            }
            int[] ai = field_34907_a.func_35018_a(i, j, k, l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                abiomegenbase[i1] = BiomeGenBase.biomeList[ai[i1]];
            }

            return abiomegenbase;
        }

        public BiomeGenBase[] loadBlockGeneratorData(BiomeGenBase[] abiomegenbase, int i, int j, int k, int l)
        {
            return func_35140_a(abiomegenbase, i, j, k, l, true);
        }

        public BiomeGenBase[] func_35140_a(BiomeGenBase[] abiomegenbase, int i, int j, int k, int l, bool flag)
        {
            IntCache.func_35550_a();
            if(abiomegenbase == null || abiomegenbase.Length < k * l)
            {
                abiomegenbase = new BiomeGenBase[k * l];
            }
            if(flag && k == 16 && l == 16 && (i & 0xf) == 0 && (j & 0xf) == 0)
            {
                BiomeGenBase[] abiomegenbase1 = biomeCache.func_35682_b(i, j);
                Array.Copy(abiomegenbase1, 0, abiomegenbase, 0, k * l);
                return abiomegenbase;
            }
            int[] ai = field_34906_b.func_35018_a(i, j, k, l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                abiomegenbase[i1] = BiomeGenBase.biomeList[ai[i1]];
            }

            return abiomegenbase;
        }

        public bool func_35141_a(int i, int j, int k, List<BiomeGenBase> list)
        {
            int l = i - k >> 2;
            int i1 = j - k >> 2;
            int j1 = i + k >> 2;
            int k1 = j + k >> 2;
            int l1 = (j1 - l) + 1;
            int i2 = (k1 - i1) + 1;
            int[] ai = field_34907_a.func_35018_a(l, i1, l1, i2);
            for(int j2 = 0; j2 < l1 * i2; j2++)
            {
                BiomeGenBase biomegenbase = BiomeGenBase.biomeList[ai[j2]];
                if(!list.Contains(biomegenbase))
                {
                    return false;
                }
            }

            return true;
        }

        public ChunkPosition func_35139_a(int i, int j, int k, List<BiomeGenBase> list, java.util.Random random)
        {
            int l = i - k >> 2;
            int i1 = j - k >> 2;
            int j1 = i + k >> 2;
            int k1 = j + k >> 2;
            int l1 = (j1 - l) + 1;
            int i2 = (k1 - i1) + 1;
            int[] ai = field_34907_a.func_35018_a(l, i1, l1, i2);
            ChunkPosition chunkposition = null;
            int j2 = 0;
            for(int k2 = 0; k2 < ai.Length; k2++)
            {
                int l2 = l + k2 % l1 << 2;
                int i3 = i1 + k2 / l1 << 2;
                BiomeGenBase biomegenbase = BiomeGenBase.biomeList[ai[k2]];
                if(list.Contains(biomegenbase) && (chunkposition == null || random.nextInt(j2 + 1) == 0))
                {
                    chunkposition = new ChunkPosition(l2, 0, i3);
                    j2++;
                }
            }

            return chunkposition;
        }

        public void func_35138_b()
        {
            biomeCache.func_35681_a();
        }

        private GenLayer field_34907_a;
        private GenLayer field_34906_b;
        private GenLayer temperatureLayer;
        private GenLayer rainfallLayer;
        private BiomeCache biomeCache;
        private List<BiomeGenBase> biomesToSpawnIn;
        public float[] field_40580_a;
    }
}
