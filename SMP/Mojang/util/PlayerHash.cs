using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class PlayerHash
    {
        public PlayerHash()
        {
            capacity = 12;
            hashArray = new PlayerHashEntry[16];
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
            for(PlayerHashEntry playerhashentry = hashArray[getHashIndex(i, hashArray.Length)]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
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

        PlayerHashEntry func_35507_c(long l)
        {
            int i = getHashedKey(l);
            for(PlayerHashEntry playerhashentry = hashArray[getHashIndex(i, hashArray.Length)]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
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
            for(PlayerHashEntry playerhashentry = hashArray[j]; playerhashentry != null; playerhashentry = playerhashentry.nextEntry)
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
            PlayerHashEntry[] aplayerhashentry = hashArray;
            int j = aplayerhashentry.Length;
            if(j == 0x40000000)
            {
                capacity = 0x7fffffff;
                return;
            } else
            {
                PlayerHashEntry[] aplayerhashentry1 = new PlayerHashEntry[i];
                copyHashTableTo(aplayerhashentry1);
                hashArray = aplayerhashentry1;
                capacity = (int)((float)i * percentUsable);
                return;
            }
        }

        private void copyHashTableTo(PlayerHashEntry[] aplayerhashentry)
        {
            PlayerHashEntry[] aplayerhashentry1 = hashArray;
            int i = aplayerhashentry.Length;
            for(int j = 0; j < aplayerhashentry1.Length; j++)
            {
                PlayerHashEntry playerhashentry = aplayerhashentry1[j];
                if(playerhashentry == null)
                {
                    continue;
                }
                aplayerhashentry1[j] = null;
                do
                {
                    PlayerHashEntry playerhashentry1 = playerhashentry.nextEntry;
                    int k = getHashIndex(playerhashentry.field_1026_d, i);
                    playerhashentry.nextEntry = aplayerhashentry[k];
                    aplayerhashentry[k] = playerhashentry;
                    playerhashentry = playerhashentry1;
                } while(playerhashentry != null);
            }

        }

        public Object remove(long l)
        {
            PlayerHashEntry playerhashentry = removeKey(l);
            return playerhashentry != null ? playerhashentry.value : null;
        }

        PlayerHashEntry removeKey(long l)
        {
            int i = getHashedKey(l);
            int j = getHashIndex(i, hashArray.Length);
            PlayerHashEntry playerhashentry = hashArray[j];
            PlayerHashEntry playerhashentry1;
            PlayerHashEntry playerhashentry2;
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
            PlayerHashEntry playerhashentry = hashArray[j];
            hashArray[j] = new PlayerHashEntry(i, l, obj, playerhashentry);
            if(numHashElements++ >= capacity)
            {
                resizeTable(2 * hashArray.Length);
            }
        }

        public static int GetHashCode(long l)
        {
            return getHashedKey(l);
        }

        [NonSerialized] private PlayerHashEntry[] hashArray;
        [NonSerialized] private int numHashElements;
        private int capacity;
        private float percentUsable = 0.75F;
        private volatile int field_950_e;
    }
}
