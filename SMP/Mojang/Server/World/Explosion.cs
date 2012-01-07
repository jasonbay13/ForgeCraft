using System;
using System.Collections.Generic;
using System.Linq;
using SMP.ENTITY;
using SMP.PLAYER;

namespace SMP
{
    public class Explosion
    {
        public Explosion(World world, /*Entity entity,*/ double d, double d1, double d2, float f)
        {
            isFlaming = false;
            ExplosionRNG = new Random();
            destroyedBlockPositions = new HashSet<Point3>();
            worldObj = world;
            //exploder = entity; // Not needed right now
            explosionSize = f;
            explosionX = d;
            explosionY = d1;
            explosionZ = d2;
        }

        public void DoExplosionA()
        {
            float f = explosionSize;
            int i = 16;
            for(int j = 0; j < i; j++)
            {
                for(int l = 0; l < i; l++)
                {
                    for(int j1 = 0; j1 < i; j1++)
                    {
                        if(j != 0 && j != i - 1 && l != 0 && l != i - 1 && j1 != 0 && j1 != i - 1)
                        {
                            continue;
                        }
                        double d = ((float)j / ((float)i - 1.0F)) * 2.0F - 1.0F;
                        double d1 = ((float)l / ((float)i - 1.0F)) * 2.0F - 1.0F;
                        double d2 = ((float)j1 / ((float)i - 1.0F)) * 2.0F - 1.0F;
                        double d3 = java.lang.Math.sqrt(d * d + d1 * d1 + d2 * d2);
                        d /= d3;
                        d1 /= d3;
                        d2 /= d3;
                        float f1 = explosionSize * (0.7F + (float)Entity.random.NextDouble() * 0.6F);
                        double d5 = explosionX;
                        double d7 = explosionY;
                        double d9 = explosionZ;
                        float f2 = 0.3F;
                        do
                        {
                            if(f1 <= 0.0F)
                            {
                                break;
                            }
                            int j4 = MathHelper.floor_double(d5);
                            int k4 = MathHelper.floor_double(d7);
                            int l4 = MathHelper.floor_double(d9);
                            int i5 = worldObj.GetBlock(j4, k4, l4);
                            if(i5 > 0)
                            {
                                f1 -= ((BlockData.GetExplosionResistance((byte)i5) + 0.3F) / 5F) * f2;
                            }
                            if(f1 > 0.0F)
                            {
                                destroyedBlockPositions.Add(new Point3(j4, k4, l4));
                            }
                            d5 += d * (double)f2;
                            d7 += d1 * (double)f2;
                            d9 += d2 * (double)f2;
                            f1 -= f2 * 0.75F;
                        } while(true);
                    }

                }

            }

            // Damage code...
            /*explosionSize *= 2.0F;
            int k = MathHelper.floor_double(explosionX - (double)explosionSize - 1.0D);
            int i1 = MathHelper.floor_double(explosionX + (double)explosionSize + 1.0D);
            int k1 = MathHelper.floor_double(explosionY - (double)explosionSize - 1.0D);
            int l1 = MathHelper.floor_double(explosionY + (double)explosionSize + 1.0D);
            int i2 = MathHelper.floor_double(explosionZ - (double)explosionSize - 1.0D);
            int j2 = MathHelper.floor_double(explosionZ + (double)explosionSize + 1.0D);
            List list = worldObj.getEntitiesWithinAABBExcludingEntity(exploder, AxisAlignedBB.getBoundingBoxFromPool(k, k1, i2, i1, l1, j2));
            Vec3D vec3d = Vec3D.createVector(explosionX, explosionY, explosionZ);
            for(int k2 = 0; k2 < list.size(); k2++)
            {
                Entity entity = (Entity)list.get(k2);
                double d4 = entity.getDistance(explosionX, explosionY, explosionZ) / (double)explosionSize;
                if(d4 <= 1.0D)
                {
                    double d6 = entity.pos.x - explosionX;
                    double d8 = entity.pos.y - explosionY;
                    double d10 = entity.pos.z - explosionZ;
                    double d11 = MathHelper.sqrt_double(d6 * d6 + d8 * d8 + d10 * d10);
                    d6 /= d11;
                    d8 /= d11;
                    d10 /= d11;
                    double d12 = worldObj.func_494_a(vec3d, entity.boundingBox);
                    double d13 = (1.0D - d4) * d12;
                    entity.attackEntityFrom(DamageSource.field_35097_k, (int)(((d13 * d13 + d13) / 2D) * 8D * (double)explosionSize + 1.0D));
                    double d14 = d13;
                    entity.motionX += d6 * d14;
                    entity.motionY += d8 * d14;
                    entity.motionZ += d10 * d14;
                }
            }
            explosionSize = f;*/
        }

