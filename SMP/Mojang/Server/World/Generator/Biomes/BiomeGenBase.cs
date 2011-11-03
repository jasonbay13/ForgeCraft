using System;

namespace SMP
{
    public class BiomeGenBase
    {
        protected BiomeGenBase(int i)
        {
            topBlock = (byte)Blocks.Grass;
            fillerBlock = (byte)Blocks.Dirt;
            field_6161_q = 0x4ee031;
            field_35527_q = 0.1F;
            field_35526_r = 0.3F;
            field_35525_s = 0.5F;
            field_35524_t = 0.5F;
            //spawnableMonsterList = new ArrayList();
            //spawnableCreatureList = new ArrayList();
            //spawnableWaterCreatureList = new ArrayList();
            enableRain = true;
            field_35528_z = new WorldGenTrees();
            field_35515_A = new WorldGenBigTree();
            field_35516_B = new WorldGenForest();
            field_35517_C = new WorldGenSwamp();
            field_35529_y = i;
            field_35521_a[i] = this;
            field_35523_u = func_35514_a();
            /*spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntitySheep.class, 12, 4, 4));
            spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityPig.class, 10, 4, 4));
            spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityChicken.class, 10, 4, 4));
            spawnableCreatureList.add(new SpawnListEntry(net.minecraft.src.EntityCow.class, 8, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntitySpider.class, 10, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntityZombie.class, 10, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntitySkeleton.class, 10, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntityCreeper.class, 10, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntitySlime.class, 10, 4, 4));
            spawnableMonsterList.add(new SpawnListEntry(net.minecraft.src.EntityEnderman.class, 2, 4, 4));
            spawnableWaterCreatureList.add(new SpawnListEntry(net.minecraft.src.EntitySquid.class, 10, 4, 4));*/
        }

        protected BiomeDecorator func_35514_a()
        {
            return new BiomeDecorator(this);
        }

        private BiomeGenBase func_35512_a(float f, float f1)
        {
            field_35525_s = f;
            field_35524_t = f1;
            return this;
        }

        private BiomeGenBase func_35511_b(float f, float f1)
        {
            field_35527_q = f;
            field_35526_r = f1;
            return this;
        }

        private BiomeGenBase setDisableRain()
        {
            enableRain = false;
            return this;
        }

        public virtual WorldGenerator getRandomWorldGenForTrees(java.util.Random random)
        {
            if(random.nextInt(10) == 0)
            {
                return field_35515_A;
            } else
            {
                return field_35528_z;
            }
        }

        protected BiomeGenBase setBiomeName(string s)
        {
            biomeName = s;
            return this;
        }

        protected BiomeGenBase func_4080_a(int i)
        {
            field_6161_q = i;
            return this;
        }

        protected BiomeGenBase setColor(int i)
        {
            color = i;
            return this;
        }

        /*public List getSpawnableList(EnumCreatureType enumcreaturetype)
        {
            if(enumcreaturetype == EnumCreatureType.monster)
            {
                return spawnableMonsterList;
            }
            if(enumcreaturetype == EnumCreatureType.creature)
            {
                return spawnableCreatureList;
            }
            if(enumcreaturetype == EnumCreatureType.waterCreature)
            {
                return spawnableWaterCreatureList;
            } else
            {
                return null;
            }
        }*/

        public bool getEnableSnow()
        {
            return enableSnow;
        }

        public bool canSpawnLightningBolt()
        {
            if(enableSnow)
            {
                return false;
            } else
            {
                return enableRain;
            }
        }

        public float getBiome()
        {
            return 0.1F;
        }

        public int func_35510_e()
        {
            return (int)(field_35524_t * 65536F);
        }

        public int func_35509_f()
        {
            return (int)(field_35525_s * 65536F);
        }

        public void func_35513_a(World world, java.util.Random random, int i, int j)
        {
            field_35523_u.func_35255_a(world, random, i, j);
        }

        public static BiomeGenBase[] field_35521_a = new BiomeGenBase[256];
        public static BiomeGenBase field_35519_b = (new BiomeGenOcean(0)).setColor(112).setBiomeName("Ocean").func_35511_b(-1F, 0.5F);
        public static BiomeGenBase field_35520_c = (new BiomeGenPlains(1)).setColor(0x8db360).setBiomeName("Plains").func_35512_a(0.8F, 0.4F);
        public static BiomeGenBase desert = (new BiomeGenDesert(2)).setColor(0xfa9418).setBiomeName("Desert").setDisableRain().func_35512_a(2.0F, 0.0F).func_35511_b(0.1F, 0.2F);
        public static BiomeGenBase field_35518_e = (new BiomeGenHills(3)).setColor(0x606060).setBiomeName("Extreme Hills").func_35511_b(0.2F, 1.8F).func_35512_a(0.2F, 0.3F);
        public static BiomeGenBase forest = (new BiomeGenForest(4)).setColor(0x56621).setBiomeName("Forest").func_4080_a(0x4eba31).func_35512_a(0.7F, 0.8F);
        public static BiomeGenBase taiga = (new BiomeGenTaiga(5)).setColor(0xb6659).setBiomeName("Taiga").func_4080_a(0x4eba31).func_35512_a(0.3F, 0.8F).func_35511_b(0.1F, 0.4F);
        public static BiomeGenBase swampland = (new BiomeGenSwamp(6)).setColor(0x7f9b2).setBiomeName("Swampland").func_4080_a(0x8baf48).func_35511_b(-0.2F, 0.1F).func_35512_a(0.8F, 0.9F);
        public static BiomeGenBase field_35522_i = (new BiomeGenRiver(7)).setColor(255).setBiomeName("River").func_35511_b(-0.5F, 0.0F);
        public static BiomeGenBase hell = (new BiomeGenHell(8)).setColor(0xff0000).setBiomeName("Hell").setDisableRain();
        public static BiomeGenBase sky = (new BiomeGenSky(9)).setColor(0x8080ff).setBiomeName("Sky").setDisableRain();
        public string biomeName;
        public int color;
        public byte topBlock;
        public byte fillerBlock;
        public int field_6161_q;
        public float field_35527_q;
        public float field_35526_r;
        public float field_35525_s;
        public float field_35524_t;
        public BiomeDecorator field_35523_u;
        //protected List spawnableMonsterList;
        //protected List spawnableCreatureList;
        //protected List spawnableWaterCreatureList;
        private bool enableSnow;
        private bool enableRain;
        public int field_35529_y;
        protected WorldGenTrees field_35528_z;
        protected WorldGenBigTree field_35515_A;
        protected WorldGenForest field_35516_B;
        protected WorldGenSwamp field_35517_C;
    }
}
