/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Ionic.Zlib;
using SMP.Generator;
using SMP.util;

namespace SMP
{
	public partial class World
	{
		public static List<World> worlds = new List<World>();
        public static Chunker chunker = new Chunker();
		public double SpawnX;
		public double SpawnY;
		public double SpawnZ;
		public float SpawnYaw;
		public float SpawnPitch;
		//public string Map_Name; // THE FUCK IS THIS SHIT!?
		public string name;
        private long Seed;
        public long seed { get { return Seed; } set { Seed = value; } }
		public long time;
        public sbyte dimension = 0; // -1: The Nether, 0: The Overworld, 1: The End
        private object genLock = new object();
        private bool initialized = false;
		public System.Timers.Timer timeupdate = new System.Timers.Timer(1000);
        public bool timerunning = false;
        public System.Threading.Thread timeincrement;
        public System.Timers.Timer blockflush = new System.Timers.Timer(20);
        public System.Timers.Timer containerupdate = new System.Timers.Timer(3000);
		private ChunkGen generator;
        public WorldChunkManager chunkManager;
		public Dictionary<Point, Chunk> chunkData;
        public Dictionary<Point, List<BlockChangeData>> blockQueue = new Dictionary<Point, List<BlockChangeData>>();
		//public Dictionary<Point3, Windows> windows = new Dictionary<Point3, Windows>();
        public Dictionary<Point3, Container> containers = new Dictionary<Point3, Container>();
		public List<Point> ToGenerate = new List<Point>();
        public Physics physics;
        public bool Raining = false;
		public byte worldYMax = 128;
        public byte worldOceanHeight;
        public byte worldYMask;
        public byte worldYBits;
        public byte field_35250_b;
        public byte height { get { return worldYMax; } set { worldYMax = value; } }
		public byte LightningRange = 16; //X is chunk offset, a player can be X chunks away from lightning and still see it
        public int ChunkLimit = 1875000;
        public int ChunkHeight = 128; // This is 1-based, not 0-based. Got it?
        #region Custom Command / Plugin Events
		//Custom Command / Plugin Events -------------------------------------------------------------------
		public delegate void OnWorldLoad(World w);
		public static event OnWorldLoad WorldLoad;
		public delegate void OnWorldSave(World w);
		public static event OnWorldSave OnSave;
		public event OnWorldLoad Save;
		public delegate void OnGeneratedChunk(World w, Chunk c, int x, int z);
        public delegate Chunk OnGenerateChunk(World w, int x, int z);
		public static event OnGeneratedChunk WorldGeneratedChunk;
        public event OnGenerateChunk ChunkGenerating;
        public static event OnGenerateChunk WorldGeneratingChunk;
		public event OnGeneratedChunk GeneratedChunk;
        public delegate void OnTimeChange();
        public event OnTimeChange TimeChanged;
        public static event OnTimeChange WorldTimeChanged;
		public delegate void OnBlockChange(int x, int y, int z, byte type, byte meta);
		public event OnBlockChange BlockChanged;
        internal bool cancelchunk;
        internal static bool cancelsave;
		//Custom Command / Plugin Events -------------------------------------------------------------------
		#endregion

        public Point3 SpawnPos
        {
            get
            {
                Point3 pos = new Point3(SpawnX + 0.5, SpawnY, SpawnZ + 0.5);

                /*java.util.Random random = new java.util.Random(seed);
                int i = 0;
                do
                {
                    if (GetFirstUncoveredBlock((int)pos.x, (int)pos.z) == (byte)Blocks.Grass)
                    {
                        break;
                    }
                    pos.x += random.nextInt(64) - random.nextInt(64);
                    pos.z += random.nextInt(64) - random.nextInt(64);
                }
                while (++i < 1000);*/

                pos.y = FindTopSolidBlock((int)pos.x, (int)pos.z);

                return pos;
            }
        }

