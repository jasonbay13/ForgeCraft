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
using SMP.API;

namespace SMP.Commands
{
    public class CmdCuboid : Command
    {
        public override string Name { get { return "cuboid"; } }
        public override List<string> Shortcuts { get { return new List<string> { "z" }; } }
        public override string Category { get { return "build"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Cuboid"; } }
        public override string PermissionNode { get { return "core.build.cuboid"; } }
        
        public override void Use(Player p, params string[] args)
        {
            CuboidData cd; cd.x = 0; cd.y = 0; cd.z = 0;
            cd.type = -1; cd.mode = CuboidType.Solid;

            if (args.Length >= 2)
            {
                try { cd.type = Convert.ToInt16(args[0]); }
                catch { cd.type = FindBlocks.FindBlock(args[0]); }
                if (!FindBlocks.ValidBlock(cd.type)) { p.SendMessage("There is no block \"" + args[0] + "\"."); return; }

                switch (args[1].ToLower())
                {
                    case "hollow":
                        cd.mode = CuboidType.Hollow;
                        break;
                    case "walls":
                        cd.mode = CuboidType.Walls;
                        break;
                    case "holes":
                        cd.mode = CuboidType.Holes;
                        break;
                    case "wire":
                        cd.mode = CuboidType.Wire;
                        break;
                    case "random":
                        cd.mode = CuboidType.Random;
                        break;
                }
            }
            else if (args.Length >= 1)
            {
                switch (args[0].ToLower())
                {
                    case "hollow":
                        cd.mode = CuboidType.Hollow;
                        break;
                    case "walls":
                        cd.mode = CuboidType.Walls;
                        break;
                    case "holes":
                        cd.mode = CuboidType.Holes;
                        break;
                    case "wire":
                        cd.mode = CuboidType.Wire;
                        break;
                    case "random":
                        cd.mode = CuboidType.Random;
                        break;
                }

                if (cd.mode == CuboidType.Solid)
                {
                    try { cd.type = Convert.ToInt16(args[0]); }
                    catch { cd.type = FindBlocks.FindBlock(args[0]); }
                    if (!FindBlocks.ValidBlock(cd.type)) { p.SendMessage("There is no block \"" + args[0] + "\"."); return; }
                }
            }

            p.ClearBlockChange();
            p.BlockChangeObject = cd;
            p.OnBlockChange += Blockchange1;
            p.SendMessage("Place/delete a block at 2 corners for the cuboid.");
            //p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        public override void Help(Player p)
        {
            p.SendMessage(Description);
        }

        void Blockchange1(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            //p.SendMessage("tile: " + x + " " + y + " " + z + " " + type);
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), p.level.GetMeta(x, y, z));
            CuboidData cd = (CuboidData)p.BlockChangeObject;
            cd.x = x; cd.y = y; cd.z = z;
            p.BlockChangeObject = cd;
            p.OnBlockChange += Blockchange2;
        }

        void Blockchange2(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            //p.SendMessage("tile: " + x + " " + y + " " + z + " " + type);
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), p.level.GetMeta(x, y, z));
            CuboidData cd = (CuboidData)p.BlockChangeObject;
            byte meta = (byte)p.inventory.current_item.meta;
            if (cd.type != -1) type = cd.type;
            if (!FindBlocks.ValidBlock(type)) type = 0;

            int sx = Math.Min(cd.x, x);
            int ex = Math.Max(cd.x, x);
            int sy = Math.Min(cd.y, y);
            int ey = Math.Max(cd.y, y);
            int sz = Math.Min(cd.z, z);
            int ez = Math.Max(cd.z, z);

