using System;
using System.Collections.Generic;
using SMP.util;
using SMP.PLAYER;

namespace SMP.API.Events.PlayerEvents
{
    public class OnPlayerSprintEvent
    {
        internal static List<OnPlayerSprintEvent> events = new List<OnPlayerSprintEvent>();
        Plugin plugin;
        Player.OnSpritChange method;
        Priority priority;
        internal OnPlayerSprintEvent(Player.OnSpritChange method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        internal static void Call(Player p)
        {
            events.ForEach(delegate(OnPlayerSprintEvent p1)
            {
                try
                {
                    p1.method(p);
                }
                catch (Exception e) { Logger.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerKick Event!"); Logger.LogErrorToFile(e); }
            });
        }
        public static void Organize()
        {
            List<OnPlayerSprintEvent> temp = new List<OnPlayerSprintEvent>();
            List<OnPlayerSprintEvent> temp2 = events;
            OnPlayerSprintEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (OnPlayerSprintEvent p in temp2)
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
        public static OnPlayerSprintEvent Find(Plugin plugin)
        {
            foreach (OnPlayerSprintEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        public static void Register(Player.OnSpritChange method, Priority priority, Plugin plugin)
        {
            if (Find(plugin) != null)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnPlayerSprintEvent(method, priority, plugin));
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
