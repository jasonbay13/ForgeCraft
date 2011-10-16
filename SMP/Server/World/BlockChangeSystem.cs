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
	public static class BlockChange
	{
		public delegate bool BCD(Player player, BCS bcs); //BlockChangeDelegate

		public static Dictionary<short, BCD> RightClickedOn = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> ItemRightClick = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> Placed = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> LeftClicked = new Dictionary<short, BCD>();
		public static Dictionary<short, BCD> Destroyed = new Dictionary<short, BCD>();


		//Init all the blockchange stuff
		public static void InitAll()
		{
			//Try to keep this organized by ID please.
			//BLOCKCHANGE METHODS HAVE TO RETURN whether or not the blockplace/digging method should continue, if these return false, the method will return.

			//Right Clicked ON Delegates (Holds delegates for when blocks are right clicked)
			RightClickedOn.Add((short)Blocks.Grass, new BCD(Till));
			RightClickedOn.Add((short)Blocks.Dirt, new BCD(Till));
			RightClickedOn.Add((short)Blocks.Dispenser, new BCD(OpenDispenser));
			RightClickedOn.Add((short)Blocks.NoteBlock, new BCD(ChangeNoteblock));
			RightClickedOn.Add((short)Blocks.Bed, new BCD(GetInBed));
			RightClickedOn.Add((short)Blocks.Chest, new BCD(OpenChest));
			RightClickedOn.Add((short)Blocks.CraftingTable, new BCD(OpenCraftingTable));
			RightClickedOn.Add((short)Blocks.Furnace, new BCD(OpenFurnace));
			RightClickedOn.Add((short)Blocks.FurnaceOn, new BCD(OpenFurnace));
			RightClickedOn.Add((short)Blocks.Jukebox, new BCD(PlayMusic));
			RightClickedOn.Add((short)Blocks.CakeBlock, new BCD(EatCake));
			RightClickedOn.Add((short)Blocks.RedstoneRepeaterOff, new BCD(ChangeRepeater));
			RightClickedOn.Add((short)Blocks.RedstoneRepeaterOn, new BCD(ChangeRepeater));
            RightClickedOn.Add((short)Blocks.Trapdoor, new BCD(OpenTrapdoor));
            RightClickedOn.Add((short)Blocks.DoorWooden, new BCD(OpenDoor));
            RightClickedOn.Add((short)Blocks.DoorIron, new BCD(DoNothing));
            RightClickedOn.Add((short)Blocks.Lever, new BCD(SwitchLever));
            RightClickedOn.Add((short)Blocks.ButtonStone, new BCD(HitButton));

			//Item RightClick Deletgates (Holds Delegates for when the player right clicks with specific items)
			ItemRightClick.Add((short)Items.FlintAndSteel, new BCD(LightFire));
			ItemRightClick.Add((short)Items.AppleRed, new BCD(EatApple));
			ItemRightClick.Add((short)Items.Bow, new BCD(FireBow));
			ItemRightClick.Add((short)Items.SoupMushroom, new BCD(EatSoup));
			ItemRightClick.Add((short)Items.Seeds, new BCD(PlantSeeds));
			ItemRightClick.Add((short)Items.Bread, new BCD(EatBread));
			ItemRightClick.Add((short)Items.PorkchopRaw, new BCD(EatPorkchopRaw));
			ItemRightClick.Add((short)Items.PorkchopCooked, new BCD(EatPorkchopCooked));
			ItemRightClick.Add((short)Items.AppleGolden, new BCD(EatGoldenApple));
			ItemRightClick.Add((short)Items.Sign, new BCD(PlaceSign));
			ItemRightClick.Add((short)Items.Paintings, new BCD(PlacePainting));
			ItemRightClick.Add((short)Items.DoorWooden, new BCD(PlaceWoodenDoor));
			ItemRightClick.Add((short)Items.DoorIron, new BCD(PlaceIronDoor));
			ItemRightClick.Add((short)Items.Bucket, new BCD(UseBucket));
			ItemRightClick.Add((short)Items.BucketWater, new BCD(UseWaterBucket));
			ItemRightClick.Add((short)Items.BucketLava, new BCD(UseLavaBucket));
			ItemRightClick.Add((short)Items.Milk, new BCD(DrinkMilk));
			ItemRightClick.Add((short)Items.Minecart, new BCD(PlaceMinecart));
			ItemRightClick.Add((short)Items.Saddle, new BCD(UseSaddle));
			ItemRightClick.Add((short)Items.Redstone, new BCD(PlaceRedstone));
			ItemRightClick.Add((short)Items.Snowball, new BCD(ThrowSnowball));
			ItemRightClick.Add((short)Items.Boat, new BCD(PlaceBoat));
			ItemRightClick.Add((short)Items.Slimeball, new BCD(ThrowSlimeball));
			ItemRightClick.Add((short)Items.MinecartPowered, new BCD(PlacePoweredMinecart));
			ItemRightClick.Add((short)Items.MinecartStorage, new BCD(PlaceStorageMinecart));
			ItemRightClick.Add((short)Items.Egg, new BCD(ThrowEgg));
			ItemRightClick.Add((short)Items.FishingRod, new BCD(UseFishingRod));
			ItemRightClick.Add((short)Items.FishRaw, new BCD(EatFishRaw));
			ItemRightClick.Add((short)Items.FishCooked, new BCD(EatFishCooked));
			ItemRightClick.Add((short)Items.Dye, new BCD(UseDye));
			ItemRightClick.Add((short)Items.Cake, new BCD(PlaceCake));
			ItemRightClick.Add((short)Items.Bed, new BCD(PlaceBed));
			ItemRightClick.Add((short)Items.RedstoneRepeater, new BCD(PlaceRepeater));
			ItemRightClick.Add((short)Items.Cookie, new BCD(EatCookie));
			//ItemRightClick.Add((short)Items.Map, new BCD(IDFK?)); //?
			ItemRightClick.Add((short)Items.Shears, new BCD(UseShears));
			ItemRightClick.Add((short)Items.GoldMusicDisc, new BCD(GoldMusicDisk));
			ItemRightClick.Add((short)Items.GreenMusicDisc, new BCD(GreenMusicDisk));

			//Block Place Delegates, like water/lava and to get furnaces/dispensers/etc to lay correctly
			Placed.Add((short)Blocks.Dirt, new BCD(PlaceDirt)); //We need a timer of sorts to change this to grass? or something... idk
			Placed.Add((short)Blocks.AWater, new BCD(PlaceWater));
			Placed.Add((short)Blocks.SWater, new BCD(PlaceWater));
			Placed.Add((short)Blocks.ALava, new BCD(PlaceLava));
			Placed.Add((short)Blocks.SLava, new BCD(PlaceLava));
			Placed.Add((short)Blocks.Sand, new BCD(PlaceSand));
			Placed.Add((short)Blocks.Gravel, new BCD(PlaceGravel));
			Placed.Add((short)Blocks.Dispenser, new BCD(PlaceDispenser));
			Placed.Add((short)Blocks.RailPowered, new BCD(PlaceRailPower));
			Placed.Add((short)Blocks.RailDetector, new BCD(PlaceRailDetect));
			Placed.Add((short)Blocks.PistonSticky, new BCD(PlaceStickyPiston));
			Placed.Add((short)Blocks.Piston, new BCD(PlaceNormalPiston));
			Placed.Add((short)Blocks.Slabs, new BCD(PlaceSlabs));
			Placed.Add((short)Blocks.Torch, new BCD(PlaceTorch));
			Placed.Add((short)Blocks.StairsWooden, new BCD(PlaceStairs));
            Placed.Add((short)Blocks.StairsCobblestone, new BCD(PlaceStairs));
            Placed.Add((short)Blocks.BrickStairs, new BCD(PlaceStairs));
            Placed.Add((short)Blocks.StoneBrickStairs, new BCD(PlaceStairs));
            Placed.Add((short)Blocks.NetherBrickStairs, new BCD(PlaceStairs));
			Placed.Add((short)Blocks.Chest, new BCD(PlaceChest));
			Placed.Add((short)Blocks.Furnace, new BCD(PlaceFurnace));
			Placed.Add((short)Blocks.Ladder, new BCD(PlaceLadder));
			Placed.Add((short)Blocks.Rails, new BCD(PlaceRail));
			Placed.Add((short)Blocks.Lever, new BCD(PlaceLever));
			Placed.Add((short)Blocks.RedstoneTorchOff, new BCD(PlaceTorch));
            Placed.Add((short)Blocks.RedstoneTorchOn, new BCD(PlaceTorch));
			Placed.Add((short)Blocks.ButtonStone, new BCD(PlaceButtonStone));
			Placed.Add((short)Blocks.Cactus, new BCD(PlaceCactus));
			Placed.Add((short)Blocks.SugarCane, new BCD(PlaceSugarCane));
			Placed.Add((short)Blocks.Fence, new BCD(PlaceFence));
			Placed.Add((short)Blocks.Pumpkin, new BCD(PlacePumpkin));
			Placed.Add((short)Blocks.JackOLantern, new BCD(PlaceJackOLantern));
			Placed.Add((short)Blocks.Trapdoor, new BCD(PlaceTrapdoor));

			//Block LeftClick Delegates: (Holds Delegates for when a player hits specific items)
			LeftClicked.Add((short)Blocks.NoteBlock, new BCD(PlayNoteblock));
			LeftClicked.Add((short)Blocks.DoorWooden, new BCD(OpenDoor));
			LeftClicked.Add((short)Blocks.Lever, new BCD(SwitchLever));
			LeftClicked.Add((short)Blocks.ButtonStone, new BCD(HitButton));
			//LeftClicked.Add((short)Blocks.Jukebox, new BCD(EjectCd)); // NO!
			LeftClicked.Add((short)Blocks.Trapdoor, new BCD(OpenTrapdoor));
            LeftClicked.Add((short)Blocks.Torch, new BCD(DestroyTorch));

			//Block Delete Delegates (Holds Delegates for when specific items are DELETED)
			Destroyed.Add((short)Blocks.Dispenser, new BCD(DestroyDispenser)); //Drop all Item's from the dispenser
			Destroyed.Add((short)Blocks.Bed, new BCD(DestroyBed)); //Delete the other half of the door
			Destroyed.Add((short)Blocks.SlabsDouble, new BCD(DestroyDoubleSlab)); //Drop two
			Destroyed.Add((short)Blocks.Chest, new BCD(DestroyChest)); //Drop Contents
			Destroyed.Add((short)Blocks.Seeds, new BCD(DestroyWheat)); //Drop Wheat, drop two seeds if needed
			Destroyed.Add((short)Blocks.Furnace, new BCD(DestroyFurnace)); //Drop Contents
			Destroyed.Add((short)Blocks.FurnaceOn, new BCD(DestroyFurnace)); //Drop Contents
			Destroyed.Add((short)Blocks.DoorWooden, new BCD(DestroyDoorWood)); //Delete the other half
			Destroyed.Add((short)Blocks.DoorIron, new BCD(DestroyDoorIron)); //Delete the other half
			Destroyed.Add((short)Blocks.RedStoneOre, new BCD(DestroyRedstoneOre)); //Drop random amount
			Destroyed.Add((short)Blocks.RedStoneOreGlow, new BCD(DestroyRedstoneOre)); //Drop random amount
			Destroyed.Add((short)Blocks.Snow, new BCD(DestroySnow)); //if(iteminhand==shovel) then drop snowball
			Destroyed.Add((short)Blocks.Cactus, new BCD(DestroyCacti)); //break/drop other cacti
			Destroyed.Add((short)Blocks.ClayBlock, new BCD(DestroyClay)); //Drop random amount
			Destroyed.Add((short)Blocks.SugarCane, new BCD(DestroySugarCane)); //Destroy Other canes
			Destroyed.Add((short)Blocks.Jukebox, new BCD(DestroyJukebox)); //Drop Contents
            Destroyed.Add((short)Blocks.GlowstoneBlock, new BCD(DestroyGlowStone)); //Drop random amount
            Destroyed.Add((short)Blocks.NoteBlock, new BCD(DestroyNoteBlock)); // Unset pitch value
            Destroyed.Add((short)Blocks.TNT, new BCD(DestroyTNT)); // For testing :)
		}

        public static bool DoNothing(Player a, BCS b)
        {
            return false;
        }
		public static bool Till(Player a, BCS b)
		{
			switch (a.inventory.current_item.item) {
                case (short)Items.DiamondHoe:
                case (short)Items.GoldHoe:
                case (short)Items.IronHoe:
                case (short)Items.StoneHoe:
                case (short)Items.WoodenHoe:
					a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.FarmLand, 0);
					a.inventory.current_item.meta++; //damage of the item
                    return false;
            }
			return true;
		}
		public static bool OpenDispenser(Player a, BCS b)
		{
			if(!a.level.windows.ContainsKey(b.pos))
			{
				new Windows(3, b.pos, a.level);
			}
			
			Windows window = a.level.windows[b.pos];
			short length = (short)window.name.Length;
			byte[] bytes = new byte[5 + (length)];

			bytes[0] = 1; //CHANGE THIS! (idk what the byte referances, i dont really see a use for it if its just a byte)
			bytes[1] = window.type;
			util.EndianBitConverter.Big.GetBytes(length).CopyTo(bytes, 2);
			UTF8Encoding.UTF8.GetBytes(window.name).CopyTo(bytes, 4);
			bytes[4 + (length)] = (byte)window.items.Length; //number of slots

			a.SendRaw(0x64, bytes);
			return false;
		}
		public static bool ChangeNoteblock(Player a, BCS b)
		{
			//Change Metadata
            ushort extra = a.level.GetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            extra++; if (extra > 24) extra = 0;
            a.level.SetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, extra);

			//Send NoteSound
            PlayNoteblock(a, b);
			return false;
		}
		public static bool GetInBed(Player a, BCS b)
        {
			return false;
		}
		public static bool OpenChest(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenCraftingTable(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenFurnace(Player a, BCS b)
		{
			return false;
		}
		public static bool PlayMusic(Player a, BCS b)
		{
            ushort meta = a.level.GetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            short item = a.inventory.current_item.item;
            if (meta != 0)
            {
                if (meta >= 2256 && meta <= 2266)
                {
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)meta);
                }

                a.level.UnsetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
                Player.GlobalSoundEffect(b.pos, 1005, 0, a.level);
            }
            else if (item >= 2256 && item <= 2266)
            {
                a.inventory.Remove(a.inventory.current_index, 1);
                a.level.SetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (ushort)item);
                Player.GlobalSoundEffect(b.pos, 1005, item, a.level);
            }
            return false;
		}
		public static bool EatCake(Player a, BCS b)
		{
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z); meta++;
            if (meta > 0x5) a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
            else a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.CakeBlock, meta);
			return false;
		}
		public static bool ChangeRepeater(Player a, BCS b)
		{
            byte meta = (byte)((a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) & 0xC) >> 2);
            meta++; if (meta > 3) meta = 0;
            meta = (byte)((meta << 2) | (a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) & 0x3));
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z), meta);
			return false;
		}

		public static bool DrinkMilk(Player a, BCS b)
		{
			return false;
		}
		public static bool EatApple(Player a, BCS b)
		{
			return false;
		}
		public static bool EatBread(Player a, BCS b)
		{
			return false;
		}
		public static bool EatCookie(Player a, BCS b)
		{
			return false;
		}
		public static bool EatFishCooked(Player a, BCS b)
		{
			return false;
		}
		public static bool EatFishRaw(Player a, BCS b)
		{
			return false;
		}
		public static bool EatGoldenApple(Player a, BCS b)
		{
			return false;
		}
		public static bool EatPorkchopCooked(Player a, BCS b)
		{
			return false;
		}
		public static bool EatPorkchopRaw(Player a, BCS b)
		{
			return false;
		}
		public static bool EatSoup(Player a, BCS b)
		{
			return false;
		}
		public static bool FireBow(Player a, BCS b)
		{
			return false;
		}
		public static bool GoldMusicDisk(Player a, BCS b)
		{
			return false;
		}
		public static bool GreenMusicDisk(Player a, BCS b)
		{
			return false;
		}
		public static bool LightFire(Player a, BCS b)
		{
            /*Point3 firePos = new Point3();
            switch (b.Direction)
            {
                case 0:
                    firePos = new Point3(b.pos.x, b.pos.y - 1, b.pos.z);
                    break;
                case 1:
                    firePos = new Point3(b.pos.x, b.pos.y + 1, b.pos.z);
                    break;
                case 2:
                    firePos = new Point3(b.pos.x, b.pos.y, b.pos.z - 1);
                    break;
                case 3:
                    firePos = new Point3(b.pos.x, b.pos.y, b.pos.z + 1);
                    break;
                case 4:
                    firePos = new Point3(b.pos.x - 1, b.pos.y, b.pos.z);
                    break;
                case 5:
                    firePos = new Point3(b.pos.x + 1, b.pos.y, b.pos.z);
                    break;
                default:
                    return false;
            }*/

            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) == (byte)Blocks.Air)
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Fire, 0);
                a.inventory.current_item.meta++;
            }
            return false;
		}
		public static bool PlaceBed(Player a, BCS b)
		{
            if (b.Direction != 1) return false;
            Point3 pos2 = b.pos;
            byte rot = DirectionByRotFlat(a, b);

            switch (rot)
            {
                case (byte)Directions.North:
                    rot = (byte)Bed.North;
                    break;
                case (byte)Directions.East:
                    rot = (byte)Bed.East;
                    break;
                case (byte)Directions.South:
                    rot = (byte)Bed.South;
                    break;
                case (byte)Directions.West:
                    rot = (byte)Bed.West;
                    break;
            }
            switch (rot)
            {
                case (byte)Bed.North:
                    pos2.x--;
                    break;
                case (byte)Bed.East:
                    pos2.z--;
                    break;
                case (byte)Bed.South:
                    pos2.x++;
                    break;
                case (byte)Bed.West:
                    pos2.z++;
                    break;
            }

            if (!BlockData.CanPlaceAgainst(a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z)) || !BlockData.CanPlaceAgainst(a.level.GetBlock((int)pos2.x, (int)pos2.y - 1, (int)pos2.z))) return false;
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Bed, rot);
            a.level.BlockChange((int)pos2.x, (int)pos2.y, (int)pos2.z, (byte)Blocks.Bed, (byte)(rot | 0x8));

			return true;
		}
		public static bool PlaceBoat(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceCake(Player a, BCS b)
		{
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z) != 0)
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.CakeBlock, 0);
                return true;
            }
			return false;
		}
		public static bool PlaceIronDoor(Player a, BCS b)
		{
            // TODO: Double doors!
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) != 0) return false;
            if (b.Direction != 1) return false;
            byte rot = DirectionByRotFlat(a, b);
            switch (rot)
            {
                case (byte)Directions.North:
                    rot = (byte)Doors.SouthWest;
                    break;
                case (byte)Directions.East:
                    rot = (byte)Doors.NorthWest;
                    break;
                case (byte)Directions.South:
                    rot = (byte)Doors.NorthEast;
                    break;
                case (byte)Directions.West:
                    rot = (byte)Doors.SouthEast;
                    break;
            }

            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.DoorIron, rot);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z, (byte)Blocks.DoorIron, (byte)(rot | 0x8));
            return true;
		}
		public static bool PlaceMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlacePainting(Player a, BCS b)
		{
			return false;
		}
		public static bool PlacePoweredMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRedstone(Player a, BCS b)
		{
            if (!BlockData.CanPlaceAgainst(a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z))) return false;
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.RedStoneWire, 0);
			return true;
		}
		public static bool PlaceRepeater(Player a, BCS b)
		{
            if (!BlockData.CanPlaceAgainst(a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z))) return false;
            byte rot = DirectionByRotFlat(a, b);
            switch (rot)
            {
                case (byte)Directions.North:
                    rot = (byte)Repeater.North;
                    break;
                case (byte)Directions.East:
                    rot = (byte)Repeater.East;
                    break;
                case (byte)Directions.South:
                    rot = (byte)Repeater.South;
                    break;
                case (byte)Directions.West:
                    rot = (byte)Repeater.West;
                    break;
            }
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.RedstoneRepeaterOff, rot);
			return false;
		}
		public static bool PlaceSign(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceStorageMinecart(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceWoodenDoor(Player a, BCS b)
		{
            // TODO: Double doors!
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) != 0) return false;
            if (b.Direction != 1) return false;
            byte rot = DirectionByRotFlat(a, b);
            switch (rot)
            {
                case (byte)Directions.North:
                    rot = (byte)Doors.SouthWest;
                    break;
                case (byte)Directions.East:
                    rot = (byte)Doors.NorthWest;
                    break;
                case (byte)Directions.South:
                    rot = (byte)Doors.NorthEast;
                    break;
                case (byte)Directions.West:
                    rot = (byte)Doors.SouthEast;
                    break;
            }

            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.DoorWooden, rot);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z, (byte)Blocks.DoorWooden, (byte)(rot | 0x8));
			return true;
		}
		public static bool PlantSeeds(Player a, BCS b)
		{
            if (Blockclicked(a, b) != (byte)Blocks.FarmLand) return false;
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Seeds, 0);
			return true;
		}
		public static bool ThrowEgg(Player a, BCS b)
		{
			return false;
		}
		public static bool ThrowSlimeball(Player a, BCS b)
		{
			return false;
		}
		public static bool ThrowSnowball(Player a, BCS b)
		{
			return false;
		}
		public static bool UseBucket(Player a, BCS b)
		{
            byte bl = a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z), blc = Blockclicked(a, b);
			if (bl == 8 || bl == 9 || blc == 8 || blc == 9)
			{
				a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
                a.inventory.Add((short)Items.BucketWater, 1, 0, a.inventory.current_index);
			}
            else if (bl == 10 || bl == 11 || blc == 10 || blc == 11)
			{
				a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
                a.inventory.Add((short)Items.BucketLava, 1, 0, a.inventory.current_index);
			}
			return false;
		}
		public static bool UseDye(Player a, BCS b)
		{
            switch (Blockclicked(a, b))
            {
                case (byte)Blocks.Sapling:
                    if (a.inventory.current_item.meta == (byte)Dye.BoneMeal)
                    {
                        Point3 pos2 = BlockclickedPos(a, b);
                        GenTrees.Normal(a.level, (int)pos2.x, (int)pos2.y, (int)pos2.z, a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z));
                        return true;
                    }
                    return false;
                case (byte)Blocks.Grass:
                    if (b.Direction != 1) return false;
                    if (a.inventory.current_item.meta == (byte)Dye.BoneMeal)
                    {
                        byte type;
                        for (int x = -3; x <= 3; x++)
                            for (int z = -3; z <= 3; z++)
                                for (int y = -1; y <= 1; y++)
                                {
                                    if (a.level.GetBlock((int)b.pos.x + x, (int)b.pos.y + y - 1, (int)b.pos.z + z) == (byte)Blocks.Grass && a.level.GetBlock((int)b.pos.x + x, (int)b.pos.y, (int)b.pos.z + z) == (byte)Blocks.Air && Entity.random.Next(2) == 0)
                                    {
                                        switch (Entity.random.Next(15))
                                        {
                                            case 0:
                                                type = (byte)Blocks.FlowerDandelion;
                                                break;
                                            case 1:
                                                type = (byte)Blocks.FlowerRose;
                                                break;
                                            default:
                                                type = (byte)Blocks.TallGrass;
                                                break;
                                        }
                                        a.level.BlockChange((int)b.pos.x + x, (int)b.pos.y + y, (int)b.pos.z + z, type, (type == (byte)Blocks.TallGrass ? (byte)TallGrass.TallGrass : (byte)0));
                                    }
                                }
                        return true;
                    }
                    return false;
                case (byte)Blocks.Seeds:
                    if (a.inventory.current_item.meta == (byte)Dye.BoneMeal)
                    {
                        Point3 pos2 = BlockclickedPos(a, b);
                        a.level.BlockChange((int)pos2.x, (int)pos2.y, (int)pos2.z, (byte)Blocks.Seeds, 0x7);
                        return true;
                    }
                    return false;
            }
			return false;
		}
		public static bool UseFishingRod(Player a, BCS b)
		{
			return false;
		}
		public static bool UseLavaBucket(Player a, BCS b)
		{
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 10, 0);
            if (Server.mode == 0) a.inventory.Add((short)Items.Bucket, 1, 0, a.inventory.current_index);
			return false;
		}
		public static bool UseSaddle(Player a, BCS b)
		{
			return false;
		}
		public static bool UseShears(Player a, BCS b)
		{
			return false;
		}
		public static bool UseWaterBucket(Player a, BCS b)
		{
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 8, 0);
            if (Server.mode == 0) a.inventory.Add((short)Items.Bucket, 1, 0, a.inventory.current_index);
			return false;
		}

		public static bool PlaceButtonStone(Player a, BCS b)
		{
			if (!BlockData.CanPlaceAgainst(Blockclicked(a, b))) return false;
			if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) != 0) return false;

			switch (b.Direction)
			{
				case (0):
				case (1):
                    return false;
				case ((byte)Directions.East):
                    b.Direction = (byte)Buttons.North;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Buttons.South;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Buttons.East;
					break;
				case ((byte)Directions.South):
					b.Direction = (byte)Buttons.West;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;

		}
		public static bool PlaceCactus(Player a, BCS b)
		{
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z) != (byte)Blocks.Sand) return false;
            // TODO: Adjacent block checks!
			return true;
		}
		public static bool PlaceChest(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceDirt(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceDispenser(Player a, BCS b)
		{
            retry:
			switch (b.Direction)
			{
				case (0):
				case (1):
                    b.Direction = DirectionByRotFlat(a, b, true);
                    goto retry;
				case ((byte)Directions.South):
					b.Direction = (byte)Dispenser.South;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Dispenser.North;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Dispenser.West;
					break;
				case ((byte)Directions.East):
					b.Direction = (byte)Dispenser.East;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceFence(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceFurnace(Player a, BCS b)
		{
			switch (b.Direction)
			{
				case (0):
				case (1):
                    b.Direction = DirectionByRotFlat(a, b);
                    break;
				case ((byte)Directions.East):
					b.Direction = (byte)Furnace.East;
					break;
				case ((byte)Directions.West):
					b.Direction = (byte)Furnace.West;
					break;
				case ((byte)Directions.North):
					b.Direction = (byte)Furnace.North;
					break;
				case ((byte)Directions.South):
					b.Direction = (byte)Furnace.South;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceGravel(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceJackOLantern(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceLadder(Player a, BCS b)
		{
			switch (b.Direction)
			{
				case (0):
				case (1):
					return false;

				case ((byte)Directions.South):
					b.Direction = (byte)Ladder.South;
					break;
				case ((byte)Directions.North):
                    b.Direction = (byte)Ladder.North;
					break;
				case ((byte)Directions.West):
                    b.Direction = (byte)Ladder.West;
					break;
				case ((byte)Directions.East):
                    b.Direction = (byte)Ladder.East;
					break;

				default:
					return false;
			}

			a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)b.ID, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceLava(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceLever(Player a, BCS b)
		{
            if (b.Direction == 0) return false;
            if (!BlockData.CanPlaceAgainst(Blockclicked(a, b))) return false;
            if (b.Direction == 1)
            {
                switch (DirectionByRotFlat(a, b))
                {
                    case (byte)Directions.East:
                    case (byte)Directions.West:
                        b.Direction = (byte)Levers.GroundWest;
                        break;
                    case (byte)Directions.North:
                    case (byte)Directions.South:
                        b.Direction = (byte)Levers.GroundSouth;
                        break;
                }
            }
            else
            {
                switch (b.Direction)
                {
                    case ((byte)Directions.East):
                        b.Direction = (byte)Levers.WallEast;
                        break;
                    case ((byte)Directions.West):
                        b.Direction = (byte)Levers.WallWest;
                        break;
                    case ((byte)Directions.North):
                        b.Direction = (byte)Levers.WallNorth;
                        break;
                    case ((byte)Directions.South):
                        b.Direction = (byte)Levers.WallSouth;
                        break;
                }
            }
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Lever, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceNormalPiston(Player a, BCS b)
		{
            if (b.pos.y >= a.pos.y + 2) b.Direction = (byte)Directions.Bottom;
            else if (b.pos.y <= a.pos.y - 1) b.Direction = DirectionByRotNOTFlat(a, b, true);
            else if (b.Direction == 0 || b.Direction == 1) b.Direction = DirectionByRotFlat(a, b, true);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Piston, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlacePumpkin(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRail(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRailDetect(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceRailPower(Player a, BCS b)
		{
			return false;
		}
		public static bool PlaceSand(Player a, BCS b)
		{
			return true;
		}
        public static bool PlaceSlabs(Player a, BCS b)
        {
            if (a.level.GetMeta((int)b.pos.X, (int)b.pos.Y - 1, (int)b.pos.Z) == (byte)a.inventory.current_item.meta && a.level.GetBlock((int)b.pos.X, (int)b.pos.Y - 1, (int)b.pos.Z) == 44)
            {
                a.SendBlockChange(b.pos, 0);
                a.level.BlockChange((int)b.pos.X, (int)b.pos.Y - 1, (int)b.pos.Z, 43, (byte)a.inventory.current_item.meta);
                if (Server.mode == 0) { a.inventory.Remove(a.inventory.current_index, 1); a.Experience.Add(a, 1); }
                return false;
            }
            return true;
        }
		public static bool PlaceStairs(Player a, BCS b)
		{
            switch (DirectionByRotFlat(a, b))
            {
                case (byte)Directions.North:
                    b.Direction = (byte)Stairs.North;
                    break;
                case (byte)Directions.South:
                    b.Direction = (byte)Stairs.South;
                    break;
                case (byte)Directions.East:
                    b.Direction = (byte)Stairs.East;
                    break;
                case (byte)Directions.West:
                    b.Direction = (byte)Stairs.West;
                    break;
            }
            a.level.BlockChange((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z, (byte)b.ID, b.Direction);
			return false;
		}
		public static bool PlaceStickyPiston(Player a, BCS b)
		{
            if (b.pos.y >= a.pos.y + 2) b.Direction = (byte)Directions.Bottom;
            else if (b.pos.y <= a.pos.y - 1) b.Direction = DirectionByRotNOTFlat(a, b, true);
            else if (b.Direction == 0 || b.Direction == 1) b.Direction = DirectionByRotFlat(a, b, true);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.PistonSticky, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceSugarCane(Player a, BCS b)
		{
			return true;
		}
		public static bool PlaceTorch(Player a, BCS b)
		{
            byte placingon = 255;
            switch (b.Direction)
            {
                case 0:
                case 1:
                    placingon = a.level.GetBlock((int)b.pos.X, (int)b.pos.Y - 1, (int)b.pos.Z);
                    b.Direction = 0;
                    break;
                case 2:
                    placingon = a.level.GetBlock((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z + 1);
                    b.Direction = 4; //East
                    break;
                case 3:
                    placingon = a.level.GetBlock((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z - 1);
                    b.Direction = 3; //West
                    break;
                case 4:
                    placingon = a.level.GetBlock((int)b.pos.X + 1, (int)b.pos.Y, (int)b.pos.Z);
                    b.Direction = 2; //North
                    break;
                case 5:
                    placingon = a.level.GetBlock((int)b.pos.X - 1, (int)b.pos.Y, (int)b.pos.Z);
                    b.Direction = 1; //South
                    break;
            }

            if (!BlockData.CanPlaceAgainst(placingon) || placingon == 20)
            {
                b.Direction = 0;
                placingon = a.level.GetBlock((int)b.pos.X, (int)b.pos.Y - 1, (int)b.pos.Z);
            }
            if (!BlockData.CanPlaceAgainst(placingon) || placingon == 20) return false;

            a.level.BlockChange((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z, (byte)b.ID, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
            return false;
		}
		public static bool PlaceTrapdoor(Player a, BCS b)
		{
            if (b.Direction == 0 || b.Direction == 1) return false;
            if (!BlockData.CanPlaceAgainst(Blockclicked(a, b))) return false;
            switch (InvertDirection(b.Direction))
            {
                case (byte)Directions.North:
                    b.Direction = (byte)TrapDoors.North;
                    break;
                case (byte)Directions.East:
                    b.Direction = (byte)TrapDoors.East;
                    break;
                case (byte)Directions.South:
                    b.Direction = (byte)TrapDoors.South;
                    break;
                case (byte)Directions.West:
                    b.Direction = (byte)TrapDoors.West;
                    break;
            }
            a.level.BlockChange((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z, (byte)Blocks.Trapdoor, b.Direction);
            if (Server.mode == 0) a.inventory.Remove(a.inventory.current_index, 1);
			return false;
		}
		public static bool PlaceWater(Player a, BCS b)
		{
			return true;
		}

		public static bool PlayNoteblock(Player a, BCS b)
		{
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) != 0) return false;

            byte instrument = 0;
            switch (BlockData.BlockMaterial(a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z)))
            {
                case Material.Wood:
                    instrument = 1;
                    break;
                case Material.Sand:
                case Material.Gravel:
                    instrument = 2;
                    break;
                case Material.Glass:
                    instrument = 3;
                    break;
                case Material.Stone:
                    instrument = 4;
                    break;
            }

            Player.GlobalBlockAction(b.pos, instrument, (byte)a.level.GetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z), a.level);
			return false;
		}
		public static bool OpenDoor(Player a, BCS b)
		{
            byte type = a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            if ((meta & 0x8) == 0)
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, type, (byte)(meta ^ 0x4));
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z, type, (byte)((meta | 0x8) ^ 0x4));
            }
            else
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, type, (byte)(meta ^ 0x4));
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z, type, (byte)((meta ^ 0x8) ^ 0x4));
            }
            Player.GlobalSoundEffect(b.pos, 1003, a.level, a);
			return false;
		}
		public static bool SwitchLever(Player a, BCS b)
		{
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Lever, (byte)(meta ^ 0x8));
            //Player.GlobalSoundEffect(b.pos, 1000, a.level); // There is no sound effect for this. DAMN YOU NOTCH!!!
			return false;
		}
		public static bool HitButton(Player a, BCS b)
		{
			return false;
		}
		public static bool EjectCd(Player a, BCS b)
		{
			return false;
		}
		public static bool OpenTrapdoor(Player a, BCS b)
		{
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (byte)Blocks.Trapdoor, (byte)(meta ^ 0x4));
            Player.GlobalSoundEffect(b.pos, 1003, a.level, a);
			return false;
		}
        public static bool DestroyTorch(Player a, BCS b)
        {
            // THIS IS ALL HANDLED ELSEWHERE
            /*a.level.BlockChange((int)b.pos.X, (int)b.pos.Y, (int)b.pos.Z, 0, 0);
            Item item = new Item(50, a.level) { count = 1, meta = 0, pos = new Point3(b.pos.X + .5, b.pos.Y + .5, b.pos.Z + .5), rot = new byte[3] { 1, 1, 1 }, OnGround = true };
            item.e.UpdateChunks(false, false);*/
            return true;
        }
		public static bool DestroyDispenser(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyBed(Player a, BCS b)
		{
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);

            Point3 storePos = b.pos;
            byte rot = (byte)(meta & 0x3);
            bool head = (meta & 0x8) != 0;
            if (head)
            {
                switch (rot)
                {
                    case (byte)Bed.North:
                        b.pos.x++;
                        break;
                    case (byte)Bed.East:
                        b.pos.z++;
                        break;
                    case (byte)Bed.South:
                        b.pos.x--;
                        break;
                    case (byte)Bed.West:
                        b.pos.z--;
                        break;
                }
            }
            else
            {
                switch (rot)
                {
                    case (byte)Bed.North:
                        b.pos.x--;
                        break;
                    case (byte)Bed.East:
                        b.pos.z--;
                        break;
                    case (byte)Bed.South:
                        b.pos.x++;
                        break;
                    case (byte)Bed.West:
                        b.pos.z++;
                        break;
                }
            }
            if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) == (byte)Blocks.Bed)
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);

            if (Server.mode == 0)
            {
                a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Bed);
            }

			return false;
		}
		public static bool DestroyDoubleSlab(Player a, BCS b)
		{
            if (Server.mode == 0)
            {
                byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
                for (int i = 0; i < 2; i++)
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Blocks.Slabs, meta);
                return false;
            }
			return true;
		}
		public static bool DestroyChest(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroyWheat(Player a, BCS b)
		{
            if (Server.mode == 0)
            {
                byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
                if (meta >= 0x7)
                {
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Wheat);
                    int amount = Entity.random.Next(4);
                    for (int i = 0; i < amount; i++)
                    {
                        a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Seeds);
                    }
                }
                else
                {
                    int amount = Entity.random.Next((int)(meta / 2) + 1);
                    for (int i = 0; i < amount; i++)
                    {
                        a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Seeds);
                    }
                }
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
                return false;
            }
			return true;
		}
		public static bool DestroyFurnace(Player a, BCS b)
		{
            return false;
		}
		public static bool DestroyDoorWood(Player a, BCS b)
		{
            byte type = a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
            if ((meta & 0x8) == 0)
            {
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z, 0, 0);
            }
            else
            {
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z, 0, 0);
            }

            if (Server.mode == 0)
            {
                a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.DoorWooden);
            }
			return false;
		}
		public static bool DestroyDoorIron(Player a, BCS b)
		{
            byte type = a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            byte meta = a.level.GetMeta((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
            if ((meta & 0x8) == 0)
            {
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y + 1, (int)b.pos.z, 0, 0);
            }
            else
            {
                if (a.level.GetBlock((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z) == type)
                    a.level.BlockChange((int)b.pos.x, (int)b.pos.y - 1, (int)b.pos.z, 0, 0);
            }

            if (Server.mode == 0)
            {
                a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.DoorIron);
            }
            return false;
		}
		public static bool DestroyRedstoneOre(Player a, BCS b)
		{
			return false;
		}
		public static bool DestroySnow(Player a, BCS b)
		{
            if (Server.mode == 0)
            {
                switch (a.inventory.current_item.item) {
                    case (short)Items.DiamondShovel:
                    case (short)Items.GoldShovel:
                    case (short)Items.IronShovel:
                    case (short)Items.StoneShovel:
                    case (short)Items.WoodenShovel:
                        a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Snowball);
                        break;
                }
            }
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
			return false;
		}
		public static bool DestroyCacti(Player a, BCS b)
		{
            while (a.level.GetBlock((int)b.pos.x, (int)b.pos.y, (int)b.pos.z) == (byte)Blocks.Cactus)
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y++, (int)b.pos.z, 0, 0);
                if (Server.mode == 0)
                {
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Blocks.Cactus);
                }
            }
			return false;
		}
		public static bool DestroyClay(Player a, BCS b)
		{
            if (Server.mode == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.Clay);
                }
            }
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
			return false;
		}
		public static bool DestroySugarCane(Player a, BCS b)
		{
            while (a.level.GetBlock((int)b.pos.x, (int)b.pos.y++, (int)b.pos.z) == (byte)Blocks.SugarCane)
            {
                a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
                if (Server.mode == 0)
                {
                    a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)Items.SugarCane);
                }
            }
			return false;
		}
		public static bool DestroyJukebox(Player a, BCS b)
		{
            // TODO: Tile entity stuff!
            ushort meta = a.level.GetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            if (meta != 0)
            {
                if (Server.mode == 0)
                {
                    if (meta >= 2256 && meta <= 2266)
                    {
                        a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, (short)meta);
                    }
                }
                a.level.UnsetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
                Player.GlobalSoundEffect(b.pos, 1005, 0, a.level);
            }
            return true;
		}
		public static bool DestroyGlowStone(Player a, BCS b)
		{
            if (Server.mode == 1) return true;
            int count = Entity.random.Next(2, 5); // 2-4 drops
            for (int i = 0; i < count; i++)
            {
                a.level.DropItem((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 348);
            }
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
            Player.GlobalBreakEffect(b.pos, b.ID, a.level, a);
            return false;
		}
        public static bool DestroyNoteBlock(Player a, BCS b)
        {
            a.level.UnsetExtra((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
            return true;
        }
        public static bool DestroyTNT(Player a, BCS b)
        {
            a.level.BlockChange((int)b.pos.x, (int)b.pos.y, (int)b.pos.z, 0, 0);
            Explosion exp = new Explosion(a.level, b.pos.x + .5, b.pos.y + .5, b.pos.z + .5, 4);
            exp.DoExplosionA();
            exp.DoExplosionB();
            return false;
        }

		public static byte DirectionByRotFlat(Player p, BCS a, bool invert = false)
		{
			byte direction = (byte)Math.Floor((double)((p.rot[0] * 4F) / 360F) + 0.5D);
            direction = (byte)(direction % 4);
            if (invert)
                switch (direction)
                {
                    case 0: return (byte)Directions.East;
                    case 1: return (byte)Directions.South;
                    case 2: return (byte)Directions.West;
                    case 3: return (byte)Directions.North;
                }
            else
                switch (direction)
                {
                    case 0: return (byte)Directions.West;
                    case 1: return (byte)Directions.North;
                    case 2: return (byte)Directions.East;
                    case 3: return (byte)Directions.South;
                }
            return 0;
		}
        public static byte DirectionByRotNOTFlat(Player p, BCS a, bool invert = false)
		{
            if (p.rot[1] > 45) return invert ? (byte)Directions.Top : (byte)Directions.Bottom;
            if (p.rot[1] < -45) return invert ? (byte)Directions.Bottom : (byte)Directions.Top;
			return DirectionByRotFlat(p, a, invert);
		}

		/// <summary>
		/// this one reverses the direction offset and returns the block id that was clicked
		/// this does not always need to be used, only if the direction offset has already been applied
		/// in the packet handling.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static byte Blockclicked(Player p, BCS a)
		{
			int x = (int)a.pos.x;
			int y = (int)a.pos.y;
			int z = (int)a.pos.z;

			switch (a.Direction)
			{
				case 0: y++; break;
				case 1: y--; break;
				case 2: z++; break;
				case 3: z--; break;
				case 4: x++; break;
				case 5: x--; break;
			}

			return p.level.GetBlock(x, y, z);
		}
        public static Point3 BlockclickedPos(Player p, BCS a)
        {
            int x = (int)a.pos.x;
            int y = (int)a.pos.y;
            int z = (int)a.pos.z;

            switch (a.Direction)
            {
                case 0: y++; break;
                case 1: y--; break;
                case 2: z++; break;
                case 3: z--; break;
                case 4: x++; break;
                case 5: x--; break;
            }

            return new Point3(x, y, z);
        }
        public static byte InvertDirection(byte direction)
        {
            switch (direction)
            {
                case (byte)Directions.Top:
                    return (byte)Directions.Bottom;
                case (byte)Directions.Bottom:
                    return (byte)Directions.Top;
                case (byte)Directions.North:
                    return (byte)Directions.South;
                case (byte)Directions.South:
                    return (byte)Directions.North;
                case (byte)Directions.East:
                    return (byte)Directions.West;
                case (byte)Directions.West:
                    return (byte)Directions.East;
            }
            return 0; // So the compiler will shut up.
        }
	}
	public struct BCS //BlockChangeStruct (This is used to hold the blockchange information)
	{
		public Point3 pos;
		public short ID;
		public byte Direction;
		public byte Amount;
		public short Damage;

		public BCS(Point3 pos, short id, byte direction)
		{
			this.pos = pos;
			ID = id;
			Direction = direction;
			Amount = 0;
			Damage = 0;
		}
		public BCS(Point3 pos, short id, byte direction, byte amount, short damage)
		{
			this.pos = pos;
			ID = id;
			Direction = direction;
			Amount = amount;
			Damage = damage;
		}
	}
}
