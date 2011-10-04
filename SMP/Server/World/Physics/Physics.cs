using System;
using System.Collections.Generic;
using System.Threading;

namespace SMP
{
    public partial class Physics
    {
        public bool paused = false;
        public int speed = 50;
        public PSetting setting;
        private World w;
        private Thread physthread;
        private List<Check> Checks = new List<Check>();
        private int wait = 0;
        #region Accessors
        public World world
        {
            get
            {
                return this.w;
            }
        }
        public int CheckCount
        {
            get
            {
                return Checks.Count;
            }
        }
        #endregion

        public Physics(World w)
        {
            this.w = w;
            setting = PSetting.Normal;
            this.wait = this.speed;
        }
        public Physics(World w, PSetting setting)
        {
            this.w = w;
            this.setting = setting;
            this.wait = this.speed;
        }

        public void Start()
        {
            if (physthread != null) return;
            physthread = new Thread(new ThreadStart(RunLoop));
            physthread.Start();
            Server.ServerLogger.Log("Physics started on " + w.name + ".");
        }

        public void Stop()
        {
            if (physthread == null) return;
            Checks.Clear();
            physthread.Abort();
            physthread.Join();
            physthread = null;
            Server.ServerLogger.Log("Physics stopped on " + w.name + ".");
        }

        private void RunLoop()
        {
            while (true)
            {
                try
                {
                    if (wait > 0) Thread.Sleep(wait);
                    if (setting == PSetting.None || paused || Checks.Count == 0)
                    {
                        wait = speed;
                        continue;
                    }

                    DateTime Start = DateTime.Now;
                    Calculate();
                    TimeSpan Took = DateTime.Now - Start;
                    wait = speed - (int)Took.TotalMilliseconds;

                    /*byte b = 0x3;
                    Console.WriteLine(Convert.ToString(b, 2) + " " + b.GetBits(1, 2));
                    b = b.SetBits(1, 2, 2);
                    Console.WriteLine(Convert.ToString(b, 2) + " " + b.GetBits(1, 2));*/
                }
                catch
                {
                    wait = speed;
                }
            }
        }

