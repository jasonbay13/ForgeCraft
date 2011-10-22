using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public abstract class GenFeatures
    {
        protected abstract int field_947_a { get; set; }
        protected abstract java.util.Random rand { get; set; }
        protected abstract World field_35530_d { get; set; }

        public GenFeatures()
        {
            field_947_a = 8;
            rand = new java.util.Random();
        }

        public void generate(World world, int i, int j, byte[] abyte0)
        {
            int k = field_947_a;
            field_35530_d = world;
            rand = new java.util.Random(world.seed);
            long l = rand.nextLong();
            long l1 = rand.nextLong();
            for(int i1 = i - k; i1 <= i + k; i1++)
            {
                for(int j1 = j - k; j1 <= j + k; j1++)
                {
                    long l2 = (long)i1 * l;
                    long l3 = (long)j1 * l1;
                    rand = new java.util.Random(l2 ^ l3 ^ world.seed);
                    recursiveGenerate(world, i1, j1, i, j, abyte0);
                }

            }

        }

        protected void recursiveGenerate(World world, int i, int j, int k, int l, byte[] abyte0)
        {
        }
    }

    public enum GenType
    {
        Trees,
        Caves
    }
}
