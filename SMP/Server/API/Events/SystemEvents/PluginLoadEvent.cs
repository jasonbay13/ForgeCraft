﻿using System;
using System.Collections.Generic;
using SMP.util;

namespace SMP.API.Events.SystemEvents
{
    public class PluginLoadEvent
    {
        internal static List<PluginLoadEvent> events = new List<PluginLoadEvent>();
        Plugin plugin;
        Plugin.PluginLoad method;
        Priority priority;
        internal PluginLoadEvent(Plugin.PluginLoad method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(Plugin p)
        {
            events.ForEach(delegate(PluginLoadEvent p1)
            {
                try
                {
                    p1.method(p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PluginLoad Event!"); Logger.LogErrorToFile(e); }
            });
        }
        public static void Organize()
        {
            List<PluginLoadEvent> temp = new List<PluginLoadEvent>();
            List<PluginLoadEvent> temp2 = events;
            PluginLoadEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (PluginLoadEvent p in temp2)
                {
                    if (temp3 == null)
                        temp3 = p;
                    else if (temp3.priority < p.priority)
                        temp3 = p;
                }
                temp.Add(temp3);
                temp2.Remove(temp3);
                temp3 = null;
                i++;
            }
            events = temp;
        }
        public static PluginLoadEvent Find(Plugin plugin)
        {
            foreach (PluginLoadEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Plugin.PluginLoad method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new PluginLoadEvent(method, priority, plugin));
            Organize();
        }
        public static void UnRegister(Plugin plugin)
        {
            if (Find(plugin) == null)
                throw new Exception("This plugin doesnt have this event registered!");
            else
                events.Remove(Find(plugin));
        }
    }
}
