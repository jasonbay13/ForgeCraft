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
        private Player p;
        private short _Level = 0;
        private short _LevelExp = 0;
        private short _Experience = 0;

        public Player player { get { return p; } }
        public float Bar { get { return (float)MathHelper.Clamp((float)_LevelExp / (float)NeededLevelExp, 0F, 1F); } }
        public short Level { get { return _Level; } set { _Level = value; } }
        public short LevelExp { get { return _LevelExp; } set { _LevelExp = value; } }
        public short NeededExp { get { return (short)(3.5 * (_Level + 1) * (_Level + 2)); } }
        public short NeededLevelExp { get { return (short)((_Level + 1) * 7); } }
        public short TotalExp { get { return _Experience; } set { _Experience = value; } }

        public Experience(Player p) : this(p, 0) { }
        public Experience(Player p, short exp)
        {
            this.p = p;
            this._Experience = exp;
        }

        /// <summary>
        /// Adds a number to players experience
        /// </summary>
        /// <param name="p">Player, duh!</param>
        /// <param name="exp">The ammount of experience to add</param>
        public void Add(short exp)
        {
            if (p.CheckEXPGain(exp))
                return;

            short oldLevel = _Level;
            for (int i = 0; i < exp; i++)
            {
                _Experience++;
                _LevelExp++;
                if (_Experience >= NeededExp)
                {
                    _LevelExp = 0;
                    _Level++;
                }
            }

            if (_Level > oldLevel) p.SendMessage("Congratulations! You are now level " + _Level); // The reward is annoying as hell!
            p.SendExperience(Bar, _Level, _Experience);
        }

        /// <summary>
        /// Removes ammount of exp (untested!)
        /// </summary>
        /// <param name="p">Player, duh!</param>
        /// <param name="exp">The ammount of experience to remove</param>
        public void Remove(short exp)
        {
            if (p.CheckEXPLost(exp))
                return;

            short oldLevel = _Level;
            for (int i = 0; i < exp; i++)
            {
                _Experience--;
                _LevelExp--;
                _Level--;
                if (_Experience >= NeededExp) _Level++;
                else _LevelExp = (short)(NeededLevelExp - 1);
            }

            if (_Level < oldLevel) p.SendMessage("You have been demoted to level " + _Level);
            p.SendExperience(Bar, _Level, _Experience);
        }

        public void AddLevel(short level)
        {
            for (int i = 0; i < level; i++)
                Add((short)(NeededLevelExp - _LevelExp));
        }

        /*void RewardItem()
        {
            switch (_Experience)
            {
                case 7: p.inventory.Add(277, 1, 0); break;
                case 21: p.inventory.Add(278, 1, 0); break;
                case 42: p.inventory.Add(345, 1, 0); break;
                case 70: p.inventory.Add(347, 1, 0); break;
                case 105: p.inventory.Add(358, 1, 0); break;
                default: break;
            }
        }*/
    }
}
