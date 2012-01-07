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

namespace SMP.Commands
{
    public class CmdSpawn : Command
    {
        public override string Name { get { return "spawn"; } }
        public override List<String> Shortcuts { get { return new List<string> { "respawnme", "respawn" }; } }
        public override string Category { get { return "general"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Spawns you at spawn"; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.mod.spawn"; } }

        public override void Use(Player p, params string[] args)
        {
            if (args.Length != 0)
            {
                Help(p);
                return;
            }
            p.Teleport_Spawn();
            //p.Teleport_Player(p.level.SpawnX, p.level.SpawnY, p.level.SpawnZ);
            /*byte[] bytes = new byte[41];
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnX).CopyTo(bytes, 0);
            util.EndianBitConverter.Big.GetBytes(p.Stance).CopyTo(bytes, 8);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnY).CopyTo(bytes, 16);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnZ).CopyTo(bytes, 24);
            util.EndianBitConverter.Big.GetBytes(p.rot[0]).CopyTo(bytes, 32);
            util.EndianBitConverter.Big.GetBytes(p.rot[1]).CopyTo(bytes, 36);
            bytes[40] = p.onground;
            p.SendRaw(0x0D, bytes);*/
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/spawn");
        }
    }
}