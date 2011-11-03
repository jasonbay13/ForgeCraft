using System;

namespace SMP
{
    public class GenLayerRiverMix : GenLayer
    {
        public GenLayerRiverMix(long l, GenLayer genlayer, GenLayer genlayer1)
            : base(l)
        {
            field_35033_b = genlayer;
            field_35034_c = genlayer1;
        }

        public override void func_35015_b(long l)
        {
            field_35033_b.func_35015_b(l);
            field_35034_c.func_35015_b(l);
            base.func_35015_b(l);
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = field_35033_b.func_35018_a(i, j, k, l);
            int[] ai1 = field_35034_c.func_35018_a(i, j, k, l);
            int[] ai2 = IntCache.func_35549_a(k * l);
            for(int i1 = 0; i1 < k * l; i1++)
            {
                if(ai[i1] == BiomeGenBase.field_35519_b.field_35529_y)
                {
                    ai2[i1] = ai[i1];
                } else
                {
                    ai2[i1] = ai1[i1] < 0 ? ai[i1] : ai1[i1];
                }
            }

            return ai2;
        }

        private GenLayer field_35033_b;
        private GenLayer field_35034_c;
    }
}
