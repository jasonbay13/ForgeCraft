using System;

namespace SMP
{
    public class ChunkPosition
    {
        public ChunkPosition(int i, int j, int k)
        {
            x = i;
            y = j;
            z = k;
        }

        public override bool Equals(object obj)
        {
            if(obj is ChunkPosition)
            {
                ChunkPosition chunkposition = (ChunkPosition)obj;
                return chunkposition.x == x && chunkposition.y == y && chunkposition.z == z;
            } else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return x * 0x88f9fa + y * 0xef88b + z;
        }

        public int x;
        public int y;
        public int z;
    }
}
