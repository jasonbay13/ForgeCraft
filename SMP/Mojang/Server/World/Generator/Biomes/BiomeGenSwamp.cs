using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class BiomeGenSwamp : BiomeGenBase
    {
        public BiomeGenSwamp(int i)
            : base(i)
        {
            field_35523_u.field_35284_r = 2;
            field_35523_u.field_35283_s = -999;
            field_35523_u.field_35281_u = 1;
            field_35523_u.field_35280_v = 8;
            field_35523_u.field_35279_w = 10;
            field_35523_u.field_35261_A = 1;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            return field_35517_C;
        }
    }
}
