using System;

namespace SMP
{
    public class WorldGenFlowers : WorldGenerator
    {
        public WorldGenFlowers(int i)
        {
            plantBlockId = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; l < 64; l++)
            {
                int i1 = (i + random.nextInt(8)) - random.nextInt(8);
                int j1 = (j + random.nextInt(4)) - random.nextInt(4);
                int k1 = (k + random.nextInt(8)) - random.nextInt(8);
                if (world.IsAirBlock(i1, j1, k1) && world.CanBlockStay((byte)plantBlockId, i1, j1, k1))
                {
                    world.SetBlock(i1, j1, k1, (byte)plantBlockId);
                }
            }

            return true;
        }

        private int plantBlockId;
    }
}
