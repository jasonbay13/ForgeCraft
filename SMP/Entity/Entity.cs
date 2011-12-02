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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SMP.util;

namespace SMP
{
	public class Entity
	{
        private static int nextId = 0;
		public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();

        public EntityType Type
        {
            get
            {
                if (isPlayer) return EntityType.Player;
                if (isAI) return EntityType.AI;
                if (isObject) return EntityType.Object;
                if (isItem) return EntityType.Item;
                return EntityType.Unknown;
            }
        }

		public Chunk c
		{
			get
			{
				return Chunk.GetChunk((int)pos.x >> 4, (int)pos.z >> 4, level);
			}
		}
		public Chunk CurrentChunk;

		public Point3 pos = Point3.Zero;
        public Point3 oldpos = Point3.Zero;
        public float[] rot = new float[2];
        public float[] oldrot = new float[2];
        public double[] velocity = new double[3];

        internal byte onground;
        public bool OnGround { get { return onground == 1; } set { onground = (byte)(value ? 1 : 0); } }

		public Player p; //Only set if this entity is a player, and it referances the player it is
		public Item I;//Only set if this entity is an item
		public AI ai; //Only set if this entity is an AI
		public McObject obj;

		//MUST BE SET
		public World level;

		public bool isPlayer;
		public bool isAI;
		public bool isItem;
		public bool isObject; //Vehicles and arrows and stuffs

		public static Random random = new Random();
        public static java.util.Random randomJava = new java.util.Random();
		public int id;

        public int age = 0;
        internal short health = 20;
        public short Health
        {
            get { return health; }
            set
            {
                health = MathHelper.Clamp(value, (short)0, (short)20);
                if (isPlayer) p.SendHealth();
            }
        }
        public object[] metadata = new object[32];

        private DateTime lastPosSync = new DateTime();
        private DateTime lastHurt = new DateTime();

		public Entity(Player pl, World l)
		{
			p = pl;
			id = FreeId();
			isPlayer = true;
			level = l;

			UpdateChunks(false, false);

			Entities.Add(id, this);
		}
		public Entity(Item i, World l)
		{
			I = i;
            if (!I.isInventory) id = FreeId();
			isItem = true;
			level = l;

            if (!I.isInventory)
            {
                UpdateChunks(false, false);
                Entities.Add(id, this);
            }
		}
		public Entity(AI ai, World l)
		{
			this.ai = ai;
			id = FreeId();
			isAI = true;
			level = l;

			Entities.Add(id, this);
		}
        public Entity(McObject obj, World l)
        {
            this.obj = obj;
            id = FreeId();
            isObject = true;
            level = l;

            Entities.Add(id, this);
        }
		public Entity(bool lightning) //Stand in entity for lightning
		{
			id = FreeId();
		}

