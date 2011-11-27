using System;

namespace SMP.Generator
{
    public class WorldGenTallGrass : WorldGenerator
    {
        public WorldGenTallGrass(int i, int j)
        {
            field_28057_a = i;
            field_28056_b = j;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; ((l = world.GetBlock(i, j, k)) == 0 || l == (byte)Blocks.Leaves) && j > 0; j--) { }
            for (int i1 = 0; i1 < 128; i1++)
            {
                int j1 = (i + random.nextInt(8)) - random.nextInt(8);
                int k1 = (j + random.nextInt(4)) - random.nextInt(4);
                int l1 = (k + random.nextInt(8)) - random.nextInt(8);
                if (world.IsAirBlock(j1, k1, l1) && world.CanBlockStay((byte)field_28057_a, j1, k1, l1))
                {
                    world.SetBlock(j1, k1, l1, (byte)field_28057_a, (byte)field_28056_b);
                }
            }

            return true;
        }

        private int field_28057_a;
        private int field_28056_b;
    }
}
