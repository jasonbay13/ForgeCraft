using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Net.Sockets;
using System.IO;
namespace SMP
{
    public class CmdCheckPort : Command
    {
        public override string Name { get { return "checkport"; } }
        public override List<string> Shortcuts { get { return new List<string> { "cp" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Checks the port"; } }
        public override string PermissionNode { get { return "core.util.cport"; } }

        public override void Use(Player p, params string[] args)
        {
            int port = 25565;

            if (args.Length == 0)
            {
                checkport(25565, p);
            }
            else if (args.Length == 1)
            {
                try { port = Convert.ToInt16(args[0]); checkport(port, p); }
                catch (Exception e) { p.SendMessage("port must be a number"); p.SendMessage(e.Message); p.SendMessage(e.Source); }
            }

        }

        public override void Help(Player p)
        {
            p.SendMessage("/checkport or /cp");
            p.SendMessage("OPTIONAL you can specify another port by doing /cp <port>");
        }
        void checkport(int port, Player p)
        {
            TcpListener listener = null;
            try
            {
                // Try to open the port. If it fails, the port is probably open already.
                try
                {
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                }
                catch
                {
                    // Port is probably open already by the server, so let's just continue :)
                    listener = null;
                }

                p.SendMessage("Testing Port: " + port);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://mcfire.tk/port.php?port=" + port);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {



                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line == "") { continue; }

                                if (line == "open")
                                {
                                    p.SendMessage(Color.Green + "Port Open!");
                                    return;
                                }

                                p.SendMessage((Color.Red + "Port " + port + " seems to be closed. You may need to set up port forwarding."));


                            }

                        }
                    }
                }
                else { p.SendMessage(Color.Red + "Could Not connect to site, aborting operation"); }


            }
            catch (Exception ex)
            {
                p.SendMessage(Color.Red + "Testing Port Failed!");

                p.SendMessage("Could not start listening on port " + port + ". Another program may be using the port.");
                Server.Log("-----------------port error----------------");
                Server.Log(ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
                Server.Log("-----------------port error----------------");
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }

        }
    }
}