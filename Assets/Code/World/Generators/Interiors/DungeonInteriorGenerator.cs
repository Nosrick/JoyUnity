using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonInteriorGenerator : IWorldSpaceGenerator
    {
        private GeneratorTileType[,] m_UntreatedTiles;

        public WorldTile[,] GenerateWorldSpace(int sizeRef)
        {
            WorldTile[,] tiles = new WorldTile[sizeRef, sizeRef];

            DungeonRoomGenerator roomGen = new DungeonRoomGenerator(sizeRef);

            m_UntreatedTiles = roomGen.GenerateRooms();

            DungeonCorridorGenerator corrGen = new DungeonCorridorGenerator(m_UntreatedTiles, roomGen.rooms, 50);
            m_UntreatedTiles = corrGen.GenerateCorridors();

            tiles = TreatTiles();

            return tiles;
        }

        private WorldTile[,] TreatTiles()
        {
            WorldTile[,] tiles = new WorldTile[m_UntreatedTiles.GetLength(0), m_UntreatedTiles.GetLength(1)];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = WorldTile.Paving;
                }
            }

            return tiles;
        }

        public List<JoyObject> GenerateWalls(WorldTile[,] worldTiles)
        {
            List<JoyObject> walls = new List<JoyObject>();
            Sprite[] sprites = ObjectIconHandler.GetSprites("Walls", "Surround");

            for (int i = 0; i < m_UntreatedTiles.GetLength(0); i++)
            {
                for (int j = 0; j < m_UntreatedTiles.GetLength(1); j++)
                {
                    if (m_UntreatedTiles[i, j] == GeneratorTileType.Perimeter ||
                        m_UntreatedTiles[i, j] == GeneratorTileType.Wall ||
                        m_UntreatedTiles[i, j] == GeneratorTileType.None)
                    {
                        walls.Add(new JoyObject("Surround", EntityDerivedValue.GetDefaultForItem(1, 1), new Vector2Int(i, j), "Walls", sprites, new string[] { "wall" }));
                    }
                }
            }

            return walls;
        }

        public void GenerateTileObjects(WorldTile[,] worldTiles)
        {
        }
    }
}