        public void DoExplosionB(/*bool flag*/)
        {
            Player.players.ForEach(delegate(Player pl)
            {
                if (pl.MapLoaded && pl.level == worldObj && pl.VisibleChunks.Contains(Chunk.GetChunk((int)explosionX >> 4, (int)explosionZ >> 4, pl.level).point))
                    pl.SendExplosion((int)explosionX, (int)explosionY, (int)explosionZ, explosionSize, destroyedBlockPositions.ToArray());
            });

            List<Point3> arraylist = new List<Point3>();
            arraylist.AddRange(destroyedBlockPositions);
            for (int i = arraylist.Count - 1; i >= 0; i--)
            {
                Point3 chunkposition = arraylist[i];
                int j = (int)chunkposition.x;
                int k = (int)chunkposition.y;
                int l = (int)chunkposition.z;
                int i1 = worldObj.GetBlock(j, k, l);
                // This is particle code, and isn't needed here.
                /*if (flag)
                {
                    double d = (float)j + worldObj.rand.nextFloat();
                    double d1 = (float)k + worldObj.rand.nextFloat();
                    double d2 = (float)l + worldObj.rand.nextFloat();
                    double d3 = d - explosionX;
                    double d4 = d1 - explosionY;
                    double d5 = d2 - explosionZ;
                    double d6 = MathHelper.sqrt_double(d3 * d3 + d4 * d4 + d5 * d5);
                    d3 /= d6;
                    d4 /= d6;
                    d5 /= d6;
                    double d7 = 0.5D / (d6 / (double)explosionSize + 0.10000000000000001D);
                    d7 *= worldObj.rand.nextFloat() * worldObj.rand.nextFloat() + 0.3F;
                    d3 *= d7;
                    d4 *= d7;
                    d5 *= d7;
                    worldObj.spawnParticle("explode", (d + explosionX * 1.0D) / 2D, (d1 + explosionY * 1.0D) / 2D, (d2 + explosionZ * 1.0D) / 2D, d3, d4, d5);
                    worldObj.spawnParticle("smoke", d, d1, d2, d3, d4, d5);
                }*/
                if (i1 > 0)
                {
                    if (ExplosionRNG.Next(10) < 3)
                    {
                        short block = Player.BlockDropSwitch((short)i1);
                        if (block != 0)
                            worldObj.DropItem(j, k, l, block);
                    }
                    worldObj.BlockChange(j, k, l, 0, 0);
                }
            }

            if (isFlaming)
            {
                for (int l2 = arraylist.Count - 1; l2 >= 0; l2--)
                {
                    Point3 chunkposition = arraylist[l2];
                    int i3 = (int)chunkposition.x;
                    int j3 = (int)chunkposition.y;
                    int k3 = (int)chunkposition.z;
                    int l3 = worldObj.GetBlock(i3, j3, k3);
                    int i4 = worldObj.GetBlock(i3, j3 - 1, k3);
                    if (l3 == 0 && i4 != 20 && BlockData.CanPlaceAgainst((byte)i4) && ExplosionRNG.Next(3) == 0)
                    {
                        worldObj.BlockChange(i3, j3, k3, (byte)Blocks.Fire, 0);
                    }
                }
            }
        }

        public bool isFlaming;
        private Random ExplosionRNG;
        private World worldObj;
        public double explosionX;
        public double explosionY;
        public double explosionZ;
        public Entity exploder;
        public float explosionSize;
        public HashSet<Point3> destroyedBlockPositions;
    }
}
