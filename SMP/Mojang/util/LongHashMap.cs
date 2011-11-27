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
            for(LongHashMapEntry playerhashentry = hashArray[getHashIndex(i, hashArray.Length)]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
            {
                if(playerhashentry.key == l)
                {
                    return playerhashentry.value;
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
            for(LongHashMapEntry playerhashentry = hashArray[getHashIndex(i, hashArray.Length)]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
            {
                if(playerhashentry.key == l)
                {
                    return playerhashentry;
                }
            }

            return null;
        }

        public void add(long l, Object obj)
        {
            int i = getHashedKey(l);
            int j = getHashIndex(i, hashArray.Length);
            for(LongHashMapEntry playerhashentry = hashArray[j]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
            {
                if(playerhashentry.key == l)
                {
                    playerhashentry.value = obj;
                }
            }

            field_950_e++;
            createKey(i, l, obj, j);
        }

        private void resizeTable(int i)
        {
            LongHashMapEntry[] aplayerhashentry = hashArray;
            int j = aplayerhashentry.Length;
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

        private void copyHashTableTo(LongHashMapEntry[] aplayerhashentry)
        {
            LongHashMapEntry[] aplayerhashentry1 = hashArray;
            int i = aplayerhashentry.Length;
            for(int j = 0; j < aplayerhashentry1.Length; j++)
            {
                LongHashMapEntry playerhashentry = aplayerhashentry1[j];
                if(playerhashentry == null)
                {
                    continue;
                }
                aplayerhashentry1[j] = null;
                do
                {
                    LongHashMapEntry playerhashentry1 = playerhashentry.nextEntry;
                    int k = getHashIndex(playerhashentry.field_1026_d, i);
                    playerhashentry.nextEntry = aplayerhashentry[k];
                    aplayerhashentry[k] = playerhashentry;
                    playerhashentry = playerhashentry1;
                } while(playerhashentry != null);
            }

        }

        public Object remove(long l)
        {
            LongHashMapEntry playerhashentry = removeKey(l);
            return playerhashentry != null ? playerhashentry.value : null;
        }

        LongHashMapEntry removeKey(long l)
        {
            int i = getHashedKey(l);
            int j = getHashIndex(i, hashArray.Length);
            LongHashMapEntry playerhashentry = hashArray[j];
            LongHashMapEntry playerhashentry1;
            LongHashMapEntry playerhashentry2;
            for(playerhashentry1 = playerhashentry; playerhashentry1 != null; playerhashentry1 = playerhashentry2)
            {
                playerhashentry2 = playerhashentry1.nextEntry;
                if(playerhashentry1.key == l)
                {
                    field_950_e++;
                    numHashElements--;
                    if(playerhashentry == playerhashentry1)
                    {
                        hashArray[j] = playerhashentry2;
                    } else
                    {
                        playerhashentry.nextEntry = playerhashentry2;
                    }
                    return playerhashentry1;
                }
                playerhashentry = playerhashentry1;
            }

            return playerhashentry1;
        }

        private void createKey(int i, long l, Object obj, int j)
        {
            LongHashMapEntry playerhashentry = hashArray[j];
            hashArray[j] = new LongHashMapEntry(i, l, obj, playerhashentry);
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
        private float percentUsable = 0.75F;
        private volatile int field_950_e;
    }
}
