using System;

namespace SMP
{
    public class GenLayerTemperatureMix : GenLayer
    {
        public GenLayerTemperatureMix(GenLayer genlayer, GenLayer genlayer1, int i)
            : base(0)
        {
            parent = genlayer1;
            field_35027_b = genlayer;
            field_35028_c = i;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = parent.func_35018_a(i, j, k, l);
            int[] ai1 = field_35027_b.func_35018_a(i, j, k, l);
            int[] ai2 = IntCache.getIntCache(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                ai2[i1] = ai1[i1] + (BiomeGenBase.biomeList[ai[i1]].func_35509_f() - ai1[i1]) / (field_35028_c * 2 + 1);
            }

            return ai2;
        }

        private GenLayer field_35027_b;
        private int field_35028_c;
    }
}
