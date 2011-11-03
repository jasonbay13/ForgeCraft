using System;

namespace SMP
{
    public class BiomeGenForest : BiomeGenBase
    {
        public BiomeGenForest(int i)
            : base(i)
        {
            //spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityWolf.class, 5, 4, 4));
            field_35523_u.field_35284_r = 10;
            field_35523_u.field_35282_t = 2;
        }

        public override WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            if(random.nextInt(5) == 0)
            {
                return field_35516_B;
            }
            if(random.nextInt(10) == 0)
            {
                return field_35515_A;
            } else
            {
                return field_35528_z;
            }
        }
    }
}
