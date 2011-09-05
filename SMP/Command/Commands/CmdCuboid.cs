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

namespace SMP
{
    public class CmdCuboid : Command
    {
        public override string Name { get { return "cuboid"; } }
        public override List<string> Shortcuts { get { return new List<string> { "z" }; } }
        public override string Category { get { return "other"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Cuboid"; } }
        public override string PermissionNode { get { return "core.build.cuboid"; } }
        Pos pos;
        string cuboidtype = "solid";
        public override void Use(Player p, params string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "help") Help(p);

                try
                {
                    pos.type = short.Parse(args[0]);
                }
                catch
                {
                    pos.type = FindBlocks.FindBlock(args[0]);
                }
                if (pos.type == -1)
                {
                    switch (args[0])
                    {
                        case "hollow":
                            cuboidtype = "hollow";
                            break;
                        case "walls":
                            cuboidtype = "walls";
                            break;
                        case "holes":
                            cuboidtype = "holes";
                            break;
                        //case "wire":
                        //cuboidtype = "wire";
                        //break;
                        case "random":
                            cuboidtype = "random";
                            break;
                        default:
                            cuboidtype = "solid";
                            break;
                    }
                }
            }
            else { pos.type = -1; }
            if (args.Length == 2)
            {
                switch (args[1])
                {
                    case "hollow":
                        cuboidtype = "hollow";
                        break;
                    case "walls":
                        cuboidtype = "walls";
                        break;
                    case "holes":
                        cuboidtype = "holes";
                        break;
                    //case "wire":
                    //cuboidtype = "wire";
                    //break;
                    case "random":
                        cuboidtype = "random";
                        break;
                    default:
                        cuboidtype = "solid";
                        break;
                }
            }
            p.SendMessage("place/delete a block at 2 corners for the cuboid.");
            p.OnBlockChange += Blockchange1;
            //p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        void Blockchange1(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            //p.SendMessage("tile: " + x + " " + y + " " + z + " " + type);
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), (byte)0);
            type = pos.type;
            pos = new Pos { x = x, y = y, z = z, type = type };
            p.OnBlockChange += Blockchange2;
        }

        void Blockchange2(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            //p.SendMessage("tile: " + x + " " + y + " " + z + " " + type);
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), (byte)0);
            if (pos.type != -1) type = pos.type;
            int xx1, zz1, x2, z2;
            byte y2, yy1;
            xx1 = Math.Min(pos.x, x);
            x2 = Math.Max(pos.x, x);
            yy1 = (byte)Math.Min(pos.y, y);
            y2 = (byte)Math.Max(pos.y, y);
            zz1 = Math.Min(pos.z, z);
            z2 = Math.Max(pos.z, z);
            if (type == 255 || type == -1) type = 20;
            //int total = Math.Abs(pos.x - x + 1) * Math.Abs(pos.z - z + 1) * Math.Abs(pos.y - y + 1);
            p.SendMessage("Cuboiding " + total(xx1, x2, yy1, y2, zz1, z2) + " Blocks.");
            if (cuboidtype == "solid")
            {
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (byte y1 = yy1; y1 <= y2; ++y1)
                        for (int z1 = zz1; z1 <= z2; ++z1)
                            p.level.BlockChange(x1, y1, z1, (byte)type, 0);
                return;
            }

            if (cuboidtype == "hollow")
            {
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (int z1 = zz1; z1 <= z2; ++z1)
                    {
                        p.level.BlockChange(x1, yy1, z1, (byte)type, 0);
                        p.level.BlockChange(x1, y2, z1, (byte)type, 0);
                    }
                for (int y1 = yy1; y1 <= y2; ++y1)
                {
                    for (int z1 = zz1; z1 <= z2; ++z1)
                    {
                        p.level.BlockChange(xx1, y1, z1, (byte)type, 0);
                        p.level.BlockChange(x2, y1, z1, (byte)type, 0);
                    }
                    for (int x1 = xx1; x1 <= x2; ++x1)
                    {
                        p.level.BlockChange(x1, y1, zz1, (byte)type, 0);
                        p.level.BlockChange(x1, y1, z2, (byte)type, 0);
                    }
                }
                return;
            }
            if (cuboidtype == "walls")
            {
                for (int y1 = yy1; y1 <= y2; ++y1)
                {
                    for (int z1 = zz1; z1 <= z2; ++z1)
                    {
                        p.level.BlockChange(xx1, y1, z1, (byte)type, 0);
                        p.level.BlockChange(x2, y1, z1, (byte)type, 0);
                    }
                    for (int x1 = xx1; x1 <= x2; ++x1)
                    {
                        p.level.BlockChange(x1, y1, zz1, (byte)type, 0);
                        p.level.BlockChange(x1, y1, z2, (byte)type, 0);
                    }
                }
                return;
            }
            if (cuboidtype == "holes")
            {
                for (int x1 = xx1; x1 <= x2; x1 += 2)
                    for (byte y1 = yy1; y1 <= y2; y1 += 2)
                        for (int z1 = zz1; z1 <= z2; z1 += 2)
                            p.level.BlockChange(x1, y1, z1, (byte)type, 0);
                return;
            }
            if (cuboidtype == "random")
            {
                Random rand = new Random();
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (byte y1 = yy1; y1 <= y2; ++y1)
                        for (int z1 = zz1; z1 <= z2; ++z1)
                            if (rand.Next(1, 11) <= 5) p.level.BlockChange(x1, y1, z1, (byte)type, 0);
                return;
            }
            //if (p.level.GetBlock(x1, y1, z1) != type)
            //p.SendBlockChange(x1, y1, z1, type, 0);
        }
        int total(int xx1, int x2, byte yy1, int y2, int zz1, int z2)
        {
            int total = 0;
            if (cuboidtype == "solid")
            {
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (byte y1 = yy1; y1 <= y2; ++y1)
                        for (int z1 = zz1; z1 <= z2; ++z1)
                            total++;
                return total;
            }

            if (cuboidtype == "hollow")
            {
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (int z1 = zz1; z1 <= z2; ++z1)
                        total++;
                for (int y1 = yy1; y1 <= y2; ++y1)
                {
                    for (int z1 = zz1; z1 <= z2; ++z1)
                        total++;
                    for (int x1 = xx1; x1 <= x2; ++x1)
                        total++;
                }
                return total;
            }
            if (cuboidtype == "walls")
            {
                for (int y1 = yy1; y1 <= y2; ++y1)
                {
                    for (int z1 = zz1; z1 <= z2; ++z1)
                        total++;
                    for (int x1 = xx1; x1 <= x2; ++x1)
                        total++;
                }
                return total;
            }
            if (cuboidtype == "holes")
            {
                for (int x1 = xx1; x1 <= x2; x1 += 2)
                    for (byte y1 = yy1; y1 <= y2; y1 += 2)
                        for (int z1 = zz1; z1 <= z2; z1 += 2)
                            total++;
                return total;
            }
            if (cuboidtype == "random")
            {
                Random rand = new Random();
                for (int x1 = xx1; x1 <= x2; ++x1)
                    for (byte y1 = yy1; y1 <= y2; ++y1)
                        for (int z1 = zz1; z1 <= z2; ++z1)
                            if (rand.Next(1, 11) <= 5) total++;
                return total;
            }
            return total;
        }
        public struct Pos
        {
            public int x, y, z;
            public short type;
        }
        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }
    }
}