        public World()
        {
            worldYBits = 7;
            field_35250_b = (byte)(worldYBits + 4);
            worldOceanHeight = (byte)(worldYMax / 2 - 1);
            worldYMask = (byte)(worldYMax - 1);
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.World"/> class and generates 49 chunks.
		/// </summary>
		/// <param name='spawnx'>
		/// Spawnx. The x spawn pos.
		/// </param>
		/// <param name='spawny'>
		/// Spawny. The y spawn pos.
		/// </param>
		/// <param name='spawnz'>
		/// Spawnz. The z spawn pos.
		/// </param>
        public World(double spawnx, double spawny, double spawnz, string name, long seed)
            : this()
        {
            this.name = name;
            this.seed = seed;
            this.SpawnX = spawnx; this.SpawnY = spawny; this.SpawnZ = spawnz;
            chunkData = new Dictionary<Point, Chunk>();
            physics = new Physics(this);
            generator = new GenStandard(this, true);
            chunkManager = new WorldChunkManager(this);
            Logger.Log("Generating " + this.name + "...");

            int cursorH = 25 + this.name.Length;
            float count = 0, total = (Server.ViewDistance * 2 + 1) * (Server.ViewDistance * 2 + 1);
            Console.SetCursorPosition(cursorH, Console.CursorTop - 1);
            Console.Write("0%");

            object derpLock = new object();
            try  // Mono 2.10.2 has Parallel.For(int) and Parallel.ForEach implemented, not sure about 2.8 though. Any version less does not support .NET 4.0
            {
                Parallel.For(((int)SpawnX >> 4) - Server.ViewDistance, ((int)SpawnX >> 4) + Server.ViewDistance + 1, delegate(int x)
                {
                    Parallel.For(((int)SpawnZ >> 4) - Server.ViewDistance, ((int)SpawnZ >> 4) + Server.ViewDistance + 1, delegate(int z)
                    {
                        LoadChunk(x, z, false, false);
                        lock (derpLock)
                        {
                            Console.SetCursorPosition(cursorH, Console.CursorTop);
                            count++; Console.Write((int)((count / total) * 100) + "%");
                        }
                    });
                    //Logger.Log(x + " Row Generated.");
                });
            }
            catch (NotImplementedException)
            {
                for (int x = ((int)SpawnX >> 4) - Server.ViewDistance; x < ((int)SpawnX >> 4) + Server.ViewDistance + 1; x++)
                {
                    for (int z = ((int)SpawnZ >> 4) - Server.ViewDistance; z < ((int)SpawnZ >> 4) + Server.ViewDistance + 1; z++)
                    {
                        LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(cursorH, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
                    }
                    //Logger.Log(x + " Row Generated.");
                }
            }

            Console.Write("\r");

            Logger.Log("Look distance = " + Server.ViewDistance);

            Init();
            physics.Start();

            if (World.WorldLoad != null)
                World.WorldLoad(this);
        }
       
		public static World Find(string name)
		{
			World tempLevel = null; bool returnNull = false;

            foreach (World world in worlds.ToArray())
            {
                if (world.name.ToLower() == name) return world;
                if (world.name.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempLevel == null) tempLevel = world;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempLevel != null) return tempLevel;
            return null;
		}

        #region Chunk Saving/Loading
        public void LoadChunk(int x, int z, bool thread = true, bool threadLoad = true, bool generate = true)
        {
            LoadChunk(x, z, this, thread, threadLoad, generate);
        }
        public static void LoadChunk(int x, int z, World w, bool thread = true, bool threadLoad = true, bool generate = true)
        {
            Chunk ch = Chunk.Load(x, z, w, thread, threadLoad, generate, false);
            if (ch != null)
            {
                Point pt = new Point(x, z);
                lock (w.chunkData)
                    if (!w.chunkData.ContainsKey(pt))
                        w.chunkData.Add(pt, ch);
                ch.PostLoad(w);
            }
        }

        public void UnloadChunk(int x, int z)
        {
            UnloadChunk(x, z, this);
        }
        public static void UnloadChunk(int x, int z, World w)
        {
            Point pt = new Point(x, z);
            try { if (w.chunkData.ContainsKey(pt) && (w.chunkData[pt].generating || w.ChunkPopulatingCheck(x, z))) return; }
            catch { return; }

            SaveChunk(x, z, w);
            if (((int)w.SpawnX >> 4) != x || ((int)w.SpawnZ >> 4) != z) // Don't unload the spawn chunks!
            {
                w.physics.RemoveChunkChecks(x, z);
                foreach (KeyValuePair<Point3, Container> kvp in w.containers.Where(KVP => (((int)KVP.Key.x >> 4) == x && ((int)KVP.Key.z >> 4) == z)).ToList())
                    w.containers.Remove(kvp.Key);
                lock (w.chunkData)
                    if (w.chunkData.ContainsKey(pt))
                    {
                        w.chunkData[pt].Dispose();
                        w.chunkData.Remove(pt);
                    }
            }
        }

        public void SaveChunk(int x, int z)
        {
            SaveChunk(x, z, this);
        }
        public static void SaveChunk(int x, int z, World w)
        {
            Chunk ch = Chunk.GetChunk(x, z, w);
            if (ch == null || !ch.Dirty) return;
            ch.Save(w);
        }
        #endregion

        #region Item Drops
        public void DropItem(int x, int y, int z, short id)
        {
            DropItem(x, y, z, id, 0, 1, this);
        }
        public void DropItem(int x, int y, int z, short id, short meta)
        {
            DropItem(x, y, z, id, meta, 1, this);
        }
        public void DropItem(int x, int y, int z, short id, short meta, byte count)
        {
            DropItem(x, y, z, id, meta, count, this);
        }
        public static void DropItem(int x, int y, int z, short id, short meta, byte count, World w)
        {
            if (!FindBlocks.ValidItem(id)) return;
            if (Server.mode == 0)
            {
                Item item = new Item(id, w) { count = count, meta = meta, pos = new double[3] { x + .5, y + .5, z + .5 }, OnGround = true };
                item.e.UpdateChunks(true, false);
            }
        }
        #endregion

        public Container GetBlockContainer(int x, int y, int z)
        {
            try
            {
                if (Chunk.GetChunk(x >> 4, z >> 4, this) != null)
                    Chunk.GetChunk(x >> 4, z >> 4, this)._dirty = true; // Temporary until we find a good way to make the chunk dirty only when the container is edited.

                Point3 point = new Point3(x, y, z);
                if (containers.ContainsKey(point)) return containers[point];
                switch (GetBlock(x, y, z))
                {
                    case (byte)Blocks.Chest:
                        containers.Add(point, new ContainerChest(this, point));
                        break;
                    default: return null;
                }
                return containers[point];
            }
            catch { return null; }
        }
        public Container GetBlockContainer(Point3 pos)
        {
            return GetBlockContainer((int)pos.x, (int)pos.y, (int)pos.z);
        }

        public static World LoadLVL(string filename)
        {
            //TODO make loading/saving better.
            //if (WorldLoad != null)
            //	WorldLoad(this);
            World w = new World() { chunkData = new Dictionary<Point, Chunk>(), name = filename };
            Logger.Log("Loading " + w.name + "...");

            /*using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream(filename + "/" + filename + ".blocks", FileMode.Open))
                {
                    byte[] comp;
                    ms.SetLength(fs.Length);
                    fs.Read(ms.GetBuffer(), 0, (int)fs.Length);
                    DecompressData(ms.GetBuffer(), out comp);
                    ms.Write(comp, 0, comp.Length);
                }
                byte[] bytes = ms.ToArray();
                long chunkcount = ms.Length / 32776;
                
				if(!Program.RunningInMono()) //mono doesn't have Parallel.For(long, long, Syatem.Action<long>) implemented
				{
					Parallel.For(0, chunkcount, i =>
	                {
	                    try
	                    {
	                        int block = (int)i * 32776;
	                        int x = BitConverter.ToInt32(bytes, 0 + block);
	                        int z = BitConverter.ToInt32(bytes, 4 + block);
	                        Chunk c = new Chunk(x, z);
	                        Array.Copy(bytes, 8 + block, c.blocks, 0, 32768);
	                        c.RecalculateLight();
	                        c.SpreadLight();
                            lock (w.chunkData) 
	                            if (!w.chunkData.ContainsKey(new Point(x, z)))
	                                w.chunkData.Add(new Point(x, z), c);
	                    }
	                    catch (Exception ex)
	                    {
	                        Logger.Log(ex.ToString());
	                    }
	                });
				}
				else{
					for (int i = 0; i < chunkcount; i++)
	                {
	                    try
	                    {
	                        int block = (int)i * 32776;
	                        int x = BitConverter.ToInt32(bytes, 0 + block);
	                        int z = BitConverter.ToInt32(bytes, 4 + block);
	                        Chunk c = new Chunk(x, z);
	                        Array.Copy(bytes, 8 + block, c.blocks, 0, 32768);
	                        c.RecalculateLight();
	                        c.SpreadLight();
                            lock (w.chunkData) 
	                            if (!w.chunkData.ContainsKey(new Point(x, z)))
	                                w.chunkData.Add(new Point(x, z), c);
	                    }
	                    catch (Exception ex)
	                    {
	                        Logger.Log(ex.ToString());
	                    }
	                }
				}
            }*/

            try
            {
                using (StreamReader sw = new StreamReader(filename + "/" + filename + ".ini"))
                {
                    w.seed = long.Parse(sw.ReadLine());
                    w.SpawnX = int.Parse(sw.ReadLine());
                    w.SpawnY = int.Parse(sw.ReadLine());
                    w.SpawnZ = int.Parse(sw.ReadLine());
                    w.ChunkLimit = int.Parse(sw.ReadLine());
                    w.time = long.Parse(sw.ReadLine());
                    w.dimension = sbyte.Parse(sw.ReadLine());
                }
            }
            catch { /*Logger.Log("Error loading world configuration!");*/ }

            w.physics = new Physics(w);
            w.generator = new GenStandard(w, true);
            w.chunkManager = new WorldChunkManager(w);

            int cursorH = 22 + w.name.Length;
            float count = 0, total = (Server.ViewDistance * 2 + 1) * (Server.ViewDistance * 2 + 1);
            Console.SetCursorPosition(cursorH, Console.CursorTop - 1);
            Console.Write("0%");

            object derpLock = new object();
            try
            {
                Parallel.For(((int)w.SpawnX >> 4) - Server.ViewDistance, ((int)w.SpawnX >> 4) + Server.ViewDistance + 1, x =>
                {
                    Parallel.For(((int)w.SpawnZ >> 4) - Server.ViewDistance, ((int)w.SpawnZ >> 4) + Server.ViewDistance + 1, z =>
                    {
                        w.LoadChunk(x, z, false, false);
                        lock (derpLock)
                        {
                            Console.SetCursorPosition(cursorH, Console.CursorTop);
                            count++; Console.Write((int)((count / total) * 100) + "%");
                        }
                    });
                });
            }
            catch (NotImplementedException)
            {
                for (int x = ((int)w.SpawnX >> 4) - Server.ViewDistance; x < ((int)w.SpawnX >> 4) + Server.ViewDistance + 1; x++)
                {
                    for (int z = ((int)w.SpawnZ >> 4) - Server.ViewDistance; z < ((int)w.SpawnZ >> 4) + Server.ViewDistance + 1; z++)
                    {
                        w.LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(cursorH, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
                    }
                }
            }

            Console.WriteLine();

            World.worlds.Add(w);
            Logger.Log(filename + " Loaded.");
            Logger.Log("Look distance = " + Server.ViewDistance);

            w.Init();
            w.physics.Start();

            if (World.WorldLoad != null)
                World.WorldLoad(w);

            return w;
        }

        public void SaveLVL(bool silent = false)
		{
			World.SaveLVL(this, silent);
		}
		
        public static void SaveLVL(World w, bool silent = false)
        {
            if (w.Save != null)
                w.Save(w);
            if (World.OnSave != null)
                World.OnSave(w);
            if (cancelsave)
            {
                cancelsave = false;
                return;
            }
            //TODO Save files
            if (!Directory.Exists(w.name)) Directory.CreateDirectory(w.name);
            using (StreamWriter sw = new StreamWriter(w.name + "/" + w.name + ".ini"))
            {
                sw.WriteLine(w.seed);
                sw.WriteLine(w.SpawnX);
                sw.WriteLine(w.SpawnY);
                sw.WriteLine(w.SpawnZ);
                sw.WriteLine(w.ChunkLimit);
                sw.WriteLine(w.time);
                sw.WriteLine(w.dimension);
            }
            /*using (MemoryStream blocks = new MemoryStream())
            {
                lock (w.chunkData)
                    foreach (Chunk ch in w.chunkData.Values)
                    {
                        //if (ch == null) continue;
                        //File.WriteAllBytes(w.name + "/" + ch.x + " " + ch.z, ch.blocks);
                        blocks.Write(BitConverter.GetBytes(ch.x), 0, 4);
                        blocks.Write(BitConverter.GetBytes(ch.z), 0, 4);
                        blocks.Write(ch.blocks, 0, ch.blocks.Length);
                    }
                byte[] bytes;
                CompressData(blocks.ToArray(), out bytes);
                using (FileStream fs = new FileStream(w.name + "/" + w.name + ".blocks", FileMode.Create))
                {
                    fs.Write(bytes, 0, (int)bytes.Length);
                }
            }*/

            lock (w.chunkData)
            {
                try
                {
                    Parallel.ForEach(w.chunkData.Keys, delegate(Point pt)
                    {
                        w.SaveChunk(pt.x, pt.z);
                    });
                }
                catch (NotImplementedException)
                {
                    foreach (Point pt in w.chunkData.Keys)
                        w.SaveChunk(pt.x, pt.z);
                }
            }
            if (!silent) Logger.Log(w.name + " Saved.");
        }

        #region Useless compression methods, use byte[].Compress() and byte[].Decompress()
        /*public static void CompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZlibStream outZStream = new ZlibStream(outMemoryStream, CompressionMode.Compress, CompressionLevel.BestCompression, true))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outData = outMemoryStream.ToArray();
            }
        }

        public static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZlibStream outZStream = new ZlibStream(outMemoryStream, CompressionMode.Decompress))
            using (MemoryStream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outData = outMemoryStream.ToArray();
            }
        }

        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }*/
        #endregion

        public void Init()
        {
            if (initialized)
                throw new Exception("World aready initialized.");
            initialized = true;
            timeupdate.Elapsed += delegate { Player.players.ForEach(delegate(Player p) { if (p.MapLoaded && p.level == this) p.SendTime(); }); };
            timeupdate.Start();
            timeincrement = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                int speed = 50, wait = speed;
                while (!Server.s.shuttingDown)
                {
                    try
                    {
                        if (!timerunning) { System.Threading.Thread.Sleep(speed); continue; }
                        if (wait > 0) System.Threading.Thread.Sleep(wait);

                        DateTime Start = DateTime.Now;

                        time++; if (time > 24000) time = 0;
                        if (TimeChanged != null)
                            TimeChanged();
                        if (WorldTimeChanged != null)
                            WorldTimeChanged();

                        TimeSpan Took = DateTime.Now - Start;
                        wait = speed - (int)Took.TotalMilliseconds;
                    }
                    catch { wait = speed; }
                }
            }));
            timeincrement.Start();
            timerunning = true;
            blockflush.Elapsed += delegate { FlushBlockChanges(); };
            blockflush.Start();
            containerupdate.Elapsed += delegate { UpdateContainers(); };
            containerupdate.Start();
            InitializeTimers();
        }
        /// <summary>
        /// Set wether its raining or not
        /// </summary>
        /// <param name="rain">is it raining?</param>
        public void Rain(bool rain)
		{
            IsRaining = rain;
			foreach (Player p in Player.players.ToArray())
			    if (p.LoggedIn && p.MapLoaded)
				    p.SendRain(rain);
		}
        /// <summary>
        /// Shoot a lighting bolt
        /// </summary>
        /// <param name="x">The x cord. of the lighting bolt</param>
        /// <param name="y">The y cord. of the lighting bolt</param>
        /// <param name="z">The z cord. of the lighting bolt</param>
		public void Lightning(int x, int y, int z)
		{
			Entity e = new Entity(true);
			int distance = 16 * LightningRange;

			foreach (Player p in Player.players.ToArray())
				if (p.LoggedIn && p.MapLoaded)
					if (p.pos.mdiff(new Point3(x, y, z)) <= distance)
                        p.SendLightning(x * 32, y * 32, z * 32, e.id);

            if (BlockData.IsSolid(GetBlock(x, y - 1, z)))
                BlockChange(x, y, z, (byte)Blocks.Fire, 0);
		}
        /// <summary>
        /// Does a chunk exist?
        /// </summary>
        /// <param name="x">The x cord. of the chunk</param>
        /// <param name="z">The z cord. of the chunk</param>
        /// <returns></returns>
        public bool ChunkExists(int x, int z)
        {
            Point pt = new Point(x, z);
            if (chunkData.ContainsKey(pt)) return true;
            //Console.WriteLine(chunkData.ContainsKey(pt) && chunkData[pt].generated && chunkData[pt].populated);
            return File.Exists(Chunk.CreatePath(this, x, z));
        }
        /// <summary>
        /// Generate a chunk at x and z
        /// </summary>
        /// <param name="x">The x cord. to generate</param>
        /// <param name="z">The z cord. to generate</param>
        /// <returns>Returns the generated chunk</returns>
		public Chunk GenerateChunk(int x, int z)
		{
            Chunk c = null; Point pt = new Point(x, z);

            if (ChunkGenerating != null)
                c = ChunkGenerating(this, x, z);
            if (WorldGeneratingChunk != null)
                c = WorldGeneratingChunk(this, x, z);
            if (cancelchunk)
            {
                cancelchunk = false;
                if (c != null) return c;
                else Logger.Log("[WARNING] Was told to cancel chunk generating but chunk was null, the event will not cancel");
            }


            lock (chunkData)
            {
                if (chunkData.ContainsKey(pt)) { c = chunkData[pt]; }
                else c = new Chunk(x, z);
            }

            c.generating = true;
            if (x >= -1875000 && z >= -1875000 && x < 1875000 && z < 1875000)
            {
                generator.Generate(c);
                c.RecalculateLight();
            }
            c.generated = true;
            c.PostGenerate(this);
            c._dirty = true;
            c.generating = false;

            if (GeneratedChunk != null)
                GeneratedChunk(this, c, x, z);
            if (WorldGeneratedChunk != null)
                WorldGeneratedChunk(this, c, x, z);

            return c;
		}
        public void PopulateChunk(int x, int z)
        {
            Chunk c; Point pt = new Point(x, z);

            lock (chunkData)
            {
                if (!chunkData.ContainsKey(pt)) return;
                c = chunkData[pt];
                c.populating = true;
            }

            //Console.WriteLine((x << 4) + "," + (z << 4));
            if (x >= -1875000 && z >= -1875000 && x < 1875000 && z < 1875000)
                generator.Populate(c);
            c.populated = true;
            c.PostPopulate(this);
            c._dirty = true;
            c.populating = false;
        }

