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

namespace SMP
{
    public class Experience
    {
        byte _Experience = 0;
        byte _Level = 0;
        short _TotalExp = 0;
        public byte Bar { get { return _Experience; } set { _Experience = value; } }
        public byte Level { get { return _Level; } set { _Level = value; } }
        public short Total { get { return _TotalExp; } set { _TotalExp = value; } }

        //public void Add(Player p, byte level) { Add(p, (short)(((short)level) * 20)); }
        /// <summary>
        /// Adds a number to players experience
        /// </summary>
        /// <param name="p">Player, duh!</param>
        /// <param name="exp">The ammount of experience to add</param>
        public void Add(Player p, short exp)
        {
            byte oldlevel = _Level;
            for (int i = 0; i < exp; i++)
            {
                _TotalExp++;
                if (_TotalExp > 780) { _Experience = 0; _Level = 12; continue; }
                _Experience++;
                if (_Experience == ((_Level + 1) * 10)) { _Experience = 0; _Level++; }
            }
            if (oldlevel < _Level) { p.SendMessage("Congratulations! You are now level " + _Level); /*RewardItem(p);*/ } // The reward is annoying as hell!
            //if (_Experience > 127) { _Experience = 127; }

            /*switch (_TotalExp)
            {
                case 10: p.inventory.Add(277, 1, 0); break;
                case 30: p.inventory.Add(278, 1, 0); break;
                case 60: p.inventory.Add(345, 1, 0); break;
                case 100: p.inventory.Add(347, 1, 0); break;
                case 210: p.inventory.Add(358, 1, 0); break;
            }*/
            //_TotalExp += exp;
            //_Level = (byte)(_TotalExp / 10);
            //_Experience = (byte)(_TotalExp - (_Level * 10));

            //Server.Log(_Experience + " " + _Level + " " + _TotalExp);
            //Player.GlobalMessage("Bar= " + _Experience + " level = " + _Level + " totalexp = " + _TotalExp);
            SendExperience(p, _Experience, _Level, _TotalExp);
        }

        /// <summary>
        /// Removes ammount of exp (untested!)
        /// </summary>
        /// <param name="p">Player, duh!</param>
        /// <param name="exp">The ammount of experience to remove</param>
        public void Remove(Player p, short exp)
        {
            byte oldlevel = _Level;
            for (int i = 0; i < exp; i++)
            {
                _TotalExp--;
                if (_TotalExp > 780) { _Experience = 0; _Level = 12; continue; }
                _Experience--;
                if (_Experience == (_Level * 10)) { _Experience = (byte)((_Level * 10) - 1); _Level--; }
            }
            if (_Level < oldlevel) p.SendMessage("You have been demoted to level " + _Level);
            SendExperience(p, _Experience, _Level, _TotalExp);
        }

        void RewardItem(Player p)
        {
            switch (_TotalExp)
            {
                case 10: p.inventory.Add(277, 1, 0); break;
                case 30: p.inventory.Add(278, 1, 0); break;
                case 60: p.inventory.Add(345, 1, 0); break;
                case 100: p.inventory.Add(347, 1, 0); break;
                case 210: p.inventory.Add(358, 1, 0); break;
                default: break;
            }
        }

        /// <summary>
        /// Updates the players experience bar
        /// </summary>
        /// <param name="expbarval">Value of the experience bar (0-19)</param>
        /// <param name="level">Ecperience level of player</param>
        /// <param name="totalexp">Players total experience</param>
        public static void SendExperience(Player p, byte expbarval, byte level, short totalexp)
        {
            byte[] bytes = new byte[4];
            bytes[0] = expbarval;
            bytes[1] = level;
            util.EndianBitConverter.Big.GetBytes(totalexp).CopyTo(bytes, 2);
            p.SendRaw(0x2B, bytes);
        }
    }
}
