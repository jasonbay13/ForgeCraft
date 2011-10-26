using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public partial class Physics
    {
        public class Handlers
        {
            public delegate bool PHD(Physics P, Check C); //PhysicsHandlerDelegate
            public static Dictionary<byte, PHD> handlers = new Dictionary<byte, PHD>();

            public static void InitAll()
            {
                // The method should return false to keep the check in the list, true to have it removed from the list.
                handlers.Add((byte)Blocks.AWater, new PHD(WaterFlow));
                handlers.Add((byte)Blocks.SWater, new PHD(WaterFlow));
                handlers.Add((byte)Blocks.ALava, new PHD(LavaFlow));
                handlers.Add((byte)Blocks.SLava, new PHD(LavaFlow));
                handlers.Add((byte)Blocks.Sponge, new PHD(SpongeSoak));
            }


            public static bool WaterFlow(Physics P, Check C)
            {
                //if (C.time < 5) { C.time++; return false; }

                byte meta = P.w.GetMeta(C.x, C.y, C.z);
                if (!P.HigherLiquidCheck(C.x, C.y, C.z, 8, meta) && !P.HigherLiquidCheck(C.x, C.y, C.z, 9, meta))
                {
                    if ((meta & 0x7) >= 0x7)
                        P.AddUpdate(C.x, C.y, C.z, 0, 0);
                    else
                    {
                        P.AddUpdate(C.x, C.y, C.z, 8, (byte)Math.Min(meta + 2, 0x7));
                        if (!P.AdjacentLiquidCheck(C.x, C.y, C.z, 8) && !P.AdjacentLiquidCheck(C.x, C.y, C.z, 9)) { C.time = 0; return false; }
                    }
                }
                else if ((meta & 0x8) != 0)
                {
                    if (!P.WaterFlowCheck(C.x, C.y - 1, C.z)) { meta = 0; goto flowOut; }
                    P.WaterFlow(C.x, C.y - 1, C.z, 0x8);
                }
                else if ((meta & 0x7) < 0x7)
                {
                    goto flowOut;
                }
                else
                {
                    P.WaterFlow(C.x, C.y - 1, C.z, 0x8);
                }
                return true;

                flowOut:
                if (P.WaterFlowCheck(C.x, C.y - 1, C.z))
                {
                    P.WaterFlow(C.x, C.y - 1, C.z, 0x8);
                    if (!P.AdjacentLiquidCheck(C.x, C.y, C.z, 8) && !P.AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                    {
                        P.WaterFlow(C.x + 1, C.y, C.z, 0x7);
                        P.WaterFlow(C.x - 1, C.y, C.z, 0x7);
                        P.WaterFlow(C.x, C.y, C.z + 1, 0x7);
                        P.WaterFlow(C.x, C.y, C.z - 1, 0x7);
                    }
                }
                else
                {
                    meta++;
                    P.WaterFlow(C.x + 1, C.y, C.z, meta);
                    P.WaterFlow(C.x - 1, C.y, C.z, meta);
                    P.WaterFlow(C.x, C.y, C.z + 1, meta);
                    P.WaterFlow(C.x, C.y, C.z - 1, meta);
                }
                return true;
            }

            public static bool LavaFlow(Physics P, Check C)
            {
                if (C.time < 6) { C.time++; return false; }

                byte meta = P.w.GetMeta(C.x, C.y, C.z);
                if (!P.HigherLiquidCheck(C.x, C.y, C.z, 10, meta) && !P.HigherLiquidCheck(C.x, C.y, C.z, 11, meta))
                {
                    if ((meta & 0x7) >= 0x6)
                        P.AddUpdate(C.x, C.y, C.z, 0, 0);
                    else
                    {
                        P.AddUpdate(C.x, C.y, C.z, 10, (byte)Math.Min(meta + 2, 0x6));
                        if (!P.AdjacentLiquidCheck(C.x, C.y, C.z, 10) && !P.AdjacentLiquidCheck(C.x, C.y, C.z, 11)) { C.time = 0; return false; }
                    }
                }
                else if ((meta & 0x8) != 0)
                {
                    if (!P.LavaFlowCheck(C.x, C.y - 1, C.z)) { meta = 0; goto flowOut; }
                    P.LavaFlow(C.x, C.y - 1, C.z, 0x8);
                }
                else if ((meta & 0x7) < 0x6)
                {
                    goto flowOut;
                }
                else
                {
                    P.LavaFlow(C.x, C.y - 1, C.z, 0x8);
                }
                return true;

                flowOut:
                if (P.LavaFlowCheck(C.x, C.y - 1, C.z))
                {
                    P.LavaFlow(C.x, C.y - 1, C.z, 0x8);
                    if (!P.AdjacentLiquidCheck(C.x, C.y, C.z, 10) && !P.AdjacentLiquidCheck(C.x, C.y, C.z, 11))
                    {
                        P.LavaFlow(C.x + 1, C.y, C.z, 0x6);
                        P.LavaFlow(C.x - 1, C.y, C.z, 0x6);
                        P.LavaFlow(C.x, C.y, C.z + 1, 0x6);
                        P.LavaFlow(C.x, C.y, C.z - 1, 0x6);
                    }
                }
                else
                {
                    if (P.AdjacentLiquidCheck(C.x, C.y, C.z, 8) || P.AdjacentLiquidCheck(C.x, C.y, C.z, 9))
                    {
                        if ((meta & 0x7) == 0)
                            P.AddUpdate(C.x, C.y, C.z, 49, 0);
                        else
                            P.AddUpdate(C.x, C.y, C.z, 4, 0);
                        Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 1004, P.w);
                        //Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 2000, 4, w);
                    }
                    else
                    {
                        meta += 2;
                        P.LavaFlow(C.x + 1, C.y, C.z, meta);
                        P.LavaFlow(C.x - 1, C.y, C.z, meta);
                        P.LavaFlow(C.x, C.y, C.z + 1, meta);
                        P.LavaFlow(C.x, C.y, C.z - 1, meta);

                        if (P.w.GetBlock(C.x, C.y - 1, C.z) == 8 || P.w.GetBlock(C.x, C.y - 1, C.z) == 9)
                        {
                            P.AddUpdate(C.x, C.y - 1, C.z, 4, 0);
                            Player.GlobalSoundEffect(C.x, (byte)C.y, C.z, 1004, P.w);
                        }
                    }
                }
                return true;
            }

            public static bool SpongeSoak(Physics P, Check C)
            {
                P.SpongePlaced(C.x, C.y, C.z);
                return true;
            }
        }
    }
}
