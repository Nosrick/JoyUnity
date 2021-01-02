using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonInteriorGenerator : IWorldSpaceGenerator
    {
        protected GeneratorTileType[,] m_UntreatedTiles;

        protected IObjectIconHandler ObjectIcons { get; set; }
        protected IDerivedValueHandler DerivedValueHandler { get; set; }
        protected RNG Roller { get; set; }

        public DungeonInteriorGenerator(
            IObjectIconHandler objectIconHandler,
            IDerivedValueHandler derivedValueHandler,
            RNG roller)
        {
            this.DerivedValueHandler = derivedValueHandler;
            this.Roller = roller;
            this.ObjectIcons = objectIconHandler;
        }

        public WorldTile[,] GenerateWorldSpace(int sizeRef, string tileSet)
        {
            this.TileSet = tileSet;
            WorldTile[,] tiles = new WorldTile[sizeRef, sizeRef];

            DungeonRoomGenerator roomGen = new DungeonRoomGenerator(sizeRef, this.Roller);

            this.m_UntreatedTiles = roomGen.GenerateRooms();

            DungeonCorridorGenerator corrGen = new DungeonCorridorGenerator(this.m_UntreatedTiles, roomGen.rooms, 50, this.Roller);
            this.m_UntreatedTiles = corrGen.GenerateCorridors();

            tiles = this.TreatTiles();

            return tiles;
        }

        private WorldTile[,] TreatTiles()
        {
            WorldTile[,] tiles = new WorldTile[this.m_UntreatedTiles.GetLength(0), this.m_UntreatedTiles.GetLength(1)];

            WorldTile[] templates = StandardWorldTiles.instance.GetByTileSet(this.TileSet).ToArray();

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
            Sprite[] sprites = this.ObjectIcons.GetSprites(this.TileSet, "surroundwall");
            List<IBasicValue<float>> values = new List<IBasicValue<float>>();
            values.Add(new ConcreteBasicFloatValue("weight", 1));
            values.Add(new ConcreteBasicFloatValue("bonus", 1));
            values.Add(new ConcreteBasicFloatValue("size", 1));
            values.Add(new ConcreteBasicFloatValue("hardness", 1));
            values.Add(new ConcreteBasicFloatValue("density", 1));

            for (int i = 0; i < this.m_UntreatedTiles.GetLength(0); i++)
            {
                for (int j = 0; j < this.m_UntreatedTiles.GetLength(1); j++)
                {
                    if (this.m_UntreatedTiles[i, j] == GeneratorTileType.Perimeter || this.m_UntreatedTiles[i, j] == GeneratorTileType.Wall || this.m_UntreatedTiles[i, j] == GeneratorTileType.None)
                    {
                        walls.Add(
                            new JoyObject(
                                "Surround", 
                                this.DerivedValueHandler.GetItemStandardBlock(values), 
                                new Vector2Int(i, j), this.TileSet, 
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
