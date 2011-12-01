using System;
using System.Collections.Generic;
using System.Net;
using SMP.util;
using System.Threading;

namespace SMP.Commands
{
    public class CmdCheckPort : Command
    {
        public override string Name { get { return "checkport"; } }
        public override List<string> Shortcuts { get { return new List<string> { "cp" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Checks the port"; } }
        public override string PermissionNode { get { return "core.util.cport"; } }

        public override void Use(Player p, params string[] args)
        {
            int port = 25565;

            if (args.Length == 0)
            {
                if (!p.IsConsole) p.SendMessage("Checking port....");
                else Logger.Log("Checking Port...");
                checkport(25565, p);
            }
            else if (args.Length == 1)
            {
                try
                {
                    port = Convert.ToInt32(args[0]);
                    if (!p.IsConsole) p.SendMessage("Checking port....");
                    else Logger.Log("Checking Port...");
                    checkport(port, p);
                }
                catch (Exception e)
                {
                    if (!p.IsConsole) p.SendMessage("port must be a number");
                    else Logger.Log("Port must be a number");
                }
            }

        }

        public override void Help(Player p)
        {
            if (p.IsConsole)
            {
                Logger.Log("/checkport or /cp");
                Logger.Log("OPTIONAL you can specify another port by doing /cp <port>");
            }
            else
            {
                p.SendMessage("/checkport or /cp");
                p.SendMessage("OPTIONAL you can specify another port by doing /cp <port>");
            }
        }
        void checkport(int port, Player p)
        {
            string response;

            new Thread(new ThreadStart(delegate
            {
                try
                {

                    using (WebClient WEB = new WebClient())
                    {
                        response = WEB.DownloadString("http://www.mcforge.net/ports.php?port=" + port);
                    }
                    if (response == "open")
                    {
                        if (!p.IsConsole) p.SendMessage(Color.Green + "Port Open!");
                        else Logger.Log("Port Open!");
                        return;
                    }
                    if (response == "closed")
                    {
                        if (!p.IsConsole) p.SendMessage(Color.Red + "Port Closed");
                        else Logger.Log("Port Closed");
                        return;

                    }
                    if (!p.IsConsole) p.SendMessage(Color.Yellow + "An Error has occured");
                    else Logger.Log("An Error has occured");
                    return;
                }
                catch
                {
                    if (!p.IsConsole) p.SendMessage(Color.Yellow + "An Error has occured");
                    else Logger.Log("An Error has occured");
                    return;
                }

            })).Start();

        }
    }
}