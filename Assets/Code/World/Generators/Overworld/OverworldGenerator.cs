using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoyLib.Code.World.Generators.Overworld
{
    public class OverworldGenerator : IWorldSpaceGenerator
    {
        public WorldTile[,] GenerateWorldSpace(int sizeRef)
        {
            WorldTile[,] tiles = new WorldTile[sizeRef, sizeRef];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for(int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = WorldTile.Plains;
                }
            }

            return tiles;
        }
    }
}
