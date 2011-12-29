using System;
using System.Collections.Generic;
using SMP.util;

namespace SMP.API
{
    public class OnPlayerCommandEvent
    {
        internal static List<OnPlayerCommandEvent> events = new List<OnPlayerCommandEvent>();
        Plugin plugin;
        Player.OnPlayerCommand method;
        Priority priority;
        internal OnPlayerCommandEvent(Player.OnPlayerCommand method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(string cmd, string message, Player p)
        {
            events.ForEach(delegate(OnPlayerCommandEvent p1)
            {
                try
                {
                    p1.method(cmd, message, p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerCommand Event!"); Logger.LogErrorToFile(e); }
            });
        }
        static void Organize()
        {
            List<OnPlayerCommandEvent> temp = new List<OnPlayerCommandEvent>();
            List<OnPlayerCommandEvent> temp2 = events;
            OnPlayerCommandEvent temp3 = null;
            temp2.ForEach(delegate(OnPlayerCommandEvent auth)
            {
                foreach (OnPlayerCommandEvent p in temp2)
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
        public static OnPlayerCommandEvent Find(Plugin plugin)
        {
            foreach (OnPlayerCommandEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnPlayerCommand method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnPlayerCommandEvent(method, priority, plugin));
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
