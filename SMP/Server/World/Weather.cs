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
using System.Threading;
using System.Timers;
using SMP.PLAYER;

namespace SMP
{

    public partial class World
    {
        public Random random = new Random();
        public bool IsRaining = false;

        //public Weather()
        //    :base(0, 127, 0, "main", new Random().Next())
        //{
        //}

        public System.Timers.Timer weatherTimer = new System.Timers.Timer();
        public System.Timers.Timer lightningTimer = new System.Timers.Timer();
        public void InitializeTimers()
        {
            weatherTimer.Elapsed += WeatherTimer;
            weatherTimer.Interval = random.Next(30 * 60000, 90 * 60000);  // 30-90 mins
            weatherTimer.Start();

            lightningTimer.Elapsed += LightningTimer;
            lightningTimer.Interval = random.Next(1000, 15000);  // 1-15 seconds
            lightningTimer.Start();
        }

        public void WeatherTimer(Object Source, ElapsedEventArgs e)
        {
            Rain(true);
            System.Threading.Thread.Sleep(20 * 60000); //lasts for 20 mins
			Rain(false);

            weatherTimer.Interval = random.Next(30 * 60000, 90 * 60000);

        }
        public void LightningTimer(Object Source, ElapsedEventArgs e)
        {
            try
            {
                // TODO: Make this not use Player stuffs.
                if (IsRaining && Player.players.Count > 0)
                {
                    Player p = Player.players[random.Next(Player.players.Count)];
                    if (p.LoggedIn && p.MapLoaded)
                    {
                        int x, z, range = p.viewdistance << 4;
                        for (int i = 0; i < 50; i++)
                        {
                            x = random.Next((int)p.pos.x - range, (int)p.pos.x + range);
                            z = random.Next((int)p.pos.z - range, (int)p.pos.z + range);

                            if (chunkManager.getBiomeGenAt(x, z).canSpawnLightningBolt())
                            {
                                Lightning(x, GetTopSolidOrLiquidBlock(x, z), z);
                                break;
                            }
                        }
                    }

                    lightningTimer.Interval = random.Next(1000, 15000);
                }
                else lightningTimer.Interval = 1000;
            }
            catch { }
        }

    }
}
