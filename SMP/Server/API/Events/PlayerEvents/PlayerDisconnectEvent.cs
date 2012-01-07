using System;
using System.Collections.Generic;
using SMP.util;
using SMP.PLAYER;

namespace SMP.API.Events.PlayerEvents
{
    public class PlayerDisconnectEvent
    {
        internal static List<PlayerDisconnectEvent> events = new List<PlayerDisconnectEvent>();
        Plugin plugin;
        Player.OnPlayerDisconnect method;
        Priority priority;
        internal PlayerDisconnectEvent(Player.OnPlayerDisconnect method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(Player p)
        {
            events.ForEach(delegate(PlayerDisconnectEvent p1)
            {
                try
                {
                    p1.method(p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the OnPlayerAuth Event!"); Logger.LogErrorToFile(e); }
            });
        }
        public static void Organize()
        {
            List<PlayerDisconnectEvent> temp = new List<PlayerDisconnectEvent>();
            List<PlayerDisconnectEvent> temp2 = events;
            PlayerDisconnectEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (PlayerDisconnectEvent p in temp2)
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
        public static PlayerDisconnectEvent Find(Plugin plugin)
        {
            foreach (PlayerDisconnectEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnPlayerDisconnect method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new PlayerDisconnectEvent(method, priority, plugin));
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
