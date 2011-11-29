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
using System.Text;
using System.Collections.Generic;
using SMP.util;

namespace SMP
{

    /// <summary>
    /// Handles packets.
    /// </summary>
	public partial class Player : System.IDisposable
    {
        #region blockchangehandler
        public delegate void BlockChangeHandler(Player p, int x, int y, int z, short type);
        public event BlockChangeHandler OnBlockChange;
        public void ClearBlockChange() { OnBlockChange = null; }
        #endregion
		#region Login
		private void HandleLogin(byte[] message)
		{
			int version = util.EndianBitConverter.Big.ToInt32(message, 0);
			short length = util.EndianBitConverter.Big.ToInt16(message, 4);
			if (length > 32) { Kick("Username too long"); return; }
			username = Encoding.BigEndianUnicode.GetString(message, 6, (length * 2));
			Logger.Log(ip + " Logged in as " + username);
			Player.GlobalMessage(Color.Announce + username + " has joined the game!");
			
			/*if (version > Server.protocolversion)  //left commented during development
            {
                Kick("Outdated server!");
                return;
            }
            if (version < Server.protocolversion)
            {
                Kick("Outdated client!");
                return;
            }*/
			
			if (!IPInPrivateRange(ip))
			{
				if (Player.players.Count >= Server.MaxPlayers)
				{
					if (Server.useviplist && Server.VIPList.Contains(username.ToLower()))
					{
						for(int i = players.Count - 1; i >= 0; i--) // kick the last joined non-vip
						{
							if (!Server.VIPList.Contains(players[i].username.ToLower()))
							{
								players[i].Kick("You have been kicked for a VIP.");
								break;
							}
						}
					}
					else if (Server.useviplist && !Server.VIPList.Contains(username.ToLower()))
					{
						Kick(Server.VIPListMessage);
					}
					else if (!Server.useviplist)
						Kick("Server is full!");	
				}
				
				if (Server.BanList.Contains(username.ToLower())) 
					Kick(Server.BanMessage);
				    
	            if (Server.usewhitelist && !Server.WhiteList.Contains(username.ToLower()))
					Kick(Server.WhiteListMessage);
			}
			
			//TODO: load Player attributes like group, and other settings
			LoadAttributes();
			
			LoggedIn = true;
			SendLoginPass();
			
			UpdateShi(this);

            int tries = 0;
            while (tries < 100 && Chunk.GetChunk((int)pos.x >> 4, (int)pos.z >> 4, level) == null)
            {
                tries++;
                System.Threading.Thread.Sleep(50);
            }

            if (Chunk.GetChunk((int)pos.x >> 4, (int)pos.z >> 4, level) == null)
                Kick("Chunk missing: " + ((int)pos.x >> 4) + "," + ((int)pos.z >> 4));
			
			if (PlayerAuth != null)
				PlayerAuth(this);
            if (OnAuth != null)
                OnAuth(this);
		}

