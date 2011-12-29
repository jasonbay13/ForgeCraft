using System;
using System.Collections.Generic;
using SMP.util;

namespace SMP.API
{
    public class OnCrouchChangeEvent
    {
        internal static List<OnCrouchChangeEvent> events = new List<OnCrouchChangeEvent>();
        Plugin plugin;
        Player.OnCrouchChange method;
        Priority priority;
        internal OnCrouchChangeEvent(Player.OnCrouchChange method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(Player p)
        {
            events.ForEach(delegate(OnCrouchChangeEvent p1)
            {
                try
                {
                    p1.method(p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerCrouchChange Event!"); Logger.LogErrorToFile(e); }
            });
        }
        static void Organize()
        {
            List<OnCrouchChangeEvent> temp = new List<OnCrouchChangeEvent>();
            List<OnCrouchChangeEvent> temp2 = events;
            OnCrouchChangeEvent temp3 = null;
            temp2.ForEach(delegate(OnCrouchChangeEvent auth)
            {
                foreach (OnCrouchChangeEvent p in temp2)
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
        public static OnCrouchChangeEvent Find(Plugin plugin)
        {
            foreach (OnCrouchChangeEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnCrouchChange method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnCrouchChangeEvent(method, priority, plugin));
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
