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
                default: return -1;
            }
        }
		
		public static bool ValidItem(short id)
		{
            if (id == 0 || id < -1) return false;
			foreach (Blocks blk in Enum.GetValues(typeof(Blocks)))
                if ((short)blk == id)
                    return true;
     		foreach (Items item in Enum.GetValues(typeof(Items)))
         		if ((short)item == id)
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
		NetherWart = 115
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
		Dye = 551,
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
	public enum StoneBrick : byte {Normal = 0x0, Mossy = 0x2, Cracked = 0x3}
	//public enum HugeMushroom : byte {Fleshy = 0x0, Corner = 0x1, Side = 0x2 ??? }
	public enum Vines : byte {Top = 0x0, West = 0x1, North = 0x2, East = 0x4, South = 0x8}
	public enum FenceGate : byte {West = 0x0, North = 0x1, East = 0x2, South = 0x3}
	public enum Directions : byte { Bottom = 0, Top = 1, East = 2, West = 3, North = 4, South = 5 };
	
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
                case (104):
                case (105):
                case (106):
                    return true;
            }
            return false;
        }

        public static bool HasPhysics(byte a)
        {
            switch (a)
            {
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
                /*case 34: // Do piston heads need physics?
                case 36:*/
                case 37:
                case 38:
                case 39:
                case 40:
                case 46: // =)
                case 50: // For removing the torch when it should be there
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
	}
}
