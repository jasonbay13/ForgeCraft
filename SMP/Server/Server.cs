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
using System.Net;
using System.Net.Sockets;
using MonoTorrent.Client;
using System.Threading;
using System.IO;
using SMP.Commands;
using SMP.util;

namespace SMP
{
	public class Server
	{
		public static Server s;
        public static bool useGUI = false;          //not a setting to choose whether using a gui or not, its a reference 
		public bool shuttingDown = false;
		public static Socket listen;
		public static World mainlevel;
		public static int protocolversion = 22;
        public static Version version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }
		public static SQLiteDatabase SQLiteDB;
		public static ItemDB ItemDB;

        public static List<string> devs = new List<string> { "silentneeb", "hypereddie10", "merlin33069", "headdetect", "the_legacy", "dmitchell", "techjar", "shade2010", "bemacized", "wouto1997" }; //add your names here (must be all lower case!)
		
		public static bool unsafe_plugin = false;
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

        public static System.Timers.Timer keepAliveTimer = new System.Timers.Timer(1000);
        public static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
        public static System.Timers.Timer playerlisttimer = new System.Timers.Timer(1000);
        public static System.Timers.Timer worldsavetimer = new System.Timers.Timer(60000);
        public static Thread entityUpdateThread;
		public static MainLoop ml;
        public static byte difficulty = 0; // 0 thru 3 for Peaceful, Easy, Normal, Hard
		public static byte mode = 0; //0=survival, 1=creative

        #region ==SETTINGS==

        public static string salt = "";
        public static string KickMessage = "You've been kicked!!";
		public static string WhiteListMessage = "Not on whitelist";
		public static string VIPListMessage = "Server is full and you are not a VIP";
		public static string BanMessage = "You are banned";
		public static string Motd = "Powered By ForgeCraft.";
		public static byte MaxPlayers = 16;
        public static string name = "sc";
        public static int port = 25565;
        public static string ConsoleName = "Console";
        public static byte genThreads = 1;
        public static bool VerifyNames = false;
        public static int ViewDistance = 3;
        //---------------//
        public static bool usewhitelist = false;
		public static bool useviplist = false;
        //---------------//
		public static List<string> BanList = new List<string>();
		public static List<string> WhiteList = new List<string>();
		public static List<string> VIPList = new List<string>();
        
        #endregion

        public Server()
		{
			s = this;
		}
		
