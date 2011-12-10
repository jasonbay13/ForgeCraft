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
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using SMP.util;
using SMP.Commands;

namespace SMP
{
    public abstract partial class Plugin
    {
        public static List<Plugin> all = new List<Plugin>();
        public abstract void Load(bool startup);
        public abstract void Unload(bool shutdown);
        public abstract string name { get; }
        public abstract Version version { get; }
        public abstract Version ForgeCraft_Version { get; }
        public abstract string welcome { get; }
	    public abstract string creator { get; }
        public abstract void Help(Player p);
		/// <summary>
		/// Find a Plugin with a name
		/// </summary>
		/// <param name='name'>
		/// Name. The name of the plugin
		/// </param>
        public static Plugin Find(string name)
        {
            List<Plugin> tempList = new List<Plugin>();
            tempList.AddRange(all);
            Plugin tempPlayer = null; bool returnNull = false;

            foreach (Plugin p in tempList)
            {
                if (p.name.ToLower() == name.ToLower()) return p;
                if (p.name.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempPlayer == null) tempPlayer = p;
                    else returnNull = true;

                }
            }

            if (returnNull == true) return null;
            if (tempPlayer != null) return tempPlayer;
            return null;
        }
		/// <summary>
		/// Load a Plugin
		/// </summary>
		/// <param name='file'>
		/// Pluginname. The path to the plugin dll
		/// </param>
		/// <param name='startup'>
		/// Startup. Is it server startup?
		/// </param>
        public static void Load(string file, bool startup)
        {
            String creator = String.Empty;
            try
            {
                object instance = null;
                Assembly lib = null;
                using (FileStream fs = File.Open(file, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read = 0;
                        while ((read = fs.Read(buffer, 0, 1024)) > 0)
                            ms.Write(buffer, 0, read);
                        lib = Assembly.Load(ms.ToArray());
                    }
                }
                try
                {
                    foreach (Type t in lib.GetTypes())
                    {
                        try
                        {
                            if (t.BaseType == typeof(Plugin))
                            {
                                try
                                {
                                    instance = Activator.CreateInstance(t);
                                    Load((Plugin)instance, startup);
                                }
                                catch
                                {
                                    if (instance != null)
                                    {
                                        Logger.LogFormat("The plugin \"{0}\" failed to load!", ((Plugin)instance).name);
                                        Logger.LogFormat("You can go bug {0} about it.", ((Plugin)instance).creator);
                                    }
                                    else Logger.Log("An unknown plugin failed to load!");
                                }
                            }
                            else if (t.BaseType == typeof(Command))
                            {
                                try
                                {
                                    instance = Activator.CreateInstance(t);
                                    Command.all.Add((Command)instance);
                                    Logger.LogFormat("Loaded command: {0}", ((Command)instance).Name);
                                }
                                catch
                                {
                                    if (instance != null) Logger.LogFormat("The command \"{0}\" failed to load!", ((Command)instance).Name);
                                    else Logger.Log("An unknown command failed to load!");
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            instance = null;
                        }
                    }
                }
                catch { }
            }
            catch (FileNotFoundException)
            {
                //Server.ErrorLog(e);
            }
            catch (BadImageFormatException)
            {
                //Server.ErrorLog(e);
            }
            catch (PathTooLongException)
            {
            }
            catch (FileLoadException)
            {
                //Server.ErrorLog(e);
            }
            catch (Exception)
            {
                //Server.ErrorLog(e);
                Logger.Log("The plugin " + file + " failed to load!");
                if (creator != "")
                    Logger.Log("You can go bug " + creator + " about it");
                Thread.Sleep(1000);
            }
        }
        private static void Load(Plugin plugin, bool startup)
        {
            if (plugin == null) throw new ArgumentNullException();
            if (plugin.ForgeCraft_Version > Server.version)
            {
                Logger.LogFormat("Plugin \"{0}\" isn't compatible with this version of ForgeCraft!", plugin.name);
                if (Server.unsafe_plugin) Logger.Log("Will attempt to load anyways.");
                else return;
            }
            Plugin.all.Add(plugin);
            plugin.Load(startup);
            Logger.LogFormat("Loaded plugin: {0} v{1}", plugin.name, plugin.version);
            Logger.Log(plugin.welcome);
        }
	    /// <summary>
	    /// Unload the specified p and shutdown.
	    /// </summary>
	    /// <param name='p'>
	    /// P. The plugin object you want to unload
	    /// </param>
	    /// <param name='shutdown'>
	    /// Shutdown. Is the server shutting down?
	    /// </param>
        public static void Unload(Plugin p, bool shutdown)
        {
            p.Unload(shutdown);
            all.Remove(p);
            Logger.Log(p.name + " was unloaded.");
        }
	    /// <summary>
	    /// Unload all plugins.
	    /// </summary>
	    public static void Unload()
	    {
		    all.ForEach(delegate(Plugin p)
		    {
			    Unload(p, true);
		    });
	    }
	    /// <summary>
	    /// Load all plugins.
	    /// </summary>
        public static void Load()
        {
            if (Directory.Exists("plugins"))
            {
                foreach (string file in Directory.GetFiles("plugins", "*.dll"))
                {
                    Load(file, true);
                }
            }
            else
                Directory.CreateDirectory("plugins");
        }
    }
}