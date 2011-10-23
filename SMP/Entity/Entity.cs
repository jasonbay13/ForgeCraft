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

namespace SMP
{
	public class Entity
	{
		public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();

		public Chunk c
		{
			get
			{
				return Chunk.GetChunk((int)(pos.x / 16), (int)(pos.z / 16), level);
			}
		}
		public Chunk CurrentChunk;

		public Point3 pos
		{
			get
			{
				if(isPlayer) return p.pos;
				if(isItem) return I.pos;
				if(isAI) return ai.pos;
				if(isObject) return obj.pos;

				return Point3.Zero;
			}
		}

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
		public int id;

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
			id = FreeId();
			isItem = true;
			level = l;

			if (I.OnGround)
				UpdateChunks(false, false);

			Entities.Add(id, this);
		}
		public Entity(AI ai, World l)
		{
			this.ai = ai;
			id = FreeId();
			isAI = true;
			level = l;

			Entities.Add(id, this);
		}
		public Entity(bool lightning) //Stand in entity for lightning
		{
			id = FreeId();
		}

        public void UpdateChunks(bool force, bool forcesend)
        {
            if (c == null)
                level.LoadChunk((int)(pos.x / 16), (int)(pos.z / 16));
            if (c == null || (c == CurrentChunk && !force))
                return;

            try
            {
                if (CurrentChunk != null) CurrentChunk.Entities.Remove(this);
                c.Entities.Add(this);
                CurrentChunk = c;
            }
            catch
            {
                Server.Log("Error Updating chunk for " + isPlayer.ToString() + " " + isItem.ToString() + " " + isAI.ToString() + " " + id);
            }
            if (isPlayer && p.LoggedIn)
            {
                //bool locked = false;
                List<Point> templist = new List<Point>();

                int sx = CurrentChunk.point.x - p.viewdistance; //StartX
                int ex = CurrentChunk.point.x + p.viewdistance; //EndX
                int sz = CurrentChunk.point.z - p.viewdistance; //StartZ
                int ez = CurrentChunk.point.z + p.viewdistance; //EndZ

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
                                p.level.LoadChunk(po.x, po.z);

                            if (p.level.chunkData.ContainsKey(po))
                            {
                                Chunk ch = p.level.chunkData[po];
                                p.SendChunk(ch);
                                ch.Update(p.level, p);
                            }
                            else
                                World.chunker.QueueChunkSend(po, p);
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

            int sx = CurrentChunk.point.x - p.viewdistance; //StartX
            int ex = CurrentChunk.point.x + p.viewdistance; //EndX
            int sz = CurrentChunk.point.z - p.viewdistance; //StartZ
            int ez = CurrentChunk.point.z + p.viewdistance; //EndZ
			for (int x = sx; x <= ex; x++)
			{
				for (int z = sz; z <= ez; z++)
				{
					if (!level.chunkData.ContainsKey(new Point(x, z))) { continue; }

					foreach (Entity e in p.level.chunkData[new Point(x, z)].Entities.ToArray())
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
							p.SpawnMob(e);
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
						if (!e.I.OnGround) continue;
						e.I.OnGround = false;

                        p.SendPickupAnimation(e.id);
                        Player.players.ForEach(delegate(Player pl)
                        {
                            if (pl != p && pl.level == p.level && pl.VisibleEntities.Contains(p.id))
                                pl.SendPickupAnimation(e.id, p.id);
                        });

						e.CurrentChunk.Entities.Remove(e);
						p.inventory.Add(e.I);
					}
				}
				if (e.isAI)
				{
					Point3 sendme = e.pos * 32;
					byte[] bytes = new byte[18];
					util.EndianBitConverter.Big.GetBytes(e.id).CopyTo(bytes, 0);
					util.EndianBitConverter.Big.GetBytes((int)sendme.x).CopyTo(bytes, 4);
					util.EndianBitConverter.Big.GetBytes((int)sendme.y).CopyTo(bytes, 8);
					util.EndianBitConverter.Big.GetBytes((int)sendme.z).CopyTo(bytes, 12);
					bytes[16] = (byte)(e.ai.yaw / 1.40625);
					bytes[17] = (byte)(e.ai.pitch / 1.40625);

					if (!p.VisibleEntities.Contains(i)) continue;
					if (!p.MapLoaded) continue;
					p.SendRaw(0x22, bytes);
				}
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
            for (int i = 0; i < Entities.Count; i++)
            {
                try
                {
                    e = Entities[i];
                    if (e.isPlayer) continue; // Players don't have physics.
                    if (e.isObject) continue; // TODO
                    if (e.isAI) e.ai.Update();
                    if (e.isItem) e.I.Physics();
                }
                catch { }
            }
        }
		
		public static int FreeId()
		{
			int i = 0;
			do {
			i = random.Next();
			} while(Entities.ContainsKey(i));
			return i;
		}
	}
}
