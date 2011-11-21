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
using System.Linq;
using System.Text;

namespace SMP
{
    public class FindBlocks
    {
        public static short FindBlock(string type)
        {
            switch (type.ToLower())
            {
                case "air": return 0;
                case "stone": return 1;
                case "grass": return 2;
                case "dirt": return 3;
                case "cobblestone": return 4;
                case "wood": return 5;
                case "plant": return 6;
                case "solid":
                case "admintite":
                case "blackrock":
                case "bedrock":
                case "adminium": return 7;
                case "activewater":
                case "active_water": return 8;
                case "water": return 9;
                case "activelava":
                case "active_lava": return 10;
                case "lava": return 11;
                case "sand": return 12;
                case "gravel": return 13;
                case "gold_ore": return 14;
                case "iron_ore": return 15;
                case "coal": return 16;
                case "tree": return 17;
                case "leaves": return 18;
                case "sponge": return 19;
                case "glass": return 20;
                case "red": return 21;
                case "orange": return 22;
                case "yellow": return 23;
                case "greenyellow": return 24;
                case "green": return 25;
                case "springgreen": return 26;
                case "cyan": return 27;
                case "blue": return 28;
                case "blueviolet": return 29;
                case "indigo": return 30;
                case "purple": return 31;
                case "magenta": return 32;
                case "pink": return 33;
                case "black": return 34;
                case "gray": return 35;
                case "white": return 36;
                case "yellow_flower": return 37;
                case "red_flower": return 38;
                case "brown_shroom": return 39;
                case "red_shroom": return 40;
                case "gold": return 41;
                case "iron": return 42;
                case "double_stair": return 43;
                case "stair": return 44;
                case "brick": return 45;
                case "tnt": return 46;
                case "bookcase": return 47;
                case "mossy_cobblestone": return 48;
                case "obsidian": return 49;
                case "torch": return 50;
                case "fire": return 51;
                case "monsterspawner": return 52;
                case "stairswooden": return 53;
                case "chest": return 54;
                case "redstonewire": return 55;
                case "diamondore": return 56;
                case "diamondblock": return 57;
                case "craftingtable": return 58;
                case "seeds": return 59;
                case "farmland": return 60;
                case "furnace": return 61;
                case "furnaceon": return 62;
                case "signpost": return 63;
                case "doorwooden": return 64;
                case "ladder": return 65;
                case "rails": return 66;
                case "stairscobblestone": return 67;
                case "signwall": return 68;
                case "lever": return 69;
                case "pressureplatestone": return 70;
                case "dooriron": return 71;
                case "pressureplatewood": return 72;
                case "restoneore": return 73;
                case "redstoneoreglow": return 74;
                case "redstonetorchoff": return 75;
                case "redstonetorchon": return 76;
                case "buttonstone": return 77;
                case "snow": return 78;
                case "ice": return 79;
                case "snowblock": return 80;
                case "cactus": return 81;
                case "clayblock": return 82;
                case "sugarcane": return 83;
                case "jukebox": return 84;
                case "fence": return 85;
                case "pumpkin": return 86;
                case "netherrack": return 87;
                case "soulsand": return 88;
                case "glowstone":
                case "glowstoneblock": return 89;
                case "portal": return 90;
                case "jackolantern": return 91;
                case "cake":
                case "cakeblock": return 92;
                case "redstonerepeateroff": return 93;
                case "redstonerepeateron": return 94;
                //case "lockedchest": return 95;
                case "trapdoor": return 96;
				case "hiddensilverfish": return 97;
				case "stonebricks": return 98;
				case "hugebrownmushroom": return 99;
				case "hugeredmushroom": return 100;
				case "ironbars": return 101;
				case "glasspane": return 102;
				case "melon": return 103;
				case "pumpkinstem": return 104;
				case "melonstem": return 105;
				case "vines": return 106;
				case "fencegate": return 107;
				case "brickstairs": return 108;
				case "stonebrickstairs": return 109;
				case "mycelium": return 110;
				case "lilypad": return 111;
				case "netherbrick": return 112;
				case "netherbrickfence": return 113;
				case "netherbrickstairs": return 114;
				case "netherwart": return 115;
				case "enchantmenttable": return 116;
				case "brewingstand": return 117;
				case "cauldron": return 118;
				case "airportal": return 119;
				case "airportalframe": return 120;
                case "whitestone": return 121;
                default: return -1;
            }
        }
		
