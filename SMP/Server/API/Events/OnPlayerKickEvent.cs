using System;
using System.Collections.Generic;
using SMP.util;

namespace SMP.API
{
    public class OnPlayerKickEvent
    {
        internal static List<OnPlayerKickEvent> events = new List<OnPlayerKickEvent>();
        Plugin plugin;
        Player.OnPlayerKicked method;
        Priority priority;
        internal OnPlayerKickEvent(Player.OnPlayerKicked method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(Player p, string reason)
        {
            events.ForEach(delegate(OnPlayerKickEvent p1)
            {
                try
                {
                    p1.method(p, reason);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerKick Event!"); Logger.LogErrorToFile(e); }
            });
        }
        static void Organize()
        {
            List<OnPlayerKickEvent> temp = new List<OnPlayerKickEvent>();
            List<OnPlayerKickEvent> temp2 = events;
            OnPlayerKickEvent temp3 = null;
            temp2.ForEach(delegate(OnPlayerKickEvent auth)
            {
                foreach (OnPlayerKickEvent p in temp2)
                {
                    if (temp3 == null)
                        temp3 = p;
                    else if (temp3.priority < p.priority)
                        temp3 = p;
                }
                temp.Add(temp3);
                temp2.Remove(temp3);
                temp3 = null;
            });
            events = temp;
        }
        public static OnPlayerKickEvent Find(Plugin plugin)
        {
            foreach (OnPlayerKickEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnPlayerKicked method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnPlayerKickEvent(method, priority, plugin));
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
