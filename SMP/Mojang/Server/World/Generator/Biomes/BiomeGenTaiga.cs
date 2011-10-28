using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class BiomeGenTaiga : BiomeGenBase
    {
        public BiomeGenTaiga(int i)
            : base(i)
        {
            //spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityWolf.class, 8, 4, 4));
            field_35523_u.field_35284_r = 10;
            field_35523_u.field_35282_t = 1;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            if(random.nextInt(3) == 0)
            {
                return new WorldGenTaiga1();
            } else
            {
                return new WorldGenTaiga2();
            }
        }
    }
}
