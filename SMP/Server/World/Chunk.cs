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
using System.IO;
using System.Collections.Generic;
using Ionic.Zlib;
using Substrate.Nbt;
using System.Linq;
using SMP.util;

namespace SMP
{
	public partial class Chunk : IDisposable
	{
		static int Width = 16;
		static int Depth = 16;
		static int Height = 128;

		public byte[] blocks;
		public byte[] Light;
		public byte[] SkyL;
		public byte[] meta;
        public byte[] heightMap;
        public int[] precipitationHeightMap;
        public Dictionary<int, ushort> extra;
		public int x;
		public int z;
		public bool mountain = true; //???
        public bool generated = false, populated = false;
        public bool generating = false, populating = false;
        internal bool _dirty = false;

        private Physics.Check[] physChecks; // Temporary array used for loading physics data!
        private List<Entity> entityLoad = new List<Entity>(); // Used to... well... load entities.

		public Point point { get { return new Point(x, z); } }
        public bool Dirty { get { return this._dirty; } }

        //public List<Entity> Entities { get { return new List<Entity>(Entity.Entities.Values).FindAll(ent => (((int)ent.pos.x >> 4) == x && ((int)ent.pos.z >> 4) == z)); } }
        private List<Entity> Entities = new List<Entity>();

		/// <summary>
		/// When a block is placed then this is called
		/// </summary>
		/// <param name='x'>
		/// X. The x position the block was placed
		/// </param>
		/// <param name='y'>
		/// Y. The y position the block was placed
		/// </param>
		/// <param name='z'>
		/// Z. The z position the block was placed
		/// </param>
		/// <param name='id'>
		/// The block id
		/// </param>
		public delegate bool OnBlockPlaced(int x, int y, int z, byte id);
		public event OnBlockPlaced BlockPlaced;

		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Chunk"/> class with the default Block Count (32768)
		/// </summary>
		/// <param name='x'>
		/// X. The x position of the chunk
		/// </param>
		/// <param name='z'>
		/// Z. The z position of the chunk
		/// </param>
        public Chunk(int x, int z)
		{
            blocks = new byte[32768];
            Light = new byte[16384];
            SkyL = new byte[16384];
            meta = new byte[16384];
            heightMap = new byte[256];
            precipitationHeightMap = Enumerable.Repeat(-999, 256).ToArray();
            extra = new Dictionary<int, ushort>();
			this.x = x; this.z = z;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Chunk"/> class with a custom Block Count
		/// </summary>
		/// <param name='x'>
		/// X. The x position of the chunk
		/// </param>
		/// <param name='z'>
		/// Z. The z position of the chunk
		/// </param>
		/// <param name='BlockCount'>
		/// Block count. The block count
		/// </param>
        [Obsolete("Does not work, should never be used!", true)]
		public Chunk(int x, int z, int BlockCount)
		{
			blocks = new byte[BlockCount];
			Light = new byte[BlockCount / 2];
			SkyL = new byte[BlockCount / 2];
			meta = new byte[BlockCount / 2];
            heightMap = new byte[256];
            extra = new Dictionary<int, ushort>();
			this.x = x; this.z = z;
		}

        public Chunk(int x, int z, bool dummy)
        {
            this.x = x; this.z = z;
        }

        public static Chunk Load(int x, int z, World w, bool thread = true, bool threadLoad = true, bool generate = true, bool dummy = true)
        {
            string file = CreatePath(w, x, z);
            if (File.Exists(file))
            {
                if (threadLoad)
                {
                    World.chunker.QueueChunkLoad(x, z, false, w);
                    return null;
                }
                try
                {
                    Chunk ch = new Chunk(x, z);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            byte[] comp;
                            ms.SetLength(fs.Length);
                            fs.Read(ms.GetBuffer(), 0, (int)fs.Length);
                            comp = ms.GetBuffer().Decompress(CompressionType.GZip);
                            ms.Write(comp, 0, comp.Length);
                        }

                        ms.Position = 0;
                        NbtTree nbt = new NbtTree(ms);
                        ch.generated = (nbt.Root["Generated"].ToTagByte().Data > 0);
                        ch.populated = (nbt.Root["Populated"].ToTagByte().Data > 0);
                        Array.Copy(nbt.Root["Blocks"].ToTagByteArray(), ch.blocks, ch.blocks.Length);
                        Array.Copy(nbt.Root["Meta"].ToTagByteArray(), ch.meta, ch.meta.Length);
                        Array.Copy(nbt.Root["BlockLight"].ToTagByteArray(), ch.Light, ch.Light.Length);
                        Array.Copy(nbt.Root["SkyLight"].ToTagByteArray(), ch.SkyL, ch.SkyL.Length);
                        Array.Copy(nbt.Root["HeightMap"].ToTagByteArray(), ch.heightMap, ch.heightMap.Length);
                        Array.Copy(nbt.Root["HeightMapPrec"].ToTagByteArray().Data.ToIntArray(), ch.precipitationHeightMap, ch.precipitationHeightMap.Length);
                        TagNodeCompound nbtCompound;
                        foreach (TagNode tag in nbt.Root["Extra"].ToTagList())
                        {
                            nbtCompound = tag.ToTagCompound();
                            ch.extra.Add(nbtCompound["Pos"].ToTagInt(), (ushort)nbtCompound["Value"].ToTagShort());
                        }
                        TagNodeList nbtList = nbt.Root["Physics"].ToTagList();
                        int count = nbtList.Count;
                        if (count > 0)
                        {
                            ch.physChecks = new Physics.Check[count]; TagNodeList nbtList2;
                            for (int i = 0; i < count; i++)
                            {
                                nbtCompound = nbtList[i].ToTagCompound();
                                nbtList2 = nbtCompound["Pos"].ToTagList();
                                ch.physChecks[i] = new Physics.Check(nbtList2[0].ToTagInt(), nbtList2[1].ToTagInt(), nbtList2[2].ToTagInt(), nbtCompound["Meta"].ToTagByte(), nbtCompound["Time"].ToTagShort());
                            }
                        }
                        AI ai; McObject obj; Item item; Entity e; TagNodeCompound nbtCompound2;
                        foreach (TagNode tag in nbt.Root["Entities"].ToTagList())
                        {
                            e = null;
                            nbtCompound = tag.ToTagCompound();
                            switch ((EntityType)(byte)nbtCompound["Type"].ToTagByte())
                            {
                                case EntityType.AI:
                                    // TODO
                                    break;
                                case EntityType.Object:
                                    // TODO
                                    break;
                                case EntityType.Item:
                                    nbtCompound2 = nbtCompound["Data"].ToTagCompound();
                                    item = new Item(true) { id = nbtCompound2["ID"].ToTagShort(), count = nbtCompound2["Count"].ToTagByte(), meta = nbtCompound2["Meta"].ToTagShort() };
                                    item.e = new Entity(w) { isItem = true, I = item };
                                    e = item.e;
                                    break;
                            }
                            if (e != null)
                            {
                                nbtList = nbtCompound["Motion"].ToTagList();
                                e.velocity = new double[] { nbtList[0].ToTagDouble(), nbtList[1].ToTagDouble(), nbtList[2].ToTagDouble() };
                                nbtList = nbtCompound["Pos"].ToTagList();
                                e.pos = new Point3(nbtList[0].ToTagDouble(), nbtList[1].ToTagDouble(), nbtList[2].ToTagDouble());
                                nbtList = nbtCompound["Rotation"].ToTagList();
                                e.rot = new float[] { nbtList[0].ToTagFloat(), nbtList[1].ToTagFloat() };
                                e.age = nbtCompound["Age"].ToTagInt();
                                e.OnGround = (nbtCompound["OnGround"].ToTagByte() > 0);
                                e.health = nbtCompound["Health"].ToTagShort();
                                ch.entityLoad.Add(e);
                            }
                        }
                        Container c; Point3 point3;
                        foreach (TagNode tag in nbt.Root["Containers"].ToTagList())
                        {
                            nbtCompound = tag.ToTagCompound();
                            nbtList = nbtCompound["Pos"].ToTagList();
                            point3 = new Point3(nbtList[0].ToTagInt(), nbtList[1].ToTagInt(), nbtList[2].ToTagInt());
                            c = Container.CreateInstance((ContainerType)(byte)nbtCompound["Type"].ToTagByte(), w, point3);
                            c.LoadNBTData(nbtCompound["Items"].ToTagList());
                            if (!w.containers.ContainsKey(point3)) w.containers.Add(point3, c);
                        }
                    }
                    //Console.WriteLine("LOADED " + x + " " + z);
                    return ch;
                }
                catch (Exception ex)
                {
                    Logger.LogToFile("Error loading chunk at " + x + "," + z + "! A new chunk will be generated in it's place.");
                    Logger.LogErrorToFile(ex);
                }
            }
            //Console.WriteLine("GENERATED " + x + " " + z);
            if (generate)
            {
                if (thread) World.chunker.QueueChunk(x, z, w);
                else return w.GenerateChunk(x, z);
                return null;
            }
            if (dummy) return new Chunk(x, z);
            return null;
        }

