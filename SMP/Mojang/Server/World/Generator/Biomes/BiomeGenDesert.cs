using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class BiomeGenDesert : BiomeGenBase
    {
        public BiomeGenDesert(int i)
            : base(i)
        {
            //spawnableCreatureList.clear();
            topBlock = (byte)Blocks.Sand;
            fillerBlock = (byte)Blocks.Sand;
            field_35523_u.field_35284_r = -999;
            field_35523_u.field_35281_u = 2;
            field_35523_u.field_35279_w = 50;
            field_35523_u.field_35289_x = 10;
        }
    }
}
