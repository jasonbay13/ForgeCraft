using System;

namespace SMP.Generator
{
    public class GenLayerDownfallMix : GenLayer
    {
        public GenLayerDownfallMix(GenLayer genlayer, GenLayer genlayer1, int i)
            : base(0)
        {
            parent = genlayer1;
            field_35035_b = genlayer;
            field_35036_c = i;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = parent.func_35018_a(i, j, k, l);
            int[] ai1 = field_35035_b.func_35018_a(i, j, k, l);
            int[] ai2 = IntCache.getIntCache(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                ai2[i1] = ai1[i1] + (BiomeGenBase.biomeList[ai[i1]].func_35510_e() - ai1[i1]) / (field_35036_c + 1);
            }

            return ai2;
        }

        private GenLayer field_35035_b;
        private int field_35036_c;
    }
}
