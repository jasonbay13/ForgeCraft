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
    public partial class World
    {
        public void SpreadLight(int x, int y, int z)
        {
            // TODO: Make this actually work worth a shit!
            //SpreadLightInternal(x, y, z);
            //SpreadBlockLightInternal(x, y, z);
        }

        private void SpreadLightInternal(int x, int y, int z, int dist = 0)
        {
            if (dist > 0xf) return;
            Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
            var currLight = chunk.GetSkyLight(x & 0xf, y, z & 0xf);
            if (currLight == 0) return;

            ForEveryAdjacentBlock(x, y, z, delegate(int xx, int yy, int zz)
            {
                var type = chunk.SGB(xx & 0xf, yy, z & 0xf);
                var light = chunk.GetSkyLight(xx & 0xf, yy, zz & 0xf);
                if (Chunk.LightOpacity[type] == 0 && light < currLight - 1)
                {
                    chunk.SetSkyLight(xx & 0xf, yy, zz & 0xf, (byte)(currLight - 1));
                    SpreadLightInternal(xx, yy, zz, dist + 1);
                }
            });
        }
        private void SpreadBlockLightInternal(int x, int y, int z, int dist = 0)
        {
            if (dist > 0xf) return;
            Chunk chunk = Chunk.GetChunk(x >> 4, z >> 4, this);
            var currLight = chunk.GetBlockLight(x & 0xf, y, z & 0xf);
            if (currLight == 0) return;

            ForEveryAdjacentBlock(x, y, z, delegate(int xx, int yy, int zz)
            {
                var type = chunk.SGB(xx & 0xf, yy, zz & 0xf);
                var light = chunk.GetBlockLight(xx & 0xf, yy, zz & 0xf);
                if (Chunk.LightOpacity[type] == 0 && light < currLight - 1)
                {
                    chunk.SetBlockLight(xx & 0xf, yy, zz & 0xf, (byte)(currLight - 1));
                    SpreadBlockLightInternal(xx, yy, zz, dist + 1);
                }
            });
        }

        delegate void BlockDel(int x, int y, int z);
        private void ForEveryAdjacentBlock(int x, int y, int z, BlockDel del)
        {
            if ((x & 0xf) > 0) del(x - 1, y, z);
            if ((x & 0xf) < 15) del(x + 1, y, z);
            if (y > 0) del(x, y - 1, z);
            if (y < 127) del(x, y + 1, z);
            if ((z & 0xf) > 0) del(x, y, z - 1);
            if ((z & 0xf) < 15) del(x, y, z + 1);
        }
    }
}
