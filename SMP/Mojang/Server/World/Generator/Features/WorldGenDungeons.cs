using System;

namespace SMP.Generator
{
    public class WorldGenDungeons : WorldGenerator
    {
        public WorldGenDungeons()
        {
        }

        public override bool generate(World world, java.util.Random random, int i, int j, int k)
        {
            byte byte0 = 3;
            int l = random.nextInt(2) + 2;
            int i1 = random.nextInt(2) + 2;
            int j1 = 0;
            for(int k1 = i - l - 1; k1 <= i + l + 1; k1++)
            {
                for(int j2 = j - 1; j2 <= j + byte0 + 1; j2++)
                {
                    for(int i3 = k - i1 - 1; i3 <= k + i1 + 1; i3++)
                    {
                        byte block = world.GetBlock(k1, j2, i3);
                        if(j2 == j - 1 && !BlockData.IsSolid(block))
                        {
                            return false;
                        }
                        if (j2 == j + byte0 + 1 && !BlockData.IsSolid(block))
                        {
                            return false;
                        }
                        if((k1 == i - l - 1 || k1 == i + l + 1 || i3 == k - i1 - 1 || i3 == k + i1 + 1) && j2 == j && world.IsAirBlock(k1, j2, i3) && world.IsAirBlock(k1, j2 + 1, i3))
                        {
                            j1++;
                        }
                    }

                }

            }

            if(j1 < 1 || j1 > 5)
            {
                return false;
            }
            for(int l1 = i - l - 1; l1 <= i + l + 1; l1++)
            {
                for(int k2 = j + byte0; k2 >= j - 1; k2--)
                {
                    for(int j3 = k - i1 - 1; j3 <= k + i1 + 1; j3++)
                    {
                        if(l1 == i - l - 1 || k2 == j - 1 || j3 == k - i1 - 1 || l1 == i + l + 1 || k2 == j + byte0 + 1 || j3 == k + i1 + 1)
                        {
                            if(k2 >= 0 && !BlockData.IsSolid(world.GetBlock(l1, k2 - 1, j3)))
                            {
                                world.SetBlock(l1, k2, j3, 0);
                                continue;
                            }
                            if (!BlockData.IsSolid(world.GetBlock(l1, k2, j3)))
                            {
                                continue;
                            }
                            if(k2 == j - 1 && random.nextInt(4) != 0)
                            {
                                world.SetBlock(l1, k2, j3, (byte)Blocks.MossStone);
                            } else
                            {
                                world.SetBlock(l1, k2, j3, (byte)Blocks.CobbleStone);
                            }
                        } else
                        {
                            world.SetBlock(l1, k2, j3, 0);
                        }
                    }

                }

            }

            for(int i2 = 0; i2 < 2; i2++)
            {
                label0:
                for(int l2 = 0; l2 < 3; l2++)
                {
                    int k3 = (i + random.nextInt(l * 2 + 1)) - l;
                    int l3 = j;
                    int i4 = (k + random.nextInt(i1 * 2 + 1)) - i1;
                    if(!world.IsAirBlock(k3, l3, i4))
                    {
                        continue;
                    }
                    int j4 = 0;
                    if (BlockData.IsSolid(world.GetBlock(k3 - 1, l3, i4)))
                    {
                        j4++;
                    }
                    if (BlockData.IsSolid(world.GetBlock(k3 + 1, l3, i4)))
                    {
                        j4++;
                    }
                    if(BlockData.IsSolid(world.GetBlock(k3, l3, i4 - 1)))
                    {
                        j4++;
                    }
                    if(BlockData.IsSolid(world.GetBlock(k3, l3, i4 + 1)))
                    {
                        j4++;
                    }
                    if(j4 != 1)
                    {
                        continue;
                    }
                    world.SetBlock(k3, l3, i4, (byte)Blocks.Chest);
                    // TODO: Put items in chests once those work!
                    /*TileEntityChest tileentitychest = (TileEntityChest)world.getBlockTileEntity(k3, l3, i4);
                    if(tileentitychest == null)
                    {
                        break;
                    }*/
                    int k4 = 0;
                    bool label0 = false;
                    do
                    {
                        if(k4 >= 8)
                        {
                            label0 = true;
                            break;
                        }
                        /*ItemStack itemstack = pickCheckLootItem(random);
                        if(itemstack != null)
                        {
                            tileentitychest.setInventorySlotContents(random.nextInt(tileentitychest.getSizeInventory()), itemstack);
                        }*/
                        k4++;
                    } while(true);
                    if (label0) break;
                }

            }

            world.SetBlock(i, j, k, (byte)Blocks.MonsterSpawner);
            world.SetExtra(i, j, k, (ushort)pickMobSpawner(random));
            //TileEntityMobSpawner tileentitymobspawner = (TileEntityMobSpawner)world.getBlockTileEntity(i, j, k);
            //tileentitymobspawner.setMobID(pickMobSpawner(random));
            return true;
        }

        // TODO
        /*private ItemStack pickCheckLootItem(java.util.Random random)
        {
            int i = random.nextInt(11);
            if (i == 0)
            {
                return new ItemStack(Item.saddle);
            }
            if (i == 1)
            {
                return new ItemStack(Item.ingotIron, random.nextInt(4) + 1);
            }
            if (i == 2)
            {
                return new ItemStack(Item.bread);
            }
            if (i == 3)
            {
                return new ItemStack(Item.wheat, random.nextInt(4) + 1);
            }
            if (i == 4)
            {
                return new ItemStack(Item.gunpowder, random.nextInt(4) + 1);
            }
            if (i == 5)
            {
                return new ItemStack(Item.silk, random.nextInt(4) + 1);
            }
            if (i == 6)
            {
                return new ItemStack(Item.bucketEmpty);
            }
            if (i == 7 && random.nextInt(100) == 0)
            {
                return new ItemStack(Item.appleGold);
            }
            if (i == 8 && random.nextInt(2) == 0)
            {
                return new ItemStack(Item.redstone, random.nextInt(4) + 1);
            }
            if (i == 9 && random.nextInt(10) == 0)
            {
                return new ItemStack(Item.itemsList[Item.record13.shiftedIndex + random.nextInt(2)]);
            }
            if (i == 10)
            {
                return new ItemStack(Item.dyePowder, 1, 3);
            }
            else
            {
                return null;
            }
        }*/

        private int pickMobSpawner(java.util.Random random)
        {
            int i = random.nextInt(4);
            if (i == 0)
            {
                return (int)MobType.Skeleton;
            }
            if (i == 1)
            {
                return (int)MobType.Zombie;
            }
            if (i == 2)
            {
                return (int)MobType.Zombie;
            }
            if (i == 3)
            {
                return (int)MobType.Spider;
            }
            else
            {
                return 0;
            }
        }
    }
}
