using System;

namespace SMP.Generator
{
    public class BiomeGenDesert : BiomeGenBase
    {
        public BiomeGenDesert(int i)
            : base(i)
        {
            //spawnableCreatureList.clear();
            topBlock = (byte)Blocks.Sand;
            fillerBlock = (byte)Blocks.Sand;
            decorator.treesPerChunk = -999;
            decorator.deadBushPerChunk = 2;
            decorator.reedsPerChunk = 50;
            decorator.field_35289_x = 10;
        }
    }
}