		public static short[] FindItem(string type)
		{
			short[] item = {FindBlock(type), 0};
			
			if (item[0] == -1)
			{
				string sid = Server.SQLiteDB.ExecuteScalar("SELECT Value FROM Item WHERE Alias = '" + type + "';");
				if(!String.IsNullOrEmpty(sid))
				{
					try{item[0] = short.Parse(sid);}
					catch{item[0] = -1;}
				}
				else item[0] = -1;
				
				string smeta = Server.SQLiteDB.ExecuteScalar("SELECT Meta FROM Item WHERE Alias = '" + type + "';");
				if (!String.IsNullOrEmpty(smeta))
				{
					try{item[1] = short.Parse(smeta);}
					catch{item[1] = 0;}
				}
				else item[1] = 0;
			}
			return item;
		}
		
		public static bool ValidItem(short id)
		{
            if (id == 0) return false;
			foreach (Blocks blk in Enum.GetValues(typeof(Blocks)))
                if ((short)blk == id)
                    return true;
     		foreach (Items item in Enum.GetValues(typeof(Items)))
         		if ((short)item == id)
					 return true;
            return false;
		}

        public static bool ValidBlock(short id)
        {
            if (id < 0) return false;
            foreach (Blocks blk in Enum.GetValues(typeof(Blocks)))
                if ((short)blk == id)
                    return true;
            return false;
        }
    } 

	public enum Blocks : byte
	{		
		Air = 0,
		Stone = 1,
		Grass = 2,
		Dirt = 3,
		CobbleStone = 4,
		WoodenPlank = 5,
		Sapling = 6,
		Bedrock = 7,
		AWater = 8,
		SWater = 9,
		ALava = 10,
		SLava = 11,
		Sand = 12,
		Gravel = 13,
		GoldOre = 14,
		IronOre = 15,
		CoalOre = 16,
		Wood = 17,
		Leaves = 18,
		Sponge = 19,
		Glass = 20,
		LapisOre = 21,
		LapisBlock = 22,
		Dispenser = 23,
		SandStone = 24,
		NoteBlock = 25,
		Bed = 26,
		RailPowered = 27,
		RailDetector = 28,
		PistonSticky = 29,
		CobWeb = 30,
		TallGrass = 31,
		DeadShrubs = 32,
		Piston = 33,
		PistonExtension = 34,
		Wool = 35,
		BlockMovedByPiston = 36,
		FlowerDandelion = 37,
		FlowerRose = 38,
		MushroomBrown = 39,
		MushroomRed = 40,
		GoldBlock = 41,
		IronBlock = 42,
		SlabsDouble = 43,
		Slabs = 44,
		Brick = 45,
		TNT = 46,
		Bookshelf = 47,
		MossStone = 48,
		Obsidian = 49,
		Torch = 50,
		Fire = 51,
		MonsterSpawner = 52,
		StairsWooden = 53,
		Chest = 54,
		RedStoneWire = 55,
		DiamondOre = 56,
		DiamondBlock = 57,
		CraftingTable = 58,
		Seeds = 59,
		FarmLand = 60,
		Furnace = 61,
		FurnaceOn = 62,
		SignPost = 63,
		DoorWooden = 64,
		Ladder = 65,
		Rails = 66,
		StairsCobblestone = 67,
		SignWall = 68,
		Lever = 69,
		PressurePlateStone = 70,
		DoorIron = 71,
		PressurePlateWood = 72,
		RedStoneOre = 73,
		RedStoneOreGlow = 74,
		RedstoneTorchOff = 75,
		RedstoneTorchOn = 76,
		ButtonStone = 77,
		Snow = 78,
		Ice = 79,
		SnowBlock = 80,
		Cactus = 81,
		ClayBlock = 82,
		SugarCane = 83,
		Jukebox = 84,
		Fence = 85,
		Pumpkin = 86,
		Netherrack = 87,
		SouldSand = 88,
		GlowstoneBlock = 89,
		Portal = 90,
		JackOLantern = 91,
		CakeBlock = 92,
		RedstoneRepeaterOff = 93,
		RedstoneRepeaterOn = 94,
		//LockedChest = 95,
		Trapdoor = 96,
		SilverFishStone = 97,
		StoneBrick = 98,
		HugeBrownMushroom = 99,
		HugeRedMushroom = 100,
		IronBars = 101,
		GlassPane = 102,
		Melon = 103,
		PumpkinStem = 104,
		MelonStem = 105,
		Vines = 106,
		FenceGate = 107,
		BrickStairs = 108,
		StoneBrickStairs = 109,
		Mycelium = 110,
		LilyPad = 111,
		NetherBrick = 112,
		NetherBrickFence = 113,
		NetherBrickStairs = 114,
		NetherWart = 115,
		EnchantmentTable = 116,
		BrewingStand = 117,
		Cauldron = 118,
		EndPortal = 119,
		EndPortalFrame = 120,
        EndStone = 121,
        DragonEgg = 122
	};
	public enum Items : short
	{
		Nothing = -1,

