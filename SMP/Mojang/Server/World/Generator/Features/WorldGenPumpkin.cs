using System;

namespace SMP
{
    public class WorldGenPumpkin : WorldGenerator
    {
        public WorldGenPumpkin()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; l < 64; l++)
            {
                int i1 = (i + random.nextInt(8)) - random.nextInt(8);
                int j1 = (j + random.nextInt(4)) - random.nextInt(4);
                int k1 = (k + random.nextInt(8)) - random.nextInt(8);
                if (world.IsAirBlock(i1, j1, k1) && world.GetBlock(i1, j1 - 1, k1) == (byte)Blocks.Grass && world.CanPlaceAt((byte)Blocks.Pumpkin, i1, j1, k1))
                {
                    world.SetBlock(i1, j1, k1, (byte)Blocks.Pumpkin, (byte)random.nextInt(4));
                }
            }

            return true;
        }
    }
}
