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
    class CmdSpheroid : Command
    {
        public override string Name { get { return "spheroid"; } }
        public override List<string> Shortcuts { get { return new List<string> { "ellipse", "e" }; } }
        public override string Category { get { return "build"; } }
        public override bool ConsoleUseable { get { return false; } }
        public override string Description { get { return "Spheroid"; } }
        public override string PermissionNode { get { return "core.build.spheroid"; } }

        public override void Use(Player p, params string[] args)
        {
            SpheroidData cd; cd.x = 0; cd.y = 0; cd.z = 0;
            cd.type = -1; cd.vertical = false;

            if (args.Length >= 2)
            {
                try { cd.type = Convert.ToInt16(args[0]); }
                catch { cd.type = FindBlocks.FindBlock(args[0]); }
                if (!FindBlocks.ValidBlock(cd.type)) { p.SendMessage("There is no block \"" + args[0] + "\"."); return; }

                cd.vertical = (args[1].ToLower() == "vertical");
            }
            else if (args.Length >= 1)
            {
                cd.vertical = (args[0].ToLower() == "vertical");

                if (!cd.vertical)
                {
                    try { cd.type = Convert.ToInt16(args[0]); }
                    catch { cd.type = FindBlocks.FindBlock(args[0]); }
                    if (!FindBlocks.ValidBlock(cd.type)) { p.SendMessage("There is no block \"" + args[0] + "\"."); return; }
                }
            }

            p.ClearBlockChange();
            p.BlockChangeObject = cd;
            p.OnBlockChange += Blockchange1;
            p.SendMessage("Place/delete a block at 2 corners for the spheroid.");
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
            SpheroidData cd = (SpheroidData)p.BlockChangeObject;
            cd.x = x; cd.y = y; cd.z = z;
            p.BlockChangeObject = cd;
            p.OnBlockChange += Blockchange2;
        }

        void Blockchange2(Player p, int x, int y, int z, short type)
        {
            p.ClearBlockChange();
            //p.SendMessage("tile: " + x + " " + y + " " + z + " " + type);
            p.SendBlockChange(x, (byte)y, z, p.level.GetBlock(x, y, z), p.level.GetMeta(x, y, z));
            SpheroidData cd = (SpheroidData)p.BlockChangeObject;
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
            p.SendMessage("Spheroiding...");
            if (!cd.vertical)
            {
                // find center points
                double cx = (ex + sx) / 2 + (((ex + sx) % 2 == 1) ? 0.5 : 0);
                double cy = (ey + sy) / 2 + (((ey + sy) % 2 == 1) ? 0.5 : 0);
                double cz = (ez + sz) / 2 + (((ez + sz) % 2 == 1) ? 0.5 : 0);

                // find axis lengths
                double rx = Convert.ToDouble(ex) - cx + 0.25;
                double ry = Convert.ToDouble(ey) - cy + 0.25;
                double rz = Convert.ToDouble(ez) - cz + 0.25;

                double rx2 = 1 / (rx * rx);
                double ry2 = 1 / (ry * ry);
                double rz2 = 1 / (rz * rz);

                //int totalBlocks = (int)(Math.PI * 0.75 * rx * ry * rz);

                for (int xx = sx; xx <= ex; xx += 8)
                    for (int yy = sy; yy <= ey; yy += 8)
                        for (int zz = sz; zz <= ez; zz += 8)
                            for (int z3 = 0; z3 < 8 && zz + z3 <= ez; z3++)
                                for (int y3 = 0; y3 < 8 && yy + y3 <= ey; y3++)
                                    for (int x3 = 0; x3 < 8 && xx + x3 <= ex; x3++)
                                    {
                                        // get relative coordinates
                                        double dx = (xx + x3 - cx);
                                        double dy = (yy + y3 - cy);
                                        double dz = (zz + z3 - cz);

                                        // test if it's inside ellipse
                                        if ((dx * dx) * rx2 + (dy * dy) * ry2 + (dz * dz) * rz2 <= 1)
                                        {
                                            p.level.BlockChange(x3 + xx, yy + y3, zz + z3, (byte)type, meta, false);
                                            total++;
                                        }
                                    }
            }
            else
            {
                // find center points
                double cx = (ex + sx) / 2 + (((ex + sx) % 2 == 1) ? 0.5 : 0);
                double cz = (ez + sz) / 2 + (((ez + sz) % 2 == 1) ? 0.5 : 0);

                // find axis lengths
                double rx = Convert.ToDouble(ex) - cx + 0.25;
                double rz = Convert.ToDouble(ez) - cz + 0.25;

                double rx2 = 1 / (rx * rx);
                double rz2 = 1 / (rz * rz);
                double smallrx2 = 1 / ((rx - 1) * (rx - 1));
                double smallrz2 = 1 / ((rz - 1) * (rz - 1));

                for (int xx = sx; xx <= ex; xx += 8)
                    for (int zz = sz; zz <= ez; zz += 8)
                        for (int z3 = 0; z3 < 8 && zz + z3 <= ez; z3++)
                            for (int x3 = 0; x3 < 8 && xx + x3 <= ex; x3++)
                            {
                                // get relative coordinates
                                double dx = (xx + x3 - cx);
                                double dz = (zz + z3 - cz);

                                // test if it's inside ellipse
                                if ((dx * dx) * rx2 + (dz * dz) * rz2 <= 1 && (dx * dx) * smallrx2 + (dz * dz) * smallrz2 > 1)
                                {
                                    p.level.BlockChange(x3 + xx, sy, zz + z3, (byte)type, meta, false);
                                    total++;
                                }
                            }
            }
            p.SendMessage(total + " blocks.");
        }

        public struct SpheroidData
        {
            public bool vertical;
            public short type;
            public int x, y, z;
        }
    }
}