        public void UpdateContainers()
        {
            try
            {
                Container c;
                foreach (Point3 pos in containers.Keys.ToArray())
                {
                    c = containers[pos];
                    c.UpdateState();
                }
            }
            catch { }
        }

        public bool ChunkPopulatingCheck(int x, int z)
        {
            lock (chunkData)
            {
                Point pt = new Point(x, z);
                if (chunkData.ContainsKey(pt) && chunkData[pt].populating)
                    return true;
                pt.x--;
                if (chunkData.ContainsKey(pt) && chunkData[pt].populating)
                    return true;
                pt.z--;
                if (chunkData.ContainsKey(pt) && chunkData[pt].populating)
                    return true;
                pt.x++;
                if (chunkData.ContainsKey(pt) && chunkData[pt].populating)
                    return true;
                return false;
            }
        }

        public void QueueBlockChange(int x, int y, int z, byte type, byte meta)
        {
            Point pt = new Point(x >> 4, z >> 4);
            BlockChangeData data = new BlockChangeData(x & 0xf, y, z & 0xf, type, meta);

            lock (blockQueue)
            {
                if (!blockQueue.ContainsKey(pt))
                    blockQueue.Add(pt, new List<BlockChangeData>());
                blockQueue[pt].RemoveAll(bl => (bl.x == data.x && bl.y == data.y && bl.z == data.z)); // We don't want multiple with the same coordinates.
                blockQueue[pt].Add(data);
            }
        }
        public void FlushBlockChanges()
        {
            try
            {
                int count = blockQueue.Count;
                if (count < 1) return;

                Dictionary<Point, List<BlockChangeData>> tempQueue;
                lock (blockQueue)
                {
                    tempQueue = new Dictionary<Point, List<BlockChangeData>>(blockQueue);
                    blockQueue.Clear();
                }

                foreach (KeyValuePair<Point, List<BlockChangeData>> kvp in tempQueue)
                {
                    foreach (Player p in Player.players.ToArray())
                    {
                        if (!p.MapLoaded || p.level != this || !p.VisibleChunks.Contains(kvp.Key)) continue;
                        p.SendMultiBlockChange(kvp.Key, kvp.Value.ToArray());
                    }
                }

                tempQueue.Clear();
                tempQueue = null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        /// <summary>
        /// Place a block at x, y, z
        /// </summary>
        /// <param name="x">The x cord. of the block</param>
        /// <param name="y">The y cord. of the block</param>
        /// <param name="z">The z cord. of the block</param>
        /// <param name="type">The type</param>
        /// <param name="meta"></param>
        /// <param name="phys"></param>
		public void BlockChange(int x, int y, int z, byte type, byte meta, bool phys = true)
		{
            try
            {
                if (y < 0 || y > ChunkHeight - 1) return;
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return;
                int cx = x >> 4, cz = z >> 4; Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return;
                byte oldBlock = chunk.SGB(x & 0xf, y, z & 0xf); byte oldMeta = chunk.GetMetaData(x & 0xf, y, z & 0xf);
                chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
                // TODO: Put lighting updates in a separate thread.
                chunk.QuickRecalculateLight(x & 0xf, y, z & 0xf);
                SpreadLight(x, y, z);
                if (phys) physics.BlockUpdate(x, y, z, oldBlock, oldMeta);
                if (BlockChanged != null)
                    BlockChanged(x, y, z, type, meta);
                if (type != oldBlock || meta != oldMeta) // Don't send block change if it's the same.
                    QueueBlockChange(x, y, z, type, meta);
            }
            catch { }
		}
        public void SetBlock(int x, int y, int z, byte type)
        {
            SetBlock(x, y, z, type, 0);
        }
        public void SetBlock(int x, int y, int z, byte type, byte meta)
        {
            try
            {
                if (y < 0 || y > ChunkHeight - 1) return;
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return;
                int cx = x >> 4, cz = z >> 4; Chunk chunk = Chunk.GetChunk(cx, cz, this, true);
                if (chunk == null) return;
                byte oldBlock = chunk.SGB(x & 0xf, y, z & 0xf); byte oldMeta = chunk.GetMetaData(x & 0xf, y, z & 0xf);
                chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
                chunk.QuickRecalculateLight(x & 0xf, y, z & 0xf);
                if (type != oldBlock || meta != oldMeta) // Don't send block change if it's the same.
                    /*QueueBlockChange(x, y, z, type, meta)*/; // Causes client freeze during level generation?
            }
            catch { }
        }
        /// <summary>
        /// Get a block at x, y, z
        /// </summary>
        /// <param name="x">The x cord.</param>
        /// <param name="y">The y cord.</param>
        /// <param name="z">The z cord.</param>
        /// <returns>The block ID</returns>
		public byte GetBlock(int x, int y, int z)
		{
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return 0;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return 0;
                return chunk.SGB(x & 0xf, y, z & 0xf);
            }
            catch { return 0; }
		}
		public byte GetMeta(int x, int y, int z)
		{
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return 0;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return 0;
                return chunk.GetMetaData(x & 0xf, y, z & 0xf);
            }
            catch { return 0; }
		}
		public void SetMeta(int x, int y, int z, byte data)
		{
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return;
                chunk.SetMetaData(x & 0xf, y, z & 0xf, data);
            }
            catch { return; }
		}
        public ushort GetExtra(int x, int y, int z)
        {
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return 0;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return 0;
                return chunk.GetExtraData(x & 0xf, y, z & 0xf);
            }
            catch { return 0; }
        }
        public void SetExtra(int x, int y, int z, ushort data)
        {
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return;
                chunk.SetExtraData(x & 0xf, y, z & 0xf, data);
            }
            catch { return; }
        }
        public void UnsetExtra(int x, int y, int z)
        {
            try
            {
                if (x < -30000000 || z < -30000000 || x >= 30000000 || z >= 30000000) return;
                int cx = x >> 4, cz = z >> 4;
                Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return;
                chunk.UnsetExtraData(x & 0xf, y, z & 0xf);
            }
            catch { return; }
        }

        public string[] GetSign(int x, int y, int z)
        {
            string[] text = new string[4];
            for (int i = 0; i < 4; i++)
                text[i] = Server.SQLiteDB.ExecuteScalar(String.Format("SELECT Line{0} FROM Sign WHERE X = {1} AND Y = {2} AND Z = {3} AND World = '{4}'", i + 1, x, y, z, this.name)); ;
            return text;
        }
        public void SetSign(int x, int y, int z, params string[] text)
        {
            if (text.Length != 4)
                throw new ArgumentException("Text must be 4 strings.");

            Server.SQLiteDB.ExecuteNonQuery(String.Format("DELETE FROM Sign WHERE X = {0} AND Y = {1} AND Z = {2} AND World = '{3}'", x, y, z, this.name));
            Server.SQLiteDB.ExecuteNonQuery(String.Format("INSERT INTO Sign VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}')", x, y, z, this.name, text[0], text[1], text[2], text[3]));

            Player.GlobalUpdateSign(this, x, (short)y, z, text);
        }
        public void UnsetSign(int x, int y, int z)
        {
            Server.SQLiteDB.ExecuteNonQuery(String.Format("DELETE FROM Sign WHERE X = {0} AND Y = {1} AND Z = {2} AND World = '{3}'", x, y, z, this.name));

            Player.GlobalUpdateSign(this, x, (short)y, z, String.Empty, String.Empty, String.Empty, String.Empty);
        }

        public void GrowTree(int x, int y, int z, byte type, java.util.Random random)
        {
            BlockChange(x, y, z, 0, 0);
            switch (type)
            {
                case (byte)Tree.Normal:
                    if (Entity.randomJava.nextInt(10) == 0)
                        new WorldGenBigTree(true).generate(this, random, x, y, z);
                    else
                        new WorldGenTrees(true).generate(this, random, x, y, z);
                    break;
                case (byte)Tree.Spruce:
                    if (Entity.randomJava.nextInt(3) == 0)
                        new WorldGenTaiga1().generate(this, random, x, y, z);
                    else
                        new WorldGenTaiga2(true).generate(this, random, x, y, z);
                    break;
                case (byte)Tree.Birch:
                    new WorldGenForest(true).generate(this, random, x, y, z);
                    break;
            }
        }

        public int GetFirstUncoveredBlock(int x, int z)
        {
            int y; for (y = 63; !IsAirBlock(x, y + 1, z); y++) ;
            return GetBlock(x, y, z);
        }

        public int GetHeightValue(int x, int z)
        {
            try
            {
                if (x < unchecked((int)0xfe363c80) || z < unchecked((int)0xfe363c80) || x >= 0x1c9c380 || z >= 0x1c9c380)
                {
                    return 0;
                }
                else
                {
                    Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                    if (chunk == null) return 0;
                    return chunk.GetHeightValue(x & 0xf, z & 0xf);
                }
            }
            catch { return 0; }
        }

        private byte GetFullLightValue(int x, int y, int z)
        {
            try
            {
                Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                if (chunk == null) return 0;
                byte skylight = chunk.GetSkyLight(x & 0xf, y, z & 0xf);
                if (skylight > 0) skylight--;
                return Math.Max(skylight, chunk.GetBlockLight(x & 0xf, y, z & 0xf));
            }
            catch { return 0; }
        }

        private byte GetBlockLightValue(int x, int y, int z)
        {
            try
            {
                Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                if (chunk == null) return 0;
                return chunk.GetBlockLight(x & 0xf, y, z & 0xf);
            }
            catch { return 0; }
        }

        private byte GetSkyLightValue(int x, int y, int z)
        {
            try
            {
                Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                if (chunk == null) return 0;
                return chunk.GetSkyLight(x & 0xf, y, z & 0xf);
            }
            catch { return 0; }
        }

        public bool IsAirBlock(int x, int y, int z)
        {
            return GetBlock(x, y, z) == 0;
        }
        public bool CanBlockSeeSky(int x, int y, int z)
        {
            try
            {
                return Chunk.GetChunk(x >> 4, z >> 4, this).CanBlockSeeSky(x & 0xf, y, z & 0xf);
            }
            catch { return false; }
        }
        public bool CanBlockStay(byte b, int x, int y, int z)
        {
            byte b2;
            switch (b)
            {
                // TODO: Light checking on stuff once lighting works correctly.
                case (byte)Blocks.FlowerRose:
                case (byte)Blocks.FlowerDandelion:
                case (byte)Blocks.TallGrass:
                    b2 = GetBlock(x, y - 1, z);
                    return (b2 == (byte)Blocks.Grass || b2 == (byte)Blocks.Dirt || b2 == (byte)Blocks.FarmLand) && CanBlockSeeSky(x, y, z);
                case (byte)Blocks.DeadShrubs:
                    b2 = GetBlock(x, y - 1, z);
                    return b2 == (byte)Blocks.Sand && CanBlockSeeSky(x, y, z);
                case (byte)Blocks.SugarCane:
                    return CanPlaceAt(b, x, y, z);
                case (byte)Blocks.Cactus:
                    if (BlockData.IsSolid(GetBlock(x - 1, y, z)))
                    {
                        return false;
                    }
                    if (BlockData.IsSolid(GetBlock(x + 1, y, z)))
                    {
                        return false;
                    }
                    if (BlockData.IsSolid(GetBlock(x, y, z - 1)))
                    {
                        return false;
                    }
                    if (BlockData.IsSolid(GetBlock(x, y, z + 1)))
                    {
                        return false;
                    } else
                    {
                        int l = GetBlock(x, y - 1, z);
                        return l == (byte)Blocks.Cactus || l == (byte)Blocks.Sand;
                    }
                case (byte)Blocks.MushroomBrown:
                case (byte)Blocks.MushroomRed:
                    if (y < 0 || y >= worldYMax)
                        return false;
                    else
                    {
                        b2 = GetBlock(x, y - 1, z);
                        return b2 == (byte)Blocks.Mycelium || (GetFullLightValue(x, y, z) < 13 && BlockData.IsOpaqueCube(b2));
                    }
                case (byte)Blocks.LilyPad:
                    if (y < 0 || y >= worldYMax)
                        return false;
                    else
                        return BlockData.BlockMaterial(GetBlock(x, y - 1, z)) == Material.Water && GetMeta(x, y - 1, z) == 0;
            }
            return true;
        }

        public bool CanPlaceAt(byte b, int x, int y, int z)
        {
            byte l, l2;
            switch (b)
            {
                case (byte)Blocks.SugarCane:
                    l = GetBlock(x, y - 1, z);
                    if (l == b)
                    {
                        return true;
                    }
                    if (l != (byte)Blocks.Grass && l != (byte)Blocks.Dirt && l != (byte)Blocks.Sand)
                    {
                        return false;
                    }
                    if (BlockData.BlockMaterial(GetBlock(x - 1, y - 1, z)) == Material.Water)
                    {
                        return true;
                    }
                    if (BlockData.BlockMaterial(GetBlock(x + 1, y - 1, z)) == Material.Water)
                    {
                        return true;
                    }
                    if (BlockData.BlockMaterial(GetBlock(x, y - 1, z - 1)) == Material.Water)
                    {
                        return true;
                    }
                    return BlockData.BlockMaterial(GetBlock(x, y - 1, z + 1)) == Material.Water;
                case (byte)Blocks.Pumpkin:
                case (byte)Blocks.JackOLantern:
                    l = GetBlock(x, y, z);
                    return (l == 0 || BlockData.IsGroundCover(l)) && IsNormalCube(x, y - 1, z);
                case (byte)Blocks.MushroomBrown:
                case (byte)Blocks.MushroomRed:
                    return BlockData.IsOpaqueCube(GetBlock(x, y - 1, z));
                case (byte)Blocks.LilyPad:
                    l = GetBlock(x, y, z);
                    l2 = GetBlock(x, y - 1, z);
                    return (l == 0 || BlockData.IsGroundCover(l)) && (l2 == (byte)Blocks.SWater || l2 == (byte)Blocks.AWater);
                case (byte)Blocks.Snow:
                    l = GetBlock(x, y - 1, z);
                    if(l == 0 || !BlockData.IsOpaqueCube(l))
                    {
                        return false;
                    } else
                    {
                        return BlockData.IsSolid(l);
                    }
            }
            return true;
        }
        public bool IsNormalCube(int x, int y, int z)
        {
            byte b = GetBlock(x, y, z);
            return BlockData.IsOpaqueCube(b) && BlockData.IsCube(b);
        }

        public int FindTopSolidBlock(int x, int z)
        {
            try
            {
                Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                int k = 127;
                if (chunk == null) return k;
                x &= 0xf;
                z &= 0xf;
                while (k > 0)
                {
                    byte l = chunk.SGB(x, k, z);
                    if (l == 0 || !BlockData.IsSolid(l) || BlockData.BlockMaterial(l) == Material.Leaves)
                    {
                        k--;
                    }
                    else
                    {
                        return k + 1;
                    }
                }
                return -1;
            }
            catch { return -1; }
        }

        public int GetTopSolidOrLiquidBlock(int x, int z)
        {
            try
            {
                Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
                if (chunk == null) return 0;
                return chunk.func_35631_c(this, x & 0xf, z & 0xf);
            }
            catch { return 0; }
        }

        public Material GetBlockMaterial(int x, int y, int z)
        {
            return BlockData.BlockMaterial(GetBlock(x, y, z));
        }

        public bool func_40210_p(int x, int y, int z)
        {
            return func_40213_b(x, y, z, false);
        }

        public bool func_40217_q(int x, int y, int z)
        {
            return func_40213_b(x, y, z, true);
        }

        public bool func_40213_b(int x, int y, int z, bool flag)
        {
            float f = chunkManager.func_40579_a(x, y, z);
            if (f > 0.15F)
            {
                return false;
            }
            if (y >= 0 && y < worldYMax && GetBlockLightValue(x, y, z) < 10)
            {
                int l = GetBlock(x, y, z);
                if ((l == (byte)Blocks.SWater || l == (byte)Blocks.AWater) && GetMeta(x, y, z) == 0)
                {
                    if (!flag)
                    {
                        return true;
                    }
                    bool flag1 = true;
                    if (flag1 && GetBlockMaterial(x - 1, y, z) != Material.Water)
                    {
                        flag1 = false;
                    }
                    if (flag1 && GetBlockMaterial(x + 1, y, z) != Material.Water)
                    {
                        flag1 = false;
                    }
                    if (flag1 && GetBlockMaterial(x, y, z - 1) != Material.Water)
                    {
                        flag1 = false;
                    }
                    if (flag1 && GetBlockMaterial(x, y, z + 1) != Material.Water)
                    {
                        flag1 = false;
                    }
                    if (!flag1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool func_40215_r(int x, int y, int z)
        {
            float f = chunkManager.func_40579_a(x, y, z);
            if (f > 0.15F)
            {
                return false;
            }
            if (y >= 0 && y < worldYMax && GetBlockLightValue(x, y, z) < 10)
            {
                int l = GetBlock(x, y - 1, z);
                int i1 = GetBlock(x, y, z);
                if (i1 == 0 && CanPlaceAt((byte)Blocks.Snow, x, y, z) && l != 0 && l != (byte)Blocks.Ice && BlockData.IsSolid((byte)l))
                {
                    return true;
                }
            }
            return false;
        }
	}


    // This holds data for the Multi Block Change stuff!
    public struct BlockChangeData
    {
        public int x, y, z;
        public byte type, meta;

        public BlockChangeData(int x, int y, int z, byte type, byte meta)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.type = type;
            this.meta = meta;
        }
    }
}

