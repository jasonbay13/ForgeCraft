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

namespace SMP.API
{
    public enum PluginEvents
    {
        PluginLoad,
        PluginUnload
    }
    public enum PlayerEvents
    {
        PlayerCommand,
        PlayerChat,
        PlayerMove,
        PlayerDig,
        MessageRecieve,
        PlayerKick,
        WindowClose,
        EXPGain,
        EXPLost,
        EntityAttack,
        EntityRightClick,
        BlockLeftClick,
        BlockRightClick,
        BlockPlace,
        BlockBreak,
        ItemUse,
        Respawn
    }
    public enum WindowEvent
    {
        RightClick,
        LeftClick
    }
    public enum PhysicsEvent
    {
        ButtonRelease,
        WaterFlow,
        LavaFlow,
    }
    public enum LevelEvent
    {
        BlockChange,
        PhysicsUpdate,
        Load,
        Save,
        Unload,
        ChunkGenerated
    }
    public enum ServerEvent
    {
        Log,
        ErrorLog
    }
}
