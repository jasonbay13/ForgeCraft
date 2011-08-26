using System;
using System.Collections.Generic;

namespace SMP
{
    public class CmdKill : Command
    {
        public override string Name { get { return "kill"; } }
        public override List<string> Shortcuts { get { return new List<string> { "murder" }; } }
        public override string Category { get { return "mod"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Tasty Murder!!"; } }
        public override string PermissionNode { get { return "core.mod.devs"; } }

        public override void Use(Player p, params string[] args)
        {
            // CURRENTLY JUST USING FOR DEBUG
            if (args.Length == 1)
            {
                string text = args[0];
                Player q = Player.FindPlayer(args[0]);
                if (text[0] == '@')
                {
                    string newtext = text;
                    if (text[0] == '@') newtext = text.Remove(0, 1).Trim();

                    Player d = Player.FindPlayer(newtext);

                    d.health = 0;
                    d.SendHealth();
                }

                q.health = 0;
                q.SendHealth();
                Player.GlobalMessage(q.username + " was destroyed by " + p.username);
                return;
            }
            else if (args.Length == 0)
            {

                p.health = 0;
                p.SendHealth();
                return;
            }
            else
            {

            }
        }

        public override void Help(Player p)
        {
            p.SendMessage("/kill (optional: <playername>)");
            p.SendMessage("place an '@'in front of playername to do it silently");
        }
    }
}