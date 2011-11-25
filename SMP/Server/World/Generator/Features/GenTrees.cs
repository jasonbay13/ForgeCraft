using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class GenTrees : MapGenBase
    {
        private int Field_947_a;
        private java.util.Random Rand;
        private World Field_35530_d;

        protected override int field_947_a { get { return Field_947_a; } set { Field_947_a = value; } }
        protected override java.util.Random rand { get { return Rand; } set { Rand = value; } }
        protected override World field_35530_d { get { return Field_35530_d; } set { Field_35530_d = value; } }


        public GenTrees()
            : base()
        {
        }
        public GenTrees(long seed)
            : base()
        {
            rand = new java.util.Random(seed);
        }

        public void Normal(World w, int x, int y, int z, byte type)
        {
            byte dist, tile;
            byte height = (byte)(rand.nextInt(4) + 3);
            byte top = (byte)(height - 2);
            short xx, yy, zz;
            int xxx, yyy, zzz;
            for (yy = 0; yy <= height; yy++)
            {
                yyy = y + yy;
                tile = tile = w.GetBlock(x, yyy, z);
                if (tile == (byte)Blocks.Air || (yyy == y && tile == (byte)Blocks.Sapling))
                    w.BlockChange(x, yyy, z, (byte)Blocks.Wood, type);
            }

            for (yy = top; yy <= height + 1; yy++)
            {
                dist = yy > height - 1 ? (byte)1 : (byte)2;
                for (xx = (short)-dist; xx <= dist; xx++)
                {
                    for (zz = (short)-dist; zz <= dist; zz++)
                    {
                        xxx = x + xx;
                        yyy = y + yy;
                        zzz = z + zz;
                        tile = w.GetBlock(xxx, yyy, zzz);
                        //Server.s.Log(String.Format("{0} {1} {2}", xxx, yyy, zzz));

                        if ((xxx == x && zzz == z && yy <= height) || tile != (byte)Blocks.Air)
                            continue;

                        if (Math.Abs(xx) == dist && Math.Abs(zz) == dist)
                        {
                            if (yy > height) continue;
                            if (rand.nextInt(2) == 0)
                                w.BlockChange(xxx, yyy, zzz, (byte)Blocks.Leaves, type);
                        }
                        else
                            w.BlockChange(xxx, yyy, zzz, (byte)Blocks.Leaves, type);
                    }
                }
            }
        }
    }
}
