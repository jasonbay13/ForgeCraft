using System;
using System.Collections.Generic;
using SMP.util;

namespace SMP.API
{
    public class OnPlayerChatEvent
    {
        internal static List<OnPlayerChatEvent> events = new List<OnPlayerChatEvent>();
        Plugin plugin;
        Player.OnPlayerChat method;
        Priority priority;
        internal OnPlayerChatEvent(Player.OnPlayerChat method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(string message, Player p)
        {
            events.ForEach(delegate(OnPlayerChatEvent p1)
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
            List<OnPlayerChatEvent> temp = new List<OnPlayerChatEvent>();
            List<OnPlayerChatEvent> temp2 = events;
            OnPlayerChatEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (OnPlayerChatEvent p in temp2)
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
        public static OnPlayerChatEvent Find(Plugin plugin)
        {
            foreach (OnPlayerChatEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnPlayerChat method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnPlayerChatEvent(method, priority, plugin));
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
