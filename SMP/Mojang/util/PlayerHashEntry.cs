using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class LongHashMapEntry
    {
        public LongHashMapEntry(int i, long l, Object obj, LongHashMapEntry playerhashentry)
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
            if(!(obj is LongHashMapEntry))
            {
                return false;
            }
            LongHashMapEntry playerhashentry = (LongHashMapEntry)obj;
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
            return LongHashMap.GetHashCode(key);
        }

        public override string ToString()
        {
            return (new StringBuilder()).Append(func_736_a()).Append("=").Append(func_735_b()).ToString();
        }

        public long key;
        public object value;
        public LongHashMapEntry nextEntry;
        public int field_1026_d;
    }
}
