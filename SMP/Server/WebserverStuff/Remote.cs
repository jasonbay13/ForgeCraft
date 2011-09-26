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
using System.Net.Sockets;

namespace SMP
{
    public class Remote
    {
        public string ip;
        public string username;

        byte[] buffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];

        bool disconnected = false;
        public bool LoggedIn { get; protected set; }

        public Socket socket;

        public int version = 1;

        public Remote()
        {

        }
        public void Start()
        {
            try
            {

                ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                Player.GlobalMessage(Color.Announce + "A Remote has connected to the server");
                Server.Log("[REMOTE] " + ip + " connected to the server.");


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
            Remote p = (Remote)result.AsyncState;
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
                p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
                                      new AsyncCallback(Receive), p);
            }
            catch (SocketException)
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
                int length = 0; byte msg = buffer[0];
                // Get the length of the message by checking the first byte
                switch (msg)
                {
                    case 0: length = 0; LoggedIn = true; break;  //Remote Connection
                    case 1: length = 4; break;   //version and type
                    case 2: length = ((BitConverter.ToInt16(buffer, 1) * 2) + 2); break; //Login Info Exchange
                    case 3: length = ((BitConverter.ToInt16(buffer, 1) * 2) + 2); break;  //Handshake
                    case 4: length = ((BitConverter.ToInt16(buffer, 1) * 2) + 2); break;  //Chat
                    //case 0x04: length = 1; break; //Entity Use
                    //case 0x05: length = 1; break; //respawn

                    //case 0x06: length = 1; break; //OnGround incoming
                    //case 0x07: length = 33; break; //Pos incoming
                    //case 0x08: length = 9; break; //???
                    case 10: length = ((BitConverter.ToInt16(buffer, 1) * 2) + 2); break; //DC
                    case 11: length = ((util.EndianBitConverter.Big.ToInt16(buffer, 1) + 2 )); break; //DC




                    default:
                        Server.Log("unhandled message id " + msg);
                        Kick("Unknown Packet id: " + msg);
                        return new byte[0];
                }
                if (buffer.Length > length)
                {
                    byte[] message = new byte[length];
                    Buffer.BlockCopy(buffer, 1, message, 0, length);

                    byte[] tempbuffer = new byte[buffer.Length - length - 1];
                    Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

                    buffer = tempbuffer;

                    switch (msg)
                    {
                        case 1: HandleInfo(message); break;
                        case 0x02: HandleRemoteLogin(message); break;
                        case 0x03: HandleRemoteHandshake(message); break;
                        case 0x04: HandleRemoteChatMessagePacket(message); break;
                       
                        case 10: HandleRemoteDCPacket(message); break; //DC
                        case 11: HandleMobileLogin(message);  break;

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

        private void HandleMobileLogin(byte[] message)
        {
            short length = util.EndianBitConverter.Big.ToInt16(message, 0);
            string msg = Encoding.UTF8.GetString(message, 2, length);
            Server.Log("MOBILE: " + msg);

            byte[] bytes = new byte[(length *2) + 2];
            util.EndianBitConverter.Big.GetBytes((short)msg.Length).CopyTo(bytes, 0);
            Encoding.BigEndianUnicode.GetBytes(msg).CopyTo(bytes, 2);
            System.Threading.Thread.Sleep(4000);
            SendData(0x03, bytes);
        }
        private void wtf(byte[] message)
        {
            
        }
        private void HandleInfo(byte[] message)
        {
            short type = BitConverter.ToInt16(message, 0);
            short version = BitConverter.ToInt16(message, 2);

            switch (type)
            {
                case 1: Server.Log("Desktop remote has joined"); break;
                case 2: Server.Log("Mobile Remote has joined"); break;
                default: Server.Log("Unknown type of remote has attempted to join"); Kick("Unknown type"); return;
            }
            if (version != this.version)
                Kick("You have a different version");
        }
        private void HandleRemoteDCPacket(byte[] message)
        {
            Server.Log("DC");
        }

        private void HandleRemoteChatMessagePacket(byte[] message)
        {
            Server.Log("REMOTE TRY TO TALK");
        }

        private void HandleRemoteHandshake(byte[] message)
        {
            Server.Log("REMOTE REQUEST DATA");
        }

        private void HandleRemoteLogin(byte[] message)
        {

            short length = BitConverter.ToInt16(message, 0);
            if (length > 32) { Kick("Username too long"); return; }
            string Info = Encoding.BigEndianUnicode.GetString(message, 2, (length * 2));

            string[] seperate = Info.Split(':');  //need a better way of sending and getting passwords
            string password = seperate[1];
            username = seperate[0];





            if (username != "admin")
            {
                byte[] bytes = new byte[1];
                bytes[0] = 1;
                SendData(RemotePacketTypes.SendStatus, bytes);
            }
            if (password != "lol")
            {
                byte[] bytes = new byte[1];
                bytes[0] = 2;
                SendData(0xFF, bytes);
                Server.Log("A Remote Attempted to login but got the password incorrect");
            }
            else
            {
                Player.GlobalMessage("[REMOTE] " + username + " has joined the server");
                Server.Log("[REMOTE] " + username + " has joined the server");
            }


        }

        private void HandleRemoteJoin(byte[] message)
        {
            Server.Log("Remote Has Joined");
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
            if (LoggedIn)
                Player.GlobalMessage("§5" + username + " §fhas been kicked from the server! (REMOTE)");
            LoggedIn = false;

            try
            {
                //send kick

            }
            catch { }

            //TODO: Despawn
            this.Dispose();
        }
        public void Disconnect()
        {
            if (disconnected) return;
            disconnected = true;

            //Server.ServerLogger.Log(LogLevel.Notice, "{0}{1} kicked: {2}",
            //	LoggedIn ? "" : "/", LoggedIn ? username : ip);
            if (LoggedIn)
                Player.GlobalMessage("§5" + username + " §fhas disconnected. (REMOTE)");
            LoggedIn = false;

            //TODO: Despawn
            
            this.Dispose();
        }
        public void Dispose()
        {
            //SaveAttributes();

            // Close stuff
            if (socket != null && socket.Connected)
            {
                try { socket.Close(); }
                catch { }
                socket = null;
            }
        }
        public void SendData(RemotePacketTypes id, byte[] send) { SendData((byte)id, send); }
        public void SendData(RemotePacketTypes id) { SendData((byte)id, new byte[0]); }
        public void SendData(int id) { SendData(id, new byte[0]); }
        public void SendData(int id, byte[] send)
        {
            if (socket == null || !socket.Connected) return;

            byte[] buffer = new byte[send.Length + 1];
            buffer[0] = (byte)id;
            send.CopyTo(buffer, 1);

            try
            {
                socket.Send(buffer);
                buffer = null;
            }
            catch (SocketException)
            {
                buffer = null;
                Disconnect();
            }
        }



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
    }
}
