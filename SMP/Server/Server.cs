using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MonoTorrent.Client;
using System.Threading;
using System.IO;

namespace SMP
{
	public class Server
	{
		public static Server s;
		public bool shuttingDown = false;
		public static Socket listen;
		public static World mainlevel;
		public static int protocolversion = 14;
		public static string Version = "0.1";
		
		public static bool unsafe_plugin = false;
		public static Logger ServerLogger = new Logger();
		internal ConsolePlayer consolePlayer;
        #region ==EVENTS==
        #region -DELEGATES-
        public delegate void OnLog(string message);
        public delegate void VoidHandler();
        #endregion
        #region -EVENT-
        public static event OnLog ServerLog = null;
        public event VoidHandler OnSettingsUpdate;
        #endregion
        
        #endregion

        public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
		public static MainLoop ml;

        #region ==SETTINGS==

        public static string salt = "";
        public static string KickMessage = "You've been kicked!!";
		public static string Motd = "Powered By ForgeCraft.";
		public static int MaxPlayers = 16;
        public static string name = "sc";
        public static int port = 25566;
        public static string ConsoleName = "Console";
        //---------------//
        public static bool whitelist = false;   //TODO
        public static string DefaultColor = "%e"; //TODO
        public static string IRCColour = "%5";   //TODO
        //---------------//
        #endregion


        public Server()
		{
			Log("Starting Server");
			s = this;
			mainlevel = new World(0, 127, 0, "main", new Random().Next());
			World.worlds.Add(mainlevel);
			ml = new MainLoop("server");
			#region updatetimer
			ml.Queue(delegate
			{
				updateTimer.Elapsed += delegate
				{
				Player.GlobalUpdate();
				}; updateTimer.Start();
			});
			#endregion
			//TODO AI Update Timer

			//Setup();
            
            
			//new Creeper(new Point3(0, 0, 0), mainlevel);
		}
		
		public bool Setup()
		{
		//TODO: (in order)

            LoadFiles();
            Properties.Load("properties/server.properties");
			Command.InitCore();
			BlockChange.InitAll();
			Plugin.Load();
			//load groups
			//load whitelist, banlist, VIPlist
            loadCleanUp();
			
			consolePlayer = new ConsolePlayer(s);
			consolePlayer.SetUsername(ConsoleName);
			Group.DefaultGroup = new DefaultGroup(); //debuggin g
            
			try
			{
				IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
				listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				listen.Bind(endpoint);
				listen.Listen((int)SocketOptionName.MaxConnections);
				
				listen.BeginAccept(new AsyncCallback(Accept), null);
				
                return true;
			}
			catch (SocketException e) { Log(e.Message + e.StackTrace); return false; }
			catch (Exception e) { Log(e.Message + e.StackTrace); return false; }
            
		}

        private void loadCleanUp()
        {
            //load any extra stuff, announce any thing after every things has been loaded

            Log("Setting up on port: " + port);
            Log("Server Started");
        }

        private void LoadFiles()
        {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("text")) Directory.CreateDirectory("text");

            if (File.Exists("server.properties")) File.Move("server.properties", "properties/server.properties");
            if (Server.whitelist) if (File.Exists("whitelist.txt")) File.Move("whitelist.txt", "ranks/whitelist.txt");

            
        }
		
		void Accept(IAsyncResult result)
		{
			if (shuttingDown == false)
			{
				Player p = null;
				bool begin = false;
				try
				{
					p = new Player();
					
					p.socket = listen.EndAccept(result);
					new Thread(new ThreadStart(p.Start)).Start();
					
					listen.BeginAccept(new AsyncCallback(Accept), null);
					begin = true;
				}
				catch (SocketException e)
				{
					if (p != null)
					p.Disconnect();
					if (!begin)
					listen.BeginAccept(new AsyncCallback(Accept), null);
				}
				catch (Exception e)
				{
				Log(e.Message);
					Log(e.StackTrace);
					if (p != null)
					p.Disconnect();
					if (!begin)
					listen.BeginAccept(new AsyncCallback(Accept), null);
				}
			}
		}
		
		/// <summary>
		/// To be depracted and replaced with ServerLogger
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Log(string message)
		{
			ServerLogger.Log(message);
		}
		
		public void Stop()
		{
            Plugin.Unload();

			if (listen != null)
            {
                listen.Close();
                listen = null;
            }
		}
		
		public void SetConsoleName(string name)
		{
			ConsoleName = name;
			consolePlayer.username = name;
		}
        internal void SettingsUpdate()
        {
            if (OnSettingsUpdate != null) OnSettingsUpdate();
        }
	}
}