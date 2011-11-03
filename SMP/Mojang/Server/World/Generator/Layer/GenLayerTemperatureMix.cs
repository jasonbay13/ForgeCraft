using System;

namespace SMP
{
    public class GenLayerTemperatureMix : GenLayer
    {
        public GenLayerTemperatureMix(GenLayer genlayer, GenLayer genlayer1, int i)
            : base(0)
        {
            field_35023_a = genlayer1;
            field_35027_b = genlayer;
            field_35028_c = i;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = field_35023_a.func_35018_a(i, j, k, l);
            int[] ai1 = field_35027_b.func_35018_a(i, j, k, l);
            int[] ai2 = IntCache.func_35549_a(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                ai2[i1] = ai1[i1] + (BiomeGenBase.field_35521_a[ai[i1]].func_35509_f() - ai1[i1]) / (field_35028_c * 2 + 1);
            }

            return ai2;
        }

        private GenLayer field_35027_b;
        private int field_35028_c;
    }
}
