using System;

namespace SMP
{
    public class LongHashMap
    {
        public LongHashMap()
        {
            capacity = 12;
            hashArray = new LongHashMapEntry[16];
        }

        private static int getHashedKey(long l)
        {
            return hash((int)((ulong)l ^ (ulong)l >> 32));
        }

        private static int hash(int i)
        {
            i ^= (int)((uint)i >> 20 ^ (uint)i >> 12);
            return i ^ (int)((uint)i >> 7 ^ (uint)i >> 4);
        }

        private static int getHashIndex(int i, int j)
        {
            return i & j - 1;
        }

        public Object getValueByKey(long l)
        {
            int i = getHashedKey(l);
            for(LongHashMapEntry longhashmapentry = hashArray[getHashIndex(i, hashArray.Length)]; longhashmapentry != null; longhashmapentry = longhashmapentry.nextEntry)
            {
                if(longhashmapentry.key == l)
                {
                    return longhashmapentry.value;
                }
            }

            return null;
        }

        public bool func_35508_b(long l)
        {
            return func_35507_c(l) != null;
        }

        LongHashMapEntry func_35507_c(long l)
        {
            int i = getHashedKey(l);
            for(LongHashMapEntry longhashmapentry = hashArray[getHashIndex(i, hashArray.Length)]; longhashmapentry != null; longhashmapentry = longhashmapentry.nextEntry)
            {
                if(longhashmapentry.key == l)
                {
                    return longhashmapentry;
                }
            }

            return null;
        }

        public void add(long l, Object obj)
        {
            int i = getHashedKey(l);
            int j = getHashIndex(i, hashArray.Length);
            for(LongHashMapEntry longhashmapentry = hashArray[j]; longhashmapentry != null; longhashmapentry = longhashmapentry.nextEntry)
            {
                if(longhashmapentry.key == l)
                {
                    longhashmapentry.value = obj;
                }
            }

            modCount++;
            createKey(i, l, obj, j);
        }

        private void resizeTable(int i)
        {
            LongHashMapEntry[] alonghashmapentry = hashArray;
            int j = alonghashmapentry.Length;
            if(j == 0x40000000)
            {
                capacity = 0x7fffffff;
                return;
            } else
            {
                LongHashMapEntry[] aplayerhashentry1 = new LongHashMapEntry[i];
                copyHashTableTo(aplayerhashentry1);
                hashArray = aplayerhashentry1;
                capacity = (int)((float)i * percentUsable);
                return;
            }
        }

        private void copyHashTableTo(LongHashMapEntry[] alonghashmapentry)
        {
            LongHashMapEntry[] alonghashmapentry1 = hashArray;
            int i = alonghashmapentry.Length;
            for(int j = 0; j < alonghashmapentry1.Length; j++)
            {
                LongHashMapEntry longhashmapentry = alonghashmapentry1[j];
                if(longhashmapentry == null)
                {
                    continue;
                }
                alonghashmapentry1[j] = null;
                do
                {
                    LongHashMapEntry playerhashentry1 = longhashmapentry.nextEntry;
                    int k = getHashIndex(longhashmapentry.field_1026_d, i);
                    longhashmapentry.nextEntry = alonghashmapentry[k];
                    alonghashmapentry[k] = longhashmapentry;
                    longhashmapentry = playerhashentry1;
                } while(longhashmapentry != null);
            }

        }

        public Object remove(long l)
        {
            LongHashMapEntry longhashmapentry = removeKey(l);
            return longhashmapentry != null ? longhashmapentry.value : null;
        }

        LongHashMapEntry removeKey(long l)
        {
            int i = getHashedKey(l);
            int j = getHashIndex(i, hashArray.Length);
            LongHashMapEntry longhashmapentry = hashArray[j];
            LongHashMapEntry longhashmapentry1;
            LongHashMapEntry longhashmapentry2;
            for(longhashmapentry1 = longhashmapentry; longhashmapentry1 != null; longhashmapentry1 = longhashmapentry2)
            {
                longhashmapentry2 = longhashmapentry1.nextEntry;
                if(longhashmapentry1.key == l)
                {
                    modCount++;
                    numHashElements--;
                    if(longhashmapentry == longhashmapentry1)
                    {
                        hashArray[j] = longhashmapentry2;
                    } else
                    {
                        longhashmapentry.nextEntry = longhashmapentry2;
                    }
                    return longhashmapentry1;
                }
                longhashmapentry = longhashmapentry1;
            }

            return longhashmapentry1;
        }

        private void createKey(int i, long l, Object obj, int j)
        {
            LongHashMapEntry longhashmapentry = hashArray[j];
            hashArray[j] = new LongHashMapEntry(i, l, obj, longhashmapentry);
            if(numHashElements++ >= capacity)
            {
                resizeTable(2 * hashArray.Length);
            }
        }

        public static int GetHashCode(long l)
        {
            return getHashedKey(l);
        }

        [NonSerialized] private LongHashMapEntry[] hashArray;
        [NonSerialized] private int numHashElements;
        private int capacity;
        private readonly float percentUsable = 0.75F;
        [NonSerialized] private volatile int modCount;
    }
}
