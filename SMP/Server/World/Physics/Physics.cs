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
        private Random random;
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

        public Physics(World w) : this(w, PSetting.Hardcore) { }
        public Physics(World w, PSetting setting)
        {
            this.w = w;
            this.setting = setting;
            this.wait = this.speed;
            this.random = new Random();
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
                    //Player.players.ForEach(delegate(Player p) { Console.WriteLine(p.rot[0]); });
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
                                    if (!HigherLiquidCheck(C.x, C.y, C.z, 8, meta) && !HigherLiquidCheck(C.x, C.y, C.z, 9, meta))
                                    {
                                        if ((meta & 0x7) >= 0x7)
                                            w.BlockChange(C.x, C.y, C.z, 0, 0);
                                        else
                                        {
                                            w.BlockChange(C.x, C.y, C.z, 8, (byte)Math.Min(meta + 2, 0x7));
                                            if (!AdjacentLiquidCheck(C.x, C.y, C.z, 8) && !AdjacentLiquidCheck(C.x, C.y, C.z, 9)) { C.time = 0; break; }
                                        }
                                    }
                                    else if ((meta & 0x8) != 0)
                                    {
                                        if (!WaterFlowCheck(C.x, C.y - 1, C.z)) { meta = 0; goto flowOut; }
                                        WaterFlow(C.x, C.y - 1, C.z, 0x8);
                                    }
                                    else if ((meta & 0x7) < 0x7)
                                    {
                                        goto flowOut;
                                    }
                                    else
                                    {
                                        WaterFlow(C.x, C.y - 1, C.z, 0x8);
                                    }
                                    C.time = short.MaxValue;
                                    break;

                                    flowOut:
                                    if (WaterFlowCheck(C.x, C.y - 1, C.z))
                                    {
                                        WaterFlow(C.x, C.y - 1, C.z, 0x8);
                                        if (!AdjacentLiquidCheck(C.x, C.y, C.z, 8) && !AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                                        {
                                            WaterFlow(C.x + 1, C.y, C.z, 0x7);
                                            WaterFlow(C.x - 1, C.y, C.z, 0x7);
                                            WaterFlow(C.x, C.y, C.z + 1, 0x7);
                                            WaterFlow(C.x, C.y, C.z - 1, 0x7);
                                        }
                                    }
                                    else
                                    {
                                        meta++;
                                        WaterFlow(C.x + 1, C.y, C.z, meta);
                                        WaterFlow(C.x - 1, C.y, C.z, meta);
                                        WaterFlow(C.x, C.y, C.z + 1, meta);
                                        WaterFlow(C.x, C.y, C.z - 1, meta);
                                    }
                                }
                                C.time = short.MaxValue;
                                break;
                            case (byte)Blocks.SLava:
                            case (byte)Blocks.ALava:
                                if (setting >= PSetting.Normal)
                                {
                                    if (C.time < 30) { C.time++; break; }

                                    byte meta = w.GetMeta(C.x, C.y, C.z);
                                    if (!HigherLiquidCheck(C.x, C.y, C.z, 10, meta) && !HigherLiquidCheck(C.x, C.y, C.z, 11, meta))
                                    {
                                        if ((meta & 0x7) >= 0x6)
                                            w.BlockChange(C.x, C.y, C.z, 0, 0);
                                        else
                                        {
                                            w.BlockChange(C.x, C.y, C.z, 10, (byte)Math.Min(meta + 2, 0x6));
                                            if (!AdjacentLiquidCheck(C.x, C.y, C.z, 10) && !AdjacentLiquidCheck(C.x, C.y, C.z, 11)) { C.time = 0; break; }
                                        }
                                    }
                                    else if ((meta & 0x8) != 0)
                                    {
                                        if (!LavaFlowCheck(C.x, C.y - 1, C.z)) { meta = 0; goto flowOut; }
                                        LavaFlow(C.x, C.y - 1, C.z, 0x8);
                                    }
                                    else if ((meta & 0x7) < 0x6)
                                    {
                                        goto flowOut;
                                    }
                                    else
                                    {
                                        LavaFlow(C.x, C.y - 1, C.z, 0x8);
                                    }
                                    C.time = short.MaxValue;
                                    break;

                                    flowOut:
                                    if (LavaFlowCheck(C.x, C.y - 1, C.z))
                                    {
                                        LavaFlow(C.x, C.y - 1, C.z, 0x8);
                                        if (!AdjacentLiquidCheck(C.x, C.y, C.z, 10) && !AdjacentLiquidCheck(C.x, C.y, C.z, 11))
                                        {
                                            LavaFlow(C.x + 1, C.y, C.z, 0x6);
                                            LavaFlow(C.x - 1, C.y, C.z, 0x6);
                                            LavaFlow(C.x, C.y, C.z + 1, 0x6);
                                            LavaFlow(C.x, C.y, C.z - 1, 0x6);
                                        }
                                    }
                                    else
                                    {
                                        if (AdjacentLiquidCheck(C.x, C.y, C.z, 8) || AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                                        {
                                            if ((meta & 0x7) == 0)
                                                w.BlockChange(C.x, C.y, C.z, 49, 0);
                                            else
                                                w.BlockChange(C.x, C.y, C.z, 4, 0);
                                            Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 1004, w);
                                            //Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 2000, 4, w);
                                        }
                                        else
                                        {
                                            meta += 2;
                                            LavaFlow(C.x + 1, C.y, C.z, meta);
                                            LavaFlow(C.x - 1, C.y, C.z, meta);
                                            LavaFlow(C.x, C.y, C.z + 1, meta);
                                            LavaFlow(C.x, C.y, C.z - 1, meta);

                                            if (w.GetBlock(C.x, C.y - 1, C.z) == 8 || w.GetBlock(C.x, C.y - 1, C.z) == 9)
                                            {
                                                w.BlockChange(C.x, C.y - 1, C.z, 4, 0);
                                                Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 1004, w);
                                            }
                                        }
                                    }
                                }
                                C.time = short.MaxValue;
                                break;
                            case (byte)Blocks.Sponge:
                                SpongePlaced(C.x, C.y, C.z);
                                C.time = short.MaxValue;
                                break;
                            default:
                                C.time = short.MaxValue;
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
                Checks.RemoveAll(Check => Check.time == short.MaxValue);
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

        public void RemoveChunkChecks(int x, int z)
        {
            Checks.RemoveAll(Check => ((Check.x >> 4) == x && (Check.z >> 4) == z));
        }

        public class Check
        {
            public byte meta;
            public short time;
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