        private void UpdateShi(Player p)
        {
            p.SendTime();
            if (Chunk.GetChunk((int)p.pos.x >> 4, (int)p.pos.z >> 4, p.level) == null)
                p.level.LoadChunk((int)p.pos.x >> 4, (int)p.pos.z >> 4);
            if (p.level.IsRaining)
                p.level.Rain(true);
        }
		private void HandleHandshake(byte[] message)
		{
			//Logger.Log("handshake-2");
			//short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			//Logger.Log(length + "");
			//Logger.Log(Encoding.BigEndianUnicode.GetString(message, 2, length * 2));

			SendHandshake();
            
		}
		#endregion
		#region Chat
		private void HandleChatMessagePacket(byte[] message)
        {
			short length = util.EndianBitConverter.Big.ToInt16(message, 0);
			string m = Encoding.BigEndianUnicode.GetString(message, 2, length * 2);

            if (m.Length > 119)
            {
                Kick("Too many characters in message!");
                return;
            }
            foreach (char ch in m)
            {
                if (ch < 32 || ch >= 127)
                {
                    Kick("Illegal character in chat message!");
                    return;
                }
            }
            
            // Test for commands
            if (m[0] == '/') //in future maybe use config defined character
            {
                m = m.Remove(0, 1);

                int pos = m.IndexOf(' ');
                if (pos == -1)
                {
                    HandleCommand(m.ToLower(), "");
                    return;
                }

                string cmd = m.Substring(0, pos).ToLower();

                HandleCommand(cmd, m);
                return;
            }
			else if (m[0] == '@')
			{
				if(m[1] != ' ')
				{
					HandleCommand("msg", m.Substring(1));
				}
				else if(m[1] == ' ')
				{
					HandleCommand("msg", m.Substring(2));
				}
			}

            if (OnChat != null)
                OnChat(m, this);
            if (PlayerChat != null)
                PlayerChat(m, this);
            if (cancelchat)
            {
                cancelchat = false;
                return;
            }
            // TODO: Rank coloring
            //GlobalMessage(this.PlayerColor + "{1}§f: {2}", WrapMethod.Chat, this.Prefix, Username, message);
			if (!DoNotDisturb)
			{
				GlobalMessage(/*Color.DarkBlue + "<" + level.name + "> " +*/ Color.Gray + "[" + group.GroupColor + group.Name + Color.Gray + "] " + this.GetColor() + GetName() + Color.White + ": " + m);
            	Logger.Log(LogLevel.Info, username + ": " + m);
			}
        }
		#endregion
		#region Movement stuffs
		private void HandlePlayerPacket(byte[] message)
		{
			try
			{
				byte onGround = message[0];

				if (onGround == onground)
					return;

				// TODO: Handle fall damage.

				onground = onGround;
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace);
			}
		}
		private void HandlePlayerPositionPacket(byte[] message)
		{
			try
			{
				double x = util.EndianBitConverter.Big.ToDouble(message, 0);
				double y = util.EndianBitConverter.Big.ToDouble(message, 8);
				double stance = util.EndianBitConverter.Big.ToDouble(message, 16);
				double z = util.EndianBitConverter.Big.ToDouble(message, 24);
				byte onGround = message[32];

				// Return if position hasn't changed.
				if (new Point3(x, y, z) == pos && stance == Stance && onGround == onground)
					return;

				// Check stance
				if (stance - y < 0.1 || stance - y > 1.65)
				{
					Kick("Illegal Stance");
					return;
				}

				// Check position
				//if (Math.Abs(x - this.X) + Math.Abs(y - this.Y) + Math.Abs(z - this.Z) > 100)
				//{
				//    Kick("You moved to quickly :( (Hacking?)");
				//    return;
				//}
				/*else */
				if (Math.Abs(x) > 3.2E7D || Math.Abs(z) > 3.2E7D)
				{
					Kick("Illegal position");
					return;
				}

				//oldpos = pos;
				pos = new Point3(x, y, z);
				onground = onGround;

				e.UpdateChunks(false, false);
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace);
			}
		}
		private void HandlePlayerLookPacket(byte[] message)
		{
			try
			{
				float yaw = util.EndianBitConverter.Big.ToSingle(message, 0);
				float pitch = util.EndianBitConverter.Big.ToSingle(message, 4);
				byte onGround = message[8];

				// Return if position hasn't changed.
				if (yaw == rot[0] && pitch == rot[1] && onGround == onground)
					return;

				rot[0] = yaw;
				rot[1] = pitch;
				onground = onGround;
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace);
			}
		}
		private void HandlePlayerPositionAndLookPacket(byte[] message)
		{
			try
			{
				double x = util.EndianBitConverter.Big.ToDouble(message, 0);
				double y = util.EndianBitConverter.Big.ToDouble(message, 8);
				double stance = util.EndianBitConverter.Big.ToDouble(message, 16);
				double z = util.EndianBitConverter.Big.ToDouble(message, 24);
				float yaw = util.EndianBitConverter.Big.ToSingle(message, 32);
				float pitch = util.EndianBitConverter.Big.ToSingle(message, 36);
				byte onGround = message[40];

				// Return if position hasn't changed.
				if (new Point3(x, y, z) == pos && stance == Stance &&
					yaw == rot[0] && pitch == rot[1] && onGround == onground)
					return;

				// Check stance
				if (stance - y < 0.1 || stance - y > 1.65)
				{
					Kick("Illegal Stance");
					return;
				}

				// Check position
				//if (Math.Abs(x - this.X) + Math.Abs(y - this.Y) + Math.Abs(z - this.Z) > 100)
				//{
				//    Kick("You moved to quickly :( (Hacking?)");
				//    return;
				//}
				/*else */
				if (Math.Abs(x) > 3.2E7D || Math.Abs(z) > 3.2E7D)
				{
					Kick("Illegal position");
					return;
				}

				//oldpos = pos;
				pos = new Point3(x, y, z);
				rot[0] = yaw;
				rot[1] = pitch;
				onground = onGround;

				e.UpdateChunks(false, false);
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace);
			}
		}
		#endregion
		#region BlockChanges
		private void HandleDigging(byte[] message)
		{
			//Send Animation, this shouldn't only be sent when digging
            //Now handled in HandleAnimation(), this code isn't needed
			/*foreach (int i in VisibleEntities.ToArray())
			{
				Entity e = Entity.Entities[i];
				if (!e.isPlayer) continue;
				Player p = e.p;
				if (p.level == level && p != this)
					p.SendAnimation(id, 1);
			}*/

			if (message[0] == 0)
			{
				int x = util.EndianBitConverter.Big.ToInt32(message, 1);
				byte y = message[5];
				int z = util.EndianBitConverter.Big.ToInt32(message, 6);
                byte direction = message[10];

                Point3 face = BlockData.GetFaceBlock(x, y, z, direction);

				byte rc = level.GetBlock(x,y,z); //block hit

                if (OnBlockChange != null)
                {
                    OnBlockChange(this, x, y, z, current_block_holding.id);
                    return;
                }

                if (OnBlockLeftClick != null)
                    OnBlockLeftClick(this, x, y, z, direction);
                if (PlayerBlockLeftClick != null)
                    PlayerBlockLeftClick(this, x, y, z, direction);
                if (cancelblockleft)
                {
                    cancelblockleft = false;
                    return;
                }

				if(BlockChange.LeftClicked.ContainsKey(rc))
				{
					BlockChange.LeftClicked[rc].DynamicInvoke(this, new BCS(new Point3(x, y, z), 0, 0, 0, 0));
				}

                if (level.GetBlock((int)face.x, (int)face.y, (int)face.z) == 51)
                {
                    level.BlockChange((int)face.x, (int)face.y, (int)face.z, 0, 0);
                    Player.GlobalSoundEffect(face, 1004, level);
                }

                if (Server.mode == 1)
                {
                    if (BlockChange.Destroyed.ContainsKey(rc))
                    {
                        if (!(bool)BlockChange.Destroyed[rc].DynamicInvoke(this, new BCS(new Point3(x, y, z), 0, 0, 0, 0)))
                        {
                            //SendBlockChange(x, y, z, level.GetBlock(x, y, z), level.GetMeta(x, y, z));
                            Logger.Log("Delegate for " + rc + " Destroyed returned false");
                            return;
                        }
                    }

                    level.BlockChange(x, y, z, 0, 0);
                    goto doSound;
                }
                else if (BlockData.CanInstantBreak(rc))
                {
                    if (BlockChange.Destroyed.ContainsKey(rc))
                    {
                        if (!(bool)BlockChange.Destroyed[rc].DynamicInvoke(this, new BCS(new Point3(x, y, z), 0, 0, 0, 0)))
                        {
                            //SendBlockChange(x, y, z, level.GetBlock(x, y, z), level.GetMeta(x, y, z));
                            Logger.Log("Delegate for " + rc + " Destroyed returned false");
                            return;
                        }
                    }

                    short dropId = BlockDropSwitch(rc);
                    if (dropId != 0)
                        level.DropItem(x, y, z, dropId, level.GetMeta(x, y, z));

                    level.BlockChange(x, y, z, 0, 0);
                    goto doSound;
                }
                return;

                doSound:
                Player.GlobalBreakEffect(x, y, z, rc, level, this);
		    }
			if (message[0] == 2)
			{
				//Player is done digging
				int x = util.EndianBitConverter.Big.ToInt32(message, 1);
				byte y = message[5];
				int z = util.EndianBitConverter.Big.ToInt32(message, 6);
                byte direction = message[10];

                Point3 face = BlockData.GetFaceBlock(x, y, z, direction);

				short id = level.GetBlock(x, y, z);
                short storeId = id;
				byte count = 1;

                if (OnBlockBreak != null)
                    OnBlockBreak(this, x, y, z, (byte)id, level.GetMeta(x, y, z));
                if (PlayerBlockBreak != null)
                    PlayerBlockBreak(this, x, y, z, (byte)id, level.GetMeta(x, y, z));
                if (cancelbreak)
                {
                    cancelbreak = false;
                    return;
                }

				if (BlockChange.Destroyed.ContainsKey(id))
				{
					if (!(bool)BlockChange.Destroyed[id].DynamicInvoke(this, new BCS(new Point3(x, y, z), 0, 0, 0, 0)))
					{
                        //SendBlockChange(x, y, z, level.GetBlock(x, y, z), level.GetMeta(x, y, z));
                        Logger.Log("Delegate for " + id + " Destroyed returned false");
						return;
					}
				}

				id = BlockDropSwitch(id);

                if (id != 0)
                    level.DropItem(x, y, z, id, level.GetMeta(x, y, z), count);
				
				level.BlockChange(x, y, z, 0, 0);

                Player.GlobalBreakEffect(x, y, z, storeId, level, this);
			}
			if (message[0] == 4)
			{
				//TODO drop one of the item the player is holding!
                //inventory.Remove(inventory.current_index, 1);
			}
            if (message[0] == 5)
            {
                // TODO: Shoot arrow!
            }
		}
		private void HandleBlockPlacementPacket(byte[] message)
		{
			int blockX = util.EndianBitConverter.Big.ToInt32(message, 0);
			byte blockY = message[4];
			int blockZ = util.EndianBitConverter.Big.ToInt32(message, 5);
			byte direction = message[9];

            if (blockX == -1 && blockY == unchecked((byte)-1) && blockZ == -1 && direction == unchecked((byte)-1))
			{
				//this is supposed to just tell the server to update food and stuffs
				return;
			}

			//short blockID = util.EndianBitConverter.Big.ToInt16(message, 10);
            short blockID = current_block_holding.id;

            byte amount = current_block_holding.count;
            short damage = current_block_holding.meta;
            /*if (message.Length == 15)  //incase it is the secondary packet size (THIS LIES, DO NOT USE!!!)
            {
                amount = message[11];
                damage = util.EndianBitConverter.Big.ToInt16(message, 12);
            }*/

            //Console.WriteLine(blockID + " " + amount + " " + damage);

            if (OnBlockRightClick != null)
                OnBlockRightClick(this, blockX, blockY, blockZ, direction);
            if (PlayerBlockRightClick != null)
                PlayerBlockRightClick(this, blockX, blockY, blockZ, direction);
            if (cancelblockright)
            {
                cancelblockright = false;
                return;
            }

			byte rc = level.GetBlock(blockX, blockY, blockZ);
			if (BlockChange.RightClickedOn.ContainsKey(rc))
			{
				if (!(bool)BlockChange.RightClickedOn[rc].DynamicInvoke(this, new BCS(new Point3(blockX, blockY, blockZ), blockID, direction, amount, damage)))
				{
                    Logger.Log("Delegate for " + rc + " RightClickedON returned false");
					return;
				}
			}


            if (level.GetBlock(blockX, blockY, blockZ) != 78 && level.GetBlock(blockX, blockY, blockZ) != 106) // You can place stuff IN snow, not on it.
                switch (direction)
                {
                    case 0: blockY--; break;
                    case 1: blockY++; break;
                    case 2: blockZ--; break;
                    case 3: blockZ++; break;
                    case 4: blockX--; break;
                    case 5: blockX++; break;
                }

            bool canPlaceOnEntity = (blockID < 0 || (blockID < 256 && BlockData.CanPlaceOnEntity((byte)blockID)) || (blockID >= 256 && BlockData.CanPlaceOnEntity(BlockData.PlaceableItemSwitch(blockID))));
            foreach (Entity e1 in new List<Entity>(Entity.Entities.Values))
			{
				Point3 block = new Point3(blockX, blockY, blockZ);
				Point3 pp = new Point3((int[])e1.pos);

                if (e1.isItem) continue;
                if (block == pp && !canPlaceOnEntity)
				{
					//Logger.Log("Entity found!");
                    SendBlockChange(blockX, blockY, blockZ, level.GetBlock(blockX, blockY, blockZ), level.GetMeta(blockX, blockY, blockZ));
                    SendItem(inventory.current_index, inventory.current_item.id, inventory.current_item.count, inventory.current_item.meta);
                    return;
                    
                    /*if (e1.isItem)
					{
						//move item
						continue;
					}
					if (e1.isObject)
					{
						//do stuff, like get in a minecart
						continue;
					}
					if (e1.isAI)
					{
						//do stuff, like shear sheep
						continue;
					}
                    if (e1.isPlayer)
					{
						//dont do anything here? is there a case where you right click a player? a snowball maybe...
						//Check the players holding item, if they need to do something with it, do it.
						//anyway, if this is a player, then we dont place a block :D so return.
						return;
					}*/
				}

                pp.y++;
                if (block == pp && !canPlaceOnEntity)
                {
                    //Logger.Log("Entity found!");
                    SendBlockChange(blockX, blockY, blockZ, level.GetBlock(blockX, blockY, blockZ), level.GetMeta(blockX, blockY, blockZ));
                    SendItem(inventory.current_index, inventory.current_item.id, inventory.current_item.count, inventory.current_item.meta);
                    return;

                    /*if (e1.isPlayer)
                    {
                        //dont do anything here? is there a case where you right click a player? a snowball maybe...
                        //Check the players holding item, if they need to do something with it, do it.
                        //anyway, if this is a player, then we dont place a block :D so return.
                        return;
                    }*/
                }
			}

			if (blockID == -1)
			{
				//Players hand is empty
				//Player right clicked with empty hand!
                if (OnBlockChange != null)
                {
                    if (level.GetBlock(blockX, blockY, blockZ) != 78 && level.GetBlock(blockX, blockY, blockZ) != 106)
                        switch (direction)
                        {
                            case 0: blockY++; break;
                            case 1: blockY--; break;
                            case 2: blockY++; break;
                            case 3: blockZ--; break;
                            case 4: blockX++; break;
                            case 5: blockX--; break;
                        }

                    OnBlockChange(this, blockX, blockY, blockZ, blockID);
                    return;
                }

				return;
			}

            if (!BlockData.CanPlaceIn(level.GetBlock(blockX, blockY, blockZ)))
            {
                SendBlockChange(blockX, blockY, blockZ, level.GetBlock(blockX, blockY, blockZ), level.GetMeta(blockX, blockY, blockZ));
                SendItem(inventory.current_index, inventory.current_item.id, inventory.current_item.count, inventory.current_item.meta);
                return;
            }

			if (blockID >= 1 && blockID <= 255)
			{
                if (OnBlockChange != null)
                {
                    OnBlockChange(this, blockX, blockY, blockZ, blockID);
                    SendBlockChange(blockX, blockY, blockZ, level.GetBlock(blockX, blockY, blockZ), level.GetMeta(blockX, blockY, blockZ));
                    return;
                }

                if (OnBlockPlace != null)
                    OnBlockPlace(this, blockX, blockY, blockZ, (byte)blockID, (byte)damage, direction);
                if (PlayerBlockPlace != null)
                    PlayerBlockPlace(this, blockX, blockY, blockZ, (byte)blockID, (byte)damage, direction);
                if (cancelplace)
                {
                    cancelplace = false;
                    return;
                }

				if (BlockChange.Placed.ContainsKey(blockID))
				{
					if (!(bool)BlockChange.Placed[blockID].DynamicInvoke(this, new BCS(new Point3(blockX, blockY, blockZ), blockID, direction, amount, damage)))
					{
                        //SendBlockChange(blockX, blockY, blockZ, level.GetBlock(blockX, blockY, blockZ), level.GetMeta(blockX, blockY, blockZ));
                        //SendItem((short)inventory.current_index, inventory.current_item.item, inventory.current_item.count, inventory.current_item.meta);
						return;
					}
				}
				level.BlockChange(blockX, blockY, blockZ, (byte)blockID, (byte)damage);
                if (Server.mode == 0) { inventory.Remove(inventory.current_index, 1); experience.Add(1); }
				return;
			}
			else
			{
                if (OnBlockChange != null)
                {
                    if (level.GetBlock(blockX, blockY, blockZ) != 78 && level.GetBlock(blockX, blockY, blockZ) != 106)
                        switch (direction)
                        {
                            case 0: blockY++; break;
                            case 1: blockY--; break;
                            case 2: blockY++; break;
                            case 3: blockZ--; break;
                            case 4: blockX++; break;
                            case 5: blockX--; break;
                        }

                    OnBlockChange(this, blockX, blockY, blockZ, blockID);
                    return;
                }

                if (ItemUse != null)
                    ItemUse(this, blockX, blockY, blockZ, current_block_holding, direction);
                if (PlayerItemUse != null)
                    PlayerItemUse(this, blockX, blockY, blockZ, current_block_holding, direction);
                if (cancelitemuse)
                {
                    cancelitemuse = false;
                    return;
                }

				if(BlockChange.ItemRightClick.ContainsKey(blockID))
				{
                    if ((bool)BlockChange.ItemRightClick[blockID].DynamicInvoke(this, new BCS(new Point3(blockX, blockY, blockZ), blockID, direction, amount, damage)))
                    {
                        if (Server.mode == 0) { inventory.Remove(inventory.current_index, 1); experience.Add(1); }
                    }
				}
				return;
			}
		}
		#endregion

		private void HandleHoldingChange(byte[] message)
		{
			try { current_slot_holding = (short)(util.EndianBitConverter.Big.ToInt16(message, 0) + 36); }
			catch { }
		}

        private void HandleEntityUse(byte[] message)
        {
            //int pid = util.EndianBitConverter.Big.ToInt32(message, 0); // Ignored by server.
            int eid = util.EndianBitConverter.Big.ToInt32(message, 4);
            bool leftClick = util.EndianBitConverter.Big.ToBoolean(message, 8);

            if (!Entity.Entities.ContainsKey(eid)) return;
            Entity ent = Entity.Entities[eid];
            if (pos.distance(ent.pos) > 4) return; // No distance hax!

            if (leftClick)
            {
                short damage = current_block_holding.AttackDamage;
                if (EntityAttack != null)
                    EntityAttack(this, ent, damage);
                if (PlayerEntityAttack != null)
                    PlayerEntityAttack(this, ent, damage);
                if (cancelentityleft)
                {
                    cancelentityleft = false;
                    return;
                }

                ent.hurt(damage);
            }
            else
            {
                if (OnEntityRightClick != null)
                    OnEntityRightClick(this, ent);
                if (PlayerEntityRightClick != null)
                    PlayerEntityRightClick(this, ent);
                if (cancelentityright)
                {
                    cancelentityright = false;
                    return;
                }

                // TODO: Put player in vehicle or something.
            }
        }

        private void HandleUpdateSign(byte[] message)
        {
            int x = util.EndianBitConverter.Big.ToInt32(message, 0);
            short y = util.EndianBitConverter.Big.ToInt16(message, 4);
            int z = util.EndianBitConverter.Big.ToInt32(message, 6);

            // String lengths
            short a = MCUtil.Protocol.GetStringLength(message, 10);
            short b = MCUtil.Protocol.GetStringLength(message, 10 + a);
            short c = MCUtil.Protocol.GetStringLength(message, 10 + a + b);
            short d = MCUtil.Protocol.GetStringLength(message, 10 + a + b + c);

            string[] text = new string[4];
            text[0] = MCUtil.Protocol.GetString(message, 10);
            text[1] = MCUtil.Protocol.GetString(message, 10 + a);
            text[2] = MCUtil.Protocol.GetString(message, 10 + a + b);
            text[3] = MCUtil.Protocol.GetString(message, 10 + a + b + c);

            level.SetSign(x, y, z, text);
        }

        private void HandleCreativeInventoryAction(byte[] message)
        {
            if (util.EndianBitConverter.Big.ToInt16(message, 0) == -1 || util.EndianBitConverter.Big.ToInt16(message, 2) == -1) return;
            inventory.Add(util.EndianBitConverter.Big.ToInt16(message, 2), message[4], util.EndianBitConverter.Big.ToInt16(message, 5), util.EndianBitConverter.Big.ToInt16(message, 0));
        }

        private void HandleAnimation(byte[] message)
		{
            int pid = util.EndianBitConverter.Big.ToInt32(message, 0);
            byte type = message[4];

            //Console.WriteLine(pid + " " + type);
            if (!Entity.Entities.ContainsKey(pid) && Entity.Entities[pid].p == null) return;
            foreach (Player p1 in Player.players)
                if (p1 != this && p1.VisibleEntities.Contains(pid))
                    p1.SendAnimation(pid, type);
		}

        private void HandleWindowClose(byte[] message)
		{
            if (WindowClose != null)
                WindowClose(this);
            if (PlayerWindowClose != null)
                PlayerWindowClose(this);
            if (cancelclose)
            {
                cancelclose = false;
                return;
            }
            HasWindowOpen = false;
			//TODO save the furnaces/dispensers, add unused stuff back to inventory etc
		}

        private void HandleWindowClick(byte[] message)
		{
			if (HasWindowOpen)
			{
				//window.HandleClick(this, message);
			}
			else
			{
				//inventory.HandleClick(message);
			}

			short slot = util.EndianBitConverter.Big.ToInt16(message, 1);
			if (!HasWindowOpen)
			{
				if (slot == 5)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 4, inventory.items[5].id, 0);
					}
				}
				else if (slot == 6)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 3, inventory.items[6].id, 0);
					}
				}
				else if (slot == 7)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 2, inventory.items[7].id, 0);
					}
				}
				else if (slot == 8)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 1, inventory.items[8].id, 0);
					}
				}
				else if (slot == inventory.current_index)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 0, inventory.current_item.id, 0);
					}
				}
			}
			else
			{
				byte currentc = (byte)((current_slot_holding - 9) + window.items.Length); //TODO TEST
				if (slot == currentc)
				{
					foreach (int i in VisibleEntities.ToArray())
					{
						Entity e = Entity.Entities[i];
						if (!e.isPlayer) continue;
						e.p.SendEntityEquipment(id, 0, inventory.current_item.id, 0);
					}
				}
			}

			if (HasWindowOpen)
			{
				if (slot < 5)
				{
                   // GlobalMessage(GetName() + " " + window.items[1].item);
				}
			}
			else
			{
				if (slot < 10)
				{
                   // GlobalMessage(GetName() + inventory.items[1].item);
				}
			}
		}

		private void HandleDC(byte[] message)
		{
			Logger.Log(username + " Disconnected.");
			GlobalMessage(username + " Left.");
            if (OnDisconnect != null)
                OnDisconnect(this);
            if (PlayerDisconnect != null)
                PlayerDisconnect(this);
			socket.Close();
			Disconnect();
			foreach (int i in VisibleEntities.ToArray())
			{
				try
				{
					Entity e = Entity.Entities[i];
					e.p.SendDespawn(id);
				}
				catch { /* Ignore Me */ }
			}
		}
        private void HandleEntityAction(byte[] message)
		{
			if (message[4] == 1)
			{
                if (OnCrouch != null)
                    OnCrouch(this);
                if (PlayerCrouch != null)
                    PlayerCrouch(this);
				crouch(true);
			}
			else if (message[4] == 2)
			{
                if (OnCrouch != null)
                    OnCrouch(this);
                if (PlayerCrouch != null)
                    PlayerCrouch(this);
				crouch(false);
			}
			else if (message[4] == 4)
			{
                e.SetMetaBit(0, 3, true);
                GlobalMetaUpdate();
			}
			else if (message[4] == 5)
			{
                e.SetMetaBit(0, 3, false);
                GlobalMetaUpdate();
			}
		}
        private void HandleRespawn(byte[] message)
        {
            if (OnRespawn != null)
                OnRespawn(this);
            if (PlayerRespawn != null)
                PlayerRespawn(this);
            if (cancelrespawn)
            {
                cancelrespawn = false;
                return;
            }

            health = 20;
            SendRespawn();
            Teleport_Spawn();
        }
		public static short BlockDropSwitch(short id)
		{
			switch (id)
			{
				case(1):
					return 4;
				case(2):
					return 3;
				case (7):
				case (8):
				case (9):
				case (10):
				case (11):
					return 0;
				case (13):
					if (Entity.random.Next(1, 10) == 5) return 318;
					return 13;
				case (16):
					return 263;
				case (18):
					return 0;
				case (20):
					return 0;
				case (21):
					return 251;
                case (30):
                    return 287;
				case (31):
					if (Entity.random.Next(1, 5) == 3) return 295;
					return 0;
				case (32):
					return 0;
				case (34):
					return 0;
				case (36):
					return 0;
				case (51):
					return 0;
				case (52):
					return 0;
				case (55):
					return 331;
				case (56):
					return 264;
                case (59):
                    return 295;
                case (60):
                    return 3;
				case (63):
					return 323;
				case (68):
					return 323;
				case (75):
					return 76;
                case (78):
				case (79):
				case (90):
				case (92):
					return 0;
				case (93):
				case (94):
                    return 356;
					//return 354; // lolwut? why cake?
                case 97:
                    return 0;
                case 99:
                    if (Entity.random.Next(1, 5) == 3) return 39;
					return 0;
                case 100:
                    if (Entity.random.Next(1, 5) == 3) return 40;
					return 0;
                case 102:
                    return 0;
                case 104:
                case 105:
                    return 0;
                case 106:
                    return 0;
                case 110:
                    return 3;
                case 117:
                    return 379;
                case 118:
                    return 380;

				default:
					return id;
			}
		}
	}
	public enum ClickType { LeftClick = 0, RightClick = 1 };
}
