using System;

namespace SMP.Generator
{
    public class BiomeGenForest : BiomeGenBase
    {
        public BiomeGenForest(int i)
            : base(i)
        {
            //spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityWolf.class, 5, 4, 4));
            decorator.treesPerChunk = 10;
            decorator.grassPerChunk = 2;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            if(random.nextInt(5) == 0)
            {
                return forestGenerator;
            }
            if(random.nextInt(10) == 0)
            {
                return bigTreeGenerator;
            } else
            {
                return treeGenerator;
            }
        }
    }
}
