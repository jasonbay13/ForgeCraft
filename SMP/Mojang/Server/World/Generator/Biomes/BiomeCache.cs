using System;
using System.Collections.Generic;

namespace SMP
{
    public class BiomeCache
    {
        public BiomeCache(WorldChunkManager worldchunkmanager)
        {
            field_35685_b = 0L;
            field_35686_c = new PlayerHash();
            field_35684_d = new List<BiomeCacheBlock>();
            field_35687_a = worldchunkmanager;
        }

        private BiomeCacheBlock func_35680_c(int i, int j)
        {
            i >>= 4;
            j >>= 4;
            long l = (long)i & 0xffffffffL | ((long)j & 0xffffffffL) << 32;
            BiomeCacheBlock biomecacheblock = (BiomeCacheBlock)field_35686_c.getValueByKey(l);
            if(biomecacheblock == null)
            {
                biomecacheblock = new BiomeCacheBlock(this, i, j);
                field_35686_c.add(l, biomecacheblock);
                field_35684_d.Add(biomecacheblock);
            }
            biomecacheblock.field_35701_f = DateTime.Now.Ticks / 10000;
            return biomecacheblock;
        }

        public BiomeGenBase func_35683_a(int i, int j)
        {
            return func_35680_c(i, j).func_35700_a(i, j);
        }

        public void func_35681_a()
        {
            long l = DateTime.Now.Ticks / 10000;
            long l1 = l - field_35685_b;
            if(l1 > 7500L || l1 < 0L)
            {
                field_35685_b = l;
                for(int i = 0; i < field_35684_d.Count; i++)
                {
                    BiomeCacheBlock biomecacheblock = (BiomeCacheBlock)field_35684_d[i];
                    long l2 = l - biomecacheblock.field_35701_f;
                    if(l2 > 30000L || l2 < 0L)
                    {
                        field_35684_d.RemoveAt(i--);
                        long l3 = (long)biomecacheblock.field_35703_d & 0xffffffffL | ((long)biomecacheblock.field_35704_e & 0xffffffffL) << 32;
                        field_35686_c.remove(l3);
                    }
                }

            }
        }

        public BiomeGenBase[] func_35682_b(int i, int j)
        {
            return func_35680_c(i, j).field_35706_c;
        }

        public static WorldChunkManager func_35679_a(BiomeCache biomecache)
        {
            return biomecache.field_35687_a;
        }

        private WorldChunkManager field_35687_a;
        private long field_35685_b;
        private PlayerHash field_35686_c;
        private List<BiomeCacheBlock> field_35684_d;
    }
}
