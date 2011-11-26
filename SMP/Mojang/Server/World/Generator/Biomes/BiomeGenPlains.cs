using System;

namespace SMP
{
    public class BiomeGenPlains : BiomeGenBase
    {
        public BiomeGenPlains(int i)
            : base(i)
        {
            decorator.treesPerChunk = -999;
            decorator.flowersPerChunk = 4;
            decorator.grassPerChunk = 10;
        }
    }
}
