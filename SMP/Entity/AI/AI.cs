/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
	
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

namespace SMP.ENTITY
{
	public interface AI
	{
		Entity e { get; }
		Vector3 pos { get; set;}
		float[] rot { get; set; }
		World level { get; }
		byte type { get; }
		byte[] meta { get; set; }

		void Update();
	}
    public class Mob
    {
        public const byte Creeper = 50;
        public const byte Skeleton = 51;
        public const byte Spider = 52;
        public const byte Giant = 53;
        public const byte Zombie = 54;
        public const byte Slime = 55;
        public const byte Ghast = 56;
        public const byte ZombiePig = 57;
        public const byte Enderman = 58;
        public const byte CaveSpider = 59;
        public const byte SilverFish = 60;
        public const byte Blaze = 61;
        public const byte MagmaSlime = 62;
        public const byte EnderDragon = 63;
        public const byte Pig = 90;
        public const byte Sheep = 91;
        public const byte Cow = 92;
        public const byte Chicken = 93;
        public const byte Squid = 94;
        public const byte Wolf = 95;
        public const byte Mooshroom = 96;
        public const byte Snowman = 97;
        public const byte Villager = 120;
        public class Mobs
        {
            /// <summary>
            /// Mob type (see AIs class)
            /// </summary>
            public byte type;
            /// <summary>
            /// x and z dimensions of the mob (width - length)
            /// </summary>
            public double width;
            /// <summary>
            /// y dimension of the mob (height)
            /// </summary>
            public double height;
            public static List<Mobs> all = new List<Mobs>();
            public Mobs(byte b, double xz, double y)
            {
                type = b;
                width = xz;
                height = y;
                all.Add(this);
            }
            public static void SetMobtypes()
            {
                new Mobs(50, 0.6, 1.8);
                new Mobs(51, 0.6, 1.8);
                new Mobs(52, 1.4, 0.9);
                new Mobs(53, 3.6, 10.8);
                new Mobs(54, 0.6, 1.8);
                new Mobs(55, 0.6, 0.6); // slime
                new Mobs(56, 4, 4);
                new Mobs(57, 0.6, 1.8);
                new Mobs(58, 0.6, 2.7); // enderman = 3 blocks, each block = + 0.9 = 2.7?
                new Mobs(59, 1.4, 0.9); // Cave spider - fix this
                new Mobs(60, 0.6, 0.6); // Silverfish - fix this
                new Mobs(61, 0.6, 1.8); // Blaze - fix this
                new Mobs(62, 0.6, 0.6); // 2nd slime
                new Mobs(63, 3.0, 3.0); // Enderdragon... BIG?! - Fix this
                new Mobs(90, 0.9, 0.9);
                new Mobs(91, 0.6, 1.3);
                new Mobs(92, 0.9, 1.3);
                new Mobs(93, 0.3, 0.4);
                new Mobs(94, 0.95, 0.95);
                new Mobs(95, 0.6, 1.8);
                new Mobs(96, 0.6, 1.3); // mooshroom - copied cow
                new Mobs(97, 0.9, 1.8); // snowman... guessed
                new Mobs(120, 0.9, 1.8); // Player size?
            }
        }
    }
}
