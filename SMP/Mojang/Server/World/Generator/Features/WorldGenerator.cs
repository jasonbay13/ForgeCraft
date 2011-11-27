using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.Generator
{
    public abstract class WorldGenerator
    {
        private readonly bool field_41044_a;

        public WorldGenerator()
        {
            field_41044_a = false;
        }

        public WorldGenerator(bool flag)
        {
            field_41044_a = flag;
        }

        public abstract bool generate(World world, java.util.Random random, int i, int j, int k);

        public virtual void func_420_a(double d, double d1, double d2)
        {
        }

        protected void func_41043_a(World world, int i, int j, int k, int l, int i1)
        {
            if (field_41044_a)
            {
                world.BlockChange(i, j, k, (byte)l, (byte)i1);
            }
            else
            {
                world.SetBlock(i, j, k, (byte)l, (byte)i1);
            }
        }
    }
}
