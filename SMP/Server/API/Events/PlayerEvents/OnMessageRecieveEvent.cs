﻿using System;
using System.Collections.Generic;
using SMP.util;
using SMP.PLAYER;

namespace SMP.API.Events.PlayerEvents
{
    public class OnMessageRecieveEvent
    {
        internal static List<OnMessageRecieveEvent> events = new List<OnMessageRecieveEvent>();
        Plugin plugin;
        Player.OnPlayerChat method;
        Priority priority;
        internal OnMessageRecieveEvent(Player.OnPlayerChat method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(string message, Player p)
        {
            events.ForEach(delegate(OnMessageRecieveEvent p1)
            {
                try
                {
                    p1.method(message, p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerChat Event!"); Logger.LogErrorToFile(e); }
            });
        }
        public static void Organize()
        {
            List<OnMessageRecieveEvent> temp = new List<OnMessageRecieveEvent>();
            List<OnMessageRecieveEvent> temp2 = events;
            OnMessageRecieveEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (OnMessageRecieveEvent p in temp2)
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
        public static OnMessageRecieveEvent Find(Plugin plugin)
        {
            foreach (OnMessageRecieveEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnPlayerChat method, Priority priority, Plugin plugin)
        {
            //hi
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnMessageRecieveEvent(method, priority, plugin));
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