		IronShovel = 256,
		IronPickaxe = 257,
		IronAxe = 258,
		FlintAndSteel = 259,
		AppleRed = 260,
		Bow = 261,
		Arrow = 262,
		Coal = 263,
		Diamond = 264,
		IronIngot = 265,
		GoldIngot = 266,
		IronSword = 267,
		WoodenSword = 268,
		WoodenShovel = 269,
		WoodenPickaxe = 270,
		WoodenAxe = 271,
		StoneSword = 272,
		StoneShovel = 273,
		StonePickaxe = 274,
		StoneAxe = 275,
		DiamondSword = 276,
		DiamondShovel = 277,
		DiamondPickaxe = 278,
		DiamondAxe = 279,
		Stick = 280,
		Bown = 281,
		SoupMushroom = 282,
		GoldSword = 283,
		GoldShovel = 284,
		GoldPickaxe = 285,
		GoldAxe = 286,
		String = 287,
		Feather = 288,
		Gunpowder = 289,
		WoodenHoe = 290,
		StoneHoe = 291,
		IronHoe = 292,
		DiamondHoe = 293,
		GoldHoe = 294,
		Seeds = 295,
		Wheat = 296,
		Bread = 297,
		LeatherCap = 298,
		LeatherTunic = 299,
		LeatherPants = 300,
		LeatherBoots = 301,
		ChainHelmet = 302,
		ChainChestplate = 303,
		ChainLeggings = 304,
		ChainBoots = 305,
		IronHelmet = 306,
		IronChestplate = 307,
		IronLeggings = 308,
		IronBoots = 309,
		DiamondHelmet = 310,
		DiamondChestplate = 311,
		DiamondLeggings = 312,
		DiamondBoots = 313,
		GoldHelmet = 314,
		GoldChestplate = 315,
		GoldLeggings = 316,
		GoldBoots = 317,
		Flint = 318,
		PorkchopRaw = 319,
		PorkchopCooked = 320,
		Paintings = 321,
		AppleGolden = 322,
		Sign = 323,
		DoorWooden = 324,
		Bucket = 325,
		BucketWater = 326,
		BucketLava = 327,
		Minecart = 328,
		Saddle = 329,
		DoorIron = 330,
		Redstone = 331,
		Snowball = 332,
		Boat = 333,
		Leather = 334,
		Milk = 335,
		ClayBrick = 336,
		Clay = 337,
		SugarCane = 338,
		Paper = 339,
		Book = 340,
		Slimeball = 341,
		MinecartStorage = 342,
		MinecartPowered = 343,
		Egg = 344,
		Compass = 345,
		FishingRod = 346,
		Clock = 347,
		GlowstoneDust = 348,
		FishRaw = 349,
		FishCooked = 350,
		Dye = 351,
		Bone = 352,
		Sugar = 353,
		Cake = 354,
		Bed = 355,
		RedstoneRepeater = 356,
		Cookie = 357,
		Map = 358,
		Shears = 359,
		MelonSlice = 360,
		PumpkinSeeds = 361,
		MelonSeeds = 362,
		SteakRaw = 363,
		SteakCooked = 364,
		ChickenRaw = 365,
		ChickenCooked = 366,
		RottenFlesh = 367,
		EnderPearl = 368,
		BlazeRod = 369,
		GoldNugget = 370,
		NetherWart = 372,
		Potions = 373,
		GlassBottle = 374,
		SpiderEye = 375,
		FermentedSpiderEye = 376,
		BlazePowder = 377,
		MagmaCream = 378,
		BrewingStand = 379,
		Cauldron = 380,
		EyeOfEnder = 381,
        GlisteringMelon = 382,
		
