using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    class BiomeGenMushroomIsland : BiomeGenBase
    {
        public BiomeGenMushroomIsland(int i)
            : base(i)
        {
            decorator.treesPerChunk = -100;
            decorator.flowersPerChunk = -100;
            decorator.grassPerChunk = -100;
            decorator.mushroomsPerChunk = 1;
            decorator.field_40318_J = 1;
            topBlock = (byte)Blocks.Mycelium;
            /*spawnableMonsterList.clear();
            spawnableCreatureList.clear();
            spawnableWaterCreatureList.clear();
            spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityMooshroom.class, 8, 4, 8));*/
        }
    }
}
