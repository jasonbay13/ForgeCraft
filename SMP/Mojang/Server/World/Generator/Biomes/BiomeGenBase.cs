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
            minHeight = 0.1F;
            maxHeight = 0.3F;
            temperature = 0.5F;
            rainfall = 0.5F;
            field_40461_A = 0xffffff;
            //spawnableMonsterList = new ArrayList();
            //spawnableCreatureList = new ArrayList();
            //spawnableWaterCreatureList = new ArrayList();
            enableRain = true;
            treeGenerator = new WorldGenTrees(false);
            bigTreeGenerator = new WorldGenBigTree(false);
            forestGenerator = new WorldGenForest(false);
            swampTreeGenerator = new WorldGenSwamp();
            biomeID = i;
            biomeList[i] = this;
            decorator = createBiomeDecorator();
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

        protected BiomeDecorator createBiomeDecorator()
        {
            return new BiomeDecorator(this);
        }

        private BiomeGenBase setTemperatureRainfall(float f, float f1)
        {
            temperature = f;
            rainfall = f1;
            return this;
        }

        private BiomeGenBase setMinMaxHeight(float f, float f1)
        {
            minHeight = f;
            maxHeight = f1;
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
                return bigTreeGenerator;
            } else
            {
                return treeGenerator;
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

        public BiomeGenBase setEnableSnow()
        {
            enableSnow = true;
            return this;
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
            return (int)(rainfall * 65536F);
        }

        public int func_35509_f()
        {
            return (int)(temperature * 65536F);
        }

        public void func_35513_a(World world, java.util.Random random, int i, int j)
        {
            decorator.Decorates(world, random, i, j);
        }

        public static readonly BiomeGenBase[] biomeList = new BiomeGenBase[256];
        public static readonly BiomeGenBase ocean = (new BiomeGenOcean(0)).setColor(112).setBiomeName("Ocean").setMinMaxHeight(-1F, 0.5F);
        public static readonly BiomeGenBase plains = (new BiomeGenPlains(1)).setColor(0x8db360).setBiomeName("Plains").setTemperatureRainfall(0.8F, 0.4F);
        public static readonly BiomeGenBase desert = (new BiomeGenDesert(2)).setColor(0xfa9418).setBiomeName("Desert").setDisableRain().setTemperatureRainfall(2.0F, 0.0F).setMinMaxHeight(0.1F, 0.2F);
        public static readonly BiomeGenBase extremeHills = (new BiomeGenHills(3)).setColor(0x606060).setBiomeName("Extreme Hills").setMinMaxHeight(0.2F, 1.8F).setTemperatureRainfall(0.2F, 0.3F);
        public static readonly BiomeGenBase forest = (new BiomeGenForest(4)).setColor(0x56621).setBiomeName("Forest").func_4080_a(0x4eba31).setTemperatureRainfall(0.7F, 0.8F);
        public static readonly BiomeGenBase taiga = (new BiomeGenTaiga(5)).setColor(0xb6659).setBiomeName("Taiga").func_4080_a(0x4eba31).setTemperatureRainfall(0.3F, 0.8F).setMinMaxHeight(0.1F, 0.4F);
        public static readonly BiomeGenBase swampland = (new BiomeGenSwamp(6)).setColor(0x7f9b2).setBiomeName("Swampland").func_4080_a(0x8baf48).setMinMaxHeight(-0.2F, 0.1F).setTemperatureRainfall(0.8F, 0.9F);
        public static readonly BiomeGenBase river = (new BiomeGenRiver(7)).setColor(255).setBiomeName("River").setMinMaxHeight(-0.5F, 0.0F);
        public static readonly BiomeGenBase hell = (new BiomeGenHell(8)).setColor(0xff0000).setBiomeName("Hell").setDisableRain();
        public static readonly BiomeGenBase sky = (new BiomeGenEnd(9)).setColor(0x8080ff).setBiomeName("Sky").setDisableRain();
        public static readonly BiomeGenBase frozenOcean = (new BiomeGenOcean(10)).setColor(0x9090a0).setBiomeName("FrozenOcean").setEnableSnow().setMinMaxHeight(-1F, 0.5F).setTemperatureRainfall(0.0F, 0.5F);
        public static readonly BiomeGenBase frozenRiver = (new BiomeGenRiver(11)).setColor(0xa0a0ff).setBiomeName("FrozenRiver").setEnableSnow().setMinMaxHeight(-0.5F, 0.0F).setTemperatureRainfall(0.0F, 0.5F);
        public static readonly BiomeGenBase icePlains = (new BiomeGenSnow(12)).setColor(0xffffff).setBiomeName("Ice Plains").setEnableSnow().setTemperatureRainfall(0.0F, 0.5F);
        public static readonly BiomeGenBase iceMountains = (new BiomeGenSnow(13)).setColor(0xa0a0a0).setBiomeName("Ice Mountains").setEnableSnow().setMinMaxHeight(0.2F, 1.8F).setTemperatureRainfall(0.0F, 0.5F);
        public static readonly BiomeGenBase mushroomIsland = (new BiomeGenMushroomIsland(14)).setColor(0xff00ff).setBiomeName("MushroomIsland").setTemperatureRainfall(0.9F, 1.0F).setMinMaxHeight(0.2F, 1.0F);
        public static readonly BiomeGenBase mushroomIslandShore = (new BiomeGenMushroomIsland(15)).setColor(0xa000ff).setBiomeName("MushroomIslandShore").setTemperatureRainfall(0.9F, 1.0F).setMinMaxHeight(-1F, 0.1F);
        public string biomeName;
        public int color;
        public byte topBlock;
        public byte fillerBlock;
        public int field_6161_q;
        public float minHeight;
        public float maxHeight;
        public float temperature;
        public float rainfall;
        public int field_40461_A;
        public BiomeDecorator decorator;
        //protected List spawnableMonsterList;
        //protected List spawnableCreatureList;
        //protected List spawnableWaterCreatureList;
        private bool enableSnow;
        private bool enableRain;
        public readonly int biomeID;
        protected WorldGenTrees treeGenerator;
        protected WorldGenBigTree bigTreeGenerator;
        protected WorldGenForest forestGenerator;
        protected WorldGenSwamp swampTreeGenerator;
    }
}
