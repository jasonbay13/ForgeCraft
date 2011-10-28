using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public abstract class WorldGenerator
    {
        public WorldGenerator()
        {
        }

        public abstract bool generate(World world, java.util.Random random, int i, int j, int k);

        public virtual void func_420_a(double d, double d1, double d2)
        {
        }
    }
}
