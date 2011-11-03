using System;

namespace SMP
{
    public class GenLayerTemperature : GenLayer
    {
        public GenLayerTemperature(GenLayer genlayer)
            : base(0)
        {
            field_35023_a = genlayer;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = field_35023_a.func_35018_a(i, j, k, l);
            int[] ai1 = IntCache.func_35549_a(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                ai1[i1] = BiomeGenBase.field_35521_a[ai[i1]].func_35509_f();
            }

            return ai1;
        }
    }
}
