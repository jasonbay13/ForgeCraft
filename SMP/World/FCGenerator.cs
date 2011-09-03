using System;

namespace SMP
{
	public static class FCGenerator
	{
		public static void FlatChunk(World l, Chunk c)
		{
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						if (y == 1)
							c.UNCHECKEDPlaceBlock(x, y, z, 7);
						if (y < 64)
							c.UNCHECKEDPlaceBlock(x, y, z, 1);
						else if (y == 64)
							c.UNCHECKEDPlaceBlock(x, y, z, 2);
						else
							c.UNCHECKEDPlaceBlock(x, y, z, 0);
					}
				}
			}
			return;
		}
	}
}

