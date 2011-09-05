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
namespace SMP
{
	public class gotoLVL : Command
	{
		public gotoLVL()
		{
		}
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
				return new System.Collections.Generic.List<string>{ };
			}
		}
		public override void Use (Player p, params string[] args)
		{
			if (World.Find(args[0]) != null && p.level.name != args[0])
			{
				Player.players.ForEach(delegate(Player p1) { if (p1.level == p.level) p1.SendDespawn(p.id); p.SendDespawn(p1.id); });
				foreach (Chunk c in p.level.chunkData.Values) { p.SendPreChunk(c, 0); }
				p.level = World.Find(args[0]);
				p.Teleport_Player(p.level.SpawnX, p.level.SpawnY, p.level.SpawnZ);
				foreach (Chunk c in p.level.chunkData.Values) { p.SendPreChunk(c, 1); System.Threading.Thread.Sleep(10); p.SendChunk(c); c.RecalculateLight(); }
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

