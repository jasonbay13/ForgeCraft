﻿using System;

namespace SMP.Generator
{
    public abstract class GenLayer
    {
        public static GenLayer[] func_35019_a(long l)
        {
            GenLayer obj = new LayerIsland(1L);
            obj = new GenLayerZoomFuzzy(2000L, ((GenLayer)(obj)));
            obj = new GenLayerIsland(1L, ((GenLayer)(obj)));
            obj = new GenLayerZoom(2001L, ((GenLayer)(obj)));
            obj = new GenLayerIsland(2L, ((GenLayer)(obj)));
            obj = new GenLayerSnow(2L, ((GenLayer)(obj)));
            obj = new GenLayerZoom(2002L, ((GenLayer)(obj)));
            obj = new GenLayerIsland(3L, ((GenLayer)(obj)));
            obj = new GenLayerZoom(2003L, ((GenLayer)(obj)));
            obj = new GenLayerIsland(4L, ((GenLayer)(obj)));
            obj = new GenLayerMushroomIsland(5L, ((GenLayer)(obj)));
            byte byte0 = 4;
            GenLayer obj1 = obj;
            obj1 = GenLayerZoom.func_35025_a(1000L, ((GenLayer)(obj1)), 0);
            obj1 = new GenLayerRiverInit(100L, ((GenLayer)(obj1)));
            obj1 = GenLayerZoom.func_35025_a(1000L, ((GenLayer)(obj1)), byte0 + 2);
            obj1 = new GenLayerRiver(1L, ((GenLayer)(obj1)));
            obj1 = new GenLayerSmooth(1000L, ((GenLayer)(obj1)));
            GenLayer obj2 = obj;
            obj2 = GenLayerZoom.func_35025_a(1000L, ((GenLayer)(obj2)), 0);
            obj2 = new GenLayerVillageLandscape(200L, ((GenLayer)(obj2)));
            obj2 = GenLayerZoom.func_35025_a(1000L, ((GenLayer)(obj2)), 2);
            GenLayer obj3 = new GenLayerTemperature(((GenLayer)(obj2)));
            GenLayer obj4 = new GenLayerDownfall(((GenLayer)(obj2)));
            for (int i = 0; i < byte0; i++)
            {
                obj2 = new GenLayerZoom(1000 + i, ((GenLayer)(obj2)));
                if (i == 0)
                {
                    obj2 = new GenLayerIsland(3L, ((GenLayer)(obj2)));
                }
                if (i == 0)
                {
                    obj2 = new GenLayerShore(1000L, ((GenLayer)(obj2)));
                }
                obj3 = new GenLayerSmoothZoom(1000 + i, ((GenLayer)(obj3)));
                obj3 = new GenLayerTemperatureMix(((GenLayer)(obj3)), ((GenLayer)(obj2)), i);
                obj4 = new GenLayerSmoothZoom(1000 + i, ((GenLayer)(obj4)));
                obj4 = new GenLayerDownfallMix(((GenLayer)(obj4)), ((GenLayer)(obj2)), i);
            }

            obj2 = new GenLayerSmooth(1000L, ((GenLayer)(obj2)));
            obj2 = new GenLayerRiverMix(100L, ((GenLayer)(obj2)), ((GenLayer)(obj1)));
            GenLayerRiverMix genlayerrivermix = ((GenLayerRiverMix)(obj2));
            obj3 = GenLayerSmoothZoom.func_35030_a(1000L, ((GenLayer)(obj3)), 2);
            obj4 = GenLayerSmoothZoom.func_35030_a(1000L, ((GenLayer)(obj4)), 2);
            GenLayerZoomVoronoi genlayerzoomvoronoi = new GenLayerZoomVoronoi(10L, ((GenLayer)(obj2)));
            ((GenLayer)(obj2)).func_35015_b(l);
            ((GenLayer)(obj3)).func_35015_b(l);
            ((GenLayer)(obj4)).func_35015_b(l);
            genlayerzoomvoronoi.func_35015_b(l);
            return (new GenLayer[] {
                obj2, genlayerzoomvoronoi, obj3, obj4, genlayerrivermix
            });
        }

        public GenLayer(long l)
        {
            baseSeed = l;
            baseSeed *= baseSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            baseSeed += l;
            baseSeed *= baseSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            baseSeed += l;
            baseSeed *= baseSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            baseSeed += l;
        }

        public virtual void func_35015_b(long l)
        {
            field_35021_b = l;
            if (parent != null)
            {
                parent.func_35015_b(l);
            }
            field_35021_b *= field_35021_b * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            field_35021_b += baseSeed;
            field_35021_b *= field_35021_b * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            field_35021_b += baseSeed;
            field_35021_b *= field_35021_b * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            field_35021_b += baseSeed;
        }

        public virtual void func_35017_a(long l, long l1)
        {
            chunkSeed = field_35021_b;
            chunkSeed *= chunkSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            chunkSeed += l;
            chunkSeed *= chunkSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            chunkSeed += l1;
            chunkSeed *= chunkSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            chunkSeed += l;
            chunkSeed *= chunkSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            chunkSeed += l1;
        }

        protected virtual int nextInt(int i)
        {
            int j = (int)((chunkSeed >> 24) % (long)i);
            if (j < 0)
            {
                j += i;
            }
            chunkSeed *= chunkSeed * 0x5851f42d4c957f2dL + 0x14057b7ef767814fL;
            chunkSeed += field_35021_b;
            return j;
        }

        public abstract int[] func_35018_a(int i, int j, int k, int l);

        private long field_35021_b;
        protected GenLayer parent;
        private long chunkSeed;
        private long baseSeed;
    }
}
