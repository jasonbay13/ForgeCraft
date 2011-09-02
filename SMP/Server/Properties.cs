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
                        string color = "";

                        switch (key.ToLower())
                        {
                            case "server-name":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                {
                                    Server.name = value;
                                }
                                else { Server.Log("server-name invalid! setting to default."); }
                                break;
                            case "motd":
                                if (ValidString(value, "=![]&:.,{}~-+()?_/\\ ")) // allow = in the motd
                                {
                                    Server.Motd = value;
                                }
                                else { Server.Log("motd invalid! setting to default."); }
                                break;
                            case "port":
                                try { Server.port = Convert.ToInt32(value); }
                                catch { Server.Log("port invalid! setting to default."); }
                                break;
                            case "max-players":
                                try
                                {
                                    if (Convert.ToByte(value) > 128)
                                    {
                                        value = "128"; Server.Log("Max players has been lowered to 128.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1"; Server.Log("Max players has been increased to 1.");
                                    }
                                    Server.MaxPlayers = Convert.ToByte(value);
                                }
                                catch { Server.Log("max-players invalid! setting to default."); }
                                break;
                            case "use-whitelist":
                                Server.usewhitelist = (value.ToLower() == "true") ? true : false;
                                break;
							case "use-viplist":
                                Server.useviplist = (value.ToLower() == "true") ? true : false;
                                break;
							case "defaultcolor":
								if (value.Length == 2)
								{
									if (value.Substring(0, 1) == "%" || value.Substring(0, 1) == "&" || value.Substring(0, 1) == "\x00A7")
									{
										if (Color.IsColorValid(Convert.ToChar(value.Substring(1,1))))
									    {
											Color.ServerDefaultColor = value;
										}
										else Server.ServerLogger.Log(LogLevel.Warning, "Invalid defaultcolor, setting to default.");
									}
								}
								else Server.ServerLogger.Log(LogLevel.Warning, "Invalid defaultcolor, setting to default.");
								break;
							case "irc-color":
								if (value.Length == 2)
									{
										if (value.Substring(0, 1) == "%" || value.Substring(0, 1) == "&" || value.Substring(0, 1) == "\x00A7")
										{
											if (Color.IsColorValid(Convert.ToChar(value.Substring(1,1))))
										    {
												Color.IRCColor = value;
											}
											else Server.ServerLogger.Log(LogLevel.Warning, "Invalid irc-color, setting to default.");
										}
									}
								else Server.ServerLogger.Log(LogLevel.Warning, "Invalid irc-color, setting to default.");
								break;
                        }

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
            w.WriteLine("use-whitelist = " + Server.usewhitelist.ToString().ToLower());
			w.WriteLine("use-VIPlist = " + Server.useviplist.ToString().ToLower());
            w.WriteLine("defaultColor = " + Color.ServerDefaultColor);
            w.WriteLine("irc-color = " + Color.IRCColor);
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
