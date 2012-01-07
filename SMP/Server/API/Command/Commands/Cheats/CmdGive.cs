
/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using SMP.API.Commands;
using SMP.PLAYER;
using SMP.PLAYER.Crafting;

namespace SMP.Commands
{
	public class CmdGive : Command
	{
		public override string Name { get { return "give"; } }
        public override List<string> Shortcuts { get { return new List<string> {"item", "i"}; } }
        public override string Category { get { return "cheat"; } }
        public override bool ConsoleUseable { get { return true; } }
        public override string Description { get { return "Spawns items."; } }
		public override string PermissionNode { get { return "core.cheat.give"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length < 1) { Help(p); return; }

            Item item = null;
            Player toPlayer = p;

            if (args.Length >= 3)
            {
                toPlayer = Player.FindPlayer(args[0]);
                if (toPlayer == null) { p.SendMessage(HelpBot + "Player not found.", WrapMethod.Chat); return; }
                item = ParseItem(p, (string[])args.TruncateStart(1));
            }
            else if (args.Length >= 2)
            {
                Player toP = Player.FindPlayer(args[0]);
                if (toP == null) item = ParseItem(p, args);
                else { toPlayer = toP; item = ParseItem(p, (string[])args.TruncateStart(1)); }
            }
            else item = ParseItem(p, args);

            if (item == null) return;
            if (toPlayer.IsConsole) { p.SendMessage(HelpBot + "You can't give items to the console.", WrapMethod.Chat); return; }

            SendItem(p, toPlayer, item.id, item.count, item.meta);
			
		}
		public void SendItem(Player from, Player to, short id, byte count, short meta)
		{
            if (id >= 1 && FindBlocks.ValidItem(id))
			{
				to.inventory.Add(id, count, meta);
                to.SendMessage(HelpBot + "Enjoy!", WrapMethod.Chat);
                if (from != to) from.SendMessage(HelpBot + "Gift sent!", WrapMethod.Chat);
			}
            else from.SendMessage(HelpBot + "Invalid item ID.", WrapMethod.Chat);
		}

        public Item ParseItem(Player p, string[] args)
        {
            Item item = Item.Nothing;

            if (args.Length < 1) { Help(p); return null; }
            if (!short.TryParse(args[0], out item.id))
            {
                if (args[0].Contains(":"))
                {
                    try
                    {
                        item.id = short.Parse(args[0].Substring(0, args[0].IndexOf(':')));
                        item.meta = short.Parse(args[0].Substring(args[0].IndexOf(':') + 1));
                    }
                    catch { p.SendMessage(HelpBot + "Something is wrong with the item ID.", WrapMethod.Chat); return null; }
                }
                else
                {
                    short[] foundItem = FindBlocks.FindItem(args[0]);
                    if (foundItem[0] == -1) { p.SendMessage(HelpBot + "Item not found.", WrapMethod.Chat); return null; }
                    item.id = foundItem[0];
                    item.meta = foundItem[1];
                }
            }
            if (args.Length >= 2 && !byte.TryParse(args[1], out item.count)) { p.SendMessage(HelpBot + "Something is wrong with the amount.", WrapMethod.Chat); return null; }
            return item;
        }

		public override void Help(Player p)
		{
			p.SendMessage("Spawns item(s), and if specified to a player.");
			p.SendMessage("/give (player) <item(:meta)> (amount)");
		}
	}
}

