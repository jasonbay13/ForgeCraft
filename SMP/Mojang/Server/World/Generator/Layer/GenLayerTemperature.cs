using System;

namespace SMP
{
    public class GenLayerTemperature : GenLayer
    {
        public GenLayerTemperature(GenLayer genlayer)
            : base(0)
        {
            parent = genlayer;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = parent.func_35018_a(i, j, k, l);
            int[] ai1 = IntCache.getIntCache(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                ai1[i1] = BiomeGenBase.biomeList[ai[i1]].func_35509_f();
            }

            return ai1;
        }
    }
}
