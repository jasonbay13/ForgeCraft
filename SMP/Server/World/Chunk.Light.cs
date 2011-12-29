﻿/*
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
	/// <summary>
	/// Handles chunk lighting.
	/// </summary>
	public partial class Chunk
	{
        public void QuickRecalculateLight(int x, int y, int z)
        {
            sbyte curLight = (sbyte)GetSkyLight(x, y + 1, z); byte block;
            for (; y >= 0; y--)
            {
                block = blocks[PosToInt(x, y, z)];
                curLight -= Math.Min(LightOpacity[block], curLight);
                SetBlockLight(x, y, z, (byte)LightOutput[block]);
                SetSkyLight(x, y, z, (byte)curLight);
                if (LightOpacity[blocks[PosToInt(x, y - 1, z)]] == 0xf) break;
            }
        }

		public void RecalculateLight(int x, int z)
		{
			sbyte curLight = 0xf; byte block;
			for (int y = Height - 1; y >= 0; y--)
			{
				block = blocks[PosToInt(x, y, z)];
                curLight -= Math.Min(LightOpacity[block], curLight);
				SetSkyLight(x, y, z, (byte)curLight);
                SetBlockLight(x, y, z, (byte)LightOutput[block]);
			}
		}

		public void RecalculateLight()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int z = 0; z < Depth; z++)
				{
					RecalculateLight(x, z);
				}
			}
		}

        #region Light spread is now in the World class!
        public void SpreadLight() {
            for( int x = 0; x < Chunk.Width; x++ ) {
                for( int z = 0; z < Chunk.Depth; z++ ) {
                    for( int y = 0; y < Chunk.Height; y++ ) {
                        byte type = blocks[x << 11 | z << 7 | y];
                        if (LightOpacity[type] == 0 && GetSkyLight(x, y, z) == 0xf)
                            SpreadLightInternal(x, y, z);
                        if (GetBlockLight(x, y, z) > 0)
                        {
                            SpreadBlockLightInternal(x, y, z);
                        }
                    }
                }
            }
        }

        internal void SpreadLightInternal( int x, int y, int z ) {
            var currLight = GetSkyLight(x, y, z);
            if (currLight == 0) return;

            ForEveryAdjacentBlock( x, y, z, delegate( int xx, int yy, int zz ) {
                var type = blocks[ xx << 11 | zz << 7 | yy ];
                var light = GetSkyLight(xx, yy, zz);
                if(LightOpacity[type] == 0 && light < currLight)
                {
                    SetSkyLight( xx, yy, zz, (byte)(currLight - 1) );
                    SpreadLightInternal( xx, yy, zz );
                }
            } );
        }
        private void SpreadBlockLightInternal(int x, int y, int z)
        {
            var currLight = GetBlockLight(x, y, z);
            if (currLight == 0) return;

            ForEveryAdjacentBlock(x, y, z, delegate(int xx, int yy, int zz)
            {
                var type = blocks[xx << 11 | zz << 7 | yy];
                var light = GetBlockLight(xx, yy, zz);
                Console.WriteLine(light);
                if (LightOpacity[type] == 0 && light < currLight)
                {
                    SetBlockLight(xx, yy, zz, (byte)(currLight - 1));
                    SpreadBlockLightInternal(xx, yy, zz);
                }
            });
        }
        #endregion

        delegate void BlockDel( int x, int y, int z );
        private void ForEveryAdjacentBlock( int x, int y, int z, BlockDel del ) {
            if( x > 0 ) del( x - 1, y, z );
            if( x < 15 ) del( x + 1, y, z );
            if( y > 0 ) del( x, y - 1, z );
            if( y < 127 ) del( x, y + 1, z );
            if( z > 0 ) del( x, y, z - 1 );
            if( z < 15 ) del( x, y, z + 1 );
        }


        public static readonly sbyte[] LightOpacity = new sbyte[] {
            0x0, 0xf, 0xf, 0xf, 0xf, 0xf, 0x0, 0xf, 0x2, 0x2, 0x0, 0x0, 0xf, 0xf, 0xf, 0xf, // 0-15
            0xf, 0xf, 0x1, 0xf, 0x0, 0xf, 0xf, 0xf, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, // 16-31
            0x0, 0x0, 0x0, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0xf, 0xf, 0xf, 0x0, 0xf, 0xf, 0xf, // 32-47
            0xf, 0xf, 0x0, 0x0, 0x0, 0xf, 0x0, 0x0, 0xf, 0xf, 0xf, 0x0, 0x0, 0xf, 0xf, 0x0, // 48-63
            0x0, 0x0, 0x0, 0xf, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0x2, // 64-79
            0xf, 0x0, 0xf, 0x0, 0xf, 0x0, 0xf, 0xf, 0xf, 0xf, 0x0, 0xf, 0x0, 0x0, 0x0, 0xf, // 80-95
            0x0, 0xf, 0xf, 0xf, 0xf, 0x0, 0x0, 0xf, 0x0, 0x0, 0x0, 0x0, 0xf, 0xf, 0xf, 0x0, // 96-111
            0xf, 0x0, 0xf, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0x0 // 112-122
        };

        public static readonly sbyte[] LightOutput = new sbyte[] {
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, // 0-15
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, // 16-31
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, // 32-47
            0x0, 0x0, 0xe, 0xf, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xd, 0x0, // 48-63
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x9, 0x0, 0x7, 0x0, 0x0, 0x0, // 64-79
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0xb, 0xf, 0x0, 0x0, 0x9, 0x0, // 80-95
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, // 96-111
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0x0, 0x0 // 112-122
        };
	}
}
