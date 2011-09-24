using System;
using System.Collections.Generic;
using System.Threading;

namespace SMP
{
    public class Data
    {
        public byte Block;
        public byte time;
        public ushort x;
        public ushort y;
        public ushort z;
        public Data(byte block, ushort x, ushort y, ushort z) { Block = block; this.x = x; this.y = y; this.z = z; }
    }
    public class Physics
    {
        public World w;
        public PSetting setting;
        private Thread physthread;
        private List<Data> Checks = new List<Data>();
        public Physics(World w)
        {
            setting = PSetting.Normal;
            this.w = w;
        }
        public Physics(PSetting setting, World w)
        {
            this.setting = setting;
            this.w = w;
        }
        public void Stop()
        {
            Checks.Clear();
            if (physthread != null)
            {
                physthread.Abort();
                physthread.Join();
            }
            setting = PSetting.None;
        }
        public void Start()
        {
            physthread = new Thread(new ThreadStart(CalcPhysics));
        }
        private void CalcPhysics()
        {
            while (true)
            {
                Chunk c = null;
                foreach (Data d in Checks)
                {
                    c = w.chunkData[new Point(d.x, d.z)];
                    switch (d.Block)
                    {
                        case (byte)Blocks.AWater:
                        case (byte)Blocks.SWater:
                            if (setting == PSetting.Normal)
                            {
                                if (d.time < 3)
                                {
                                    d.time++;
                                    break;
                                }
                                //Magma flow
                                if (w.GetBlock(d.x, d.y - 1, d.z) == 0)
                                {
                                    c.PlaceBlock(d.x, d.y - 1, d.z, (byte)Blocks.AWater);
                                    d.time = 0;
                                    break;
                                }
                                if (w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                    c.PlaceBlock(d.x + 1, d.y, d.z, (byte)Blocks.AWater);
                                if (w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                    c.PlaceBlock(d.x - 1, d.y, d.z, (byte)Blocks.AWater);
                                if (w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                    c.PlaceBlock(d.x, d.y, d.z + 1, (byte)Blocks.AWater);
                                if (w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                    c.PlaceBlock(d.x, d.y, d.z - 1, (byte)Blocks.AWater);
                                d.time = 0;
                            }
                            break;
                        case (byte)Blocks.SLava:
                        case (byte)Blocks.ALava:
                            if (setting == PSetting.Normal)
                            {
                                if (d.time < 6)
                                {
                                    d.time++;
                                    break;
                                }
                                //Magma flow
                                if (w.GetBlock(d.x, d.y - 1, d.z) == 0)
                                {
                                    c.PlaceBlock(d.x, d.y - 1, d.z, (byte)Blocks.ALava);
                                    d.time = 0;
                                    break;
                                }
                                if (w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                    c.PlaceBlock(d.x + 1, d.y, d.z, (byte)Blocks.ALava);
                                if (w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                    c.PlaceBlock(d.x - 1, d.y, d.z, (byte)Blocks.ALava);
                                if (w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                    c.PlaceBlock(d.x, d.y, d.z + 1, (byte)Blocks.ALava);
                                if (w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                    c.PlaceBlock(d.x, d.y, d.z - 1, (byte)Blocks.ALava);
                                d.time = 0;
                            }
                            break;
                    }
                }
            }
        }
    }
}
