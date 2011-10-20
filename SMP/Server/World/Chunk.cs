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
using zlib;

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
        public Dictionary<int, ushort> extra;
		public int x;
		public int z;
		public bool mountain = true;
        private bool _dirty = false;

		public Point point { get { return new Point(x, z); } }
        public bool Dirty { get { return this._dirty; } }

		public List<Entity> Entities = new List<Entity>();

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
		public Chunk (int x, int z)
		{
			blocks = new byte[32768];
			Light = new byte[16384];
			SkyL = new byte[16384];
			meta = new byte[16384];
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
		public Chunk(int x, int z, int BlockCount)
		{
			blocks = new byte[BlockCount];
			Light = new byte[BlockCount / 2];
			SkyL = new byte[BlockCount / 2];
			meta = new byte[BlockCount / 2];
            extra = new Dictionary<int, ushort>();
			this.x = x; this.z = z;
		}

        public static Chunk Load(int x, int z, World w, bool thread = true)
        {
            string file = String.Format("{0}/chunks/{1}/{2}/{3}.{4}.chunk", w.name, Convert.ToString(x & 0x3f, 16), Convert.ToString(z & 0x3f, 16), x.ToString(), z.ToString());
            if (File.Exists(file))
            {
                try
                {
                    Chunk ch = new Chunk(x, z);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(file, FileMode.Open))
                        {
                            byte[] comp;
                            ms.SetLength(fs.Length);
                            fs.Read(ms.GetBuffer(), 0, (int)fs.Length);
                            comp = ms.GetBuffer().Decompress();
                            ms.Write(comp, 0, comp.Length);
                        }
                        byte[] bytes = ms.ToArray();
                        Array.Copy(bytes, ch.blocks, 32768);
                        Array.Copy(bytes, 32768, ch.meta, 0, 16384);
                        Array.Copy(bytes, 49152, ch.SkyL, 0, 16384);
                        Array.Copy(bytes, 65536, ch.Light, 0, 16384);
                        int index = 81922;
                        short extraCount = BitConverter.ToInt16(bytes, index - 2);
                        lock (ch.extra)
                            for (int i = 0; i < extraCount; i++)
                            {
                                ch.extra.Add(BitConverter.ToInt32(bytes, index), BitConverter.ToUInt16(bytes, index + 4));
                                index += 6;
                            }
                        short physCount = BitConverter.ToInt16(bytes, index);
                        index += 2;
                        for (int i = 0; i < physCount; i++)
                        {
                            w.physics.AddCheck(new Physics.Check(BitConverter.ToInt32(bytes, index), BitConverter.ToInt32(bytes, index + 4), BitConverter.ToInt32(bytes, index + 8), bytes[index + 14], BitConverter.ToInt16(bytes, index + 12)));
                            index += 15;
                        }
                    }
                    //Console.WriteLine("LOADED " + x + " " + z);
                    return ch;
                }
                catch (Exception ex)
                {
                    Server.ServerLogger.Log("Error loading chunk at " + x + "," + z + "! A new chunk will be generated in it's place.");
                    Server.ServerLogger.LogError(ex);
                }
            }
            //Console.WriteLine("GENERATED " + x + " " + z);
            if (thread)
                World.chunker.QueueChunk(x, z, w);
            else
                return w.GenerateChunk(x, z);
            return null;
        }

        public void Save(World w)
        {
            string path = String.Format("{0}/chunks/{1}/{2}", w.name, Convert.ToString(x & 0x3f, 16), Convert.ToString(z & 0x3f, 16));
            string file = String.Format("{0}/{1}.{2}.chunk", path, x.ToString(), z.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (MemoryStream data = new MemoryStream())
            {
                data.Write(blocks, 0, blocks.Length);
                data.Write(meta, 0, meta.Length);
                data.Write(SkyL, 0, SkyL.Length);
                data.Write(Light, 0, Light.Length);
                data.Write(BitConverter.GetBytes((short)extra.Count), 0, 2);
                lock (extra)
                    foreach (KeyValuePair<int, ushort> kvp in extra)
                    {
                        data.Write(BitConverter.GetBytes((int)kvp.Key), 0, 4);
                        data.Write(BitConverter.GetBytes((ushort)kvp.Value), 0, 2);
                    }
                List<Physics.Check> physChecks = w.physics.GetChunkChecks(x, z);
                data.Write(BitConverter.GetBytes((short)physChecks.Count), 0, 2);
                foreach (Physics.Check check in physChecks)
                {
                    data.Write(BitConverter.GetBytes(check.x), 0, 4);
                    data.Write(BitConverter.GetBytes(check.y), 0, 4);
                    data.Write(BitConverter.GetBytes(check.z), 0, 4);
                    data.Write(BitConverter.GetBytes(check.time), 0, 2);
                    data.WriteByte(check.meta);
                }

                byte[] bytes;
                bytes = data.ToArray().Compress();
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    fs.Write(bytes, 0, (int)bytes.Length);
                }
            }
            this._dirty = false;
            //Console.WriteLine("SAVED " + x + " " + z);
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
            for (int xx = 0; xx < Width; xx++)
                for (int zz = 0; zz < Depth; zz++)
                    for (int yy = 0; yy < Height; yy++)
                    {
                        bType = GetBlock(xx, yy, zz);

                        if (bType == (byte)Blocks.SignPost || bType == (byte)Blocks.SignWall)
                            p.SendUpdateSign(xx, (short)yy, zz, w.GetSign(xx, yy, zz));
                        if (bType == (byte)Blocks.Jukebox)
                        {
                            bExtra = GetExtraData(xx, yy, zz);
                            if (bExtra >= 2256 && bExtra <= 2266)
                                p.SendSoundEffect(xx, (byte)yy, zz, 1005, bExtra);
                        }
                    }
        }

        public void SetBlockLight(int x, int y, int z, byte light)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, light, ref Light[index / 2]);
			}
            this._dirty = true;
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
			}
            this._dirty = true;
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
			}
            this._dirty = true;
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
            int index = PosToInt(x, y, z);
            if (!extra.ContainsKey(index)) extra.Add(index, data);
            else extra[index] = data;
            this._dirty = true;
        }
        public void UnsetExtraData(int x, int y, int z)
        {
            int index = PosToInt(x, y, z);
            if (extra.ContainsKey(index)) extra.Remove(index);
            this._dirty = true;
        }
        public ushort GetExtraData(int x, int y, int z)
        {
            int index = PosToInt(x, y, z);
            if (!extra.ContainsKey(index)) return 0;
            return extra[index];
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
            byte[] compressed;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZOutputStream zout = new ZOutputStream(ms, zlibConst.Z_BEST_COMPRESSION))
                {
                    // Write block types
                    zout.Write(blocks, 0, blocks.Length);

                    // Write metadata
                    zout.Write(meta, 0, meta.Length);

                    // Write block light
                    zout.Write(Light, 0, Light.Length);

                    // Write sky light
                    zout.Write(SkyL, 0, SkyL.Length);
                }
                compressed = ms.ToArray();
            }
            return compressed;
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
			{
				blocks[PosToInt(x, y, z)] = id;
				SetMetaData(x, y, z, meta);
			}
            this._dirty = true;
		}
		public void UNCHECKEDPlaceBlock(int x, int y, int z, byte id)
		{
			UNCHECKEDPlaceBlock(x, y, z, id, 0);
		}
		public void UNCHECKEDPlaceBlock(int x, int y, int z, byte id, byte meta)
		{
			blocks[PosToInt(x, y, z)] = id;
			if(meta != 0) SetMetaData(x, y, z, meta);
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
        public static Chunk GetChunk(int x, int z, World world)
        {
            Point po = new Point(x, z);
            if (world.chunkData.ContainsKey(po))
                return world.chunkData[po];
            return null;
        }

        public void Dispose()
        {
            blocks = null;
            meta = null;
            SkyL = null;
            Light = null;
            Entities = null;
            extra.Clear();
            extra = null;
        }
	}
}

