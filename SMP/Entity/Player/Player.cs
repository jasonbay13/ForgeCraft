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
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SMP.Commands;
using Substrate.Nbt;

namespace SMP
{
	public partial class Player : System.IDisposable
	{
		public static List<Player> players = new List<Player>();
		public Socket socket;
		public World level { get { return e.level; } set { e.level = value; } }
		public int viewdistance = Server.ViewDistance;
		byte mode = Server.mode;
        #region Mode Thingy
        public byte Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                if (LoggedIn) SendState(3, mode);
            }
        }
        #endregion

        public short current_slot_holding { get { return inventory.current_index; } set { inventory.current_index = value; current_block_holding = inventory.items[value]; } }
        public Item current_block_holding { get { return inventory.current_item; } set { inventory.current_item = value; SendItem(inventory.current_index, inventory.current_item.item, inventory.current_item.count, inventory.current_item.meta); } }

		byte[] buffer = new byte[0];
		byte[] tempbuffer = new byte[0xFF];

		public bool disconnected = false;
        public bool LoggedIn { get; protected set; }
		bool MapSent = false;
		public bool MapLoaded = false;
		//Health Stuff
        public short health = 20;
		public short food = 20;
		public float Saturation = 5.0f;
		//END Health Stuff
		public double Stance;
		public Point3 pos;
		public Point3 oldpos = Point3.Zero;
		public float[] rot;
        public float[] oldrot = new float[2];
		byte onground;
        public bool OnGround { get { return onground == 1; } set { onground = (byte)(value ? 1 : 0); } }
		public int id { get { return e.id; } }
        private DateTime pingdate = new DateTime();
        private DateTime lastPosSync = new DateTime();
        public short Ping = 500;

		public Chunk chunk { get { return e.CurrentChunk; } }
        public Chunk chunknew { get { return e.c; } }

		public Inventory inventory;
		public bool OpenWindow = false; //Tells the inventory system if the player has an open window (Not used for player inventory)
		public Windows window; //The window that is currently open (this isnt used for player inventory)
		public Item OnMouse = Item.Nothing; //The Item the player currently has picked up
        public Experience Experience = new Experience();

		public List<Point> VisibleChunks = new List<Point>();
		public List<int> VisibleEntities = new List<int>();
		public List<Point3> FlyList = new List<Point3>();

        #region Custom Command / Plugin Event
        //Events for Custom Command and Plugins ------------------------------------
        public delegate void OnPlayerConnect(Player p);
        public delegate void OnPlayerAuth(Player p);
        public static event OnPlayerConnect PlayerConnect;
        public event OnPlayerAuth OnAuth;
        public static event OnPlayerAuth PlayerAuth;
        public delegate void OnPlayerChat(string message, Player p);
        public event OnPlayerChat OnChat;
        public static event OnPlayerChat PlayerChat;
        public delegate void OnPlayerCommand(string cmd, string message, Player p);
        public event OnPlayerCommand OnCommand;
        public static event OnPlayerCommand PlayerCommand;
        public delegate void OnPlayerDisconnect(Player p);
        public static event OnPlayerDisconnect PlayerDisconnect;
        public event OnPlayerDisconnect OnDisconnect;
        public delegate void OnPlayerKicked(Player p, string reason);
        public static event OnPlayerKicked PlayerKicked;
        public event OnPlayerKicked OnKicked;
        public delegate void OnCrouchChange(Player p);
        public event OnCrouchChange OnCrouch;
        public static event OnCrouchChange PlayerCrouch;
        public delegate void OnSpritChange(Player p);
        public event OnSpritChange OnSprint;
        public static event OnSpritChange PlayerSprint;
        public delegate void OnPlayerRespawn(Player p);
        public static event OnPlayerRespawn PlayerRespawn;
        public event OnPlayerRespawn OnRespawn;
        public delegate void OnEXPGain(Player p, short expgain);
        public event OnEXPGain EXPGain;
        public static event OnEXPGain PlayerEXPGain;
        public delegate void OnEXPLost(Player p, short expgain);
        public event OnEXPLost EXPLost;
        public static event OnEXPLost PlayerEXPLost;
        public delegate void OnWindowClose(Player p);
        public event OnWindowClose WindowClose;
        public static event OnWindowClose PlayerWindowClose;
        //Other things for plugins ----------
        internal bool cancelchat = false;
        internal bool cancelcommand = false;
        internal bool cancelmove = false;
        internal bool canceldig = false;
        internal bool cancelkick = false;
        internal bool cancelgain = false;
        internal bool cancellost = false;
        internal bool cancelclose = false;
        internal bool cancelleft = false;
        internal bool cancelright = false;
        internal bool cancelplace = false;
        internal bool canceldestroy = false;
        internal bool cancelrespawn = false;
        internal bool CheckEXPGain(short exp)
        {
            if (EXPGain != null)
                EXPGain(this, exp);
            if (PlayerEXPGain != null)
                PlayerEXPGain(this, exp);
            if (cancelgain)
            {
                cancelgain = false;
                return true;
            }
            return false;
        }
        internal bool CheckEXPLost(short exp)
        {
            if (EXPLost != null)
                EXPLost(this, exp);
            if (PlayerEXPLost != null)
                PlayerEXPLost(this, exp);
            if (cancellost)
            {
                cancellost = false;
                return true;
            }
            return false;
        }
        //Other things for plugins ----------
        //Events for Custom Command and Plugins -------------------------------------
        #endregion


		//Groups and Permissions
		public Group group;
		public List<string> AdditionalPermissions = new List<string>();
		public List<Group> SubGroups = new List<Group>();
		public string Prefix = "";
		public string Suffix = "";
		public string color = "";
		public bool CanBuild = false;
		public string NickName = "";
		
		//Other Player settings Donotdisturb, god mode etc.
		public bool DoNotDisturb = false; //blocks all incoming chat except pm's
		public bool GodMode = false; //obvious, but not used anywhere yet
		public bool AFK = false;
        public bool Crouching = false;
        public bool IsOnFire = false;
        public bool isFlying = false;
        public int FlyingUpdate = 100;
		public Account DefaultAccount;
		public List<Account> Accounts = new List<Account>();

        // This is for commands and stuff
        public object BlockChangeObject;
		
		Entity e;
		public string ip;
		public string username;
		bool hidden = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Player"/> class.
		/// </summary>
		public Player()
		{
			
		}
		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			try
			{
				pos = new double[3] { 0, 72, 0 };
				//oldpos = new double[3] { 0, 0, 0 };
				rot = new float[2] { 0,0 };
				Stance = 72;

				e = new Entity(this, Server.mainlevel);
				
				ip = socket.RemoteEndPoint.ToString().Split(':')[0];
				
				inventory = new Inventory(this);
				players.Add(this);
				//Event --------------------
				if (PlayerConnect != null)
					PlayerConnect(this);
				//Event --------------------
				socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
		}
		static void Receive(IAsyncResult result)
		{
			Player p = (Player)result.AsyncState;
			if (p.disconnected || p.socket == null)
				return;
            try
            {
                int length = p.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] b = new byte[p.buffer.Length + length];
                Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
                Buffer.BlockCopy(p.tempbuffer, 0, b, p.buffer.Length, length);

                p.buffer = p.HandleMessage(b);
                p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), p);
            }
            catch (SocketException)
            {
                p.Disconnect();
            }
            catch (ObjectDisposedException)
            {
                p.Disconnect();
            }
            catch (NullReferenceException)
            {
                p.Disconnect();
            }
            catch (Exception e)
            {
                p.Disconnect();
                Server.Log(e.Message);
                Server.Log(e.StackTrace);
            }
		}
		byte[] HandleMessage(byte[] buffer)
		{
			try
			{
                //Console.WriteLine(buffer[0] + " " + (buffer.Length - 1));
				int length = 0; byte msg = buffer[0];
				// Get the length of the message by checking the first byte
				switch (msg)
				{
                    case 0x00: length = 4; if (buffer.Length < 5 || util.EndianBitConverter.Big.ToInt32(buffer, 1) == 1337) ping(); break; //Keep alive
					case 0x01: /*Server.Log("auth start");*/ length = ((util.EndianBitConverter.Big.ToInt16(buffer, 5) * 2) + 22); break; //Login Request
					case 0x02: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Handshake
					case 0x03: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //Chat
					case 0x07: length = 9; break; //Entity Use
					case 0x09: length = 13; break; //Respawn
					
					case 0x0A: length = 1; break; //OnGround incoming
					case 0x0B: length = 33; break; //Pos incoming
					case 0x0C: length = 9; break; //Look Incoming
					case 0x0D: length = 41; break; //Pos and look incoming

					case 0x0E: length = 11; break; //Digging
                    case 0x0F: length = 10 + Item.GetDataLength(buffer, 11); break; //Block Placement
					case 0x10: length = 2; break; //Holding Change
					case 0x12: length = 5; break; //Animation Change
                    case 0x13: length = 5; break; //Entity Action

					case 0x65: length = 1; break; //Close Window
					case 0x66: //Window click
                        length = 7 + Item.GetDataLength(buffer, 8);
						/*if (util.EndianBitConverter.Big.ToInt16(buffer, 7) != -1) length += 3;
                        if (BlockData.IsItemDamageable(util.EndianBitConverter.Big.ToInt16(buffer, 7)))
                        {
                            length += 2;
                            Console.WriteLine("length " + util.EndianBitConverter.Big.ToInt16(buffer, 12));
                            if (util.EndianBitConverter.Big.ToInt16(buffer, 12) != -1)
                            {
                                length += util.EndianBitConverter.Big.ToInt16(buffer, 12);
                                for (int i = 0; i < util.EndianBitConverter.Big.ToInt16(buffer, 12); i++)
                                    Console.Write(buffer[14 + i] + " ");
                                Console.WriteLine();
                            }
                        }*/
                        /*for (int i = 0; i < util.EndianBitConverter.Big.ToInt16(buffer, 12); i++)
                            Console.Write(buffer[14 + i] + " ");
                        Console.WriteLine();*/
						break;
                    case 0x6A: length = 4; break; //Transaction
                    case 0x6B: //Creative inventory action
                        length = 2 + Item.GetDataLength(buffer, 3);
                        /*if (util.EndianBitConverter.Big.ToInt16(buffer, 2) != -1) length += 3;
                        if (BlockData.IsItemDamageable(util.EndianBitConverter.Big.ToInt16(buffer, 2))) {
                            length += 2;
                            if (util.EndianBitConverter.Big.ToInt16(buffer, 8) != -1)
                                length += util.EndianBitConverter.Big.ToInt16(buffer, 8);
                        }*/
                        break;
                    case 0x6C: length = 2; break; //Enchant item
					case 0x82: //Update sign
                        short a = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 11) * 2);
                        short b = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 13 + a) * 2);
                        short c = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 15 + a + b) * 2);
                        short d = (short)(util.EndianBitConverter.Big.ToInt16(buffer, 17 + a + b + c) * 2);
                        length = 18 + a + b + c + d;
						break;
					case 0xFE: length = 0; //Server list ping
						Kick(Server.Motd + "§" + (Player.players.Count - 1) + "§" + Server.MaxPlayers);
						//socket.Close();
						Disconnect();
						return new byte[0];
					case 0xFF: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) * 2) + 2); break; //DC

					default:
                        Server.ServerLogger.Log("Unhandled message ID: " + msg);
					    Kick("Unknown packet ID: " + msg);
						return new byte[0];
				}
				if (buffer.Length > length)
				{
					byte[] message = new byte[length];
					Buffer.BlockCopy(buffer, 1, message, 0, length);

					byte[] tempbuffer = new byte[buffer.Length - length - 1];
					Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

					buffer = tempbuffer;

					//if(username!= "Merlin33069") Server.Log(msg + "");
					switch (msg)
					{
						case 0x01:
							//Server.Log("Authentication");
							HandleLogin(message);
							break;
						case 0x02:
							//Server.Log("Handshake");
							HandleHandshake(message);
							break;
						case 0x03:
							//Server.Log("Chat Message");
							HandleChatMessagePacket(message);
							break;
                        case 0x09: HandleRespawn(message); break; //when user presses respawn button
						case 0x0A: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPacket(message); break; //Player onground Incoming
						case 0x0B: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPositionPacket(message); break; //Pos incoming
						case 0x0C: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerLookPacket(message); break; //Look incoming
						case 0x0D: if (!MapSent) { MapSent = true; SendMap(); } HandlePlayerPositionAndLookPacket(message); break; //Pos and look incoming
						case 0x0E: HandleDigging(message); break; //Digging
					    case 0x0F: HandleBlockPlacementPacket(message); break; //Block Placement
						case 0x10: HandleHoldingChange(message); break; //Holding Change
						case 0x12: HandleAnimation(message); break;
						case 0x13: HandleEntityAction(message); break;
						case 0x65: HandleWindowClose(message); break; //Window Closed
						case 0x66: HandleWindowClick(message); break; //Window Click
                        case 0x6B: HandleCreativeInventoryAction(message); break;
                        case 0x82: HandleUpdateSign(message); break;
                        case 0xFF: HandleDC(message); break; //DC
					}
                    if (buffer.Length > 0)
                        buffer = HandleMessage(buffer);
                    else
                        return new byte[0];
				}
			}
			catch (Exception e)
			{
				Server.Log(e.Message);
				Server.Log(e.StackTrace);
			}
			return buffer;
		}

		#region OUTGOING
			#region Raw
			void SendRaw(byte id)
			{
				SendRaw(id, new byte[0]);
			}
			/// <summary>
			/// Send Data over to the client
			/// </summary>
			/// <param name='id'>
			/// Identifier. The packet ID that you want to send
			/// </param>
			/// <param name='send'>
			/// Send. The byte[] information you want to send
			/// </param>
			public void SendRaw(byte id, byte[] send)
			{
				//if (id != 0 && id != 4 && id != 50 && id != 51 && id != 22) LogPacket(id, send);
				//Console.WriteLine(id);
				if (socket == null || !socket.Connected)
					return;
				byte[] buffer = new byte[send.Length + 1];
				buffer[0] = (byte)id;
				send.CopyTo(buffer, 1);

				try
				{
                    socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);
					buffer = null;
				}
				catch (SocketException)
				{
					buffer = null;
					Disconnect();
				}
			}
			#endregion
			#region Loop Stuff, Time/Pos
			/// <summary>
			/// Update the players time
			/// </summary>
			public void SendTime()
			{
				if (!LoggedIn) return;

				byte[] tosend = new byte[8];
				util.EndianBitConverter.Big.GetBytes(level.time).CopyTo(tosend, 0);
				SendRaw(0x04, tosend);
			}
            public void ping()
            {
                Ping = 500;
                Ping = (short)((DateTime.Now - pingdate).Milliseconds);
                UpdatePList(true);
            }
			public static void GlobalUpdate()
			{
				players.ForEach(delegate(Player p)
				{
					if (!p.LoggedIn) return;
					if (!p.hidden)
					{
						p.UpdatePosition();
					}
				});
			}
            public static void PlayerlistUpdate()
            {
                players.ForEach(delegate(Player p)
                {
                    if (!p.LoggedIn) return;
                    p.pingdate = DateTime.Now;
                    p.SendRaw(0, util.EndianBitConverter.Big.GetBytes(1337));
                });
            } 
			public void SendKeepAlive()
			{
				byte[] bytes = new byte[4];
				util.EndianBitConverter.Big.GetBytes(Entity.random.Next()).CopyTo(bytes, 0);
				SendRaw(0, bytes);
			}
			void UpdatePosition()
			{
				e.UpdateEntities();
				if (!LoggedIn) return;

                bool forceTp = false;
                if ((DateTime.Now - lastPosSync).TotalSeconds >= 10)
                {
                    lastPosSync = DateTime.Now;
                    forceTp = true;
                }

                Point3 temppos = (pos * 32) / new Point3(32), tempoldpos = (oldpos * 32) / new Point3(32);
                Point3 diff = temppos - tempoldpos;
                double diff1 = temppos.mdiff(tempoldpos);

				//TODO move this?
				if(isFlying) FlyCode();

                if ((int)(diff1 * 32) == 0 && !forceTp)
				{
                    if ((int)(rot[0] - oldrot[0]) != 0 || (int)(rot[1] - oldrot[1]) != 0)
                    {
                        byte[] bytes = new byte[6];
                        util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                        bytes[4] = (byte)(rot[0] / 1.40625);
                        bytes[5] = (byte)(rot[1] / 1.40625);
                        foreach (int i in VisibleEntities.ToArray())
                        {
                            Entity e1 = Entity.Entities[i];
                            if (!e1.isPlayer) continue;
                            if (e1.p == this) continue;
                            if (!e1.p.MapLoaded) continue;
                            e1.p.SendRaw(0x20, bytes);
                        }
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
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e1 = Entity.Entities[i];
						if (!e1.isPlayer) continue;
                        if (e1.p == this) continue;
						if (!e1.p.MapLoaded) continue;
						e1.p.SendRaw(0x21, bytes);
					}
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
					foreach (int i in VisibleEntities.ToArray())
					{
						if(!Entity.Entities.ContainsKey(i)) continue;
						Entity e1 = Entity.Entities[i];
						if (!e1.isPlayer) continue;
                        if (e1.p == this) continue;
						if (!e1.p.MapLoaded) continue;
						e1.p.SendRaw(0x22, bytes);
					}
					oldpos = pos;
                    rot.CopyTo(oldrot, 0);
				}
			}
			#endregion
			#region Misc Packets Sending
			/// <summary>
			/// Sends an animation to the player.
			/// </summary>
			public void SendAnimation( int eid, byte type )
			{
				if (!MapLoaded) return;

				byte[] data = new byte[5];
				util.EndianBitConverter.Big.GetBytes( eid ).CopyTo( data, 0 );
				data[4] = type;
				SendRaw( 0x12, data );
			}
			/// <summary>
			/// Update the players health
			/// </summary>
			public void SendHealth()
			{
				byte[] tosend = new byte[8];
				util.EndianBitConverter.Big.GetBytes(health).CopyTo(tosend, 0);
				util.EndianBitConverter.Big.GetBytes(food).CopyTo(tosend, 2);
				util.EndianBitConverter.Big.GetBytes(Saturation).CopyTo(tosend, 4);
				SendRaw(0x08, tosend);
			}
            /// <summary>
            /// Adds effect to player
            /// </summary>
            /// <param name="effect">See http://mc.kev009.com/Protocol#Entity_Effect_.280x29.29 for values</param>
            public void SendEntityEffect(byte effect, byte amplifier, short duration)
            {
                byte[] bytes = new byte[8];
                util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                bytes[4] = effect;
                bytes[5] = amplifier;
                util.EndianBitConverter.Big.GetBytes(duration).CopyTo(bytes, 6);
                SendRaw(0x29, bytes);
            }
			void CheckOnFire()
			{
				// check for players on fire before join map.
				for (int i = 0; i < Player.players.Count; i++)
				{
					if (players[i].IsOnFire && players[i] != this && VisibleEntities.Contains(players[i].id))
					{
						byte[] bytes = new byte[7];
						util.EndianBitConverter.Big.GetBytes(players[i].id).CopyTo(bytes, 0);
						bytes[4] = 0x00;
						bytes[5] = 0x01;
						bytes[6] = 0x7F;
						SendRaw(0x28, bytes);
					}
				}
			}
			void crouch(bool crouching)
			{
				if (!MapLoaded) return;

				Crouching = crouching;

				if (!Crouching && IsOnFire) { SetFire(true); }
				
				byte[] bytes2 = new byte[7];
				
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes2, 0);
				bytes2[4] = 0x00;
				if (Crouching && !IsOnFire) bytes2[5] = 0x02;
				else if (Crouching) bytes2[5] = 0x03;
				else bytes2[5] = 0x00;
				bytes2[6] = 0x7F;
				
				for (int i = 0; i < players.Count; i++)
				{
					if (players[i] != this && players[i].LoggedIn)
					{
						players[i].SendRaw(0x28, bytes2);
					}
				}
			}
			public void SetFire(bool onoff)
			{
				byte[] bytes2 = new byte[7];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes2, 0);
				bytes2[4] = 0x00;
				if (onoff) bytes2[5] = 0x01;
				else bytes2[5] = 0x00;
				bytes2[6] = 0x7F;
				for (int i = 0; i < players.Count; i++)
				{
					if (players[i] != this && players[i].LoggedIn)
					{
						players[i].SendRaw(0x28, bytes2);
					}
				}
				IsOnFire = onoff;
				//if (Crouching) crouch();
			}
			/// <summary>
			/// Send the player the spawn point (Only usable after login)
			/// </summary>
			public void SendSpawnPoint()
			{
				byte[] bytes = new byte[12];
				util.EndianBitConverter.Big.GetBytes((int)level.SpawnX).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes((int)level.SpawnY).CopyTo(bytes, 4);
				util.EndianBitConverter.Big.GetBytes((int)level.SpawnZ).CopyTo(bytes, 8);
				SendRaw(0x06, bytes);
			}
			/// <summary>
			/// Sends a player a blockchange
			/// </summary>
			/// <param name='x'>
			/// X. The x cords of the block
			/// </param>
			/// <param name='y'>
			/// Y. The y cords of the block
			/// </param>
			/// <param name='z'>
			/// Z. The z cords of the block
			/// </param>
			/// <param name='type'>
			/// Type. The ID of the block
			/// </param>
			/// <param name='meta'>
			/// Meta. The meta data of the block
			/// </param>
			public void SendBlockChange(int x, byte y, int z, byte type, byte meta)
			{
				byte[] bytes = new byte[11];
				util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
				bytes[4] = y;
				util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 5);
				bytes[9] = type;
				bytes[10] = meta;
				SendRaw(0x35, bytes);

                // USE FOR TESTING PURPOSES ONLY!
                /*List<Point3> points = new List<Point3>();
                for (double xx = x - 2; xx <= x + 2; xx++)
                    for (double yy = y - 2; yy <= y + 2; yy++)
                        for (double zz = z - 2; zz <= z + 2; zz++)
                            points.Add(new Point3(xx, yy, zz));
                SendExplosion(x, y, z, 2, points.ToArray());*/
			}
            public void SendBlockChange(int x, byte y, int z, byte type)
			{
				SendBlockChange(x, y, z, type, 0);
			}
			public void SendBlockChange(Point3 a, byte type, byte meta)
			{
				SendBlockChange((int)a.x, (byte)a.y, (int)a.z, type, meta);
			}
			public void SendBlockChange(Point3 a, byte type)
			{
				SendBlockChange(a, type, 0);
			}
            public void SendMultiBlockChange(int x, int z, BlockChangeData[] blocks)
            {
                blocks = (BlockChangeData[])blocks.Truncate(short.MaxValue);
                byte[] bytes = new byte[10 + (blocks.Length * 4)];
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes((short)blocks.Length).CopyTo(bytes, 8);

                BlockChangeData block;
                short[] coord = new short[blocks.Length];
                byte[] type = new byte[blocks.Length];
                byte[] meta = new byte[blocks.Length];
                for (int i = 0; i < blocks.Length; i++)
                {
                    block = blocks[i];
                    coord[i] = (short)((block.x << 12) | (block.z << 8) | block.y);
                    type[i] = block.type;
                    meta[i] = block.meta;
                }
                for (int i = 0; i < coord.Length; i++)
                    util.EndianBitConverter.Big.GetBytes(coord[i]).CopyTo(bytes, 10 + (i * 2));
                type.CopyTo(bytes, 10 + (blocks.Length * 2));
                meta.CopyTo(bytes, 10 + (blocks.Length * 3));

                SendRaw(0x34, bytes);
            }
            public void SendMultiBlockChange(Point a, BlockChangeData[] blocks)
            {
                SendMultiBlockChange(a.x, a.z, blocks);
            }
            public void SendSoundEffect(int x, byte y, int z, int type, int data)
            {
                byte[] bytes = new byte[17];
                util.EndianBitConverter.Big.GetBytes(type).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 4);
                bytes[8] = y;
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 9);
                util.EndianBitConverter.Big.GetBytes(data).CopyTo(bytes, 13);
                SendRaw(0x3D, bytes);
            }
            public void SendSoundEffect(int x, byte y, int z, int type)
            {
                SendSoundEffect(x, y, z, type, 0);
            }
            public void SendSoundEffect(Point3 a, int type, int data)
            {
                SendSoundEffect((int)a.x, (byte)a.y, (int)a.z, type, data);
            }
            public void SendSoundEffect(Point3 a, int type)
            {
                SendSoundEffect(a, type, 0);
            }
            public void SendExplosion(double x, double y, double z, float radius, Point3[] records)
            {
                byte[] bytes = new byte[32 + (records.Length * 3)];
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 8);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 16);
                util.EndianBitConverter.Big.GetBytes(radius).CopyTo(bytes, 24);
                util.EndianBitConverter.Big.GetBytes(records.Length).CopyTo(bytes, 28);

                Point3 record, position = new Point3((int)x, (int)y, (int)z);
                for (int i = 0; i < records.Length; i++)
                {
                    record = records[i] - position;
                    bytes[32 + (i * 3)] = (byte)record.x;
                    bytes[33 + (i * 3)] = (byte)record.y;
                    bytes[34 + (i * 3)] = (byte)record.z;
                }

                SendRaw(0x3C, bytes);
            }
            public void SendExplosion(Point3 a, float radius, Point3[] records)
            {
                SendExplosion(a.x, a.y, a.z, radius, records);
            }
            public void SendState(byte state, byte mode)
            {
                byte[] bytes = new byte[2];
                bytes[0] = state;
                bytes[1] = mode;
                SendRaw(0x46, bytes);
            }
            public void SendState(byte state)
            {
                SendState(state, 0);
            }
            public void SendBlockAction(int x, short y, int z, byte byte1, byte byte2)
            {
                byte[] bytes = new byte[12];
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 6);
                bytes[10] = byte1;
                bytes[11] = byte2;
                SendRaw(0x36, bytes);
            }
            public void SendBlockAction(Point3 a, byte byte1, byte byte2)
            {
                SendBlockAction((int)a.x, (short)a.y, (int)a.z, byte1, byte2);
            }
            public void SendItemData(short id, short meta, byte[] data) // This is for maps, not inventory!
            {
                data = (byte[])data.Truncate(255);
                byte[] bytes = new byte[5 + data.Length];
                util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(meta).CopyTo(bytes, 2);
                bytes[4] = (byte)data.Length;
                data.CopyTo(bytes, 5);
                SendRaw(0x83, bytes);
            }

            public void SendUseBed(int eid, int x, byte y, int z)
            {
                byte[] bytes = new byte[14];
                util.EndianBitConverter.Big.GetBytes(eid).CopyTo(bytes, 0);
                bytes[4] = 0; // What is this?
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 5);
                bytes[9] = y;
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 10);
                SendRaw(0x11, bytes);
            }
            public void SendUseBed(int eid, Point3 a)
            {
                SendUseBed(eid, (int)a.x, (byte)a.y, (int)a.z);
            }
            public void SendUpdateSign(int x, short y, int z, bool additions, params string[] text)
            {
                if (text.Length != 4)
                    throw new ArgumentException("Text must be 4 strings.");

                if (additions)
                    for (int i = 0; i < text.Length; i++)
                        text[i] = MessageAdditions(text[i]);

                byte[] bytes = new byte[18 + (text[0].Length * 2) + (text[1].Length * 2) + (text[2].Length * 2) + (text[3].Length * 2)];
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 6);

                util.EndianBitConverter.Big.GetBytes((short)text[0].Length).CopyTo(bytes, 10);
                Encoding.BigEndianUnicode.GetBytes(text[0]).CopyTo(bytes, 12);
                util.EndianBitConverter.Big.GetBytes((short)text[1].Length).CopyTo(bytes, 12 + (text[0].Length * 2));
                Encoding.BigEndianUnicode.GetBytes(text[1]).CopyTo(bytes, 14 + (text[0].Length * 2));
                util.EndianBitConverter.Big.GetBytes((short)text[2].Length).CopyTo(bytes, 14 + (text[0].Length * 2) + (text[1].Length * 2));
                Encoding.BigEndianUnicode.GetBytes(text[2]).CopyTo(bytes, 16 + (text[0].Length * 2) + (text[1].Length * 2));
                util.EndianBitConverter.Big.GetBytes((short)text[3].Length).CopyTo(bytes, 16 + (text[0].Length * 2) + (text[1].Length * 2) + (text[2].Length * 2));
                Encoding.BigEndianUnicode.GetBytes(text[3]).CopyTo(bytes, 18 + (text[0].Length * 2) + (text[1].Length * 2) + (text[2].Length * 2));

                SendRaw(0x82, bytes);
            }
            public void SendUpdateSign(int x, short y, int z, params string[] text)
            {
                SendUpdateSign(x, y, z, true, text);
            }

            public static void GlobalUpdateSign(World wld, int x, short y, int z, params string[] text)
            {
                Player.players.ForEach(delegate(Player pl)
                {
                    if (pl.MapLoaded && pl.level == wld && pl.VisibleChunks.Contains(Chunk.GetChunk(x >> 4, z >> 4, pl.level).point))
                        pl.SendUpdateSign(x, y, z, text);
                });
            }
            public static void GlobalBlockAction(int x, short y, int z, byte byte1, byte byte2, World wld)
            {
                Player.players.ForEach(delegate(Player p1)
                {
                    if (p1.MapLoaded && p1.level == wld && p1.VisibleChunks.Contains(Chunk.GetChunk(x >> 4, z >> 4, p1.level).point))
                        p1.SendBlockAction(x, y, z, byte1, byte2);
                });
            }
            public static void GlobalBlockAction(Point3 a, byte byte1, byte byte2, World wld)
            {
                GlobalBlockAction((int)a.x, (short)a.y, (int)a.z, byte1, byte2, wld);
            }
            public static void GlobalSoundEffect(int x, byte y, int z, int type, int data, World wld, Player exclude = null)
            {
                Player.players.ForEach(delegate(Player p1)
                {
                    if (p1 != exclude && p1.MapLoaded && p1.level == wld && p1.VisibleChunks.Contains(Chunk.GetChunk(x >> 4, z >> 4, p1.level).point))
                        p1.SendSoundEffect(x, y, z, type, data);
                });
            }
            public static void GlobalSoundEffect(int x, byte y, int z, int type, World wld, Player exclude = null)
            {
                GlobalSoundEffect(x, y, z, type, 0, wld, exclude);
            }
            public static void GlobalSoundEffect(Point3 a, int type, int data, World wld, Player exclude = null)
            {
                GlobalSoundEffect((int)a.x, (byte)a.y, (int)a.z, type, data, wld, exclude);
            }
            public static void GlobalSoundEffect(Point3 a, int type, World wld, Player exclude = null)
            {
                GlobalSoundEffect(a, type, 0, wld, exclude);
            }

            public static void GlobalBreakEffect(int x, byte y, int z, int type, World wld, Player exclude = null)
            {
                GlobalSoundEffect(x, y, z, type, wld, exclude);
            }
            public static void GlobalBreakEffect(Point3 a, int type, World wld, Player exclude = null)
            {
                GlobalBreakEffect((int)a.x, (byte)a.y, (int)a.z, type, wld, exclude);
            }
            #endregion
            #region Teleport Player
            public void Teleport_Player(double x, double y, double z)
			{
				Teleport_Player(x, y, z, rot[0], rot[1]);
			}
			public void Teleport_Player(double[] a)
			{
				Teleport_Player(a[0], a[1], a[2], rot[0], rot[1]);
			}
			public void Teleport_Player(Point3 a)
			{
				Teleport_Player(a.x, a.y, a.z, rot[0], rot[1]);
			}
            public void Teleport_Player(Point3 a, float yaw, float pitch)
            {
                Teleport_Player(a.x, a.y, a.z, yaw, pitch);
            }
			public void Teleport_Player(double x, double y, double z, float yaw, float pitch)
			{
				if (!MapLoaded) return;

				byte[] tosend = new byte[41];
				util.EndianBitConverter.Big.GetBytes(x).CopyTo(tosend, 0);
				util.EndianBitConverter.Big.GetBytes(y + 1.65).CopyTo(tosend, 8);
				util.EndianBitConverter.Big.GetBytes(y).CopyTo(tosend, 16);
				util.EndianBitConverter.Big.GetBytes(z).CopyTo(tosend, 24);
				util.EndianBitConverter.Big.GetBytes(yaw).CopyTo(tosend, 32);
				util.EndianBitConverter.Big.GetBytes(pitch).CopyTo(tosend, 36);
				tosend[40] = onground;
				SendRaw(0x0D, tosend);
			}
            public void Teleport_Spawn()
            {
                Teleport_Player(level.SpawnPos, level.SpawnYaw, level.SpawnPitch);
            }
			#endregion
			#region Login Stuffs
			void SendLoginPass()
			{
				try
				{
					short length = (short)Server.name.Length;
					byte[] bytes = new byte[(length * 2) + 22];

					util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0); //id
					util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 4); //String
					Encoding.BigEndianUnicode.GetBytes(Server.name).CopyTo(bytes, 6); //String (actual string)
					util.EndianBitConverter.Big.GetBytes((long)level.seed).CopyTo(bytes, bytes.Length - 16); 
					bytes[bytes.Length - 5] = Server.mode;
					bytes[bytes.Length - 4] = (byte)level.dimension;
                    bytes[bytes.Length - 3] = Server.difficulty;
					bytes[bytes.Length - 2] = level.height;
					bytes[bytes.Length - 1] = Server.MaxPlayers;

					SendRaw(1, bytes);
				}
				catch(Exception e)
				{
					Server.Log(e.Message);
					Server.Log(e.StackTrace);
				}
				//SendMap();
			}
			void SendHandshake()
			{
			
				//Server.Log("Handshake out");
                string st = Server.VerifyNames ? Convert.ToString(new java.util.Random().nextLong(), 16) : "-";
				byte[] bytes = new byte[(st.Length * 2) + 2];
				util.EndianBitConverter.Big.GetBytes((short)st.Length).CopyTo(bytes, 0);
				Encoding.BigEndianUnicode.GetBytes(st).CopyTo(bytes, 2);
				//foreach (byte b in bytes)
				//{
				//    Server.Log(b + " <");
				//}
				//Server.Log("Handshake out-1");
				SendRaw(2, bytes);
				//Server.Log("Handshake out-2");
			}
			void SendLoginDone()
			{
				//Server.Log("Login Done");

				byte[] bytes = new byte[41];
				util.EndianBitConverter.Big.GetBytes(pos.x).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(Stance).CopyTo(bytes, 8);
				util.EndianBitConverter.Big.GetBytes(pos.y).CopyTo(bytes, 16);
				util.EndianBitConverter.Big.GetBytes(pos.z).CopyTo(bytes, 24);
				util.EndianBitConverter.Big.GetBytes(rot[0]).CopyTo(bytes, 32);
				util.EndianBitConverter.Big.GetBytes(rot[1]).CopyTo(bytes, 36);
				bytes[40] = onground;
				SendRaw(0x0D, bytes);

				//Server.Log(pos[0] + " " + pos[1] + " " + pos[2]);
			}
			#endregion
			#region Inventory stuff
			void SendInventory()
			{
				List<byte> data = new List<byte>();
                byte[] nbt;
				
				for(int i = 0; i <= 44; i++)
				{
					data.AddRange(util.BigEndianBitConverter.Big.GetBytes((short)this.inventory.items[i].item));
						
						if (this.inventory.items[i].item != -1 && this.inventory.items[i].item != 0)
						{
							data.Add(this.inventory.items[i].count);
							data.AddRange(util.BigEndianBitConverter.Big.GetBytes((short)this.inventory.items[i].meta));
                            if (this.inventory.items[i].IsDamageable())
                            {
                                nbt = this.inventory.items[i].GetEnchantmentNBTData();
                                data.AddRange(util.BigEndianBitConverter.Big.GetBytes((short)nbt.Length));
                                data.AddRange(nbt);
                            }
						}		
				}
				SendWindow(0, 45, data.ToArray());
			}
            public void SendItem(short slot, Item item) { SendItem(0, slot, item); }
            public void SendItem(short slot, short id, byte count, short use) { SendItem(0, slot, id, count, use); }
            public void SendItem(short slot, short id, byte count, short use, List<Enchantment> enchantments) { SendItem(0, slot, id, count, use, enchantments); }
            public void SendItem(byte windowID, short slot, Item item) { SendItem(windowID, slot, item.item, item.count, item.meta, item.enchantments); }
            public void SendItem(byte windowID, short slot, short id, byte count, short use) { SendItem(windowID, slot, id, count, use, new List<Enchantment>()); }
            public void SendItem(byte windowID, short slot, short id, byte count, short use, List<Enchantment> enchantments)
			{
				if (!FindBlocks.ValidItem(id))
					return;
			
				if (!MapLoaded) return;

                byte[] nbtData = new byte[0];
                if (BlockData.IsItemDamageable(id))
                    nbtData = Item.GetEnchantmentNBTData(enchantments);

				byte[] tosend;
                if (id == -1) tosend = new byte[5];
                else if (BlockData.IsItemDamageable(id)) tosend = new byte[10 + nbtData.Length];
                else tosend = new byte[8];
				tosend[0] = windowID;
				util.EndianBitConverter.Big.GetBytes(slot).CopyTo(tosend, 1);
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(tosend, 3);
				if (id != -1)
				{
					tosend[5] = count;
					util.EndianBitConverter.Big.GetBytes(use).CopyTo(tosend, 6);
                    if (BlockData.IsItemDamageable(id))
                    {
                        util.EndianBitConverter.Big.GetBytes((short)nbtData.Length).CopyTo(tosend, 8);
                        nbtData.CopyTo(tosend, 10);
                    }
				}
				SendRaw(0x67, tosend);
			}
			public void SendWindow(byte windowID, short count, byte[] items)
			{
				byte[] data = new byte[3 + items.Length];
				data[0] = windowID;
				util.BigEndianBitConverter.Big.GetBytes(count).CopyTo(data, 1);
				items.CopyTo(data, 3);
				SendRaw(0x68, data);
			}
            public void SendUpdateWindowProperty(byte windowID, short property, short value)
            {
                byte[] bytes = new byte[5];
                bytes[0] = windowID;
                util.EndianBitConverter.Big.GetBytes(property).CopyTo(bytes, 1);
                util.EndianBitConverter.Big.GetBytes(value).CopyTo(bytes, 3);
                SendRaw(0x69, bytes);
            }
            public void SendTransaction(byte windowID, short action, bool accepted)
            {
                byte[] bytes = new byte[4];
                bytes[0] = windowID;
                util.BigEndianBitConverter.Big.GetBytes(action).CopyTo(bytes, 1);
                util.BigEndianBitConverter.Big.GetBytes(accepted).CopyTo(bytes, 3);
                SendRaw(0x6A, bytes);
            }
			#endregion
			#region Map Stuff
			void SendMap()
			{
				//Server.Log("Sending");
				//int i = 0;
				//foreach (Chunk c in Server.mainlevel.chunkData.Values.ToArray())
				//{
				//	SendChunk(c);
				//	i++;
				//}
				//Server.Log(i + " Chunks sent");

                pos = level.SpawnPos;
				SendSpawnPoint();
				SendLoginDone();
				SendInventory();
                SendChunk(e.c);
                //e.UpdateChunks(true, false, true);
				MapLoaded = true;
				
			}
			/// <summary>
			/// Sends a player a Chunk
			/// </summary>
			/// <param name='c'>
			/// C. The chunk to send
			/// </param>
			public void SendChunk(Chunk c)
			{
                if (c == null) return;

                byte[] CompressedData = c.GetCompressedData();
                if (CompressedData == null) { SendPreChunk(c, 0); return; }

				SendPreChunk(c, 1);

				//Send Chunk Data
				byte[] bytes = new byte[17 + CompressedData.Length];
				util.EndianBitConverter.Big.GetBytes((int)(c.x * 16)).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes((int)0).CopyTo(bytes, 4);
				util.EndianBitConverter.Big.GetBytes((int)(c.z * 16)).CopyTo(bytes, 6);
				bytes[10] = 15;
				bytes[11] = 127;
				bytes[12] = 15;
				util.EndianBitConverter.Big.GetBytes(CompressedData.Length).CopyTo(bytes, 13);
				CompressedData.CopyTo(bytes, 17);
				SendRaw(0x33, bytes);

                c.Update(level, this);

				if (!VisibleChunks.Contains(c.point)) VisibleChunks.Add(c.point);
			}
			/// <summary>
			/// Prepare the client before sending the chunk
			/// </summary>
			/// <param name='c'>
			/// C. The chunk to send
			/// </param>
			/// <param name='load'>
			/// Load. Weather to unload or load the chunk (0 is unload otherwise it will load)
			/// </param>
			public void SendPreChunk(Chunk c, byte load)
			{
				byte[] bytes = new byte[9];
				util.EndianBitConverter.Big.GetBytes(c.x).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(c.z).CopyTo(bytes, 4);
				bytes[8] = load;
				SendRaw(0x32, bytes);
			}
			/// <summary>
			/// Updates players chunks.
			/// </summary>
			/// <param name='force'>
			/// Force. Force it to update the current chunk
			/// </param>
			/// <param name='forcesend'>
			/// Forcesend. Force it to send all the chunk, even if the player already see's it (Good for map switching)
			/// </param>
			public void UpdateChunks(bool force, bool forcesend)
			{
                e.UpdateChunks(force, forcesend);
			}
			#endregion
			#region Entity Handling
			public void SendNamedEntitySpawn(Player p)
			{
				//Console.WriteLine(username + " " + p.username);
				try
				{
                    if (p == null)
                    {
                        //if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id); // WHAT THE FUCK
                        return;
                    }
                    if (!LoggedIn)
					{
						if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id);
						return;
					}
					if (!p.LoggedIn)
					{
						if(VisibleEntities.Contains(p.id)) VisibleEntities.Remove(p.id);
						return;
					}
				
					short length = (short)p.username.Length;
					byte[] bytes = new byte[22 + (length * 2)];

					util.EndianBitConverter.Big.GetBytes(p.id).CopyTo(bytes, 0);
					util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 4);

					Encoding.BigEndianUnicode.GetBytes(p.username).CopyTo(bytes, 6);

					Point3 sendme = p.pos * 32;
					util.EndianBitConverter.Big.GetBytes((int)(sendme.x)).CopyTo(bytes, (22 + (length * 2)) - 16);
					util.EndianBitConverter.Big.GetBytes((int)(sendme.y)).CopyTo(bytes, (22 + (length * 2)) - 12);
					util.EndianBitConverter.Big.GetBytes((int)(sendme.z)).CopyTo(bytes, (22 + (length * 2)) - 8);

					bytes[(22 + (length * 2)) - 4] = (byte)(rot[0] / 1.40625);
					bytes[(22 + (length * 2)) - 3] = (byte)(rot[1] / 1.40625);

                    util.EndianBitConverter.Big.GetBytes((short)(p.current_block_holding.item == -1 ? 0 : p.current_block_holding.item)).CopyTo(bytes, (22 + (length * 2)) - 2);

					SendRaw(0x14, bytes);

					CheckOnFire();
					SendEntityEquipment(p);
				}
				catch (Exception e)
				{
					Server.Log(e.Message);
					Server.Log(e.StackTrace);
				}
			}
            public void SendObjectSpawn(Entity e1)
            {
                if (!MapLoaded)
                {
                    if (VisibleEntities.Contains(e1.id)) VisibleEntities.Remove(e1.id);
                    return;
                }

                SendRaw(0x1E, util.EndianBitConverter.Big.GetBytes(e1.id));

                byte[] bytes = new byte[21]; // 27 bytes if ghast fireball?
                util.EndianBitConverter.Big.GetBytes(e1.id).CopyTo(bytes, 0);
                bytes[4] = e1.obj.type;
                Point3 sendme = e1.obj.pos * 32;
                util.EndianBitConverter.Big.GetBytes((int)sendme.x).CopyTo(bytes, 5);
                util.EndianBitConverter.Big.GetBytes((int)sendme.y).CopyTo(bytes, 9);
                util.EndianBitConverter.Big.GetBytes((int)sendme.z).CopyTo(bytes, 13);
                util.EndianBitConverter.Big.GetBytes(0).CopyTo(bytes, 17); // TODO: Fireball thrower's entity ID.

                if (bytes.Length == 27) // Seems to be used in calculation of the fireball's position?
                {
                    util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 21);
                    util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 23);
                    util.EndianBitConverter.Big.GetBytes((short)0).CopyTo(bytes, 25);
                }
            }
			public void SendPickupSpawn(Entity e1)
			{
				if (!MapLoaded)
				{
					if(VisibleEntities.Contains(e1.id)) VisibleEntities.Remove(e1.id);
					return;
				}
				if(!e1.I.OnGround)
				{
					if (VisibleEntities.Contains(e1.id)) VisibleEntities.Remove(e1.id);
					return;
				}
				//Server.Log("Pickup Spawning " + e1.id);

				SendRaw(0x1E, util.EndianBitConverter.Big.GetBytes(e1.id));

				byte[] bytes = new byte[24];
				util.EndianBitConverter.Big.GetBytes(e1.id).CopyTo(bytes, 0);
				//Server.Log(e1.itype + "");
				util.EndianBitConverter.Big.GetBytes(e1.I.item).CopyTo(bytes, 4);
				bytes[6] = e1.I.count;
				util.EndianBitConverter.Big.GetBytes(e1.I.meta).CopyTo(bytes, 7);
				Point3 sendme = e1.I.pos * 32;
				util.EndianBitConverter.Big.GetBytes((int)sendme.x).CopyTo(bytes, 9);
				util.EndianBitConverter.Big.GetBytes((int)sendme.y).CopyTo(bytes, 13);
				util.EndianBitConverter.Big.GetBytes((int)sendme.z).CopyTo(bytes, 17);
				bytes[21] = e1.I.rot[0];
				bytes[22] = e1.I.rot[1];
				bytes[23] = e1.I.rot[2];
				SendRaw(0x15, bytes);
			}
            public void SendPickupAnimation(int eid, int pid)
            {
                byte[] bytes = new byte[8];
                util.EndianBitConverter.Big.GetBytes(eid).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(pid).CopyTo(bytes, 4);
                SendRaw(0x16, bytes);
            }
            public void SendPickupAnimation(int eid)
            {
                SendPickupAnimation(eid, this.id);
            }

			public void SendEntityVelocity(int eid, short x, short y, short z)
			{
				if (!MapLoaded) return;
                byte[] bytes = new byte[10];
                util.EndianBitConverter.Big.GetBytes(eid).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 6);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 8);
                SendRaw(0x1C, bytes);
			}
            public void SendEntityVelocity(int eid, Point3 a)
            {
                SendEntityVelocity(eid, (short)a.x, (short)a.y, (short)a.z);
            }

			public void SendEntityEquipment(Player p)
			{
				SendEntityEquipment(p.id, 4, p.inventory.items[5].item, 0);
				SendEntityEquipment(p.id, 3, p.inventory.items[6].item, 0);
				SendEntityEquipment(p.id, 2, p.inventory.items[7].item, 0);
				SendEntityEquipment(p.id, 1, p.inventory.items[8].item, 0);
				SendEntityEquipment(p.id, 0, p.current_block_holding.item, 0); //for some reason, this one seems to work when send elsewhere, but not here...
			}
			public void SendEntityEquipment(int id, short slot, short ItemId, short a)
			{
				byte[] bytes = new byte[10];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(slot).CopyTo(bytes, 4);
				util.EndianBitConverter.Big.GetBytes(ItemId).CopyTo(bytes, 6);
				util.EndianBitConverter.Big.GetBytes(a).CopyTo(bytes, 8);
				SendRaw(0x05, bytes);
			}

            public void SendEntityStatus(int eid, byte status)
            {
                byte[] bytes = new byte[5];
                util.EndianBitConverter.Big.GetBytes(eid).CopyTo(bytes, 0);
                bytes[4] = status;
                SendRaw(0x26, bytes);
            }

            public void SendAttachEntity(int eid, int eid2)
            {
                byte[] bytes = new byte[8];
                util.EndianBitConverter.Big.GetBytes(eid).CopyTo(bytes, 0);
                util.EndianBitConverter.Big.GetBytes(eid2).CopyTo(bytes, 4);
                SendRaw(0x27, bytes);
            }

            public void SendEntityMeta(int eid, params object[] data)
            {
                List<byte> bytes = new List<byte>();
                bytes.AddRange(util.EndianBitConverter.Big.GetBytes(eid));
                foreach (object obj in data)
                {
                    if (obj == null) continue;
                    if (obj.GetType() == typeof(byte[]))
                    {
                        bytes.AddRange((byte[])obj);
                    }
                    else if (obj.GetType() == typeof(byte))
                    {
                        bytes.Add(0x00);
                        bytes.Add((byte)obj);
                    }
                    else if (obj.GetType() == typeof(short))
                    {
                        bytes.Add(0x01);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((short)obj));
                    }
                    else if (obj.GetType() == typeof(int))
                    {
                        bytes.Add(0x02);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)obj));
                    }
                    else if (obj.GetType() == typeof(float))
                    {
                        bytes.Add(0x03);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((float)obj));
                    }
                    else if (obj.GetType() == typeof(string))
                    {
                        bytes.Add(0x04);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((short)((string)obj).Length));
                        bytes.AddRange(Encoding.BigEndianUnicode.GetBytes((string)obj));
                    }
                    else if (obj.GetType() == typeof(Item))
                    {
                        Item item = (Item)obj;
                        bytes.Add(0x05);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.item));
                        bytes.Add(item.count);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.meta));
                    }
                    else if (obj.GetType() == typeof(Point3))
                    {
                        Point3 point = (Point3)obj;
                        bytes.Add(0x06);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.x));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.y));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.z));
                    }
                }
                bytes.Add(0x7F);
                SendRaw(0x28, bytes.ToArray());
            }

			public void SendDespawn(int id) //Despawn ALL types of Entities (player mod item)
			{
				if (!LoggedIn)
				{
					if (!VisibleEntities.Contains(id))
						VisibleEntities.Add(id);
					return;
				}
				byte[] bytes = new byte[4];
				util.EndianBitConverter.Big.GetBytes(id).CopyTo(bytes, 0);
				SendRaw(0x1D, bytes);
			}
            public void SendRespawn()
            {
                byte[] bytes = new byte[13];

                bytes[0] = (byte)level.dimension;
				bytes[1] = Server.difficulty;
                bytes[2] = mode;
				util.BigEndianBitConverter.Big.GetBytes((short)level.height).CopyTo(bytes, 3);
				util.BigEndianBitConverter.Big.GetBytes((long)level.seed).CopyTo(bytes, 5);
                SendRaw(0x09, bytes);
            }
			#endregion
			#region Weather
			public void SendLightning(int x, int y, int z, int EntityId)
			{
				byte[] bytes = new byte[17];
				util.EndianBitConverter.Big.GetBytes(EntityId).CopyTo(bytes, 0);
				util.EndianBitConverter.Big.GetBytes(true).CopyTo(bytes, 4);
                util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 5);
                util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 9);
                util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 13);
				SendRaw(0x47, bytes);
			}
			public void SendRain(bool on)
			{
                SendState(on ? (byte)1 : (byte)2);
			}
			#endregion
		#endregion
		#region INCOMING
		public void HandleCommand(string cmd, string message)
		{
            if (OnCommand != null)
                OnCommand(cmd, message, this);
            if (PlayerCommand != null)
                PlayerCommand(cmd, message, this);
            if (cancelcommand)
            {
                cancelcommand = false;
                return;
            }
		  	Command command = Command.all.Find(cmd);
            if (command == null)
            {
                Server.ServerLogger.Log(LogLevel.Info, this.username + " tried using /" + cmd);
                Server.ServerLogger.Log(LogLevel.Info, "Unrecognised command: " + cmd);
                SendMessage(Command.HelpBot + "Command /" + cmd + " not recognized");
                return;
            }
				
	        if (Group.CheckPermission(this, command.PermissionNode))
	        {
	            List<string> args = new List<string>();
	            while (true)
	            {
	                if (message.IndexOf(' ') != -1)
	                {
	                    message = message.Substring(message.IndexOf(' ') + 1);
	                    if (message.IndexOf(' ') != -1)
	                        args.Add(message.Substring(0, message.IndexOf(' ')));
	                    else
	                    {
	                        args.Add(message);
	                        break;
	                    }
	                }
	                else if (message.IndexOf(' ') == -1)
	                    break;
	            }

                command.Use(this, args.ToArray());
	            Server.ServerLogger.Log(LogLevel.Info, this.username + " used /" + command.Name);
	        }
	        else
	        {
	            Server.ServerLogger.Log(LogLevel.Info, this.username + " tried using /" + cmd + ", but doesn't have appropiate permissions.");
	            SendMessage(Color.Purple + "HelpBot V12: You don't have access to command /" + cmd + ".");
			}
		}
		#endregion
		#region Messaging
		#region GLOBAL
        public static void GlobalMessage(string message)
        {
            foreach (Player p in players.ToArray())
            {
                p.SendMessage(message);
            }
        }
        //public static void GlobalMessage(string message)
        //{
        //    GlobalMessage(message, WrapMethod.Default);
        //}
        //public static void GlobalMessage(string message, WrapMethod method)
        //{
        //    string[] lines = WordWrap.GetWrappedText(message, method);
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        byte[] bytes = new byte[(lines[i].Length * 2) + 2];
        //        util.EndianBitConverter.Big.GetBytes((ushort)lines[i].Length).CopyTo(bytes, 0);
        //        Encoding.BigEndianUnicode.GetBytes(lines[i]).CopyTo(bytes, 2);

        //        for (int j = 0; j < players.Count; j++)
        //        {
        //            if (!players[j].disconnected)
        //            {
        //                if (!players[j].DoNotDisturb)
        //                {
        //                    players[j].SendRaw((byte)KnownPackets.ChatMessage, bytes);
        //                }
        //            }
        //        }
        //    }
            
            
        //}
        //public static void GlobalMessage(string message, WrapMethod method, params object[] args)
        //{
        //    if (method == WrapMethod.None)
        //        GlobalMessage(string.Format(message, args));
        //    else
        //        GlobalMessage(string.Format(message, args), method);
        //}
		#endregion
		#region TARGETED
        protected virtual void SendMessageInternal(string message)
        {		
            message = MessageAdditions(message);
			//Server.Log(message);
            byte[] bytes = new byte[(message.Length * 2) + 2];
            util.EndianBitConverter.Big.GetBytes((ushort)message.Length).CopyTo(bytes, 0);
            Encoding.BigEndianUnicode.GetBytes(message).CopyTo(bytes, 2);
            this.SendRaw(0x03, bytes);

        }
        public void SendMessage(string message)
        {
            SendMessage(this.MessageAdditions(message), WrapMethod.Default);
        }
        public void SendMessage(string message, WrapMethod method)
        {
            string[] lines = WordWrap.GetWrappedText(this.MessageAdditions(message), method);
            for (int i = 0; i < lines.Length; i++)
            {
                SendMessageInternal(lines[i]);
            }
        }
        public void SendMessage(string message, WrapMethod method, params object[] args)
        {
            if (method == WrapMethod.None)
                SendMessageInternal(string.Format(this.MessageAdditions(message), args));
            else
                SendMessage(string.Format(this.MessageAdditions(message), args), method);
        }
        #endregion
        public string MessageAdditions(string message)
        {
            StringBuilder sb = new StringBuilder(message);

            for (int i = 0; i <= 9; i++)
                sb.Replace("%" + i, Color.Signal + i);
            for (char c = 'a'; c <= 'f'; c++)
                sb.Replace("%" + c, Color.Signal + c);
            for (char c = 'A'; c <= 'F'; c++)
                sb.Replace("%" + c, Color.Signal + c);

            sb.Replace("$name", this.username);
            sb.Replace("$server", Server.name);
            sb.Replace("$ip", this.ip);
            //sb.Replace("$rank", this.group.Name); // NullReferenceException?

            return sb.ToString();
        }
		#endregion

		void FlyCode()
		{
			List<Point3> temp = new List<Point3>();
			Point3 point = pos.RD();
			
			Point3 p1 = new Point3(point.x, point.y - 1, point.z);
			temp.Add(p1);
			if ((level.GetBlock((int)point.x, (int)(point.y) - 1, (int)point.z) == 0) && !FlyList.Contains(p1))
			{
				SendBlockChange(p1, 20);
				FlyList.Add(p1);
			}

			//8 below for catching
			for (int x = -1; x <= 1; x++)
			{
				for (int z = -1; z <= 1; z++)
				{
					Point3 p = new Point3(point.x - x, point.y - 2, point.z - z);
					temp.Add(p);
					if ((level.GetBlock((int)point.x - x, (int)(point.y) - 2, (int)point.z - z) == 0) && !FlyList.Contains(p))
					//if (!FlyList.Contains(p))
					{
						SendBlockChange(p, 20);
						FlyList.Add(p);
					}
				}
			}

			//surrounding 25
			for (int x = -2; x <= 2; x++)
			{
				for (int z = -2; z <= 2; z++)
				{
					if (x == 0 && z == 0) continue;
					Point3 p = new Point3(point.x - x, point.y - 1, point.z - z);
					temp.Add(p);
					if ((level.GetBlock((int)point.x - x, (int)(point.y) - 1, (int)point.z - z) == 0) && !FlyList.Contains(p))
					//if (!FlyList.Contains(p))
					{
						SendBlockChange(p, 20);
						FlyList.Add(p);
					}
				}
			}

			//16 for the wall
			for (int x = -2; x <= 2; x++)
			{
				for (int z = -2; z <= 2; z++)
				{
					if (Math.Abs(x) <= 1 && Math.Abs(z) <= 1) continue;
					Point3 p = new Point3(point.x - x, point.y, point.z - z);
					temp.Add(p);
					if ((level.GetBlock((int)point.x-x, (int)(point.y), (int)point.z-z) == 0) && !FlyList.Contains(p))
					//if(!FlyList.Contains(p))
					{
						SendBlockChange(p, 20);
						FlyList.Add(p);
					}
				}
			}

			foreach (Point3 po in FlyList.ToArray())
			{
				if (!temp.Contains(po))
				{
					FlyList.Remove(po);
					SendBlockChange(po, 0);
				}
			}
			
		}
		public void Kick(string message)
		{
			if (disconnected) return;
			
			disconnected = true;
			
			if (message != null)
			{
			//	Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //    	LoggedIn ? "" : "/", LoggedIn ? username : ip, message);
			}
			else
			{
			//	Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //    	LoggedIn ? "" : "/", LoggedIn ? username : ip, Server.KickMessage);				
			}
            if (OnKicked != null)
                OnKicked(this, message);
            if (PlayerKicked != null)
                PlayerKicked(this, message);
            if (cancelkick)
            {
                cancelkick = false;
                return;
            }
            if (LoggedIn)
                GlobalMessage("§5" + username +" §fhas been kicked from the server!");
			
			try
			{
				//hopefully it is right
				byte[] bytes = new byte[(message.Length * 2) + 2];
				util.EndianBitConverter.Big.GetBytes((ushort)message.Length).CopyTo(bytes, 0);
				Encoding.BigEndianUnicode.GetBytes(message).CopyTo(bytes, 2);
				this.SendRaw(0xFF, bytes);
			}
			catch{}
			
			//TODO: Despawn
			this.Dispose();
		}
		public void Disconnect()
		{
			if (disconnected) return;
			disconnected = true;
			
            if (LoggedIn)
                GlobalMessage("§5" + username + " §fhas disconnected.");
			
			//TODO: Despawn
			this.Dispose();
		}
		public void Dispose()
		{
			if (LoggedIn)
			{
				SaveAttributes(false);
				UpdatePList(false);
				players.Remove(this);
				e.CurrentChunk.Entities.Remove(e);
				Entity.Entities.Remove(id);
                LoggedIn = false;

                // Despawn the player
                foreach (Player p in players)
                    if (p.VisibleEntities.Contains(id))
                        p.SendDespawn(id);

				// Close stuff
				if (socket != null && socket.Connected)
				{
					try { socket.Close(); }
					catch { }
					socket = null;
				}
			}
			players.Remove(this);
		}

        private void UpdatePList(bool keep)
        {
            string uname = username.Truncate(16);
            byte[] bytes = new byte[5 + (username.Length * 2)];
            util.EndianBitConverter.Big.GetBytes((short)uname.Length).CopyTo(bytes, 0);
            Encoding.BigEndianUnicode.GetBytes(uname).CopyTo(bytes, 2);
            util.EndianBitConverter.Big.GetBytes(keep).CopyTo(bytes, bytes.Length - 3);
            util.EndianBitConverter.Big.GetBytes(Ping).CopyTo(bytes, bytes.Length - 2);
            players.ForEach((p) => p.SendRaw(0xC9, bytes));
            //Server.Log(Ping.ToString());
        } 
        
        public void hurt(short Amount)
        {
            health -= Amount;
            SendHealth();
            if (health <= 0) { health = 20; }
        }
        public void hurt()
        {
            hurt(1);
        }
        public void SpawnMob(Entity e)
        {
			/*if (e == null) // What is this I don't even...
			{
				if (VisibleEntities.Contains(e.id)) VisibleEntities.Remove(e.id);
				return;
			}*/
			if (!LoggedIn)
			{
				if (VisibleEntities.Contains(e.id)) VisibleEntities.Remove(e.id);
				return;
			}
			if (!MapLoaded)
			{
				if (VisibleEntities.Contains(e.id)) VisibleEntities.Remove(e.id);
				return;
			}

			byte[] metaarray = GetMetaByteArray(e);
			byte[] bytes = new byte[20 + metaarray.Length];
			//byte[] bytes = new byte[20];

            util.EndianBitConverter.Big.GetBytes(e.id).CopyTo(bytes, 0);
			//bytes[4] = 51;
            bytes[4] = e.ai.type;
            util.EndianBitConverter.Big.GetBytes((int)(e.ai.pos.x*32)).CopyTo(bytes, 5);
			util.EndianBitConverter.Big.GetBytes((int)(e.ai.pos.y*32)).CopyTo(bytes, 9);
			util.EndianBitConverter.Big.GetBytes((int)(e.ai.pos.z*32)).CopyTo(bytes, 13);
			bytes[17] = (byte)(e.ai.yaw / 1.40625);
			bytes[18] = (byte)(e.ai.pitch / 1.40625);

			//Add in the metadata
			metaarray.CopyTo(bytes, 19);
			bytes[bytes.Length - 1] = 127;

			//LogPacket(0x18, bytes);
			SendRaw(0x18, bytes);
        }
		#region TOOLS
		
		private void SaveAttributes(bool newplayer)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			
			try
			{
				if (newplayer)
					data.Add("Name", username);
				
				data.Add("ip", ip);
				data.Add("Exp", Experience.Total.ToString());
				data.Add("NickName", NickName);
				
				if (CanBuild)
					data.Add("CanBuild", "1");
				else
					data.Add("CanBuild", "0");
				
				data.Add("Prefix", Prefix);
				data.Add("Suffix", Suffix);
				data.Add("Color", color);
				
				if (DoNotDisturb)
					data.Add("DND", "1");
				else
					data.Add("DND", "0");
				
				//TODO accounts
				
				#region Groups
				string gid = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Groups WHERE Name = '" + this.group.Name + "';");
				if(!String.IsNullOrEmpty(gid))
				{
					data.Add("GroupID", gid);
				}
				else
				{
					gid = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Groups WHERE Name = '" + Group.DefaultGroup.Name + "';");
					data.Add("GroupID", gid);
				}
				
				StringBuilder sb = new StringBuilder("");
				foreach(Group sg in this.SubGroups)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM groups WHERE Name = '" + sg.Name + "';");
					if(String.IsNullOrEmpty(id))
					{
						id = sg.Save().ToString();	
					}
					sb.Append(id + ",");
				}
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("SubGroups", sb.ToString());
				sb.Clear();
				
				foreach(string s in this.AdditionalPermissions)
				{
					string id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
					if (String.IsNullOrEmpty(id))
					{
						Server.SQLiteDB.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES ('" + s + "');");	
						id = Server.SQLiteDB.ExecuteScalar("SELECT ID FROM Permission WHERE Node = '" + s + "';").ToString();
					}
					
					sb.Append(id + ",");
				}
				
				if (sb.Length > 1)
					sb.Remove(sb.Length - 1, 1);
				
				data.Add("ExtraPerms", sb.ToString());
				sb.Clear();
				#endregion
				
				#region INVENTORY
				if (newplayer)
				{
					int invid = CreateInventory();
					data.Add("InventoryID", invid.ToString());
				}
				else
				{
					int invid = 0;
					if (!Int32.TryParse(Server.SQLiteDB.ExecuteScalar("SELECT InventoryID FROM Player WHERE Name = '" + username +"';"), out invid)) invid = CreateInventory();
					else
					{
						Dictionary<string, string> dict = new Dictionary<string, string>();
						dict.Add("ID", invid.ToString());
						for (short i = 0; i <= 44; i++)
						{
							dict.Add("slot" + i, String.Format("{0}:{1}:{2}", this.inventory.items[i].item, this.inventory.items[i].meta, this.inventory.items[i].count));
						}
						Server.SQLiteDB.Update("Inventory", dict, "ID = '" + invid.ToString() + "'");
					}
					
					data.Add("InventoryID", invid.ToString());
				}
				#endregion
				
				if(!newplayer)
					Server.SQLiteDB.Update("Player", data, "Name = '" + username + "'"); 
				else
					Server.SQLiteDB.Insert("Player", data);
				
			}
			catch
			{
				Server.Log("Could not save " + username + "'s data");	
			}
		}
		private void LoadAttributes()
		{
			System.Data.DataTable DT = new System.Data.DataTable();
			DT = Server.SQLiteDB.GetDataTable("SELECT * FROM Player WHERE Name = '" + username + "';");
			
			if(DT.Rows.Count > 0)
			{
				NickName = DT.Rows[0]["NickName"].ToString();
				
				//short sout = 0;
				//if(Int16.TryParse(DT.Rows[0]["Exp"].ToString(), out sout));
				//	this.Experience.Add(this, sout);
				
				if (DT.Rows[0]["CanBuild"].ToString() == "1")
					CanBuild = true;
				else
					CanBuild = false;
				
				Prefix = DT.Rows[0]["Prefix"].ToString();
				Suffix = DT.Rows[0]["Suffix"].ToString();
				
				string tcolor = DT.Rows[0]["Color"].ToString();
				
				if (!String.IsNullOrEmpty(tcolor))
				    {
				if (tcolor.Length == 2 && tcolor[0] == '%' || tcolor[0] == '§' || tcolor[0] == '&')
					if (Color.IsColorValid((char)tcolor[1]))
					    color = "§" + tcolor[1];
				else if (tcolor.Length == 1 && Color.IsColorValid((char)tcolor[0]))
				 	color = "§" + tcolor[1];
				}
				
				if (DT.Rows[0]["DND"].ToString() == "1")
					DoNotDisturb = true;
				else
					DoNotDisturb = false;
				
				if (DT.Rows[0]["GodMode"].ToString() == "1")
					GodMode = true;
				else
					GodMode = false;
				
				//TODO Accounts
				#region ECONOMY
				//TODO
				#endregion
				
				#region GROUPS
				string groupid = DT.Rows[0]["GroupID"].ToString();
				
				Group gr = Group.FindGroup(Server.SQLiteDB.ExecuteScalar("SELECT Name FROM Groups WHERE ID = '" + groupid + "';"));
				
				if (gr != null)
					this.group = gr;
				else
					this.group = Group.DefaultGroup;
				
				string temp = DT.Rows[0]["SubGroups"].ToString().Replace(" ", "");
				string[] subgroups = temp.Split(',');
				if (subgroups.Length >= 1)
				{
					foreach(string s in subgroups)
					{
						if (!String.IsNullOrEmpty(s))
						{
							Group g = Group.FindGroup(Server.SQLiteDB.ExecuteScalar("SELCT Name FROM Groups WHERE ID = '" + s + "';"));
							
							if (g != null)
								this.SubGroups.Add(g);
						}
					}
				}
				
				string[] perms = DT.Rows[0]["ExtraPerms"].ToString().Replace(" ", "").Split(',');
				foreach(string s in perms)
				{
                    if (String.IsNullOrEmpty(s)) continue;
					string perm;
					if (s[0] == '-')
						perm = "-" + Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s.Substring(1) + "';");
					else
						perm = Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s + "';");
					
					if (perm.Substring(0,1) == "-" && !this.AdditionalPermissions.Contains(perm.Substring(1)))
						this.AdditionalPermissions.Add(perm);
					else if (perm.Substring(0,1) != "-" && !this.AdditionalPermissions.Contains("-" + perm))
						this.AdditionalPermissions.Add(perm);
				}
				#endregion
				
				#region INVENTORY
				string invid = DT.Rows[0]["InventoryID"].ToString();
				
				if(!String.IsNullOrEmpty(invid))
				{
					System.Data.DataTable invDT = new System.Data.DataTable();
					invDT = Server.SQLiteDB.GetDataTable("Select * FROM Inventory WHERE ID = '" + invid + "';");
					if (invDT.Rows.Count == 0) CreateInventory();
					
					for (int i = 0; i <= 44; i++)
					{
						string data = invDT.Rows[0]["slot" + i].ToString();
						string[] item = data.Split(':');
						short id = -1;
						short meta = 0;
						byte count = 1;
						
						if (!Int16.TryParse(item[0], out id))
						{
							continue;
						}
						if (item.Length >= 2)
						{
							if(!Int16.TryParse(item[1], out meta))
							{
								meta = 0;
							}
						}
						if (item.Length >= 3)
						{
							if(!Byte.TryParse(item[2], out count))
							{
								count = 1;	
							}
						}
						if (count > 64) count = 64;
						
						if (id > 0 && count > 0)
						{
							this.inventory.Add(id, count, meta, i); 	
						}
					}
				}
				else CreateInventory();
				
				#endregion
				
				
			}
			else
			{
				Server.Log(String.Format("Creating new Database entry for {0}.", this.username));
				
				this.group = Group.DefaultGroup;
				//TODO Set Default to default group, setup accounts etc
				SaveAttributes(true);
			}
		}
		
		/// <summary>
        /// Finds a player by string or partial string
        /// </summary>
        /// <param name="name">username to search for</param>
        /// <returns>Player</returns>
        public static Player FindPlayer(string name)
        {
            List<Player> tempList = new List<Player>(players);
            Player tempPlayer = null; 
			bool returnNull = false;

            foreach (Player p in tempList)
            {
                if (p.username.ToLower() == name.ToLower()) return p;
                if (p.username.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempPlayer == null) tempPlayer = p;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempPlayer != null) return tempPlayer;
            return null;
        }

        public static bool IPInPrivateRange(string ip)
        {
            //Official loopback is 127.0.0.1/8
            if (ip.StartsWith("127.0.0.") || ip.StartsWith("192.168.") || ip.StartsWith("10."))
                return true;

            if (ip.StartsWith("172."))
            {
                string[] split = ip.Split('.');
                if (split.Length >= 2)
                {
                    try
                    {
                        int secondPart = Convert.ToInt32(split[1]);
                        return (secondPart >= 16 && secondPart <= 31);
                    }
                    catch { return false; }
                }
            }

            return false;
        }
		
		public string GetPrefix()
		{
			if(this.Prefix == "" && this.group.Prefix != "")
				return this.group.Prefix;
			else if (this.Prefix != "")
				return this.Prefix;
			else
				return "";
		}
		
		public string GetSuffix()
		{
			if(this.Suffix == "" && this.group.Suffix != "")
				return this.group.Suffix;
			else if(this.Suffix != "")
				return this.Suffix;
			else
				return "";
		}
		
		public string GetColor()
		{
			if(this.color == "" || this.color == null)
				return this.group.GroupColor;
			else
				return this.color;
		}
		
		public bool GetCanBuild()
		{
			if(this.CanBuild || this.group.CanBuild)
				return true;
			else
				return false;
		}
		
		/// <summary>
		///used for getting the name to use in chat, whether a nick or not 
		/// </summary>
		public string GetName()
		{
			if(String.IsNullOrEmpty(this.NickName))
				return this.username;
			else
				return "~" + this.NickName;
		}
		#endregion

		#region META DATA HANDLER
		byte[] GetMetaByteArray(Entity e)
		{
			switch (e.ai.type)
			{
				case 50: //Creeper
					byte[] bytes = new byte[0];
					bytes = AddVar(e, bytes, (byte)0, 16);
					bytes = AddVar(e, bytes, (byte)0, 17);
					return bytes;
				case 51: //Skeleton
					break;
				case 52: //Spider
					break;
				case 53: //Giant Zombie
					break;
				case 54: //Zombie
					break;
				case 55: //Slime
					break;
				case 56: //Ghast
					break;
				case 57: //Zombie Pigman
					break;
				case 90: //Pig
					break;
				case 91: //Sheep
					break;
				case 92: //Cow
					break;
				case 93: //Hen
					break;
				case 94: //Squid
					break;
				case 95: //Wolf
					break;
			}
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, byte a, byte index)
		{
			int i = (0 << 5 | index & 0x1f) & 0xff;
			
			byte[] NewArray = new byte[Array.Length + 2];
			Array.CopyTo(NewArray, 0);
			NewArray[NewArray.Length-2] = (byte)i;
			NewArray[NewArray.Length-1] = a;

			return NewArray;
		}
		byte[] AddVar(Entity e, byte[] Array, short a, byte index)
		{
			int i = (1 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, int a, byte index)
		{
			int i = (2 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, float a, byte index)
		{
			int i = (3 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, string a, byte index)
		{
			int i = (4 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, Item a, byte index) //Item Stack
		{
			int i = (5 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		byte[] AddVar(Entity e, byte[] Array, Point3 a, byte index) //CHUNK Coordinates
		{
			int i = (6 << 5 | index & 0x1f) & 0xff;
			return null;
		}
		#endregion

		void LogPacket(byte id, byte[] packet)
		{
			string s = "";

			if (packet.Length >= 1)
			{
				foreach (byte b in packet)
				{
					s += b + ", ";
				}
                Server.Log("Packet " + id + " { " + s + "}");
			}
			else
			{
                Server.Log("Packet " + id + " had no DATA!");
			}
		}
		
		//untested
		private int CreateInventory()
		{
			int id = Int32.Parse(Server.SQLiteDB.ExecuteScalar("SELECT MAX(ID) FROM Inventory;"));
			id++;
			Dictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add("ID", id.ToString());
			for (short i = 0; i <= 44; i++)
			{
				dict.Add("slot" + i, String.Format("{0}:{1}:{2}", inventory.items[i].item, inventory.items[i].meta, inventory.items[0].count));	
			}
			Server.SQLiteDB.Insert("Inventory", dict);
			
			return id;		
		}
	}
}
