﻿/*
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
    public class CmdTeleport : Command
    {
        public override string Name { get { return "teleport"; } }
        public override List<String> Shortcuts { get { return new List<string> { "tp" }; } }
        public override string Category { get { return "Mod"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Teleports to specified player or location"; } } //used for displaying what the commands does when using /help
        public override string PermissionNode { get { return "core.mod.teleport"; } }

        public override void Use(Player p, params string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    goto default;
                case 1:
                    if (Player.FindPlayer(args[0]) != null)
                        p.Teleport_Player(Player.FindPlayer(args[0]).pos);
                    else p.SendMessage("Cannot find player");
                    break;
                case 2:
                    if (args[0].ToLower() == "here")
                    {
                        if (Player.FindPlayer(args[1]) != null)
                            Player.FindPlayer(args[1]).Teleport_Player(p.pos);
                        else p.SendMessage("Cannot find player");
                    }
                    break;
                case 3:
                    try
                    {
                        p.pos = new double[3] { int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]) };
                        if (p.chunknew != p.chunk) { }
                        p.Teleport_Player((double)(int.Parse(args[0])), (double)(int.Parse(args[1])), (double)(int.Parse(args[2])));
                    }
                    catch { p.SendMessage("Cannot tp to ungenerated chunks."); }
                    break;
                default:
                    Help(p);
                    break;
            }
            #region oldcode
            /*if (args.Length == 0)
            {
                Help(p);
                return;
            }
            if (args.Length == 1)
            {
                Player who = Player.FindPlayer(args[0]); // cannot use a using here or players dissapear.
                if (who != null)
                {
                    p.Teleport_Player(who.pos[0], who.pos[1], who.pos[2]);
                    return;
                }
            }
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "here")
                {
                    Player who = Player.FindPlayer(args[0]); // cannot use a using here or players dissapear.
                    if (who != null)
                    {
                        who.Teleport_Player(p.pos[0], p.pos[1], p.pos[2]);
                        return;
                    }
                }
            }
            if (args.Length == 3)
            {
                try
                {
                    p.pos = new double[3] { int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]) };
                    if (p.chunknew != p.chunk) { }
                    p.Teleport_Player((double)(int.Parse(args[0])), (double)(int.Parse(args[1])), (double)(int.Parse(args[2])));
                }
                catch { p.SendMessage("Cannot tp to ungenerated chunks."); return; }
                return;
            }
            Help(p);*/
            /*byte[] bytes = new byte[41]; // some extra code.
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnX).CopyTo(bytes, 0);
            util.EndianBitConverter.Big.GetBytes(p.Stance).CopyTo(bytes, 8);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnY).CopyTo(bytes, 16);
            util.EndianBitConverter.Big.GetBytes(p.level.SpawnZ).CopyTo(bytes, 24);
            util.EndianBitConverter.Big.GetBytes(p.rot[0]).CopyTo(bytes, 32);
            util.EndianBitConverter.Big.GetBytes(p.rot[1]).CopyTo(bytes, 36);
            bytes[40] = p.onground;
            p.SendRaw(0x0D, bytes);*/

            #endregion
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
            p.SendMessage("/tp [player]");
        }
    }
}