        public void hurt(short amount, bool overRide = false)
        {
            if (isPlayer && (p.GodMode || p.Mode == 1 || Server.mode == 1)) return;
            if (Health > 0)
            {
                if (!overRide && (DateTime.Now - lastHurt).TotalMilliseconds < 500) return;
                lastHurt = DateTime.Now;
                Health -= Math.Min(amount, Health);
                foreach (Player pl in Player.players.ToArray())
                {
                    pl.SendEntityStatus(id, 2);
                    if (Health <= 0 && pl != p)
                    {
                        if (isPlayer) p.inventory.Clear();
                        pl.SendEntityStatus(id, 3); // Gets stuck dead, removed until that's fixed.
                    }
                }

                if (Health <= 0)
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Thread.Sleep(1000);
                        Player.GlobalDespawn(this);
                        if (!isPlayer) RemoveEntity(this);
                    })).Start();
                }
            }
        }

        public void UpdateChunks(bool force, bool forcesend, int queue = 0)
        {
            if (c == null)
                level.LoadChunk((int)pos.x >> 4, (int)pos.z >> 4, false, false);
            if (c == null || (c == CurrentChunk && !force))
                return;

            try
            {
                if (CurrentChunk != null) CurrentChunk.RemoveEntity(this);
                c.AddEntity(this);
                CurrentChunk = c;
            }
            catch
            {
                Logger.Log("Error updating chunk: " + this.ToString());
            }
            if (isPlayer && p.LoggedIn)
            {
                //p.SendMessage(level.chunkManager.getBiomeGenAt(CurrentChunk.x << 4, CurrentChunk.z << 4).biomeName); // Debug!
                //bool locked = false;
                List<Point> templist = new List<Point>();

                int sx = CurrentChunk.x - p.viewdistance; //StartX
                int ex = CurrentChunk.x + p.viewdistance; //EndX
                int sz = CurrentChunk.z - p.viewdistance; //StartZ
                int ez = CurrentChunk.z + p.viewdistance; //EndZ

                // This can cause severe lag! DO NOT USE IT!!!
                /*Parallel.For(sx, ex + 1, delegate(int x)
                {
                    Parallel.For(sz, ez + 1, delegate(int z)
                    {
                        Point po = new Point(x, z);
                        while (locked) Thread.Sleep(1);

                        lock (templist)
                        {
                            templist.Add(po);
                        }
                        //templist.Add(po);

                        if ((!p.VisibleChunks.Contains(po) || forcesend) && (Math.Abs(po.x) < p.level.ChunkLimit && Math.Abs(po.z) < p.level.ChunkLimit))
                        {
                            if (!p.level.chunkData.ContainsKey(po))
                                p.level.LoadChunk(po.x, po.z);
                            p.SendChunk(p.level.chunkData[po]);
                        }
                    });
                });*/

                for (int x = sx; x <= ex; x++)
                {
                    for (int z = sz; z <= ez; z++)
                    {
                        Point po = new Point(x, z);
                        templist.Add(po);
                        if ((!p.VisibleChunks.Contains(po) || forcesend) && (Math.Abs(po.x) < p.level.ChunkLimit && Math.Abs(po.z) < p.level.ChunkLimit))
                        {
                            if (!p.level.chunkData.ContainsKey(po))
                                p.level.LoadChunk(po.x, po.z, queue != -1, queue != -1);

                            try
                            {
                                if (p.level.chunkData.ContainsKey(po))
                                {
                                    if (!p.level.chunkData[po].generated)
                                    {
                                        World.chunker.QueueChunk(po, p.level);
                                        if (queue == -1)
                                        {
                                            while (!p.level.chunkData[po].generated) Thread.Sleep(50);
                                            p.SendChunk(p.level.chunkData[po]);
                                        }
                                        else World.chunker.QueueChunkSend(po, p);
                                    }
                                    else if (queue == 1)
                                    {
                                        if (!p.level.chunkData[po].populated) World.chunker.QueueChunk(po, p.level, false);
                                        World.chunker.QueueChunkSend(po, p);
                                    }
                                    else
                                    {
                                        if (!p.level.chunkData[po].populated) World.chunker.QueueChunk(po, p.level, false);
                                        p.SendChunk(p.level.chunkData[po]);
                                        //p.level.chunkData[po].Update(p.level, p);
                                    }
                                }
                                else
                                {
                                    if (queue == -1)
                                    {
                                        while (!p.level.chunkData.ContainsKey(po)) Thread.Sleep(50);
                                        p.SendChunk(p.level.chunkData[po]);
                                    }
                                    else World.chunker.QueueChunkSend(po, p);
                                }
                            }
                            catch { p.SendPreChunk(new Chunk(po.x, po.z, true), 0); }
                        }
                    }
                }

                //UNLOAD CHUNKS THE PLAYER CANNOT SEE
                foreach (Point point in p.VisibleChunks.ToArray())
                {
                    if (!templist.Contains(point))
                    {
                        p.SendPreChunk(new Chunk(point.x, point.z, true), 0);
                        p.VisibleChunks.Remove(point);

                        bool unloadChunk = true;
                        Player.players.ForEach(delegate(Player pl) { if (pl.VisibleChunks.Contains(point)) { unloadChunk = false; return; } });
                        if (unloadChunk) World.chunker.QueueChunkLoad(point.x, point.z, true, p.level); //p.level.UnloadChunk(point.x, point.z);
                    }
                }
            }
        }
		public void UpdateEntities()
		{
            if (CurrentChunk == null)
                return;

			List<int> tempelist = new List<int>();

            int sx = CurrentChunk.x - p.viewdistance; //StartX
            int ex = CurrentChunk.x + p.viewdistance; //EndX
            int sz = CurrentChunk.z - p.viewdistance; //StartZ
            int ez = CurrentChunk.z + p.viewdistance; //EndZ
			for (int x = sx; x <= ex; x++)
			{
				for (int z = sz; z <= ez; z++)
				{
					if (!level.chunkData.ContainsKey(new Point(x, z))) { continue; }

					foreach (Entity e in p.level.chunkData[new Point(x, z)].GetEntities().ToArray())
					{
						tempelist.Add(e.id);
						if (p.VisibleEntities.Contains(e.id))
						{
							continue; //Continue if the player already has this entity
						}
						if (e.isPlayer)
						{
							if (e.p == p) continue;
							p.VisibleEntities.Add(e.id);
							p.SendNamedEntitySpawn(e.p);
							if (!e.p.VisibleEntities.Contains(id))
							{
								e.p.VisibleEntities.Add(id);
								e.p.SendNamedEntitySpawn(p);
							}
							continue;
						}
						else if (e.isItem)
						{
							p.VisibleEntities.Add(e.id);
							p.SendPickupSpawn(e);
							continue;
						}
						else if (e.isAI)
						{
							p.VisibleEntities.Add(e.id);
							p.SendMobSpawn(e);
							continue;
						}
                        else if (e.isObject)
                        {
                            p.VisibleEntities.Add(e.id);
                            p.SendObjectSpawn(e);
                            continue;
                        }
					}
				}
			}
			foreach (int i in p.VisibleEntities.ToArray())
			{
				if (!Entities.ContainsKey(i))
				{
					p.VisibleEntities.Remove(i);
					continue;
				}

				Entity e = Entities[i];
				if (e.isItem)
				{
					Point3 diff = pos - e.pos;

                    //Console.WriteLine(diff.x + " " + diff.z);
                    if (Math.Abs(diff.x) <= 1.5 && Math.Ceiling(diff.y) <= 0 && Math.Ceiling(diff.y) >= -1 && Math.Abs(diff.z) <= 1.5)
					{
						if (!e.I.OnGround || e.age < 10) continue;
						e.I.OnGround = false;

                        p.SendPickupAnimation(e.id);
                        Player.players.ForEach(delegate(Player pl)
                        {
                            if (pl != p && pl.level == p.level && pl.VisibleEntities.Contains(p.id))
                                pl.SendPickupAnimation(e.id, p.id);
                        });

						p.inventory.Add(e.I);
                        RemoveEntity(e);
					}
				}
				/*if (e.isAI)
				{
					Point3 sendme = e.pos * 32;
					byte[] bytes = new byte[18];
					util.EndianBitConverter.Big.GetBytes(e.id).CopyTo(bytes, 0);
					util.EndianBitConverter.Big.GetBytes((int)sendme.x).CopyTo(bytes, 4);
					util.EndianBitConverter.Big.GetBytes((int)sendme.y).CopyTo(bytes, 8);
					util.EndianBitConverter.Big.GetBytes((int)sendme.z).CopyTo(bytes, 12);
					bytes[16] = (byte)(e.ai.rot[0] / 1.40625);
					bytes[17] = (byte)(e.ai.rot[1] / 1.40625);

					if (!p.VisibleEntities.Contains(i)) continue;
					if (!p.MapLoaded) continue;
					p.SendRaw(0x22, bytes);
				}*/
			}
			foreach (int i in p.VisibleEntities.ToArray())
			{
				if (!tempelist.Contains(i))
				{
					p.VisibleEntities.Remove(i);
					p.SendDespawn(i);
				}
			}
		}
        public static void EntityPhysics()
        {
            Entity e;
            foreach (int i in Entities.Keys.ToArray())
            {
                try
                {
                    if (!Entities.ContainsKey(i)) continue;
                    e = Entities[i]; e.Tick();
                    if (e.isPlayer) continue; // Players don't have physics.
                    if (e.isObject) e.obj.Physics();
                    if (e.isAI) e.ai.Update();
                    if (e.isItem) e.I.Physics();
                    if (!e.isPlayer) e.UpdatePosition();
                }
                catch { }
            }
        }

        #region META DATA HANDLER
        public byte[] GetMetaByteArray()
        {
            return MCUtil.Entities.GetMetaBytes(metadata);
        }

        public void SetMeta(byte index, object obj)
        {
            metadata[index] = obj;
        }
        public void SetMetaBit(byte index, byte bit, bool value)
        {
            if (bit < 0) throw new ArgumentOutOfRangeException("Bit cannot be less than 0.");
            if (bit > 7) throw new ArgumentOutOfRangeException("Bit cannot be greater than 7.");
            if (metadata[index] != null && metadata[index].GetType() == typeof(byte))
            {
                byte b = (byte)metadata[index];
                if (value) b |= (byte)(1 << bit);
                else if ((b >> bit & 1) == 1) b ^= (byte)(1 << bit);
                metadata[index] = b;
            }
            else metadata[index] = (byte)(1 << bit);
        }
        #endregion

        public void Tick()
        {
            age++;
        }

        public static void AddEntity(Entity e)
        {
            if (!Entities.ContainsKey(e.id))
            {
                Entities.Add(e.id, e);
                Point pt = new Point((int)e.pos.x >> 4, (int)e.pos.z >> 4);
                if (e.level.chunkData.ContainsKey(pt))
                    e.level.chunkData[pt].AddEntity(e);
            }
        }
        public static void RemoveEntity(Entity e)
        {
            if (Entities.ContainsKey(e.id))
            {
                Entities.Remove(e.id);
                Point pt = new Point((int)e.pos.x >> 4, (int)e.pos.z >> 4);
                if (e.level.chunkData.ContainsKey(pt))
                    e.level.chunkData[pt].RemoveEntity(e);
            }
        }
		
		public static int FreeId()
		{
			/*int i = 0;
			do {
			i = random.Next();
			} while(Entities.ContainsKey(i));
			return i;*/

            return nextId++;
		}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Entity{").Append(id).Append(':').Append((int)pos.x).Append(',').Append((int)pos.y).Append(',').Append((int)pos.z).Append(':');
            if (isPlayer) sb.Append(p.username);
            else if (isItem) sb.Append("Item{").Append(I.id).Append(":").Append(I.count).Append(":").Append(I.meta).Append('}');
            else if (isAI) sb.Append(ai.GetType().Name);
            else if (isObject) sb.Append(obj.GetType().Name);
            else sb.Append("UNKNOWN");
            sb.Append('}');
            return sb.ToString();
        }

        internal void UpdatePosition()
        {
            bool forceTp = false;
            if ((DateTime.Now - lastPosSync).TotalSeconds >= 60)
            {
                lastPosSync = DateTime.Now;
                forceTp = true;
            }

            Point3 temppos = (pos * 32) / new Point3(32), tempoldpos = (oldpos * 32) / new Point3(32);
            Point3 diff = temppos - tempoldpos;
            double diff1 = temppos.mdiff(tempoldpos);

            //TODO move this?
            if (isPlayer && p.isFlying) p.FlyCode();

            if ((int)(diff1 * 32) == 0 && !forceTp)
            {
                if ((int)(rot[0] - oldrot[0]) != 0 || (int)(rot[1] - oldrot[1]) != 0)
                {
                    byte[] bytes = new byte[6];
                    util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                    bytes[4] = (byte)(rot[0] / 1.40625);
                    bytes[5] = (byte)(rot[1] / 1.40625);
                    Player.players.ForEach(delegate(Player pl)
                    {
                        if (pl != p && pl.MapLoaded && pl.VisibleEntities.Contains(id))
                            pl.SendRaw(0x20, bytes);
                    });
                    rot.CopyTo(oldrot, 0);
                }
            }
            else if ((int)(diff1 * 32) <= 127 && !forceTp)
            {
                Point3 sendme = diff * 32;
                byte[] bytes = new byte[9];
                util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                bytes[4] = (byte)sendme.x;
                bytes[5] = (byte)sendme.y;
                bytes[6] = (byte)sendme.z;
                bytes[7] = (byte)(rot[0] / 1.40625);
                bytes[8] = (byte)(rot[1] / 1.40625);
                Player.players.ForEach(delegate(Player pl)
                {
                    if (pl != p && pl.MapLoaded && pl.VisibleEntities.Contains(id))
                        pl.SendRaw(0x21, bytes);
                });
                if (Math.Abs(sendme.x) > 0) oldpos.x = pos.x;
                if (Math.Abs(sendme.y) > 0) oldpos.y = pos.y;
                if (Math.Abs(sendme.z) > 0) oldpos.z = pos.z;
                rot.CopyTo(oldrot, 0);
            }
            else
            {
                Point3 sendme = pos * 32;
                byte[] bytes = new byte[18];
                util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes((int)sendme.x).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes((int)sendme.y).CopyTo(bytes, 8);
                util.EndianBitConverter.Big.GetBytes((int)sendme.z).CopyTo(bytes, 12);
                bytes[16] = (byte)(rot[0] / 1.40625);
                bytes[17] = (byte)(rot[1] / 1.40625);
                Player.players.ForEach(delegate(Player pl)
                {
                    if (pl != p && pl.MapLoaded && pl.VisibleEntities.Contains(id))
                        pl.SendRaw(0x22, bytes);
                });
                oldpos = pos;
                rot.CopyTo(oldrot, 0);
            }
        }
	}
}