		GoldMusicDisc = 2256,
		GreenMusicDisc = 2257,
		blocksMusicDisc = 2258,
		chirpMusicDisc = 2259,
		farMusicDisc = 2260,
		mallMusicDisc = 2261,
		mellohiMusicDisc = 2262,
		stalMusicDisc = 2263,
		stradMusicDisc = 2264,
		wardMusicDisc = 2265,
		elevenMusicDisc = 2266
		
	};
    public enum Mobs : byte
    {
        Creeper = 50,
        Skeleton = 51,
        Spider = 52,
        GiantZombie = 53,
        Zombie = 54,
        Slime = 55,
        Ghast = 56,
        ZombiePigman = 57,
        Enderman = 58,
        CaveSpider = 59,
        Silverfish = 60,
        Blaze = 61,
        MagmaCube = 62,
        EnderDragon = 63,
        Pig = 90,
        Sheep = 91,
        Cow = 92,
        Chicken = 93,
        Squid = 94,
        Wolf = 95,
        Mooshroom = 96,
        SnowGolem = 97,
        Villager = 120
    }
    public enum Objects : byte
    {
        Boat = 1,
        Minecart = 10,
        StorageMinecart = 11,
        PoweredMinecart = 23,
        ActivatedTNT = 50,
        EnderCrystal = 51,
        FiredArrow = 60,
        ThrownSnowball = 61,
        ThrownEgg = 62,
        FallingSand = 70,
        FallingGravel = 71,
        EyeOfEnder = 72,
        FishingFloat = 90
    }
    public enum SoundEffect : int
    {
        Click2 = 1000,
        Click1 = 1001,
        BowFire = 1002,
        DoorToggle = 1003,
        Extinguish = 1004,
        RecordPlay = 1005,
        GhastFire = 1007,
        BlazeFire = 1008,
        BlazeFire2 = 1009,
        Smoke = 2000,
        BlockBreak = 2001,
        SpashPotion = 2002,
        PortalParticle = 2003,
        BlazeParticle = 2004
    }
	/// <summary>
	/// Goes for wood types AND leaves
	/// </summary>
	public enum Tree : byte { Normal = 0, Spruce, Birch };
	public enum Coal : byte { Coal = 0, Charcoal };
	public enum Jukebox : byte { Nothing = 0, GoldDisk, GreenDisk };
	public enum Wool : byte { White = 0, Orange, Magenta, LightBlue, Yellow, LightGreen, Pink, Gray, LightGray, Cyan, Purple, Blue, Brown, DarkGreen, Red, Black };
	public enum Dye : byte { InkSac = 0, RoseRed, CactusGreen, CocoaBeans, LapisLazuli, PurpleDye, CyanDye, LightGrayDye, GrayDye, PinkDye, LimeDye, DandelionYellow, LightBlueDye, MagentaDye, OrangeDye, BoneMeal };
	public enum Torch : byte { South = 0x1, North = 0x2, West = 0x3, East = 0x4, Standing = 0x5 };
	public enum Rail : byte { EastWest = 0x0, NorthSouth = 0x1, AscendingSouth = 0x2, AscendingNorth = 0x3, AscendingEast = 0x4, AscendingWest = 0x5, CornerNorthEast = 0x6, CornerSouthEast = 0x7, CornerSouthWest = 0x8, CornerNorthWest = 0x9 };
	public enum Ladder : byte { East = 0x2, West = 0x3, North = 0x4, South = 0x5 };
	public enum Stairs : byte { South = 0x0, North = 0x1, West = 0x2, East = 0x3 };
	public enum Levers : byte { WallSouth = 0x1, WallNorth = 0x2, WallWest = 0x3, WallEast = 0x4, GroundWest = 0x5, GroundSouth = 0x6, LeverOn = 0x8 };
	public enum Doors : byte { NorthEast = 0x0, SouthEast = 0x1, SouthWest = 0x2, NorthWest = 0x3, TopHalf = 0x8, Open = 0x4 };
	public enum Buttons : byte { Pressed = 0x8, West = 0x1, East = 0x2, South = 0x3, North = 0x4 };
	public enum SignPost : byte { West = 0x0, West_NorthWest = 0x1, NorthWest = 0x2, North_NorthWest = 0x3, North = 0x4, North_NorthEast = 0x5, NorthEast = 0x6, East_NorthEast = 0x7, East = 0x8, East_SouthEast = 0x9, SouthEast = 0xA, South_SouthEast = 0xB, South = 0xC, South_SouthWest = 0xD, SouthWest = 0xE, West_SouthWest = 0xF };
	public enum WallSigns : byte { East = 0x2, West = 0x3, North = 0x4, South = 0x5 };
	public enum Furnace : byte { East = 0x2, West = 0x3, North = 0x4, South = 0x5 };
	public enum Dispenser : byte { East = 0x2, West = 0x3, North = 0x4, South = 0x5 };
	public enum Pumpkin : byte { East = 0x2, West = 0x3, North = 0x4, South = 0x5 };
	public enum PressurePlate : byte { NotPressed = 0x0, Pressed = 0x1 };
	public enum Slab : byte { Stone = 0x0, SandStone = 0x1, Wooden = 0x2, Cobblestone = 0x3, Brick = 0x4, StoneBrick = 0x5, Stone2 = 0x6 };
	public enum Bed : byte { Isfoot = 0x8, West = 0x0, North = 0x1, East = 0x2, South = 0x3 };
	public enum Repeater : byte { East = 0x0, South = 0x1, West = 0x2, North = 0x3, Tick1 = 0x5, Tick2 = 0x6, Tick3 = 0x7, Tick4 = 0x8 };
	public enum TallGrass : byte { DeadShrub = 0x0, TallGrass = 0x1, Fern = 0x2 };
	public enum TrapDoors : byte { West = 0x0, East = 0x1, South = 0x2, North = 0x3, Open = 0x4 };
	public enum Piston : byte { Down = 0x0, Up = 0x1, East = 0x2, West = 0x3, North = 0x4, South = 0x5, On = 0x8 };
	public enum PistonExtension : byte { Down = 0x0, Up = 0x1, East = 0x2, West = 0x3, North = 0x4, South = 0x5, Sticky = 0x8 };
	public enum StoneBrick : byte { Normal = 0x0, Mossy = 0x1, Cracked = 0x2 }
	public enum HugeMushroom : byte { Fleshy = 0x0, CornerNorthWest = 0x1, SideNorth = 0x2, CornerNorthEast = 0x3, SideWest = 0x4, Top = 0x5, SideEast = 0x6, CornerSouthWest = 0x7, SideSouth = 0x8, CornerSouthEast = 0x9, Stem = 0xA }
	public enum Vines : byte { Top = 0x0, West = 0x1, North = 0x2, East = 0x4, South = 0x8 }
	public enum FenceGate : byte { West = 0x0, North = 0x1, East = 0x2, South = 0x3, Open = 0x4 }
	public enum Directions : byte { Bottom = 0, Top = 1, East = 2, West = 3, North = 4, South = 5 };
    public enum Material { Air, Iron, Wool, Grass, Gravel, Sand, Snow, Stone, Wood, Glass, Dirt, Water, Lava, Leaves, Unknown };
	
