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

namespace SMP
{
    /// <summary>
    /// This class will allow plugin / custom command devs to cancel events
    /// To cancel an event you would call
    /// Plugin.CancelEvent
    /// And depending on what type of event you want to cancel, fill in the ( )
    /// </summary>
	public partial class Plugin
	{
        public static bool IsEventCancled(LevelEvent e, World w)
        {
            return true;
        }
        public static bool IsEventCancled(ServerEvent e)
        {
            return true;
        }
        /// <summary>
        /// Check to see if a Player event is stopped
        /// </summary>
        /// <param name="e">The event to check</param>
        /// <param name="p">The Player that event is related to</param>
        /// <returns>This returns true or false, true means its stopped, false means its not</returns>
        public static bool IsEventCancled(PlayerEvents e, Player p)
        {
            switch (e)
            {
                case PlayerEvents.PlayerBlockChange:
                    return p.cancelBlock;
                case PlayerEvents.PlayerChat:
                    return p.cancelchat;
                case PlayerEvents.PlayerCommand:
                    return p.cancelcommand;
                case PlayerEvents.PlayerMove:
                    return p.cancelmove;
                case PlayerEvents.PlayerDig:
                    return p.canceldig;
                default:
                    return false;
            }
        }
        public static void CancelEvent(LevelEvent e, World w)
        {

        }
        public static void CancelEvent(ServerEvent e)
        {

        }
        /// <summary>
        /// Cancel a Player event
        /// </summary>
        /// <param name="e">The event that you want to cancel</param>
        /// <param name="p">The Player that event is related to (null if not dealing with player event)</param>
        public static void CancelEvent(PlayerEvents e, Player p) {
            //TODO
            //Add some more events to be canceled
            switch (e)
            {
                case PlayerEvents.PlayerBlockChange:
                    p.cancelBlock = true;
                    break;
                case PlayerEvents.PlayerChat:
                    p.cancelchat = true;
                    break;
                case PlayerEvents.PlayerCommand:
                    p.cancelcommand = true;
                    break;
                case PlayerEvents.PlayerMove:
                    p.cancelmove = true;
                    break;
                case PlayerEvents.PlayerDig:
                    p.canceldig = true;
                    break;
            }
        }
	}
}
