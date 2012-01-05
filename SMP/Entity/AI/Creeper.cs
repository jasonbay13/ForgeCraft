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
	class Creeper : AI
	{
		public Entity e { get { return mye; } }
		public World level { get { return mylevel; } }
        public Point3 pos { get { return e.pos; } set { e.pos = value; } }
        public float[] rot { get { return e.rot; } set { e.rot = value; } }
		public byte type { get { return mytype; } }
		public byte[] meta { get { return mymeta; } set { mymeta = value; } }

		Entity mye;
		World mylevel = Server.mainlevel;
		byte mytype = Mob.Creeper;
		byte[] mymeta = new byte[4] { 0, 0, 0, 0 };

		public Creeper(Point3 pos, World level)
		{
			mye = new Entity(this, level);
			mylevel = level;
			e.pos = pos;

			e.UpdateChunks(false, false);
		}

		public void Update()
		{
            Player nearest = GetNearestPlayer();
			//TODO move this entity, you dont need to send movement, thats done in the player class... i think
		}
        public bool IsNear(Player p)
        {
            if (p.pos.diff(pos).x <= 5 && p.pos.diff(pos).y <= 1 && p.pos.diff(pos).z <= 5)
                return true;
            return false;
        }
        //If the Player and Creeper are staring at each other, then the creeper will explode (If there close enough)
        //If the creeper sees the player but the player doesnt see the creeper, the creeper will "creep" behind him
        public bool IsStaring(Player p)
        {
            return false;
        }
        public bool IsInView(Player p)
        {
            return true;
        }
        public Player GetNearestPlayer()
        {
            Player p1 = null;
            foreach (Player p in Player.players.ToArray())
            {
                if (IsNear(p) && IsInView(p))
                {
                    if ((p1.pos.diff(pos).x > p.pos.diff(pos).x || p1.pos.diff(pos).z > p.pos.diff(pos).z) && p1.pos.diff(pos).y >= p.pos.diff(pos).y)//p1 is always null
                        p1 = p;
                }
            }
            return p1;
        }


	}
}