        private void Calculate()
        {
            try
            {
                Checks.ForEach(delegate(Check C)
                {
                    try
                    {
                        #region Physics Calculations
                        switch (w.GetBlock(C.x, C.y, C.z))
                        {
                            case (byte)Blocks.AWater:
                            case (byte)Blocks.SWater:
                                if (setting >= PSetting.Normal)
                                {
                                    if (C.time < 5) { C.time++; break; }

                                    byte meta = w.GetMeta(C.x, C.y, C.z);
                                    if (WaterFlowCheck(C.x, C.y - 1, C.z))
                                    {
                                        WaterFlow(C.x, C.y - 1, C.z, WaterFlowCheck(C.x, C.y - 2, C.z) ? (byte)0x8 : (byte)0x0);
                                        if (!AdjacentLiquidCheck(C.x, C.y, C.z, 8) && !AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                                        {
                                            WaterFlow(C.x + 1, C.y, C.z, 0x7 | 0x8);
                                            WaterFlow(C.x - 1, C.y, C.z, 0x7 | 0x8);
                                            WaterFlow(C.x, C.y, C.z + 1, 0x7 | 0x8);
                                            WaterFlow(C.x, C.y, C.z - 1, 0x7 | 0x8);
                                        }
                                    }
                                    else
                                    {
                                        meta = meta.SetBits(3, 0);
                                        if (meta.GetBits(0, 3) >= 0x7) { C.time = 255; break; }
                                        WaterFlow(C.x, C.y - 1, C.z, 0);
                                        WaterFlow(C.x + 1, C.y, C.z, (byte)(meta + 1));
                                        WaterFlow(C.x - 1, C.y, C.z, (byte)(meta + 1));
                                        WaterFlow(C.x, C.y, C.z + 1, (byte)(meta + 1));
                                        WaterFlow(C.x, C.y, C.z - 1, (byte)(meta + 1));
                                    }
                                }
                                C.time = 255;
                                break;
                            case (byte)Blocks.SLava:
                            case (byte)Blocks.ALava:
                                if (setting >= PSetting.Normal)
                                {
                                    if (C.time < 30) { C.time++; break; }

                                    byte meta = w.GetMeta(C.x, C.y, C.z);
                                    if (LavaFlowCheck(C.x, C.y - 1, C.z))
                                    {
                                        LavaFlow(C.x, C.y - 1, C.z, LavaFlowCheck(C.x, C.y - 2, C.z) ? (byte)0x8 : (byte)0x0);
                                        if (!AdjacentLiquidCheck(C.x, C.y, C.z, 10) && !AdjacentLiquidCheck(C.x, C.y, C.z, 11))
                                        {
                                            LavaFlow(C.x + 1, C.y, C.z, 0x6 | 0x8);
                                            LavaFlow(C.x - 1, C.y, C.z, 0x6 | 0x8);
                                            LavaFlow(C.x, C.y, C.z + 1, 0x6 | 0x8);
                                            LavaFlow(C.x, C.y, C.z - 1, 0x6 | 0x8);
                                        }
                                    }
                                    else if (AdjacentLiquidCheck(C.x, C.y, C.z, 8) || AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                                    {
                                        if (meta.GetBits(0, 3) == 0)
                                            w.BlockChange(C.x, C.y, C.z, 49, 0);
                                        else
                                            w.BlockChange(C.x, C.y, C.z, 4, 0);
                                        Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 1004, w);
                                        Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 2000, 4, w);
                                    }
                                    else
                                    {
                                        meta = meta.SetBits(3, 0);
                                        if (meta.GetBits(0, 3) >= 0x6) { C.time = 255; break; }
                                        LavaFlow(C.x, C.y - 1, C.z, 0);
                                        LavaFlow(C.x + 1, C.y, C.z, (byte)(meta + 2));
                                        LavaFlow(C.x - 1, C.y, C.z, (byte)(meta + 2));
                                        LavaFlow(C.x, C.y, C.z + 1, (byte)(meta + 2));
                                        LavaFlow(C.x, C.y, C.z - 1, (byte)(meta + 2));
                                    }
                                }
                                C.time = 255;
                                break;
                            case (byte)Blocks.Sponge:
                                SpongePlaced(C.x, C.y, C.z);
                                C.time = 255;
                                break;
                            default:
                                C.time = 255;
                                break;
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        Server.ServerLogger.Log("Physics error on " + w.name + "!");
                        Server.ServerLogger.LogError(e);
                        Checks.Remove(C);
                    }
                });
                Checks.RemoveAll(Check => Check.time == 255);
            }
            catch (Exception e)
            {
                Server.ServerLogger.Log("Physics error on " + w.name + "!");
                Server.ServerLogger.LogError(e);
            }
        }


        public void AddCheck(int x, int y, int z, byte meta, bool overRide)
        {
            try
            {
                if (!Checks.Exists(Check => (Check != null && Check.x == x && Check.y == y && Check.z == z)))
                {
                    Checks.Add(new Check(x, y, z, meta));
                }
                else
                {
                    if (overRide)
                    {
                        foreach (Check C in Checks)
                        {
                            if (C.x == x && C.y == y && C.z == z)
                            {
                                C.meta = meta;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Server.ServerLogger.LogError(e);
            }
        }
        public void AddCheck(int x, int y, int z, byte meta)
        {
            AddCheck(x, y, z, meta, false);
        }
        public void AddCheck(int x, int y, int z)
        {
            AddCheck(x, y, z, 0, false);
        }

        public class Check
        {
            public byte time, meta;
            public int x, y, z;

            public Check(int x, int y, int z, byte meta)
            {
                this.time = 0;
                this.meta = meta;
                this.x = x; this.y = y; this.z = z;
            }
            public Check(Point3 a, byte type, byte meta)
            {
                this.time = 0;
                this.meta = meta;
                this.x = (int)a.x; this.y = (int)a.y; this.z = (int)a.z;
            }
        }
    }

    public enum PSetting : byte
    {
        None = 0,
        Normal = 1,
        Advanced = 2,
        Hardcore = 3
    }
}