        public void Save(World w)
        {
            try
            {
                string path = CreatePath(w, x, z, true);
                string file = CreatePath(w, x, z);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                NbtTree nbt = new NbtTree();
                nbt.Root.Add("Generated", new TagNodeByte((byte)(generated ? 1 : 0)));
                nbt.Root.Add("Populated", new TagNodeByte((byte)(populated ? 1 : 0)));
                nbt.Root.Add("Blocks", new TagNodeByteArray(blocks));
                nbt.Root.Add("Meta", new TagNodeByteArray(meta));
                nbt.Root.Add("BlockLight", new TagNodeByteArray(Light));
                nbt.Root.Add("SkyLight", new TagNodeByteArray(SkyL));
                nbt.Root.Add("HeightMap", new TagNodeByteArray(heightMap));
                nbt.Root.Add("HeightMapPrec", new TagNodeByteArray(precipitationHeightMap.ToByteArray()));
                TagNodeList nbtList = new TagNodeList(TagType.TAG_COMPOUND);
                TagNodeCompound nbtCompound;
                lock (extra)
                    foreach (KeyValuePair<int, ushort> kvp in extra)
                    {
                        nbtCompound = new TagNodeCompound();
                        nbtCompound.Add("Pos", new TagNodeInt(kvp.Key));
                        nbtCompound.Add("Value", new TagNodeShort((short)kvp.Value));
                        nbtList.Add(nbtCompound);
                    }
                nbt.Root.Add("Extra", nbtList);
                nbtList = new TagNodeList(TagType.TAG_COMPOUND);
                List<Physics.Check> physChecks = w.physics.GetChunkChecks(x, z);
                foreach (Physics.Check check in physChecks)
                {
                    nbtCompound = new TagNodeCompound();
                    nbtCompound.Add("Pos", new TagNodeList(TagType.TAG_INT) { new TagNodeInt(check.x), new TagNodeInt(check.y), new TagNodeInt(check.z) });
                    nbtCompound.Add("Meta", new TagNodeByte(check.meta));
                    nbtCompound.Add("Time", new TagNodeShort(check.time));
                    nbtList.Add(nbtCompound);
                }
                nbt.Root.Add("Physics", nbtList);
                nbtList = new TagNodeList(TagType.TAG_COMPOUND);
                List<Entity> entities = Entities; TagNodeCompound nbtCompound2;
                foreach (Entity e in entities)
                {
                    if (e.isPlayer) continue;
                    nbtCompound = new TagNodeCompound();
                    nbtCompound.Add("Motion", new TagNodeList(TagType.TAG_DOUBLE) { new TagNodeDouble(e.velocity[0]), new TagNodeDouble(e.velocity[1]), new TagNodeDouble(e.velocity[2]) });
                    nbtCompound.Add("Pos", new TagNodeList(TagType.TAG_DOUBLE) { new TagNodeDouble(e.pos.x), new TagNodeDouble(e.pos.y), new TagNodeDouble(e.pos.z) });
                    nbtCompound.Add("Rotation", new TagNodeList(TagType.TAG_FLOAT) { new TagNodeFloat(e.rot[0]), new TagNodeFloat(e.rot[1]) });
                    nbtCompound.Add("Type", new TagNodeByte((byte)e.Type));
                    nbtCompound.Add("Age", new TagNodeInt(e.age));
                    nbtCompound.Add("OnGround", new TagNodeByte(e.onground));
                    nbtCompound.Add("Health", new TagNodeShort(e.Health));
                    nbtCompound2 = new TagNodeCompound();
                    switch (e.Type)
                    {
                        case EntityType.AI:
                            nbtCompound2.Add("Type", new TagNodeByte(e.ai.type));
                            break;
                        case EntityType.Object:
                            nbtCompound2.Add("Type", new TagNodeByte(e.obj.type));
                            break;
                        case EntityType.Item:
                            nbtCompound2.Add("ID", new TagNodeShort(e.I.id));
                            nbtCompound2.Add("Count", new TagNodeByte(e.I.count));
                            nbtCompound2.Add("Meta", new TagNodeShort(e.I.meta));
                            break;
                    }
                    nbtCompound.Add("Data", nbtCompound2);
                    nbtList.Add(nbtCompound);
                }
                nbt.Root.Add("Entities", nbtList);
                nbtList = new TagNodeList(TagType.TAG_COMPOUND);
                foreach (Container c in GetContainers(w))
                {
                    nbtCompound = new TagNodeCompound();
                    nbtCompound.Add("Type", new TagNodeByte((byte)c.Type));
                    nbtCompound.Add("Pos", new TagNodeList(TagType.TAG_INT) { new TagNodeInt((int)c.Pos.x), new TagNodeInt((int)c.Pos.y), new TagNodeInt((int)c.Pos.z) });
                    nbtCompound.Add("Items", c.GetNBTData());
                    nbtList.Add(nbtCompound);
                    //Console.WriteLine("SAVED CONTAINER @ " + (int)c.Pos.x + "," + (int)c.Pos.y + "," + (int)c.Pos.z + " @ " + x + "," + z);
                }
                nbt.Root.Add("Containers", nbtList);

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        nbt.WriteTo(ms);
                        byte[] bytes = ms.ToArray().Compress(CompressionLevel.BestCompression, CompressionType.GZip);
                        using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                            fs.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogToFile("Error saving chunk at " + x + "," + z + "!");
                    Logger.LogErrorToFile(ex);
                }

                this._dirty = false;
                //Console.WriteLine("SAVED " + x + " " + z);
            }
            catch (Exception ex) { Logger.LogError(ex); }
        }

        private List<Container> GetContainers(World w)
        {
            List<Container> containers;
            containers = new List<Container>(w.containers.Values);
            return containers.FindAll(container => (((int)container.Pos.x >> 4) == x && ((int)container.Pos.z >> 4) == z));
        }

        public void AddEntity(Entity e)
        {
            if (!Entities.Contains(e))
            {
                Entities.Add(e);
                this._dirty = true;
            }
        }
        public void RemoveEntity(Entity e)
        {
            if (Entities.Contains(e))
            {
                Entities.Remove(e);
                this._dirty = true;
            }
        }
        public List<Entity> GetEntities()
        {
            lock (Entities)
                return new List<Entity>(Entities);
        }

        public void GenerateHeightMap()
        {
            int i = 128 - 1;
            for (int j = 0; j < 16; j++)
            {
                for (int l = 0; l < 16; l++)
                {
                    int j1 = 128 - 1;
                    int k1;
                    for (k1 = j << 11 | l << 7; j1 > 0 && Chunk.LightOpacity[blocks[(k1 + j1) - 1] & 0xff] == 0; j1--) { }
                    heightMap[l << 4 | j] = (byte)j1;
                    if (j1 < i)
                    {
                        i = j1;
                    }
                }
            }
        }

        public void PostLoad(World w)
        {
            try
            {
                if (!populated && w.ChunkExists(x + 1, z + 1) && w.ChunkExists(x, z + 1) && w.ChunkExists(x + 1, z))
                    w.PopulateChunk(x, z);
                if (w.ChunkExists(x - 1, z) && !GetChunk(x - 1, z, w).populated && w.ChunkExists(x - 1, z + 1) && w.ChunkExists(x, z + 1))
                    w.PopulateChunk(x - 1, z);
                if (w.ChunkExists(x, z - 1) && !GetChunk(x, z - 1, w).populated && w.ChunkExists(x + 1, z - 1) && w.ChunkExists(x + 1, z))
                    w.PopulateChunk(x, z - 1);
                if (w.ChunkExists(x - 1, z - 1) && !GetChunk(x - 1, z - 1, w).populated && w.ChunkExists(x, z - 1) && w.ChunkExists(x - 1, z))
                    w.PopulateChunk(x - 1, z - 1);
            }
            catch { }

            if (physChecks != null)
            {
                w.physics.AddChunkChecks(physChecks);
                physChecks = null;
            }

            if (entityLoad.Count > 0)
            {
                foreach (Entity e in entityLoad)
                {
                    Entity.Entities.Add(e.id, e);
                    e.UpdateChunks(false, false);
                }
                entityLoad.Clear();
            }

            byte bType;
            for (int xx = 0; xx < Width; xx++)
                for (int zz = 0; zz < Depth; zz++)
                    for (int yy = 0; yy < Height; yy++)
                    {
                        bType = GetBlock(xx, yy, zz);
                        if (bType < 0 || bType > 122) UNCHECKEDPlaceBlock(xx, yy, zz, 0);
                    }
        }

        public void PostGenerate(World w)
        {
            this._dirty = true;
        }
        public void PostPopulate(World w)
        {
            byte bType, bMeta;
            ushort bExtra;
            int xxx, zzz;
            for (int xx = 0; xx < Width; xx++)
                for (int zz = 0; zz < Depth; zz++)
                    for (int yy = 0; yy < Height; yy++)
                    {
                        xxx = (x << 4) + xx; zzz = (z << 4) + zz;
                        bType = GetBlock(xx, yy, zz);

                        if (!w.CanBlockStay(bType, xxx, yy, zzz))
                            w.SetBlock(xxx, yy, zzz, 0, 0);
                    }
        }

        public void GlobalUpdate(World w)
        {
            Player.players.ForEach(delegate(Player pl)
            {
                if (pl.MapLoaded && pl.level == w && pl.VisibleChunks.Contains(this.point))
                    this.Update(w, pl);
            });
        }
        public void Update(World w, Player p)
        {
            byte bType, bMeta;
            ushort bExtra;
            int xxx, zzz;
            Container c;
            for (int xx = 0; xx < Width; xx++)
                for (int zz = 0; zz < Depth; zz++)
                    for (int yy = 0; yy < Height; yy++)
                    {
                        xxx = (x << 4) + xx; zzz = (z << 4) + zz;
                        bType = GetBlock(xx, yy, zz);
                        bMeta = GetMetaData(xx, yy, zz);

                        if (bType == (byte)Blocks.SignPost || bType == (byte)Blocks.SignWall)
                            p.SendUpdateSign(xxx, (short)yy, zzz, w.GetSign(xxx, yy, zzz));
                        if (bType == (byte)Blocks.Jukebox)
                        {
                            bExtra = GetExtraData(xx, yy, zz);
                            if (bExtra >= 2256 && bExtra <= 2266)
                                p.SendSoundEffect(xxx, (byte)yy, zzz, 1005, bExtra);
                        }
                        if (bType == (byte)Blocks.Chest)
                        {
                            p.SendBlockChange(xxx, (byte)yy, zzz, bType, bMeta);
                            c = w.GetBlockContainer(xxx, yy, zzz);
                            if (c != null) c.UpdateState();
                        }
                    }
        }

        public bool CanBlockSeeSky(int x, int y, int z)
        {
            return y >= (heightMap[z << 4 | x] & 0xff);
        }
        public int GetHeightValue(int x, int z)
        {
            return heightMap[z << 4 | x] & 0xff;
        }

        public void SetBlockLight(int x, int y, int z, byte light)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, light, ref Light[index / 2]);
                this._dirty = true;
			}
		}
		public byte GetBlockLight(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				// (y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				int index = PosToInt(x, y, z);
				return getHalf(index, Light[index / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
		public void SetSkyLight(int x, int y, int z, byte light)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, light, ref SkyL[index / 2]);
                this._dirty = true;
			}
		}
		public byte GetSkyLight(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				//(y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				int index = PosToInt(x, y, z);
				return getHalf(index, SkyL[PosToInt(x, y, z) / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
		public void SetMetaData(int x, int y, int z, byte data)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, data, ref meta[PosToInt(x, y, z) / 2]);
                this._dirty = true;
			}
		}
		public byte GetMetaData(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				// (y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				int index = PosToInt(x, y, z);
				return getHalf(index, meta[PosToInt(x, y, z) / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
        public void SetExtraData(int x, int y, int z, ushort data)
        {
            if (InBound(x, y, z))
            {
                int index = PosToInt(x, y, z);
                if (!extra.ContainsKey(index)) extra.Add(index, data);
                else extra[index] = data;
                this._dirty = true;
            }
        }
        public void UnsetExtraData(int x, int y, int z)
        {
            if (InBound(x, y, z))
            {
                int index = PosToInt(x, y, z);
                if (extra.ContainsKey(index)) extra.Remove(index);
                this._dirty = true;
            }
        }
        public ushort GetExtraData(int x, int y, int z)
        {
            if (InBound(x, y, z))
            {
                int index = PosToInt(x, y, z);
                if (!extra.ContainsKey(index)) return 0;
                return extra[index];
            }
            else
            {
                return 0xFFFF;
            }
        }
		private void SetHalf(int index, byte value, ref byte data)
		{
			if (index % 2 == 0)
			{
				// Set the lower 4 bits
				byte high = (byte)((data & 0xF0) >> 4);
				data = (byte)((high << 4) | value);
			}
			else
			{
				// Set the upper 4 bits
				byte low = (byte)(data & 0x0F);
				data = (byte)((value << 4) | low);
			}
		}
		private byte getHalf(int index, byte data)
		{
			return (index % 2 == 0) ? (byte)(data & 0x0F) : (byte)((data >> 4) & 0x0F);
		}

		public bool InBound(int x, int y, int z)
		{
            if (x < 0 || y < 0 || z < 0 || x >= Width || z >= Depth || y >= Height)
				return false;
			return true;
		}
		/// <summary>
        /// Returns a compressed copy of the chunk's block data.
        /// </summary>
        public byte[] GetCompressedData()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Write block types
                    ms.Write(blocks, 0, blocks.Length);

                    // Write metadata
                    ms.Write(meta, 0, meta.Length);

                    // Write block light
                    ms.Write(Light, 0, Light.Length);

                    // Write sky light
                    ms.Write(SkyL, 0, SkyL.Length);

                    // Compress the data
                    return ms.ToArray().Compress(CompressionLevel.BestCompression);
                }
            }
            catch { return null; }
        }

        public int func_35631_c(World w, int i, int j)
        {
            int k = i | j << 4;
            int l = precipitationHeightMap[k];
            if (l == -999)
            {
                int i1 = w.worldYMax - 1;
                for (l = -1; i1 > 0 && l == -1; )
                {
                    byte j1 = GetBlock(i, i1, j);
                    //Material material = j1 != 0 ? BlockData.BlockMaterial(j1) : Material.Air;
                    if (!BlockData.IsSolid(j1) && !BlockData.IsLiquid(j1))
                        i1--;
                    else
                        l = i1 + 1;
                }

                precipitationHeightMap[k] = l;
            }
            return l;
        }

		/// <summary>
		/// Places the block at a x, y, z.
		/// </summary>
		/// <param name='x'>
		/// X. The x pos that the block will be pladed.
		/// </param>
		/// <param name='y'>
		/// Y. The y pos that the  block will be placed.
		/// </param>
		/// <param name='z'>
		/// Z. The z pos that the block will be placed.
		/// </param>
		/// <param name='id'>
		/// Block id.
		/// </param>
		public void PlaceBlock(int x, int y, int z, byte id)
		{
			PlaceBlock(x, y, z, id, 0);
		}
		public void PlaceBlock(int x, int y, int z, byte id, byte meta)
		{
			if (InBound(x, y, z))
                UNCHECKEDPlaceBlock(x, y, z, id, meta);
		}
		public void UNCHECKEDPlaceBlock(int x, int y, int z, byte id)
		{
			UNCHECKEDPlaceBlock(x, y, z, id, 0);
		}
		public void UNCHECKEDPlaceBlock(int x, int y, int z, byte id, byte meta)
		{
            int i = z << 4 | x;
            if (y >= precipitationHeightMap[i] - 1)
                precipitationHeightMap[i] = -999;

			blocks[PosToInt(x, y, z)] = id;
			SetMetaData(x, y, z, meta);
            this._dirty = true;
		}

		//DO NOT USE ANY FORM OF CHUNK.GETBLOCK UNLESS YOU REALLY KNOW YOU WANT TO, USE WORLD.GETBLOCK INSTEAD (TRUST ME, YOU WILL ONLY CAUSE ERRORS IF YOU USE THIS)
		#region CHUNK GET BLOCK AREA
		/// <summary>
		/// *DO NOT USE THIS* unless you know what your doing, THIS does NOT get the block in world coordinates, use World.GetBlock for that, this will NOT be what you want. If you really need to use this function, please do NOT change this to public, use SGB() which calls this function to get the block.
		/// </summary>
		/// <param name="x">do not use</param>
		/// <param name="y">do not use me either</param>
		/// <param name="z">are you really still entering variables? DO NOT USE THIS METHOD</param>
		/// <returns>>_></returns>
		private byte GetBlock(int x, int y, int z)
		{
			if (InBound(x, y, z))
				return blocks[PosToInt(x, y, z)];
			return 0;
		}
		public byte SGB(int x, int y, int z)
		{
			return GetBlock(x, y, z);
		}
		#endregion

		/// <summary>
		/// Places the block at a x, y, z.
		/// </summary>
		/// <param name='x'>
		/// X. The x pos that the block will be pladed.
		/// </param>
		/// <param name='y'>
		/// Y. The y pos that the  block will be placed.
		/// </param>
		/// <param name='z'>
		/// Z. The z pos that the block will be placed.
		/// </param>
		/// <param name='id'>
		/// Block id.
		/// </param>
		/*public void PlaceBlock(int x, int y, int z, DataValues id)
		{
			if (BlockPlaced != null) { if (BlockPlaced(x, y, z, id)) return; }
			if (InBound(x, y, z))
				blocks[PosToInt(x, y, z)] = id;
		}*/
		public static int PosToInt(int x, int y, int z)
        {
			return (x * Depth + z) * Height + y;
        }
		//We should never default
		//public static Chunk GetChunk(int x, int z)
		//{
		//        return Server.mainlevel.chunkData[new Point(x, z)];
		//}
        public static Chunk GetChunk(int x, int z, World world, bool overRide = false)
        {
            Point po = new Point(x, z);

            if (overRide && !world.chunkData.ContainsKey(po))
                world.LoadChunk(z, z, false, false);
            if (world.chunkData.ContainsKey(po))
                return world.chunkData[po];
            return null;
        }

        public static string CreatePath(World w, int x, int z, bool folderOnly = false)
        {
            if (folderOnly) return String.Format("{0}/chunks/{1}/{2}/", w.name, Convert.ToString(x & 0x3f, 16), Convert.ToString(z & 0x3f, 16));
            return String.Format("{0}/chunks/{1}/{2}/{3}.{4}.nbt", w.name, Convert.ToString(x & 0x3f, 16), Convert.ToString(z & 0x3f, 16), x.ToString(), z.ToString());
        }

        public void Dispose()
        {
            blocks = null;
            meta = null;
            SkyL = null;
            Light = null;
            heightMap = null;
            Entities.Clear();
            Entities = null;
            extra.Clear();
            extra = null;
        }
	}
}

