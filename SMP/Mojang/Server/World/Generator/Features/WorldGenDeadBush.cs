using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class WorldGenDeadBush : WorldGenerator
    {
        public WorldGenDeadBush(int i)
        {
            field_28055_a = i;
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            for (int l = 0; ((l = world.GetBlock(i, j, k)) == 0 || l == (byte)Blocks.Leaves) && j > 0; j--) { }
            for (int i1 = 0; i1 < 4; i1++)
            {
                int j1 = (i + random.nextInt(8)) - random.nextInt(8);
                int k1 = (j + random.nextInt(4)) - random.nextInt(4);
                int l1 = (k + random.nextInt(8)) - random.nextInt(8);
                if (world.IsAirBlock(j1, k1, l1) && world.CanBlockStay((byte)field_28055_a, j1, k1, l1))
                {
                    world.SetBlock(j1, k1, l1, (byte)field_28055_a);
                }
            }

            return true;
        }

        private int field_28055_a;
    }
}
