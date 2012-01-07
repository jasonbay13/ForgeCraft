using System;
using System.Collections.Generic;
using SMP.INVENTORY;
using SMP.PLAYER.Crafting;

namespace SMP.Generator
{
    public abstract class StructureComponent
    {
        protected StructureBoundingBox boundingBox;
        protected int coordBaseMode;
        protected int field_35318_i;

        protected StructureComponent(int i)
        {
            field_35318_i = i;
            coordBaseMode = -1;
        }

        public void buildComponent(StructureComponent structurecomponent, List<StructureComponent> list, java.util.Random random)
        {
        }

        public abstract bool addComponentParts(World world, java.util.Random random, StructureBoundingBox structureboundingbox);

        public StructureBoundingBox getStructureBoundingBox()
        {
            return boundingBox;
        }

        public int func_35305_c()
        {
            return field_35318_i;
        }

        public static StructureComponent canFitInside(List<StructureComponent> list, StructureBoundingBox structureboundingbox)
        {
            for(IEnumerator<StructureComponent> iterator = list.GetEnumerator(); iterator.MoveNext();)
            {
                StructureComponent structurecomponent = (StructureComponent)iterator.Current;
                if(structurecomponent.getStructureBoundingBox() != null && structurecomponent.getStructureBoundingBox().canFitInside(structureboundingbox))
                {
                    return structurecomponent;
                }
            }

            return null;
        }

        public ChunkPosition func_40281_b_()
        {
            return new ChunkPosition(boundingBox.func_40623_e(), boundingBox.func_40622_f(), boundingBox.func_40624_g());
        }

