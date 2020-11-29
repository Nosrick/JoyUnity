using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonInteriorGenerator : IWorldSpaceGenerator
    {
        protected GeneratorTileType[,] m_UntreatedTiles;

        protected IObjectIconHandler ObjectIcons { get; set; }
        protected RNG Roller { get; set; }

        public DungeonInteriorGenerator(IObjectIconHandler objectIconHandler, RNG roller)
        {
            Roller = roller;
            ObjectIcons = objectIconHandler;
        }

        public WorldTile[,] GenerateWorldSpace(int sizeRef, string tileSet)
        {
            TileSet = tileSet;
            WorldTile[,] tiles = new WorldTile[sizeRef, sizeRef];

            DungeonRoomGenerator roomGen = new DungeonRoomGenerator(sizeRef, Roller);

            m_UntreatedTiles = roomGen.GenerateRooms();

            DungeonCorridorGenerator corrGen = new DungeonCorridorGenerator(m_UntreatedTiles, roomGen.rooms, 50, Roller);
            m_UntreatedTiles = corrGen.GenerateCorridors();

            tiles = TreatTiles();

            return tiles;
        }

        private WorldTile[,] TreatTiles()
        {
            WorldTile[,] tiles = new WorldTile[m_UntreatedTiles.GetLength(0), m_UntreatedTiles.GetLength(1)];

            WorldTile[] templates = StandardWorldTiles.instance.GetByTileSet(TileSet).ToArray();

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    //TODO: Redo this!
                    tiles[i, j] = templates[0];
                }
            }

            return tiles;
        }

        public List<JoyObject> GenerateWalls(WorldTile[,] worldTiles)
        {
            List<JoyObject> walls = new List<JoyObject>();
            Sprite[] sprites = ObjectIcons.GetSprites(TileSet, "surroundwall");

            for (int i = 0; i < m_UntreatedTiles.GetLength(0); i++)
            {
                for (int j = 0; j < m_UntreatedTiles.GetLength(1); j++)
                {
                    if (m_UntreatedTiles[i, j] == GeneratorTileType.Perimeter ||
                        m_UntreatedTiles[i, j] == GeneratorTileType.Wall ||
                        m_UntreatedTiles[i, j] == GeneratorTileType.None)
                    {
                        walls.Add(
                            new JoyObject(
                                "Surround", 
                                EntityDerivedValue.GetDefaultForItem(1, 1), 
                                new Vector2Int(i, j), 
                                TileSet, 
                                new string[] {}, 
                                sprites,
                                null,
                                new string[] { "wall", "interior" }));
                    }
                }
            }

            return walls;
        }

        public void GenerateTileObjects(WorldTile[,] worldTiles)
        {
        }

        protected string TileSet
        {
            get;
            set;
        }
    }
}
