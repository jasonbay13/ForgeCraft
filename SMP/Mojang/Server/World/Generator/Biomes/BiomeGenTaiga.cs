using System;

namespace SMP
{
    public class BiomeGenTaiga : BiomeGenBase
    {
        public BiomeGenTaiga(int i)
            : base(i)
        {
            //spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityWolf.class, 8, 4, 4));
            decorator.treesPerChunk = 10;
            decorator.grassPerChunk = 1;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            if(random.nextInt(3) == 0)
            {
                return new WorldGenTaiga1();
            } else
            {
                return new WorldGenTaiga2(false);
            }
        }
    }
}
