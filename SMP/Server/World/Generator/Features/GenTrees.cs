using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class GenTrees
    {
        private static Random rand = new Random();

        public static void Normal(World w, int x, int y, int z, byte type)
        {
            byte dist, tile;
            byte height = (byte)rand.Next(3, 7);
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
                            if (rand.Next(2) == 0)
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
