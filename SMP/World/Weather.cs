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
        public void rain(bool on, Player p)
        {
            
           if (on)
           {
               
                byte[] bytes = new byte[1];
                byte thisin = 1;
                bytes[0] = thisin;
                p.SendRaw(0x46, bytes);
                Israining = true;
               // p.SendMessage("Weather is: " + Israining.ToString());
                return;

            }
            if(!on)
            {
                byte[] bytes = new byte[1];
                bytes[0] = 2;
                p.SendRaw(0x46, bytes);
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
        public void rainTimer()
        { }
        public bool isRain()
        {
            if (Israining)
            {
                return true;
            }
            else
                return false;
        }
        public void setthemotherfinrainon()
        {
            try
            {
                Israining.Equals(true);
                Israining = true;
            }
            catch (Exception e)
            { Player.GlobalMessage(e.Message); }
        }

   
    }
}
