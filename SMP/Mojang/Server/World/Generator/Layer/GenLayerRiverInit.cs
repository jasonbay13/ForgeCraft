using System;

namespace SMP
{
    public class GenLayerRiverInit : GenLayer
    {
        public GenLayerRiverInit(long l, GenLayer genlayer)
            : base(l)
        {
            field_35023_a = genlayer;
        }

        public override int[] func_35018_a(int i, int j, int k, int l)
        {
            int[] ai = field_35023_a.func_35018_a(i, j, k, l);
            int[] ai1 = IntCache.func_35549_a(k * l);
            for(int i1 = 0; i1 < l; i1++)
            {
                for(int j1 = 0; j1 < k; j1++)
                {
                    func_35017_a(j1 + i, i1 + j);
                    ai1[j1 + i1 * k] = ai[j1 + i1 * k] <= 0 ? 0 : func_35016_a(2) + 2;
                }

            }

            return ai1;
        }
    }
}
