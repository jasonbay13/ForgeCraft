using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class PlayerHashEntry
    {
        public PlayerHashEntry(int i, long l, Object obj, PlayerHashEntry playerhashentry)
        {
            value = obj;
            nextEntry = playerhashentry;
            key = l;
            field_1026_d = i;
        }

        public long func_736_a()
        {
            return key;
        }

        public object func_735_b()
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is PlayerHashEntry))
            {
                return false;
            }
            PlayerHashEntry playerhashentry = (PlayerHashEntry)obj;
            long long1 = func_736_a();
            long long2 = playerhashentry.func_736_a();
            if(long1 == long2 || long1 != null && long1.Equals(long2))
            {
                object obj1 = func_735_b();
                object obj2 = playerhashentry.func_735_b();
                if(obj1 == obj2 || obj1 != null && obj1.Equals(obj2))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return PlayerHash.GetHashCode(key);
        }

        public override string ToString()
        {
            return (new StringBuilder()).Append(func_736_a()).Append("=").Append(func_735_b()).ToString();
        }

        public long key;
        public object value;
        public PlayerHashEntry nextEntry;
        public int field_1026_d;
    }
}
