using System;

namespace SMP.Generator
{
    public class BiomeGenSwamp : BiomeGenBase
    {
        public BiomeGenSwamp(int i)
            : base(i)
        {
            decorator.treesPerChunk = 2;
            decorator.flowersPerChunk = -999;
            decorator.deadBushPerChunk = 1;
            decorator.mushroomsPerChunk = 8;
            decorator.reedsPerChunk = 10;
            decorator.clayPerChunk = 1;
            decorator.field_40321_y = 4;
            field_40461_A = 0xe0ff70;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            return swampTreeGenerator;
        }
    }
}
