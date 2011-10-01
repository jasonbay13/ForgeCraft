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
                foreach (Data d in Checks)
                {
                    switch (d.Block)
                    {
                        case (byte)Blocks.AWater:
                        case (byte)Blocks.SWater:
                            if (setting == PSetting.Normal)
                            {
                                if (d.time < 3)
                                {
                                    d.time++;
                                    Thread.Sleep(500);
                                    break;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z) == 0)
                                {
                                    w.BlockChange(d.x, d.y - 1, d.z, (byte)Blocks.AWater, 0);
                                    d.time = 0;
                                    Thread.Sleep(500);
                                    break;
                                }
                                bool continuemagma = true;
                                //Is in a conner?
                                if (w.GetBlock(d.x + 1, d.y - 1, d.z) == 0 && w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                {
                                    w.BlockChange(d.x + 1, d.y, d.z, (byte)Blocks.AWater, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x - 1, d.y - 1, d.z) == 0 && w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                {
                                    w.BlockChange(d.x - 1, d.y, d.z, (byte)Blocks.AWater, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z + 1) == 0 && w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                {
                                    w.BlockChange(d.x, d.y, d.z + 1, (byte)Blocks.AWater, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z - 1) == 0 && w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                {
                                    w.BlockChange(d.x, d.y, d.z - 1, (byte)Blocks.AWater, 0);
                                    continuemagma = false;
                                }
                                if (!continuemagma)
                                {
                                    Thread.Sleep(500);
                                    break;
                                }
                                //Magma flow
                                if (w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                    w.BlockChange(d.x + 1, d.y, d.z, (byte)Blocks.AWater, 0);
                                if (w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                    w.BlockChange(d.x - 1, d.y, d.z, (byte)Blocks.AWater, 0);
                                if (w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                    w.BlockChange(d.x, d.y, d.z + 1, (byte)Blocks.AWater, 0);
                                if (w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                    w.BlockChange(d.x, d.y, d.z - 1, (byte)Blocks.AWater, 0);
                                d.time = 0;
                            }
                            Thread.Sleep(500);
                            break;
                        case (byte)Blocks.SLava:
                        case (byte)Blocks.ALava:
                            if (setting == PSetting.Normal)
                            {
                                if (d.time < 6)
                                {
                                    d.time++;
                                    Thread.Sleep(1000);
                                    break;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z) == 0)
                                {
                                    w.BlockChange(d.x, d.y - 1, d.z, (byte)Blocks.ALava, 0);
                                    d.time = 0;
                                    Thread.Sleep(1000);
                                    break;
                                }
                                bool continuemagma = true;
                                //Is in a conner?
                                if (w.GetBlock(d.x + 1, d.y - 1, d.z) == 0 && w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                {
                                    w.BlockChange(d.x + 1, d.y, d.z, (byte)Blocks.ALava, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x - 1, d.y - 1, d.z) == 0 && w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                {
                                    w.BlockChange(d.x - 1, d.y, d.z, (byte)Blocks.ALava, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z + 1) == 0 && w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                {
                                    w.BlockChange(d.x, d.y, d.z + 1, (byte)Blocks.ALava, 0);
                                    continuemagma = false;
                                }
                                if (w.GetBlock(d.x, d.y - 1, d.z - 1) == 0 && w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                {
                                    w.BlockChange(d.x, d.y, d.z + 1, (byte)Blocks.ALava, 0);
                                    continuemagma = false;
                                }
                                if (!continuemagma)
                                {
                                    Thread.Sleep(1000);
                                    break;
                                }
                                if (w.GetBlock(d.x + 1, d.y, d.z) == 0)
                                    w.BlockChange(d.x + 1, d.y, d.z, (byte)Blocks.ALava, 0);
                                if (w.GetBlock(d.x - 1, d.y, d.z) == 0)
                                    w.BlockChange(d.x - 1, d.y, d.z, (byte)Blocks.ALava, 0);
                                if (w.GetBlock(d.x, d.y, d.z + 1) == 0)
                                    w.BlockChange(d.x, d.y, d.z + 1, (byte)Blocks.ALava, 0);
                                if (w.GetBlock(d.x, d.y, d.z - 1) == 0)
                                    w.BlockChange(d.x, d.y, d.z - 1, (byte)Blocks.ALava, 0);
                                d.time = 0;
                            }
                            Thread.Sleep(1000);
                            break;
                    }
                }
            }
        }
    }
}