            int total = 0;
            p.SendMessage("Cuboiding...");
            switch (cd.mode)
            {
                case CuboidType.Solid:
                    for (int xx = sx; xx <= ex; xx++)
                        for (int yy = sy; yy <= ey; yy++)
                            for (int zz = sz; zz <= ez; zz++)
                            {
                                p.level.BlockChange(xx, yy, zz, (byte)type, meta, false);
                                total++;
                            }
                    break;
                case CuboidType.Hollow:
                    for (int xx = sx; xx <= ex; xx++)
                        for (int zz = sz; zz <= ez; zz++)
                        {
                            p.level.BlockChange(xx, sy, zz, (byte)type, meta, false);
                            p.level.BlockChange(xx, ey, zz, (byte)type, meta, false);
                            total += 2;
                        }
                    for (int yy = sy; yy <= ey; yy++)
                    {
                        for (int xx = sx; xx <= ex; xx++)
                        {
                            p.level.BlockChange(xx, yy, sz, (byte)type, meta, false);
                            p.level.BlockChange(xx, yy, ez, (byte)type, meta, false);
                            total += 2;
                        }
                        for (int zz = sz; zz <= ez; zz++)
                        {
                            p.level.BlockChange(sx, yy, zz, (byte)type, meta, false);
                            p.level.BlockChange(ex, yy, zz, (byte)type, meta, false);
                            total += 2;
                        }
                    }
                    break;
                case CuboidType.Walls:
                    for (int yy = sy; yy <= ey; yy++)
                    {
                        for (int xx = sx; xx <= ex; xx++)
                        {
                            p.level.BlockChange(xx, yy, sz, (byte)type, meta, false);
                            p.level.BlockChange(xx, yy, ez, (byte)type, meta, false);
                            total += 2;
                        }
                        for (int zz = sz; zz <= ez; zz++)
                        {
                            p.level.BlockChange(sx, yy, zz, (byte)type, meta, false);
                            p.level.BlockChange(ex, yy, zz, (byte)type, meta, false);
                            total += 2;
                        }
                    }
                    break;
                case CuboidType.Holes:
                    bool Checked = true, startZ, startY;
                    for (int xx = sx; xx <= ex; xx++)
                    {
                        startY = Checked;
                        for (int yy = sy; yy <= ey; yy++)
                        {
                            startZ = Checked;
                            for (int zz = sz; zz <= ez; zz++)
                            {
                                Checked = !Checked;
                                if (Checked) { p.level.BlockChange(xx, yy, zz, (byte)type, meta, false); total++; }
                            }
                            Checked = !startZ;
                        }
                        Checked = !startY;
                    }
                    break;
                case CuboidType.Wire:
                    for (int xx = sx; xx <= ex; xx++)
                    {
                        p.level.BlockChange(xx, sy, sz, (byte)type, meta, false);
                        p.level.BlockChange(xx, sy, ez, (byte)type, meta, false);
                        p.level.BlockChange(xx, ey, sz, (byte)type, meta, false);
                        p.level.BlockChange(xx, ey, ez, (byte)type, meta, false);
                        total += 4;
                    }
                    for (int yy = sy; yy <= ey; yy++)
                    {
                        p.level.BlockChange(sx, yy, sz, (byte)type, meta, false);
                        p.level.BlockChange(sx, yy, ez, (byte)type, meta, false);
                        p.level.BlockChange(ex, yy, sz, (byte)type, meta, false);
                        p.level.BlockChange(ex, yy, ez, (byte)type, meta, false);
                        total += 4;
                    }
                    for (int zz = sz; zz <= ez; zz++)
                    {
                        p.level.BlockChange(sx, sy, zz, (byte)type, meta, false);
                        p.level.BlockChange(sx, ey, zz, (byte)type, meta, false);
                        p.level.BlockChange(ex, sy, zz, (byte)type, meta, false);
                        p.level.BlockChange(ex, ey, zz, (byte)type, meta, false);
                        total += 4;
                    }
                    break;
                case CuboidType.Random:
                    Random rand = new Random();
                    for (int xx = sx; xx <= ex; xx++)
                        for (int yy = sy; yy <= ey; yy++)
                            for (int zz = sz; zz <= ez; zz++)
                                if (rand.Next(1, 11) <= 5)
                                {
                                    p.level.BlockChange(xx, yy, zz, (byte)type, meta, false);
                                    total++;
                                }
                    break;
            }
            p.SendMessage(total + " blocks.");
        }

        public enum CuboidType { Solid, Hollow, Walls, Holes, Wire, Random };
        public struct CuboidData
        {
            public short type;
            public int x, y, z;
            public CuboidType mode;
        }
    }
}