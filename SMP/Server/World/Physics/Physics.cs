using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMP
{
    public partial class Physics
    {
        public bool paused = false;
        public int speed;
        public PSetting setting;
        public World w;
        private Thread physthread;
        private List<Check> Checks = new List<Check>();
        private List<Update> Updates = new List<Update>();
        private int wait = 0, lastCheck = 0, lastUpdate = 0;
        private Random random;
        #region Accessors
        public int CheckCount
        {
            get
            {
                return this.lastCheck;
            }
        }
        public int UpdateCount
        {
            get
            {
                return this.lastUpdate;
            }
        }
        #endregion
        #region Custom Command / Plugin Events
        public delegate bool OnPhysics(World world, int x, int y, int z, byte type, byte meta);
        public static event OnPhysics WorldPhysicsUpdate;
        public event OnPhysics PhysicsUpdate;
        #endregion

        public Physics(World w) : this(w, PSetting.Hardcore) { }
        public Physics(World w, PSetting setting) : this(w, setting, 250) { }
        public Physics(World w, int speed) : this(w, PSetting.Hardcore, speed) { }
        public Physics(World w, PSetting setting, int speed)
        {
            this.w = w;
            this.setting = setting;
            this.speed = speed;
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
            //Checks.Clear();
            try { physthread.Abort(); physthread.Join(); }
            catch { }
            physthread = null;
            Server.ServerLogger.Log("Physics stopped on " + w.name + ".");
        }

        public void Clear()
        {
            Checks.Clear();
        }

        private void RunLoop()
        {
            while (!Server.s.shuttingDown)
            {
                try
                {
                    if (wait > 0) Thread.Sleep(wait);
                    lastCheck = Checks.Count;
                    if (paused || setting == PSetting.None || lastCheck == 0)
                    {
                        wait = speed;
                        lastUpdate = 0;
                        continue;
                    }

                    DateTime Start = DateTime.Now;
                    Calculate();
                    TimeSpan Took = DateTime.Now - Start;
                    wait = speed - (int)Took.TotalMilliseconds;
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
                byte type;
                Checks.ForEach(delegate(Check C)
                {
                    try
                    {
                        if (C == null) { Checks.Remove(C); return; }
                        type = w.GetBlock(C.x, C.y, C.z);
                        if (Handlers.handlers.ContainsKey(type))
                            if (!(bool)Handlers.handlers[type].DynamicInvoke(this, C))
                                return;
                        C.time = short.MaxValue;
                    }
                    catch (ThreadAbortException) { }
                    catch (Exception e)
                    {
                        Server.ServerLogger.Log("Physics error on " + w.name + "!");
                        Server.ServerLogger.LogError(e);
                        Checks.Remove(C);
                    }
                });
                Checks.RemoveAll(Check => (Check != null && Check.time == short.MaxValue));

                lastUpdate = Updates.Count;
                Updates.ForEach(delegate(Update U)
                {
                    try
                    {
                        if (WorldPhysicsUpdate != null)
                            if (!WorldPhysicsUpdate(w, U.x, U.y, U.z, U.type, U.meta))
                                return;
                        if (PhysicsUpdate != null)
                            if (!PhysicsUpdate(w, U.x, U.y, U.z, U.type, U.meta))
                                return;

                        w.BlockChange(U.x, U.y, U.z, U.type, U.meta);
                    }
                    catch (ThreadAbortException) { }
                    catch
                    {
                        Server.ServerLogger.Log("Physics update error on " + w.name + "!");
                    }
                });
                Updates.Clear();
            }
            catch (Exception e)
            {
                Server.ServerLogger.Log("Physics error on " + w.name + "!");
                Server.ServerLogger.LogError(e);
            }
        }


        public void AddCheck(Check check, bool overRide)
        {
            try
            {
                if (!Checks.Exists(Check => (Check != null && Check.x == check.x && Check.y == check.y && Check.z == check.z)))
                {
                    Checks.Add(check);
                }
                else
                {
                    if (overRide)
                    {
                        lock (Checks)
                        {
                            foreach (Check C in Checks)
                            {
                                if (C.x == check.x && C.y == check.y && C.z == check.z)
                                {
                                    C.meta = check.meta;
                                    break;
                                }
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
        public void AddCheck(Check check)
        {
            AddCheck(check, false);
        }

        public void AddCheck(int x, int y, int z, byte meta, bool overRide)
        {
            AddCheck(new Check(x, y, z, meta), overRide);
        }
        public void AddCheck(int x, int y, int z, byte meta)
        {
            AddCheck(x, y, z, meta, false);
        }
        public void AddCheck(int x, int y, int z, bool overRide)
        {
            AddCheck(x, y, z, 0, overRide);
        }
        public void AddCheck(int x, int y, int z)
        {
            AddCheck(x, y, z, 0, false);
        }

        public void AddChecks(List<Check> checks)
        {
            AddChecks(checks.ToArray());
        }
        public void AddChecks(Check[] checks)
        {
            foreach (Check check in checks)
                AddCheck(check);
        }

        public List<Check> GetChunkChecks(int x, int z)
        {
            return Checks.FindAll(Check => (Check != null && Check.time != short.MaxValue && (Check.x >> 4) == x && (Check.z >> 4) == z));
        }
        public void AddChunkChecks(Check[] checks)
        {
            foreach (Check check in checks)
                Checks.Add(check);
        }
        public void RemoveChunkChecks(int x, int z)
        {
            Checks.RemoveAll(Check => (Check != null && (Check.x >> 4) == x && (Check.z >> 4) == z));
        }

        public bool AddUpdate(Update update, bool overRide = false)
        {
            try
            {
                if (!Updates.Exists(Update => (Update != null && Update.x == update.x && Update.y == update.y && Update.z == update.z)))
                {
                    Updates.Add(update);
                    return true;
                }
                else
                {
                    if (overRide)
                    {
                        Updates.RemoveAll(Update => (Update != null && Update.x == update.x && Update.y == update.y && Update.z == update.z));
                        Updates.Add(update);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Server.ServerLogger.LogError(e);
            }
            return false;
        }

        public void AddUpdate(int x, int y, int z, byte type, byte meta, bool overRide = false)
        {
            AddUpdate(new Update(x, y, z, type, meta), overRide);
        }

        public class Check
        {
            public byte meta;
            public short time;
            public int x, y, z;

            public Check(Point3 a, byte meta) : this((int)a.x, (int)a.y, (int)a.z, meta, 0) { }
            public Check(int x, int y, int z, byte meta) : this(x, y, z, meta, 0) { }
            public Check(int x, int y, int z, byte meta, short time)
            {
                this.time = time;
                this.meta = meta;
                this.x = x; this.y = y; this.z = z;
            }
        }

        public class Update
        {
            public int x, y, z;
            public byte type, meta;

            public Update(int x, int y, int z, byte type, byte meta)
            {
                this.x = x; this.y = y; this.z = z;
                this.type = type;
                this.meta = meta;
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
