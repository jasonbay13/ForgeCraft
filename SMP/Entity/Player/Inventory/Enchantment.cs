using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.PLAYER.Enchantments
{
    public class Enchantment
    {
        public short id;
        public short level;

        public Enchantment(short id, short level)
        {
            this.id = id;
            this.level = level;
        }

        // TODO: Add other stuff needed for enchantments!
    }
}
