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

namespace SMP
{

    public partial class World
    {


        public bool Israining = false;

        //public Weather()
        //    :base(0, 127, 0, "main", new Random().Next())
        //{
        //}

        public void SendLightning(int x, int y, int z, int EntityId, Player p)
        {
            byte[] bytes = new byte[17];
            util.EndianBitConverter.Big.GetBytes(EntityId).CopyTo(bytes, 0);
            util.EndianBitConverter.Big.GetBytes(true).CopyTo(bytes, 4);
            util.EndianBitConverter.Big.GetBytes(x).CopyTo(bytes, 5);
            util.EndianBitConverter.Big.GetBytes(y).CopyTo(bytes, 9);
            util.EndianBitConverter.Big.GetBytes(z).CopyTo(bytes, 13);
            p.SendRaw(0x47, bytes);
        }
        public void rain(bool on)
        {

            if (on)
            {

                byte[] bytes = new byte[1];
                byte thisin = 1;
                bytes[0] = thisin;
                foreach (Player p in Player.players)
                {
                    p.SendRaw(0x46, bytes);
                }
                Israining = true;
                // p.SendMessage("Weather is: " + Israining.ToString());
                return;

            }
            if (!on)
            {
                byte[] bytes = new byte[1];
                bytes[0] = 2;
                foreach (Player p in Player.players)
                {
                    p.SendRaw(0x46, bytes);
                }
                Israining = false;
                return;
                // p.SendMessage("Weather is: " + Israining.ToString());
            }
            //
            //{

            //    Israining = false;
            //}
            //else
            //{
            //    Israining = true;
            //}




        }


        public System.Timers.Timer weatherTimer = new System.Timers.Timer();
        public System.Timers.Timer lightningTimer = new System.Timers.Timer();
        public void InitializeTimers()
        {
            Random rnd = new Random();
            this.weatherTimer.Elapsed += new ElapsedEventHandler(WeatherTimer);
            this.weatherTimer.Interval = rnd.Next(1800000, 5400000);  //30 mins to 90 mins
            this.weatherTimer.Enabled = true;

            this.lightningTimer.Elapsed += new ElapsedEventHandler(LightningTimer);
            this.lightningTimer.Interval = 300000;  //5 mins
            this.lightningTimer.Enabled = false;
        }

        public void WeatherTimer(Object Source, ElapsedEventArgs e)
        {
            rain(true);
            lightningTimer.Start();

            System.Threading.Thread.Sleep(1200000); //lasts for 20 mins

            rain(false);
            lightningTimer.Stop();

        }
        public void LightningTimer(Object Source, ElapsedEventArgs e)
        {

            if (Israining && weatherTimer.Enabled)
            {
                foreach (Player p in Player.players)
                {
                    Random rnd = new Random();
                    SendLightning(rnd.Next(3000), rnd.Next(3000), rnd.Next(3000), 10, p);
                }

            }

        }

    }
}