	public static class BlockData
	{
		/// <summary>
		/// This method returns weather or not special blocks can be placed against this one, for example, torches cant be placed on torches.
		/// </summary>
		/// <param name="a"></param>
		public static bool CanPlaceAgainst(byte a)
		{
			switch (a)
			{
				case (0):
				case (6):
				case (8):
				case (9):
				case (10):
				case (11):
		        case (26):
				case (27):
		        case (28):
		        case (30):
		        case (31):
		        case (32):
		        case (34):
		        case (36):
		        case (37):
		        case (38):
		        case (39):
		        case (40):
		        case (43):
		        case (44):
		        case (50):
		        case (51):
		        case (53):
		        case (55):
				case (59):
				case (63):
		        case (64):
		        case (65):
		        case (66):
		        case (67):
		        case (68):
		        case (69):
		        case (70):
		        case (71):
		        case (72):
		        case (75):
		        case (76):
		        case (77):
		        case (83):
		        case (85):
		        case (90):
		        case (92):
		        case (93):
		        case (94):
				case (96):
					return false;
					
			}
				
			return true;	

		}

        public static bool CanInstantBreak(byte a)
        {
            switch (a)
            {
                case (6):
                case (31):
                case (32):
                case (37):
                case (38):
                case (39):
                case (40):
                case (46):
                case (50):
                case (55):
                case (59):
                case (75):
                case (76):
                case (93):
                case (94):
                case (104):
                case (105):
                case (111):
                case (115):
                    return true;
            }
            return false;
        }