        protected bool isLiquidInStructureBoundingBox(World world, StructureBoundingBox structureboundingbox)
        {
            int i = Math.Max(boundingBox.x1 - 1, structureboundingbox.x1);
            int j = Math.Max(boundingBox.y1 - 1, structureboundingbox.y1);
            int k = Math.Max(boundingBox.z1 - 1, structureboundingbox.z1);
            int l = Math.Min(boundingBox.x2 + 1, structureboundingbox.x2);
            int i1 = Math.Min(boundingBox.y2 + 1, structureboundingbox.y2);
            int j1 = Math.Min(boundingBox.z2 + 1, structureboundingbox.z2);
            for(int k1 = i; k1 <= l; k1++)
            {
                for(int j2 = k; j2 <= j1; j2++)
                {
                    int i3 = world.GetBlock(k1, j, j2);
                    if (i3 > 0 && BlockData.IsLiquid((byte)i3))
                    {
                        return true;
                    }
                    i3 = world.GetBlock(k1, i1, j2);
                    if (i3 > 0 && BlockData.IsLiquid((byte)i3))
                    {
                        return true;
                    }
                }

            }

            for(int l1 = i; l1 <= l; l1++)
            {
                for(int k2 = j; k2 <= i1; k2++)
                {
                    int j3 = world.GetBlock(l1, k2, k);
                    if (j3 > 0 && BlockData.IsLiquid((byte)j3))
                    {
                        return true;
                    }
                    j3 = world.GetBlock(l1, k2, j1);
                    if (j3 > 0 && BlockData.IsLiquid((byte)j3))
                    {
                        return true;
                    }
                }

            }

            for(int i2 = k; i2 <= j1; i2++)
            {
                for(int l2 = j; l2 <= i1; l2++)
                {
                    int k3 = world.GetBlock(i, l2, i2);
                    if (k3 > 0 && BlockData.IsLiquid((byte)k3))
                    {
                        return true;
                    }
                    k3 = world.GetBlock(l, l2, i2);
                    if (k3 > 0 && BlockData.IsLiquid((byte)k3))
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        protected int func_35306_a(int i, int j)
        {
            switch(coordBaseMode)
            {
            case 0: // '\0'
            case 2: // '\002'
                return boundingBox.x1 + i;

            case 1: // '\001'
                return boundingBox.x2 - j;

            case 3: // '\003'
                return boundingBox.x1 + j;
            }
            return i;
        }

        protected int func_35300_a(int i)
        {
            if(coordBaseMode == -1)
            {
                return i;
            } else
            {
                return i + boundingBox.y1;
            }
        }

        protected int func_35296_b(int i, int j)
        {
            switch(coordBaseMode)
            {
            case 2: // '\002'
                return boundingBox.z2 - j;

            case 0: // '\0'
                return boundingBox.z1 + j;

            case 1: // '\001'
            case 3: // '\003'
                return boundingBox.z1 + i;
            }
            return j;
        }

        protected int func_35301_c(int i, int j)
        {
            if(i == (byte)Blocks.Rails)
            {
                if(coordBaseMode == 1 || coordBaseMode == 3)
                {
                    return j != 1 ? 1 : 0;
                }
            } else
            if (i == (byte)Blocks.DoorWooden || i == (byte)Blocks.DoorIron)
            {
                if(coordBaseMode == 0)
                {
                    if(j == 0)
                    {
                        return 2;
                    }
                    if(j == 2)
                    {
                        return 0;
                    }
                } else
                {
                    if(coordBaseMode == 1)
                    {
                        return j + 1 & 3;
                    }
                    if(coordBaseMode == 3)
                    {
                        return j + 3 & 3;
                    }
                }
            } else
            if (i == (byte)Blocks.StairsCobblestone || i == (byte)Blocks.StairsWooden || i == (byte)Blocks.StairsNetherBrick || i == (byte)Blocks.StairsStoneBrick)
            {
                if(coordBaseMode == 0)
                {
                    if(j == 2)
                    {
                        return 3;
                    }
                    if(j == 3)
                    {
                        return 2;
                    }
                } else
                if(coordBaseMode == 1)
                {
                    if(j == 0)
                    {
                        return 2;
                    }
                    if(j == 1)
                    {
                        return 3;
                    }
                    if(j == 2)
                    {
                        return 0;
                    }
                    if(j == 3)
                    {
                        return 1;
                    }
                } else
                if(coordBaseMode == 3)
                {
                    if(j == 0)
                    {
                        return 2;
                    }
                    if(j == 1)
                    {
                        return 3;
                    }
                    if(j == 2)
                    {
                        return 1;
                    }
                    if(j == 3)
                    {
                        return 0;
                    }
                }
            } else
            if(i == (byte)Blocks.Ladder)
            {
                if(coordBaseMode == 0)
                {
                    if(j == 2)
                    {
                        return 3;
                    }
                    if(j == 3)
                    {
                        return 2;
                    }
                } else
                if(coordBaseMode == 1)
                {
                    if(j == 2)
                    {
                        return 4;
                    }
                    if(j == 3)
                    {
                        return 5;
                    }
                    if(j == 4)
                    {
                        return 2;
                    }
                    if(j == 5)
                    {
                        return 3;
                    }
                } else
                if(coordBaseMode == 3)
                {
                    if(j == 2)
                    {
                        return 5;
                    }
                    if(j == 3)
                    {
                        return 4;
                    }
                    if(j == 4)
                    {
                        return 2;
                    }
                    if(j == 5)
                    {
                        return 3;
                    }
                }
            } else
            if(i == (byte)Blocks.ButtonStone)
            {
                if(coordBaseMode == 0)
                {
                    if(j == 3)
                    {
                        return 4;
                    }
                    if(j == 4)
                    {
                        return 3;
                    }
                } else
                if(coordBaseMode == 1)
                {
                    if(j == 3)
                    {
                        return 1;
                    }
                    if(j == 4)
                    {
                        return 2;
                    }
                    if(j == 2)
                    {
                        return 3;
                    }
                    if(j == 1)
                    {
                        return 4;
                    }
                } else
                if(coordBaseMode == 3)
                {
                    if(j == 3)
                    {
                        return 2;
                    }
                    if(j == 4)
                    {
                        return 1;
                    }
                    if(j == 2)
                    {
                        return 3;
                    }
                    if(j == 1)
                    {
                        return 4;
                    }
                }
            }
            return j;
        }

        protected void func_35309_a(World world, int i, int j, int k, int l, int i1, StructureBoundingBox structureboundingbox)
        {
            int j1 = func_35306_a(k, i1);
            int k1 = func_35300_a(l);
            int l1 = func_35296_b(k, i1);
            if(!structureboundingbox.isInBbVolume(j1, k1, l1))
            {
                return;
            } else
            {
                world.SetBlock(j1, k1, l1, (byte)i, (byte)j);
                return;
            }
        }

        protected int func_35297_a(World world, int i, int j, int k, StructureBoundingBox structureboundingbox)
        {
            int l = func_35306_a(i, k);
            int i1 = func_35300_a(j);
            int j1 = func_35296_b(i, k);
            if(!structureboundingbox.isInBbVolume(l, i1, j1))
            {
                return 0;
            } else
            {
                return world.GetBlock(l, i1, j1);
            }
        }

        protected void fillWithBlocks(World world, StructureBoundingBox structureboundingbox, int i, int j, int k, int l, int i1, 
                int j1, int k1, int l1, bool flag)
        {
            for(int i2 = j; i2 <= i1; i2++)
            {
                for(int j2 = i; j2 <= l; j2++)
                {
                    for(int k2 = k; k2 <= j1; k2++)
                    {
                        if(flag && func_35297_a(world, j2, i2, k2, structureboundingbox) == 0)
                        {
                            continue;
                        }
                        if(i2 == j || i2 == i1 || j2 == i || j2 == l || k2 == k || k2 == j1)
                        {
                            func_35309_a(world, k1, 0, j2, i2, k2, structureboundingbox);
                        } else
                        {
                            func_35309_a(world, l1, 0, j2, i2, k2, structureboundingbox);
                        }
                    }

                }

            }

        }

        protected void func_35307_a(World world, StructureBoundingBox structureboundingbox, int i, int j, int k, int l, int i1, int j1, bool flag, java.util.Random random, StructurePieceBlockSelector structurepieceblockselector)
        {
            for(int k1 = j; k1 <= i1; k1++)
            {
                for(int l1 = i; l1 <= l; l1++)
                {
                    for(int i2 = k; i2 <= j1; i2++)
                    {
                        if(!flag || func_35297_a(world, l1, k1, i2, structureboundingbox) != 0)
                        {
                            structurepieceblockselector.selectBlocks(random, l1, k1, i2, k1 == j || k1 == i1 || l1 == i || l1 == l || i2 == k || i2 == j1);
                            func_35309_a(world, structurepieceblockselector.func_35566_a(), structurepieceblockselector.func_35567_b(), l1, k1, i2, structureboundingbox);
                        }
                    }

                }

            }

        }

        protected void randomlyFillWithBlocks(World world, StructureBoundingBox structureboundingbox, java.util.Random random, float f, int i, int j, int k, 
                int l, int i1, int j1, int k1, int l1, bool flag)
        {
            for(int i2 = j; i2 <= i1; i2++)
            {
                for(int j2 = i; j2 <= l; j2++)
                {
                    for(int k2 = k; k2 <= j1; k2++)
                    {
                        if(random.nextFloat() > f || flag && func_35297_a(world, j2, i2, k2, structureboundingbox) == 0)
                        {
                            continue;
                        }
                        if(i2 == j || i2 == i1 || j2 == i || j2 == l || k2 == k || k2 == j1)
                        {
                            func_35309_a(world, k1, 0, j2, i2, k2, structureboundingbox);
                        } else
                        {
                            func_35309_a(world, l1, 0, j2, i2, k2, structureboundingbox);
                        }
                    }

                }

            }

        }

        protected void randomlyPlaceBlock(World world, StructureBoundingBox structureboundingbox, java.util.Random random, float f, int i, int j, int k, 
                int l, int i1)
        {
            if(random.nextFloat() < f)
            {
                func_35309_a(world, l, i1, i, j, k, structureboundingbox);
            }
        }

        protected void func_35304_a(World world, StructureBoundingBox structureboundingbox, int i, int j, int k, int l, int i1, int j1, int k1, bool flag)
        {
            float f = (l - i) + 1;
            float f1 = (i1 - j) + 1;
            float f2 = (j1 - k) + 1;
            float f3 = (float)i + f / 2.0F;
            float f4 = (float)k + f2 / 2.0F;
            for(int l1 = j; l1 <= i1; l1++)
            {
                float f5 = (float)(l1 - j) / f1;
                for(int i2 = i; i2 <= l; i2++)
                {
                    float f6 = ((float)i2 - f3) / (f * 0.5F);
                    for(int j2 = k; j2 <= j1; j2++)
                    {
                        float f7 = ((float)j2 - f4) / (f2 * 0.5F);
                        if(flag && func_35297_a(world, i2, l1, j2, structureboundingbox) == 0)
                        {
                            continue;
                        }
                        float f8 = f6 * f6 + f5 * f5 + f7 * f7;
                        if(f8 <= 1.05F)
                        {
                            func_35309_a(world, k1, 0, i2, l1, j2, structureboundingbox);
                        }
                    }

                }

            }

        }

        protected void func_35314_b(World world, int i, int j, int k, StructureBoundingBox structureboundingbox)
        {
            int l = func_35306_a(i, k);
            int i1 = func_35300_a(j);
            int j1 = func_35296_b(i, k);
            if(!structureboundingbox.isInBbVolume(l, i1, j1))
            {
                return;
            }
            for(; !world.IsAirBlock(l, i1, j1) && i1 < world.worldYMask; i1++)
            {
                world.SetBlock(l, i1, j1, 0, 0);
            }

        }

        protected void func_35303_b(World world, int i, int j, int k, int l, int i1, StructureBoundingBox structureboundingbox)
        {
            int j1 = func_35306_a(k, i1);
            int k1 = func_35300_a(l);
            int l1 = func_35296_b(k, i1);
            if(!structureboundingbox.isInBbVolume(j1, k1, l1))
            {
                return;
            }
            for(; (world.IsAirBlock(j1, k1, l1) || BlockData.IsLiquid(world.GetBlock(j1, k1, l1))) && k1 > 1; k1--)
            {
                world.SetBlock(j1, k1, l1, (byte)i, (byte)j);
            }

        }

        protected void func_35299_a(World world, StructureBoundingBox structureboundingbox, java.util.Random random, int i, int j, int k, StructurePieceTreasure[] astructurepiecetreasure, 
                int l)
        {
            int i1 = func_35306_a(i, k);
            int j1 = func_35300_a(j);
            int k1 = func_35296_b(i, k);
            if(structureboundingbox.isInBbVolume(i1, j1, k1) && world.GetBlock(i1, j1, k1) != (byte)Blocks.Chest)
            {
                world.SetBlock(i1, j1, k1, (byte)Blocks.Chest);
                Container tileentitychest = world.GetBlockContainer(i1, j1, k1);
                if (tileentitychest != null && tileentitychest is ContainerChest)
                {
                    func_35311_a(random, astructurepiecetreasure, (ContainerChest)tileentitychest, l);
                }
            }
        }

        private static void func_35311_a(java.util.Random random, StructurePieceTreasure[] astructurepiecetreasure, ContainerChest tileentitychest, int i)
        {
            for(int j = 0; j < i; j++)
            {
                StructurePieceTreasure structurepiecetreasure = (StructurePieceTreasure)WeightedRandom.func_35691_a(random, astructurepiecetreasure);
                int k = structurepiecetreasure.field_35488_c + random.nextInt((structurepiecetreasure.field_35486_e - structurepiecetreasure.field_35488_c) + 1);
                if(Inventory.isStackable((short)structurepiecetreasure.field_35489_a) >= k)
                {
                    tileentitychest.SetSlot(random.nextInt(tileentitychest.Size), new Item((short)structurepiecetreasure.field_35489_a, (byte)k, (short)structurepiecetreasure.field_35487_b));
                    continue;
                }
                for(int l = 0; l < k; l++)
                {
                    tileentitychest.SetSlot(random.nextInt(tileentitychest.Size), new Item((short)structurepiecetreasure.field_35489_a, 1, (short)structurepiecetreasure.field_35487_b));
                }

            }

        }

        protected void func_35298_a(World world, StructureBoundingBox structureboundingbox, java.util.Random random, int i, int j, int k, int l)
        {
            int i1 = func_35306_a(i, k);
            int j1 = func_35300_a(j);
            int k1 = func_35296_b(i, k);
            if(structureboundingbox.isInBbVolume(i1, j1, k1))
            {
                BlockHelper.PlaceDoor(world, i1, j1, k1, l, (byte)Blocks.DoorWooden);
            }
        }
    }
}
