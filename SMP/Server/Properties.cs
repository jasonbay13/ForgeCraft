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
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SMP
{
    public static class Properties
    {
        public static void Load(string givenPath, bool skipsalt = false)
        {
            if (!skipsalt)
            {
                RandomNumberGenerator prng = RandomNumberGenerator.Create();
                StringBuilder sb = new StringBuilder();
                byte[] oneChar = new byte[1];
                while (sb.Length < 16)
                {
                    prng.GetBytes(oneChar);
                    if (Char.IsLetterOrDigit((char)oneChar[0]))
                    {
                        sb.Append((char)oneChar[0]);
                    }
                }

                Server.salt = sb.ToString();
            }
            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = "";
                        if (line.IndexOf('=') >= 0)
                            value = line.Substring(line.IndexOf('=') + 1).Trim(); // allowing = in the values

                        try
                        {
                            switch (key.ToLower())
                            {
                                case "server-name":
                                    if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                    {
                                        Server.name = value;
                                    }
                                    else { throw new Exception(); }
                                    break;
                                case "motd":
                                    if (ValidString(value, "=![]&:.,{}~-+()?_/\\ ")) // allow = in the motd
                                    {
                                        Server.Motd = value;
                                    }
                                    else { throw new Exception(); }
                                    break;
                                case "port":
                                    Server.port = Convert.ToInt32(value);
                                    break;
                                case "max-players":
                                    if (Convert.ToByte(value) > 128)
                                    {
                                        value = "128"; Server.Log("Max players has been lowered to 128.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.Log("Max players has been increased to 1.");
                                    }
                                    Server.MaxPlayers = Convert.ToByte(value);
                                    break;
                                case "verify-names":
                                    Server.VerifyNames = Convert.ToBoolean(value);
                                    break;
                                case "view-distance":
                                    if (Convert.ToInt32(value) > 15)
                                    {
                                        value = "15"; Server.Log("View distance has been lowered to 15.");
                                    }
                                    else if (Convert.ToInt32(value) < 3)
                                    {
                                        value = "3"; Server.Log("View distance has been increased to 3.");
                                    }
                                    Server.ViewDistance = Convert.ToInt32(value);
                                    break;
                                case "use-whitelist":
                                    Server.usewhitelist = Convert.ToBoolean(value);
                                    break;
                                case "use-viplist":
                                    Server.useviplist = Convert.ToBoolean(value);
                                    break;
                                case "defaultcolor":
                                    if (value.Length == 2)
                                    {
                                        if (value.Substring(0, 1) == "%" || value.Substring(0, 1) == "&" || value.Substring(0, 1) == "\x00A7")
                                        {
                                            if (Color.IsColorValid(Convert.ToChar(value.Substring(1, 1))))
                                            {
                                                Color.ServerDefaultColor = value;
                                                break;
                                            }
                                        }
                                    }
                                    throw new Exception();
                                case "irc-color":
                                    if (value.Length == 2)
                                    {
                                        if (value.Substring(0, 1) == "%" || value.Substring(0, 1) == "&" || value.Substring(0, 1) == Color.Signal)
                                        {
                                            if (Color.IsColorValid(Convert.ToChar(value.Substring(1, 1))))
                                            {
                                                Color.IRCColor = value;
                                                break;
                                            }
                                        }
                                    }
                                    throw new Exception();
                                case "generator-threads":
                                    if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.Log("Generator threads has been increased to 1.");
                                    }
                                    Server.genThreads = Convert.ToByte(value);
                                    break;
                            }
                        }
                        catch { Server.ServerLogger.Log(LogLevel.Warning, "Invalid " + key.ToLower() + "! Setting to default."); }
                    }
                }
                Server.s.SettingsUpdate();
                Save(givenPath);
            }
            else Save(givenPath);
        }

        public static bool ValidString(string str, string allowed)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach (char ch in str)
            {
                if (allowedchars.IndexOf(ch) == -1)
                {
                    return false;
                }
            } return true;
        }
        static void Save(string givenPath)
        {
            try
            {
                File.Create(givenPath).Dispose();
                using (StreamWriter w = File.CreateText(givenPath))
                {
                    if (givenPath.IndexOf("server") != -1)
                    {
                        SaveProps(w);
                    }
                }
            }
            catch
            {
                Server.Log("SAVE FAILED! " + givenPath);
            }
        }
        static void SaveProps(StreamWriter w)
        {
            w.WriteLine("server-name = " + Server.name);
            w.WriteLine("motd = " + Server.Motd);
            w.WriteLine("port = " + Server.port.ToString());
            w.WriteLine("max-players = " + Server.MaxPlayers.ToString());
            w.WriteLine("verify-names = " + Server.VerifyNames.ToString().ToLower());
            w.WriteLine("view-distance = " + Server.ViewDistance.ToString());
            w.WriteLine("use-whitelist = " + Server.usewhitelist.ToString().ToLower());
			w.WriteLine("use-VIPlist = " + Server.useviplist.ToString().ToLower());
            w.WriteLine("defaultColor = " + Color.ServerDefaultColor);
            w.WriteLine("irc-color = " + Color.IRCColor);
            w.WriteLine("generator-threads = " + Server.genThreads.ToString());
        }
		
		public static List<string> LoadList(string file)
        {
            if (!File.Exists(file))
            {
                StreamWriter fh = File.AppendText(file);
                fh.Close();
                fh.Dispose();
            }

            List<string> players = new List<string>();
            FileStream fs = File.OpenRead(file);
            StreamReader sr = new StreamReader(fs);
            string line;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line == null || (line = line.Trim()).Length == 0)
                    continue;

                if (line.Length >= 2 && line[0] == '/' && line[1] == '/' || line[0] == '#')
                    continue;

                players.Add(line);
            }

            sr.Close();
            sr.Dispose();
            fs.Close();
            fs.Dispose();

            return players;
        }

		public static void WriteList(List<string> list, string file)
        {
            if (!Directory.Exists("properties"))
                Directory.CreateDirectory("properties");

            if (!File.Exists(file))
            {
                StreamWriter fh = File.AppendText(file);
                fh.Close();
                fh.Dispose();
            }

            FileStream fs = File.OpenWrite(file);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != "")
                    sw.WriteLine(list[i]);   
            }
            sw.WriteLine();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();

        }
    }
}