        public static bool HasPhysics(byte a)
        {
            switch (a)
            {
                case 2:
                case 3:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 18:
                case 19:
                case 27:
                case 28:
                case 29:
                case 33:
                //case 34: // Do piston heads need physics?
                case 37:
                case 38:
                case 39:
                case 40:
                case 46: // =)
                case 50: // For removing the torch when it shouldn't be there
                case 51:
                case 52: // We gotta spawn mobs somehow
                case 55:
                case 59: // How else will it grow? :P
                case 60:
                case 63:
                case 64:
                case 65:
                case 66:
                case 68:
                case 69: // Aw yeah...
                case 70:
                case 71:
                case 72:
                case 75:
                case 76:
                case 77:
                case 78: // Snow and ice melt, we'll figure it out later
                case 79:
                case 81: 
                case 83:
                case 90: // Portal blocks are weird
                case 93:
                case 94:
                case 96:
                case 104:
                case 105:
                case 106:
                case 110:
                case 111:
                case 115:
                    return true;
            }
            return false;
        }

        public static bool LiquidDestroy(byte a)
        {
            switch (a)
            {
                case 6:
                case 27:
                case 28:
                case 30:
                case 31:
                case 32:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 55:
                case 66:
                case 69:
                case 70:
                case 72:
                case 75:
                case 76:
                case 77:
                case 78:
                case 83:
                case 93:
                case 94:
                case 104:
                case 105:
                case 106:
                    return true;
            }
            return false;
        }

        public static bool CanWalkThrough(byte a)
        {
            switch (a)
            {
                case 0:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                case 27:
                case 28:
                case 30:
                case 31:
                case 32:
                case 37:
                case 38:
                case 39:
                case 40:
                case 44:
                case 50:
                case 51:
                case 55:
                case 59:
                case 63:
                case 64:
                case 65:
                case 66:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 75:
                case 76:
                case 77:
                case 78:
                case 83:
                case 90:
                case 92:
                case 93:
                case 94:
                case 96:
                case 104:
                case 105:
                case 106:
                case 107:
                case 111:
                case 115:
                case 119:
                    return true;
            }
            return false;
        }

        public static Material BlockMaterial(byte a)
        {
            switch (a)
            {
                case 0:
                    return Material.Air;
                case 3:
                    return Material.Dirt;
                case 20:
                case 79:
                case 90:
                case 102:
                    return Material.Glass;
                case 2:
                case 6:
                case 19:
                case 31:
                case 32:
                case 37:
                case 38:
                case 39:
                case 40:
                case 46:
                case 59:
                case 83:
                case 104:
                case 105:
                case 106:
                case 110:
                case 111:
                case 115:
                    return Material.Grass;
                case 13:
                case 60:
                case 82:
                    return Material.Gravel;
                case 12:
                case 88:
                    return Material.Sand;
                case 78:
                case 80:
                    return Material.Snow;
                case 1:
                case 4:
                case 7:
                case 14:
                case 15:
                case 16:
                case 21:
                case 22:
                case 23:
                case 24:
                case 27:
                case 28:
                case 29:
                case 30:
                case 33:
                case 34:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 48:
                case 49:
                case 52:
                case 55:
                case 56:
                case 57:
                case 61:
                case 62:
                case 66:
                case 67:
                case 70:
                case 71:
                case 73:
                case 74:
                case 77:
                case 87:
                case 89:
                case 97:
                case 98:
                case 101:
                case 108:
                case 109:
                case 112:
                case 113:
                case 114:
                    return Material.Stone;
                case 5:
                case 17:
                case 25:
                case 47:
                case 50:
                case 53:
                case 54:
                case 58:
                case 63:
                case 64:
                case 65:
                case 68:
                case 69:
                case 72:
                case 75:
                case 76:
                case 84:
                case 85:
                case 86:
                case 91:
                case 93:
                case 94:
                case 95:
                case 96:
                case 99:
                case 100:
                case 103:
                case 107:
                    return Material.Wood;
                case 35:
                case 81:
                case 92:
                    return Material.Wool;
                case 8:
                case 9:
                    return Material.Water;
                case 10:
                case 11:
                    return Material.Lava;
                case 18:
                    return Material.Leaves;
            }
            return Material.Unknown;
        }

        public static bool CanPlaceIn(byte a)
        {
            switch (a)
            {
                case 0:
                case 8:
                case 9:
                case 10:
                case 11:
                case 51:
                case 78:
                case 106:
                    return true;
            }
            return false;
        }

