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
using zlib;

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
		public string Map_Name;
		public string name;
        private long Seed;
        public long seed { get { return Seed; } set { Seed = value; } }
		public long time;
        private object genLock = new object();
		public System.Timers.Timer timeupdate = new System.Timers.Timer(1000);
        public System.Timers.Timer blockflush = new System.Timers.Timer(20);
		public ChunkGen generator;
        public WorldChunkManager chunkManager;
		public Dictionary<Point, Chunk> chunkData;
        public Dictionary<Point, List<BlockChangeData>> blockQueue = new Dictionary<Point, List<BlockChangeData>>();
		public Dictionary<Point3, Windows> windows = new Dictionary<Point3, Windows>();
		public List<Point> ToGenerate = new List<Point>();
        public DateTime lastBlockChange = new DateTime(1);
        public Physics physics;
        public bool Raining = false;
		public byte height = 128;
		public byte LightningRange = 16; //X is chunk offset, a player can be X chunks away from lightning and still see it
        public int ChunkLimit = int.MaxValue;
        public int ChunkHeight = 128; // This is 1-based, not 0-based. Got it?
        #region Custom Command / Plugin Events
		//Custom Command / Plugin Events -------------------------------------------------------------------
		public delegate void OnWorldLoad(World w);
		public static event OnWorldLoad WorldLoad;
		public delegate void OnWorldSave(World w);
		public static event OnWorldSave OnSave;
		public event OnWorldLoad Save;
		public delegate void OnGenerateChunk(World w, Chunk c, int x, int z);
		public static event OnGenerateChunk WorldGenerateChunk;
		public event OnGenerateChunk GeneratedChunk;
		public delegate void OnBlockChange(int x, int y, int z, byte type, byte meta);
		public event OnBlockChange BlockChanged;
		//Custom Command / Plugin Events -------------------------------------------------------------------
		#endregion

        public World() { }
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
		public World (double spawnx, double spawny, double spawnz, string name, long seed)
		{
            this.seed = seed;
			chunkData = new Dictionary<Point, Chunk>();
            physics = new Physics(this);
            generator = new GenStandard(this, true);
            chunkManager = new WorldChunkManager(this);
            this.name = name;
			Server.Log("Generating...");

            float count = 0, total = 7 * 7;
            Console.SetCursorPosition(24, Console.CursorTop - 1);
            Console.Write("0%");

			try  // Mono 2.10.2 has Parallel.For(int) and Parallel.ForEach implemented, not sure about 2.8 though. Any version less does not support .NET 4.0
			{
	            Parallel.For(-3, 4, delegate(int x)
	            {
	                Parallel.For(-3, 4, delegate(int z)
	                {
                        LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(24, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
	                });
	                //Server.Log(x + " Row Generated.");
	            });
			}
			catch(NotImplementedException)
			{
				for (int x = -3; x < 4; x++)
				{
				    for (int z = -3; z < 4; z++)
				    {
                        LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(24, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
				    }
			    	//Server.Log(x + " Row Generated.");
				}		
			}

            Console.Write("\r");

            Server.Log("Look distance = 3");
			this.SpawnX = spawnx; this.SpawnY = spawny; this.SpawnZ = spawnz;
			timeupdate.Elapsed += delegate {
				time += 20;
				if (time > 24000)
					time = 0;
				Player.players.ForEach(delegate(Player p) { if (p.MapLoaded && p.level == this) p.SendTime(); });
			};
			timeupdate.Start();
            blockflush.Elapsed += delegate { FlushBlockChanges(); };
            blockflush.Start();

            physics.Start();

            if (World.WorldLoad != null)
                World.WorldLoad(this);
		}
       
		public static World Find(string name)
		{
			World tempLevel = null; bool returnNull = false;

            foreach (World world in worlds)
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
        public void LoadChunk(int x, int z, bool thread = true, bool threadLoad = true)
        {
            LoadChunk(x, z, this, thread, threadLoad);
        }
        public static void LoadChunk(int x, int z, World w, bool thread = true, bool threadLoad = true)
        {
            Chunk ch = Chunk.Load(x, z, w, thread, threadLoad);
            if (ch != null)
            {
                Point pt = new Point(x, z);
                lock (w.chunkData)
                    if (!w.chunkData.ContainsKey(pt) || !w.chunkData[pt].generated)
                    {
                        if (w.chunkData.ContainsKey(pt))
                            w.chunkData.Remove(pt);
                        w.chunkData.Add(pt, ch);
                    }
                if (!ch.generated)
                {
                    if (thread) World.chunker.QueueChunk(x, z, w);
                    else w.GenerateChunk(x, z);
                }
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
            try { if (w.chunkData.ContainsKey(pt) && w.chunkData[pt].generating) return; }
            catch { return; }

            SaveChunk(x, z, w);
            if (((int)w.SpawnX >> 4) != x || ((int)w.SpawnZ >> 4) != z) // Don't unload the spawn chunks!
            {
                w.physics.RemoveChunkChecks(x, z);
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
                Item item = new Item(id, w) { count = 1, meta = w.GetMeta(x, y, z), pos = new double[3] { x + .5, y + .5, z + .5 }, rot = new byte[3] { 1, 1, 1 }, OnGround = true };
                item.e.UpdateChunks(false, false);
            }
        }
        #endregion

        public static World LoadLVL(string filename)
        {
            //TODO make loading/saving better.
            //if (WorldLoad != null)
            //	WorldLoad(this);
            World w = new World() { chunkData = new Dictionary<Point, Chunk>(), name = filename };
            Server.Log("Loading...");

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
	                        Server.Log(ex.ToString());
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
	                        Server.Log(ex.ToString());
	                    }
	                }
				}
            }*/

            using (StreamReader sw = new StreamReader(filename + "/" + filename + ".ini"))
            {
                w.seed = long.Parse(sw.ReadLine());
                w.SpawnX = int.Parse(sw.ReadLine());
                w.SpawnY = int.Parse(sw.ReadLine());
                w.SpawnZ = int.Parse(sw.ReadLine());
                w.ChunkLimit = int.Parse(sw.ReadLine());
                w.time = long.Parse(sw.ReadLine());
            }

            w.physics = new Physics(w);
            w.generator = new GenStandard(w, true);
            w.chunkManager = new WorldChunkManager(w);

            float count = 0, total = 7 * 7;
            Console.SetCursorPosition(21, Console.CursorTop - 1);
            Console.Write("0%");

            try
            {
                Parallel.For(-3, 4, x =>
                {
                    Parallel.For(-3, 4, z =>
                    {
                        w.LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(21, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
                    });
                });
            }
            catch (NotImplementedException)
            {
                for (int x = -3; x < 4; x++)
                {
                    for (int z = -3; z < 4; z++)
                    {
                        w.LoadChunk(x, z, false, false);
                        Console.SetCursorPosition(21, Console.CursorTop);
                        count++; Console.Write((int)((count / total) * 100) + "%");
                    }
                }
            }

            Console.WriteLine();

            World.worlds.Add(w);
            Server.Log(filename + " Loaded.");
            Server.Log("Look distance = 3");
            w.timeupdate.Elapsed += delegate
            {
                w.time += 20;
                if (w.time > 24000)
                    w.time = 0;
                Player.players.ForEach(delegate(Player p) { if (p.MapLoaded && p.level == w) p.SendTime(); });
            };
            w.timeupdate.Start();
            w.blockflush.Elapsed += delegate { w.FlushBlockChanges(); };
            w.blockflush.Start();

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
            if (!silent) Server.Log(w.name + " Saved.");
        }
        public static void CompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
        }

        public static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (MemoryStream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
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
        }   

		public void Rain(bool rain)
		{
			foreach (Player p in Player.players)
			    if (p.MapLoaded && !p.disconnected)
				    p.SendRain(rain);
		}
		public void Lightning(int x, int y, int z)
		{
			Entity e = new Entity(true);
			int distance = 16 * LightningRange;

			foreach (Player p in Player.players.ToArray())
				if (p.MapLoaded && !p.disconnected)
					if (p.pos.mdiff(new Point3(x, y, z)) <= distance)
                        p.SendLightning(x * 32, y * 32, z * 32, e.id);

			//Not sure if lightning needs depsawned, seems to not require it.
		}

		public void GenerateChunk(int x, int z)
		{
            DateTime start; Chunk c; Point pt = new Point(x, z);
            lock (this.genLock)
            {
                start = DateTime.Now;
                if (chunkData.ContainsKey(pt)) { c = chunkData[pt]; }
                else c = new Chunk(x, z) { generated = false };
                c.generating = true;
                generator.Generate(c);
                c.RecalculateLight();
                lock (chunkData) {
                    if (chunkData.ContainsKey(pt))
                        chunkData.Remove(pt);
                    chunkData.Add(pt, c);
                }
                //generator.Populate(c);
                c.generating = false;
            }
            c.generated = true;
            TimeSpan took = DateTime.Now - start;
            //Console.WriteLine(took.TotalMilliseconds);

            if (GeneratedChunk != null)
                GeneratedChunk(this, c, x, z);
            if (WorldGenerateChunk != null)
                WorldGenerateChunk(this, c, x, z);
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
                if (blockQueue.Count < 1) return;

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
                        if (!p.MapLoaded || !p.VisibleChunks.Contains(kvp.Key)) continue;
                        if (p.level == this)
                            p.SendMultiBlockChange(kvp.Key, kvp.Value.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Server.ServerLogger.LogError(ex);
            }
        }
		public void BlockChange(int x, int y, int z, byte type, byte meta, bool phys = true)
		{
            try
            {
                if (y < 0 || y > ChunkHeight - 1) return;
                int cx = x >> 4, cz = z >> 4; Chunk chunk = Chunk.GetChunk(cx, cz, this);
                if (chunk == null) return;
                byte oldBlock = GetBlock(x, y, z); byte oldMeta = GetMeta(x, y, z);
                chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
                // TODO: Put lighting updates in a separate thread.
                chunk.RecalculateLight(x & 0xf, z & 0xf);
                SpreadLight(x, y, z);
                if (phys) physics.BlockUpdate(x, y, z, oldBlock, oldMeta);
                if (BlockChanged != null)
                    BlockChanged(x, y, z, type, meta);

                if (type != oldBlock || meta != oldMeta) // Don't send block change if it's the same.
                {
                    TimeSpan diff = DateTime.Now - lastBlockChange;
                    if (diff.TotalMilliseconds < 10)
                    {
                        QueueBlockChange(x, y, z, type, meta);
                    }
                    else
                    {
                        if (blockQueue.ContainsKey(chunk.point))
                            blockQueue[chunk.point].RemoveAll(bl => (bl.x == x && bl.y == y && bl.z == z));

                        foreach (Player p in Player.players.ToArray())
                        {
                            if (!p.VisibleChunks.Contains(chunk.point)) continue;
                            if (p.level == this)
                                p.SendBlockChange(x, (byte)y, z, type, meta);
                        }
                        
                        lastBlockChange = DateTime.Now;
                    }
                }
            }
            catch { return; }
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
                int cx = x >> 4, cz = z >> 4; Chunk chunk = Chunk.GetChunk(cx, cz, this, true);
                if (chunk == null) return;
                chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
                chunk.QuickRecalculateLight(x & 0xf, y, z & 0xf);
            }
            catch { }
        }
		public byte GetBlock(int x, int y, int z)
		{
            DateTime start = DateTime.Now;
            try
            {
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
                    return chunk.GetHeightValue(x & 0xf, z & 0xf);
                }
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
                    label0:
                    {
                        bool label0 = false;
                        if(y >= 0)
                        {
                            if(y < 128)
                            {
                                label0 = true;
                            }
                        }
                        if (!label0) return false;
                    }
                    return /*world.getFullBlockLightValue(i, j, k) < 13 &&*/ BlockData.IsOpaqueCube(GetBlock(x, y - 1, z));
            }
            return true;
        }
        public bool CanPlaceAt(byte b, int x, int y, int z)
        {
            byte l;
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
                Chunk chunk = Chunk.GetChunk(x, z, this);
                int k = 127;
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

