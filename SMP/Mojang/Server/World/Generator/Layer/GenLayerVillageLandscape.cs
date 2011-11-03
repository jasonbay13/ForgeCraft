using System;

namespace SMP
{
    public class GenLayerVillageLandscape : GenLayer
    {
        public GenLayerVillageLandscape(long l, GenLayer genlayer)
            : base(l)
        {
            field_35029_b = (new BiomeGenBase[] {
                BiomeGenBase.desert, BiomeGenBase.forest, BiomeGenBase.field_35518_e, BiomeGenBase.swampland, BiomeGenBase.field_35520_c, BiomeGenBase.taiga
            });
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
                    ai1[j1 + i1 * k] = ai[j1 + i1 * k] <= 0 ? 0 : field_35029_b[func_35016_a(field_35029_b.Length)].field_35529_y;
                }

            }

            return ai1;
        }

        private BiomeGenBase[] field_35029_b;
    }
}
