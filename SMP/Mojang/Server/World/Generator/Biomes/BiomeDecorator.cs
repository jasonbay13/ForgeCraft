using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class BiomeDecorator
    {
        public BiomeDecorator(BiomeGenBase biomegenbase)
        {
            field_35270_a = new WorldGenClay(4);
            field_35268_b = new WorldGenSand(7, (byte)Blocks.Sand);
            field_35269_c = new WorldGenSand(6, (byte)Blocks.Gravel);
            field_35266_d = new WorldGenMinable((byte)Blocks.Dirt, 32);
            field_35267_e = new WorldGenMinable((byte)Blocks.Gravel, 32);
            field_35264_f = new WorldGenMinable((byte)Blocks.CoalOre, 16);
            field_35265_g = new WorldGenMinable((byte)Blocks.IronOre, 8);
            field_35277_h = new WorldGenMinable((byte)Blocks.GoldOre, 8);
            field_35278_i = new WorldGenMinable((byte)Blocks.RedStoneOre, 7);
            field_35275_j = new WorldGenMinable((byte)Blocks.DiamondOre, 7);
            field_35276_k = new WorldGenMinable((byte)Blocks.LapisOre, 6);
            field_35273_l = new WorldGenFlowers((byte)Blocks.FlowerDandelion);
            field_35274_m = new WorldGenFlowers((byte)Blocks.FlowerRose);
            field_35271_n = new WorldGenFlowers((byte)Blocks.MushroomBrown);
            field_35272_o = new WorldGenFlowers((byte)Blocks.MushroomRed);
            field_35286_p = new WorldGenReed();
            field_35285_q = new WorldGenCactus();
            field_35284_r = 0;
            field_35283_s = 2;
            field_35282_t = 1;
            field_35281_u = 0;
            field_35280_v = 0;
            field_35279_w = 0;
            field_35289_x = 0;
            field_35288_y = 1;
            field_35287_z = 3;
            field_35261_A = 1;
            field_35260_F = biomegenbase;
        }

        public void func_35255_a(World world, java.util.Random random, int i, int j)
        {
            if (field_35262_B != null)
            {
                throw new Exception("Already decorating!!");
            }
            else
            {
                field_35262_B = world;
                field_35263_C = random;
                field_35258_D = i;
                field_35259_E = j;
                func_35256_b();
                field_35262_B = null;
                field_35263_C = null;
                return;
            }
        }

        private void func_35256_b()
        {
            func_35253_a();
            for (int i = 0; i < field_35287_z; i++)
            {
                int i1 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int i5 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35268_b.generate(field_35262_B, field_35263_C, i1, field_35262_B.FindTopSolidBlock(i1, i5), i5);
            }

            for (int j = 0; j < field_35261_A; j++)
            {
                int j1 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int j5 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35270_a.generate(field_35262_B, field_35263_C, j1, field_35262_B.FindTopSolidBlock(j1, j5), j5);
            }

            for (int k = 0; k < field_35288_y; k++)
            {
                int k1 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k5 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35268_b.generate(field_35262_B, field_35263_C, k1, field_35262_B.FindTopSolidBlock(k1, k5), k5);
            }

            int l = field_35284_r;
            if (field_35263_C.nextInt(10) == 0)
            {
                l++;
            }
            for (int l1 = 0; l1 < l; l1++)
            {
                int l5 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k9 = field_35259_E + field_35263_C.nextInt(16) + 8;
                WorldGenerator worldgenerator = field_35260_F.getRandomWorldGenForTrees(field_35263_C);
                worldgenerator.func_420_a(1.0D, 1.0D, 1.0D);
                worldgenerator.generate(field_35262_B, field_35263_C, l5, field_35262_B.GetHeightValue(l5, k9), k9);
            }

            for (int i2 = 0; i2 < field_35283_s; i2++)
            {
                int i6 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int l9 = field_35263_C.nextInt(128);
                int j13 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35273_l.generate(field_35262_B, field_35263_C, i6, l9, j13);
                if (field_35263_C.nextInt(4) == 0)
                {
                    int j6 = field_35258_D + field_35263_C.nextInt(16) + 8;
                    int i10 = field_35263_C.nextInt(128);
                    int k13 = field_35259_E + field_35263_C.nextInt(16) + 8;
                    field_35274_m.generate(field_35262_B, field_35263_C, j6, i10, k13);
                }
            }

            for (int j2 = 0; j2 < field_35282_t; j2++)
            {
                int k6 = 1;
                int j10 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int l13 = field_35263_C.nextInt(128);
                int i16 = field_35259_E + field_35263_C.nextInt(16) + 8;
                (new WorldGenTallGrass((byte)Blocks.TallGrass, k6)).generate(field_35262_B, field_35263_C, j10, l13, i16);
            }
            
            for (int k2 = 0; k2 < field_35281_u; k2++)
            {
                int l6 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k10 = field_35263_C.nextInt(128);
                int i14 = field_35259_E + field_35263_C.nextInt(16) + 8;
                (new WorldGenDeadBush((byte)Blocks.DeadShrubs)).generate(field_35262_B, field_35263_C, l6, k10, i14);
            }

            for (int l2 = 0; l2 < field_35280_v; l2++)
            {
                if (field_35263_C.nextInt(4) == 0)
                {
                    int i7 = field_35258_D + field_35263_C.nextInt(16) + 8;
                    int l10 = field_35259_E + field_35263_C.nextInt(16) + 8;
                    int j14 = field_35262_B.GetHeightValue(i7, l10);
                    field_35271_n.generate(field_35262_B, field_35263_C, i7, j14, l10);
                }
                if (field_35263_C.nextInt(8) == 0)
                {
                    int j7 = field_35258_D + field_35263_C.nextInt(16) + 8;
                    int i11 = field_35259_E + field_35263_C.nextInt(16) + 8;
                    int k14 = field_35263_C.nextInt(128);
                    field_35272_o.generate(field_35262_B, field_35263_C, j7, k14, i11);
                }
            }

            if (field_35263_C.nextInt(4) == 0)
            {
                int i3 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k7 = field_35263_C.nextInt(128);
                int j11 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35271_n.generate(field_35262_B, field_35263_C, i3, k7, j11);
            }
            if (field_35263_C.nextInt(8) == 0)
            {
                int j3 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int l7 = field_35263_C.nextInt(128);
                int k11 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35272_o.generate(field_35262_B, field_35263_C, j3, l7, k11);
            }
            for (int k3 = 0; k3 < field_35279_w; k3++)
            {
                int i8 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int l11 = field_35259_E + field_35263_C.nextInt(16) + 8;
                int l14 = field_35263_C.nextInt(128);
                field_35286_p.generate(field_35262_B, field_35263_C, i8, l14, l11);
            }

            for (int l3 = 0; l3 < 10; l3++)
            {
                int j8 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int i12 = field_35263_C.nextInt(128);
                int i15 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35286_p.generate(field_35262_B, field_35263_C, j8, i12, i15);
            }

            if (field_35263_C.nextInt(32) == 0)
            {
                int i4 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k8 = field_35263_C.nextInt(128);
                int j12 = field_35259_E + field_35263_C.nextInt(16) + 8;
                (new WorldGenPumpkin()).generate(field_35262_B, field_35263_C, i4, k8, j12);
            }
            for (int j4 = 0; j4 < field_35289_x; j4++)
            {
                int l8 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int k12 = field_35263_C.nextInt(128);
                int j15 = field_35259_E + field_35263_C.nextInt(16) + 8;
                field_35285_q.generate(field_35262_B, field_35263_C, l8, k12, j15);
            }

            for (int k4 = 0; k4 < 50; k4++)
            {
                int i9 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int l12 = field_35263_C.nextInt(field_35263_C.nextInt(128 - 8) + 8);
                int k15 = field_35259_E + field_35263_C.nextInt(16) + 8;
                (new WorldGenLiquids((byte)Blocks.AWater)).generate(field_35262_B, field_35263_C, i9, l12, k15);
            }

            for (int l4 = 0; l4 < 20; l4++)
            {
                int j9 = field_35258_D + field_35263_C.nextInt(16) + 8;
                int i13 = field_35263_C.nextInt(field_35263_C.nextInt(field_35263_C.nextInt(128 - 16) + 8) + 8);
                int l15 = field_35259_E + field_35263_C.nextInt(16) + 8;
                (new WorldGenLiquids((byte)Blocks.ALava)).generate(field_35262_B, field_35263_C, j9, i13, l15);
            }

        }

        protected void func_35257_a(int i, WorldGenerator worldgenerator, int j, int k)
        {
            for (int l = 0; l < i; l++)
            {
                int i1 = field_35258_D + field_35263_C.nextInt(16);
                int j1 = field_35263_C.nextInt(k - j) + j;
                int k1 = field_35259_E + field_35263_C.nextInt(16);
                worldgenerator.generate(field_35262_B, field_35263_C, i1, j1, k1);
            }

        }

        protected void func_35254_b(int i, WorldGenerator worldgenerator, int j, int k)
        {
            for (int l = 0; l < i; l++)
            {
                int i1 = field_35258_D + field_35263_C.nextInt(16);
                int j1 = field_35263_C.nextInt(k) + field_35263_C.nextInt(k) + (j - k);
                int k1 = field_35259_E + field_35263_C.nextInt(16);
                worldgenerator.generate(field_35262_B, field_35263_C, i1, j1, k1);
            }

        }

        protected void func_35253_a()
        {
            func_35257_a(20, field_35266_d, 0, 128);
            func_35257_a(10, field_35267_e, 0, 128);
            func_35257_a(20, field_35264_f, 0, 128);
            func_35257_a(20, field_35265_g, 0, 128 / 2);
            func_35257_a(2, field_35277_h, 0, 128 / 4);
            func_35257_a(8, field_35278_i, 0, 128 / 8);
            func_35257_a(1, field_35275_j, 0, 128 / 8);
            func_35254_b(1, field_35276_k, 128 / 8, 128 / 8);
        }

        private World field_35262_B;
        private java.util.Random field_35263_C;
        private int field_35258_D;
        private int field_35259_E;
        private BiomeGenBase field_35260_F;
        protected WorldGenerator field_35270_a;
        protected WorldGenerator field_35268_b;
        protected WorldGenerator field_35269_c;
        protected WorldGenerator field_35266_d;
        protected WorldGenerator field_35267_e;
        protected WorldGenerator field_35264_f;
        protected WorldGenerator field_35265_g;
        protected WorldGenerator field_35277_h;
        protected WorldGenerator field_35278_i;
        protected WorldGenerator field_35275_j;
        protected WorldGenerator field_35276_k;
        protected WorldGenerator field_35273_l;
        protected WorldGenerator field_35274_m;
        protected WorldGenerator field_35271_n;
        protected WorldGenerator field_35272_o;
        protected WorldGenerator field_35286_p;
        protected WorldGenerator field_35285_q;
        public int field_35284_r;
        public int field_35283_s;
        public int field_35282_t;
        public int field_35281_u;
        public int field_35280_v;
        public int field_35279_w;
        public int field_35289_x;
        public int field_35288_y;
        public int field_35287_z;
        public int field_35261_A;
    }
}
