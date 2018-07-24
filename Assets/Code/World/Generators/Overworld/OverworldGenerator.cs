using JoyLib.Code.Graphics;
using System.Collections.Generic;

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

        public List<JoyObject> GenerateTileObjects(WorldTile[,] tiles)
        {
            List<JoyObject> objects = new List<JoyObject>(tiles.GetLength(0) * tiles.GetLength(1));

            for(int i = 0; i < tiles.GetLength(0); i++)
            {
                for(int j = 0; j < tiles.GetLength(1); j++)
                {
                    JoyObject joyObject = new JoyObject("Terrain", 1, new UnityEngine.Vector2Int(i, j), ObjectIcons.GetSprites("Overworld", "Grassland"), "Terrain", false, false, true);
                    objects.Add(joyObject);
                }
            }

            return objects;
        }
    }
}
