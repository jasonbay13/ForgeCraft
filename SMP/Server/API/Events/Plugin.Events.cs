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
using SMP.PLAYER;

namespace SMP.API
{
    /// <summary>
    /// This class will allow plugin / custom command devs to cancel events
    /// To cancel an event you would call
    /// Plugin.CancelEvent
    /// And depending on what type of event you want to cancel, fill in the ( )
    /// </summary>
	public partial class Plugin
	{
        /// <summary>
        /// Check to see if World Event is canceled
        /// </summary>
        /// <param name="e">The event to check</param>
        /// <param name="w">The world</param>
        /// <returns></returns>
        public static bool IsEventCancled(LevelEvent e, World w)
        {
            switch (e)
            {
                case LevelEvent.PhysicsUpdate:
                    return w.physics.cancelphysics;
                case LevelEvent.ChunkGenerated:
                    return w.cancelchunk;
                case LevelEvent.Save:
                    return World.cancelsave;
                default:
                    return false;
            }
        }
        public static bool IsEventCancled(ServerEvent e)
        {
            return false;
        }
        /// <summary>
        /// Cancel a window event
        /// </summary>
        /// <param name="e">The event to cancel</param>
        /// <param name="w">The window</param>
        /// <returns></returns>
        public static void CancelEvent(WindowEvent e, Windows w)
        {
            switch (e)
            {
                case WindowEvent.LeftClick:
                    w.cancelclick = true;
                    break;
                case WindowEvent.RightClick:
                    w.cancelright = true;
                    break;
            }
        }
        /// <summary>
        /// Check to see if a event is canceled
        /// </summary>
        /// <param name="e">The event to check</param>
        /// <param name="w">The window</param>
        /// <returns></returns>
        public static bool IsEventCancled(WindowEvent e, Windows w)
        {
            switch (e)
            {
                case WindowEvent.LeftClick:
                    return w.cancelclick;
                case WindowEvent.RightClick:
                    return w.cancelright;
                default:
                    return false;
            }
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
                case PlayerEvents.PlayerChat:
                    return p.cancelchat;
                case PlayerEvents.PlayerCommand:
                    return p.cancelcommand;
                case PlayerEvents.PlayerMove:
                    return p.cancelmove;
                case PlayerEvents.PlayerDig:
                    return p.canceldig;
                case PlayerEvents.PlayerKick:
                    return p.cancelkick;
                case PlayerEvents.WindowClose:
                    return p.cancelclose;
                case PlayerEvents.EXPGain:
                    return p.cancelgain;
                case PlayerEvents.EXPLost:
                    return p.cancellost;
                case PlayerEvents.Respawn:
                    return p.cancelrespawn;
                case PlayerEvents.EntityAttack:
                    return p.cancelentityleft;
                case PlayerEvents.EntityRightClick:
                    return p.cancelentityright;
                case PlayerEvents.BlockRightClick:
                    return p.cancelblockright;
                case PlayerEvents.BlockLeftClick:
                    return p.cancelblockleft;
                case PlayerEvents.BlockPlace:
                    return p.cancelplace;
                case PlayerEvents.BlockBreak:
                    return p.cancelbreak;
                case PlayerEvents.ItemUse:
                    return p.cancelitemuse;
                case PlayerEvents.MessageRecieve:
                    return p.cancelmessage;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Cancel a World Event
        /// </summary>
        /// <param name="e">The event to cancel</param>
        /// <param name="w">The world</param>
        public static void CancelEvent(LevelEvent e, World w)
        {
            switch (e)
            {
                case LevelEvent.PhysicsUpdate:
                    w.physics.cancelphysics = true;
                    break;
                case LevelEvent.Save:
                    World.cancelsave = true;
                    break;
                case LevelEvent.ChunkGenerated:
                    w.cancelchunk = true;
                    break;
            }
        }
        public static void CancelEvent(ServerEvent e)
        {

        }
        /// <summary>
        /// Cancel a Player event
        /// </summary>
        /// <param name="e">The event that you want to cancel</param>
        /// <param name="p">The Player</param>
        public static void CancelEvent(PlayerEvents e, Player p) {
            //TODO
            //Add some more events to be canceled
            switch (e)
            {
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
                case PlayerEvents.PlayerKick:
                    p.cancelkick = true;
                    break;
                case PlayerEvents.EXPGain:
                    p.cancelgain = true;
                    break;
                case PlayerEvents.EXPLost:
                    p.cancellost = true;
                    break;
                case PlayerEvents.WindowClose:
                    p.cancelclose = true;
                    break;
                case PlayerEvents.Respawn:
                    p.cancelrespawn = true;
                    break;
                case PlayerEvents.EntityAttack:
                    p.cancelentityleft = true;
                    break;
                case PlayerEvents.EntityRightClick:
                    p.cancelentityright = true;
                    break;
                case PlayerEvents.BlockRightClick:
                    p.cancelblockright = true;
                    break;
                case PlayerEvents.BlockLeftClick:
                    p.cancelblockleft = true;
                    break;
                case PlayerEvents.BlockPlace:
                    p.cancelplace = true;
                    break;
                case PlayerEvents.BlockBreak:
                    p.cancelbreak = true;
                    break;
                case PlayerEvents.ItemUse:
                    p.cancelitemuse = true;
                    break;
                case PlayerEvents.MessageRecieve:
                    p.cancelmessage = true;
                    break;
            }
        }
	}
}