		public bool Setup()
		{
		    //TODO: (in order)
            LoadEvents();
            Logger.Init();
            Logger.Log("Starting Server");
            SQLiteDB  = new SQLiteDatabase(); //
			UpdateDB();
			//ItemDB = new ItemDB();
            LoadFiles();
		    
            Properties.Load("properties/server.properties");
			Command.InitCore();
			BlockChange.InitAll();
            Physics.Handlers.InitAll();
			Plugin.Load();
			
            //Get latest developerlist
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    WebClient wc = new WebClient();
                    string devstring = wc.DownloadString("http://software.mcforge.net/devs.txt");
                    if (devstring.Contains(":"))
                    {
                        devs.Clear();
                        foreach (string dev in devstring.Split(':')) { devs.Add(dev.ToLower()); }
                    }
                }
                catch { }
            })).Start();

			//load groups
			consolePlayer = new ConsolePlayer(s);
			consolePlayer.SetUsername(ConsoleName);
			//Group.DefaultGroup = new DefaultGroup(); //debug
			Group.LoadGroups();
			
			BanList.AddRange(Properties.LoadList("properties/banned.txt"));
            if (usewhitelist)
                WhiteList.AddRange(Properties.LoadList("properties/whitelist.txt"));
            if (useviplist)
               VIPList.AddRange(Properties.LoadList("properties/viplist.txt"));

            loadLevels();
            loadCleanUp();
			
			try
			{
				IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
				listen = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				listen.Bind(endpoint);
				listen.Listen(int.MaxValue);
				
				listen.BeginAccept(new AsyncCallback(Accept), null);
				
                return true;
			}
            catch (SocketException e) { Logger.Log(e.Message + e.StackTrace); return false; }
            catch (Exception e) { Logger.Log(e.Message + e.StackTrace); return false; }
		}

	    private void LoadEvents()
	    {
	        Logger.OnLog += Console.WriteLine;
	    }

	    private void loadCleanUp()
        {
            //load any extra stuff, announce any thing after every things has been loaded

            ml = new MainLoop("server");
            #region updatetimer
            ml.Queue(delegate
            {
                keepAliveTimer.Elapsed += delegate
                {
                    Player.players.ForEach(delegate(Player p) { p.SendKeepAlive(); });
                }; keepAliveTimer.Start();
            });
            ml.Queue(delegate
            {
                updateTimer.Elapsed += delegate
                {
                    Player.GlobalUpdate();
                }; updateTimer.Start();
            });
            ml.Queue(delegate
            {
                playerlisttimer.Elapsed += delegate
                {
                    Player.PlayerlistUpdate();
                }; playerlisttimer.Start();
            });
            ml.Queue(delegate
            {
                worldsavetimer.Elapsed += delegate
                {
                    World.worlds.ForEach(delegate(World w)
                    {
                        w.SaveLVL(true);
                    });
                }; worldsavetimer.Start();
            });
            ml.Queue(delegate
            {
                entityUpdateThread = new Thread(new ThreadStart(delegate
                {
                    int wait = 0, speed = 50;
                    while (!Server.s.shuttingDown)
                    {
                        try
                        {
                            if (wait > 0) Thread.Sleep(wait);

                            DateTime Start = DateTime.Now;
                            Entity.EntityPhysics();
                            TimeSpan Took = DateTime.Now - Start;
                            wait = speed - (int)Took.TotalMilliseconds;
                        }
                        catch { wait = speed; }
                    }
                })); entityUpdateThread.Start();
            });
            ml.Queue(delegate
            {
                World.chunker.Start();
            });
            #endregion
            //TODO AI Update Timer

            //Setup();

            //new Creeper(new Point3(0, 72, 0), mainlevel);


            Logger.Log("Setting up on port: " + port);
            Logger.Log("Server Started");
        }

        private void loadLevels()
        {
            // TODO: autoload.txt
            if (Directory.Exists("main"))
            {
                mainlevel = World.LoadLVL("main");
            }
            else
            {
                //mainlevel = new World(0, 127, 0, "main", 0); // Flatgrass
                mainlevel = new World(0, 127, 0, "main", new java.util.Random().nextLong()); // Perlin
                mainlevel.SaveLVL();
                World.worlds.Add(mainlevel);
            } //changed to seed 0 for now
        }

        private void LoadFiles()
        {
            if (!Directory.Exists("properties")) Directory.CreateDirectory("properties");
            if (!Directory.Exists("text")) Directory.CreateDirectory("text");

            if (File.Exists("server.properties")) File.Move("server.properties", "properties/server.properties");
            if (Server.usewhitelist) if (File.Exists("whitelist.txt")) File.Move("whitelist.txt", "properties/whitelist.txt");
            
        }
		
		void Accept(IAsyncResult result)
		{
		    if (shuttingDown) return;
		    Player p = null;
		    bool begin = false;
		    try
		    {
		        p = new Player {socket = listen.EndAccept(result)};

		        new Thread(p.Start).Start();
					
		        listen.BeginAccept(Accept, null);
		        begin = true;
		    }
		    catch (SocketException)
		    {
		        if (p != null)
		            p.Disconnect();
		        if (!begin)
		            listen.BeginAccept(Accept, null);
		    }
		    catch (Exception e)
		    {
                Logger.Log(e.Message);
                Logger.Log(e.StackTrace);
		        if (p != null)
		            p.Disconnect();
		        if (!begin)
		            listen.BeginAccept(new AsyncCallback(Accept), null);
		    }
		}
		
		/// <summary>
        /// To be deprecated and replaced with ServerLogger
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
        [Obsolete("Please use Logger.Log()", true)]
        public static void Log(string message)
		{
			Logger.Log(message);
		}

		
		public void Stop()
		{
            s.shuttingDown = true;

            World.worlds.ForEach(delegate(World w)
            {
                w.physics.Stop();
                w.SaveLVL();
            });
			
            Plugin.Unload();
			
			Group.SaveGroups();
			
			Properties.WriteList(BanList, "properties/banned.txt");
			Properties.WriteList(WhiteList, "properties/whitelist.txt");
			Properties.WriteList(VIPList, "properties/viplist.txt");
				
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
		
		//Update db, i.e. new columns, tables etc
		//right now irrelevant
        //anything in here should be removed prior to first release
        private void UpdateDB()
        {
            SQLiteDB.ExecuteNonQuery(
                        "CREATE TABLE IF NOT EXISTS Sign(" +
                        "X			INTEGER, " +
                        "Y			INTEGER, " +
                        "Z			INTEGER, " +
                        "World  	TEXT, " +
                        "Line1		TEXT, " +
                        "Line2		TEXT, " +
                        "Line3		TEXT, " +
                        "Line4		TEXT " +
                        ");"
                                     );
        }

	}
}