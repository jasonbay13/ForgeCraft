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
namespace SMP.Commands
{
	public class CmdGotoLVL : Command
	{
		public override string Category {
			get {
				return "Mod";
			}
		}
		public override bool ConsoleUseable {
			get {
				return false;
			}
		}
		public override string Description {
			get {
				return "Go to a world!";
			}
		}
		public override string Name {
			get {
				return "goto";
			}
		}
		public override string PermissionNode {
			get {
				return "core.world.goto";
			}
		}
		public override System.Collections.Generic.List<string> Shortcuts {
			get {
				return new System.Collections.Generic.List<string>{ "g" };
			}
		}
		public override void Use (Player p, params string[] args)
		{
            if (args.Length < 1)
                Help(p);

            string level = args[0].ToLower();
            if (World.Find(level) != null && p.level.name.ToLower() != level)
			{
				Player.players.ForEach(delegate(Player p1) { if (p1.level == p.level) p1.SendDespawn(p.id); p.SendDespawn(p1.id); });
                foreach (Point pt in p.VisibleChunks) { p.SendPreChunk(pt.x, pt.z, 0); }
                p.level = World.Find(level);
                p.Teleport_Spawn();
				//foreach (Chunk c in p.level.chunkData.Values) { p.SendPreChunk(c, 1); System.Threading.Thread.Sleep(10); p.SendChunk(c); c.RecalculateLight(); } // This is a bad idea!
				p.VisibleChunks.Clear();
				p.UpdateChunks(true, true);
				return;
			}
			p.SendMessage("GOTO FAILED");
		}
		public override void Help (Player p)
		{
			p.SendMessage("Goto a new level");
		}
	}
}

