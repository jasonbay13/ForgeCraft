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
using System.Reflection;
using System.Threading;
using SMP.Commands;
using SMP.API;
using SMP.Commands;
using SMP.util;

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
namespace SMP
{
    internal static class Program
    {
        private static Server Server;
        public static WebServer webServer;
        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException_Handler;
            AppDomain.CurrentDomain.ProcessExit += (CurrentDomain_ProcessExit);
        }

        public static bool RunningInMono()
        {
            return (Type.GetType("Mono.Runtime") != null);
        }

        [MTAThread]
        public static void Main(string[] args)
        {
            if(RunningInMono())
				Console.WriteLine("Mono Framework detected!");
            if (!Server.useGUI)
                Console.Title = "ForgeCraft v" + Server.version.ToString();

            StartServer();
            StartInput();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (Server != null)
                Server.Stop();
        }

        private static void Exit()
        {
            Server.Stop();
            Server = null;
        }

        private static void StartServer()
        {
        	(Server = new Server()).Setup();
            (webServer = new WebServer()).Start();
        }

        private static void StartInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == null || (input = input.Trim()).Length == 0)
                    continue;

                string[] inputParts = input.Split();

                if (inputParts[0].StartsWith("/"))
                {
                    string cmd = inputParts[0].Substring(1).ToLower();
                    
                    switch (cmd)
                    {
                        case "stop":
                            Logger.Log(LogLevel.Info, "Stopping Server...");
							for(int i =Player.players.Count -1; i >= 0; i--)
							{
								Player.players[i].Kick("Server Shutting Down!");
							}
                            Exit();
                            return;
						case "id":
							new Thread(new ThreadStart(InventoryDebug)).Start();
							break;
                        default:
                            Command command = Command.all.Find(cmd);
                            if (command == null)
                            {
                                Logger.Log(LogLevel.Info, "Unrecognized command: " + cmd);
                                break;
                            } 

                            if (command.ConsoleUseable)
                            {
                                string[] args = new string[inputParts.Length - 1];
                                
                                Array.Copy(inputParts, 1, args, 0, inputParts.Length - 1);

                                try { command.Use(Server.consolePlayer, args); }
                                catch (Exception e) { Logger.LogError(e); }
                                break;
                            }
                            else
                            {
                                Logger.Log(LogLevel.Info, cmd + " command not useable in the console.");
                                break;
                            }
                    }
                }
                else if (inputParts[0].StartsWith("@"))
                {
                    string name = inputParts[0].Substring(1);
                    string message = "";
                    Player p = Player.FindPlayer(name);
                    Logger.Log(name + " : " );
                    
                    if (p != null)
                    {
                        if (inputParts.Length <= 1)
                        {
                            Logger.Log(LogLevel.Warning, "Please enter a message to send");
                        }
                        else if (input.Length > 1)
                        {
                            for (int i = 1; i < inputParts.Length; i++)
                            {
                                message += inputParts[i];
                            }
                            p.SendMessage(Color.PrivateMsg + "[Server >> Me] " + Color.ServerDefaultColor + message);
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warning, "Please enter a valid username");
                    }
                }
                else
                {
                    Player.GlobalMessage(Color.Announce + "[" + Server.consolePlayer.username + "]: " + Color.Blue + input);
                }
            }
        }
        private static void UnhandledException_Handler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError((Exception)e.ExceptionObject);
        }



		static void InventoryDebug()
		{
			
			GUI.InventoryDebug dbg = new GUI.InventoryDebug();
			dbg.ShowDialog();
		}
    }
}