        public static bool CanPlaceOnEntity(byte a)
        {
            switch (a)
            {
                case 0:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                case 27:
                case 28:
                case 30:
                case 31:
                case 32:
                case 37:
                case 38:
                case 39:
                case 40:
                case 50:
                case 51:
                case 55:
                case 59:
                case 63:
                case 64:
                case 65:
                case 66:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 75:
                case 76:
                case 77:
                case 78:
                case 83:
                case 104:
                case 105:
                case 106:
                case 111:
                case 115:
                case 119:
                    return true;
            }
            return false;
        }

        public static byte PlaceableItemSwitch(short a)
        {
            switch (a)
            {
                case 259:
                    return 51;
                case 295:
                    return 59;
                case 323:
                    return 63;
                case 324:
                    return 64;
                case 326:
                    return 8;
                case 327:
                    return 10;
                case 330:
                    return 71;
                case 331:
                    return 55;
                case 338:
                    return 83;
                case 354:
                    return 92;
                case 355:
                    return 26;
                case 356:
                    return 93;
                case 361:
                    return 104;
                case 362:
                    return 105;
                case 372:
                    return 115;
            }
            return 0;
        }

        public static float GetExplosionResistance(byte a)
        {
            switch (a)
            {
                case 7:
                    return 18000000;
                case 49:
                    return 6000;
                case 8:
                case 9:
                case 10:
                case 11:
                    return 500;
                case 121:
                    return 35;
                case 45:
                case 98:
                case 4:
                case 57:
                case 41:
                case 42:
                case 84:
                case 48:
                case 67:
                case 44:
                case 43:
                case 1:
                case 97:
                case 101:
                case 108:
                case 109:
                case 112:
                case 114:
                    return 30;
                case 71:
                case 52:
                    return 25;
                case 23:
                case 61:
                case 62:
                    return 17.5F;
                case 16:
                case 56:
                case 64:
                case 85:
                case 14:
                case 15:
                case 22:
                case 21:
                case 73:
                case 74:
                case 53:
                case 5:
                case 96:
                case 107:
                    return 15;
                case 54:
                case 58:
                    return 12.5F;
                case 17:
                    return 10;
                case 47:
                    return 7.5F;
                case 91:
                case 86:
                case 63:
                case 68:
                    return 5;
                case 25:
                case 24:
                case 35:
                    return 4;
                case 27:
                case 28:
                case 66:
                    return 3.5F;
                case 82:
                case 60:
                case 2:
                case 13:
                case 19:
                    return 3;
                case 92:
                case 3:
                case 79:
                case 69:
                case 70:
                case 72:
                case 12:
                case 88:
                case 77:
                case 110:
                    return 2.5F;
                case 81:
                case 65:
                case 87:
                    return 2;
                case 20:
                case 89:
                    return 1.5F;
                case 26:
                case 18:
                case 80:
                case 106:
                    return 1;
                case 78:
                    return 0.5F;
            }
            return 0;
        }

        public static bool IsGroundCover(byte a)
        {
            switch (a)
            {
                case 78:
                    return true;
            }
            return false;
        }

        public static bool IsOpaqueCube(byte a)
        {
            return Chunk.LightOpacity[a] == 0xf;
        }

        public static bool IsCube(byte a)
        {
            switch (a)
            {
                case 0:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 37:
                case 38:
                case 39:
                case 40:
                case 44:
                case 50:
                case 51:
                case 53:
                case 54:
                case 55:
                case 59:
                case 60:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 75:
                case 76:
                case 77:
                case 78:
                case 81:
                case 83:
                case 85:
                case 90:
                case 92:
                case 93:
                case 94:
                case 96:
                case 101:
                case 102:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 111:
                case 113:
                case 114:
                case 115:
                    return false;
            }
            return true;
        }

        public static bool IsSolid(byte a)
        {
            return !CanWalkThrough(a);
        }

        public static bool IsLiquid(byte a)
        {
            switch (BlockMaterial(a))
            {
                case Material.Water:
                case Material.Lava:
                    return true;
            }
            return false;
        }

        public static bool IsItemDamageable(short id)
        {
            if ((id >= 267 && id <= 279) || (id >= 298 && id <= 317)) return true;
            switch (id)
            {
                case 256:
                case 257:
                case 258:
                case 259:
                case 261:
                case 346:
                case 283:
                case 284:
                case 285:
                case 286:
                case 290:
                case 291:
                case 292:
                case 293:
                case 294:
                case 359:
                    return true;
            }
            return false;
        }
	}
}
