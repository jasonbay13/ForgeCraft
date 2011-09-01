using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

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
                                Server.whitelist = (value.ToLower() == "true") ? true : false;
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
            w.WriteLine("use-whitelist = " + Server.whitelist.ToString().ToLower());
            w.WriteLine("defaultColor = " + Server.DefaultColor);
            w.WriteLine("irc-color = " + Server.IRCColour);
        }
    }
}
