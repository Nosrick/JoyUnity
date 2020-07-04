using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using System.Collections.Generic;

namespace JoyLib.Code.World.Generators.Interiors
{
    public static class DungeonGenerator
    {
        public static WorldInstance GenerateDungeon(WorldInfo worldInfo, int size, int levels)
        {
            DungeonInteriorGenerator interiorGenerator = new DungeonInteriorGenerator();
            SpawnPointPlacer spawnPointPlacer = new SpawnPointPlacer();
            DungeonItemPlacer itemPlacer = new DungeonItemPlacer();
            DungeonEntityPlacer entityPlacer = new DungeonEntityPlacer();

            List<string> entitiesToPlace = new List<string>();
            entitiesToPlace.AddRange(worldInfo.inhabitants);

            WorldInstance root = null;
            WorldInstance current = null;
            for (int i = 1; i <= levels; i++)
            {
                WorldTile[,] tiles = interiorGenerator.GenerateWorldSpace(size, worldInfo.tileset);
                WorldInstance worldInstance = new WorldInstance(tiles, worldInfo.tags, worldInfo.name + " " + i);

                List<JoyObject> walls = interiorGenerator.GenerateWalls(tiles);
                foreach(JoyObject wall in walls)
                {
                    worldInstance.AddObject(wall);
                }

                List<ItemInstance> items = itemPlacer.PlaceItems(worldInstance);
                foreach(ItemInstance item in items)
                {
                    worldInstance.AddObject(item);
                }

                List<Entity> entities = entityPlacer.PlaceEntities(worldInstance, entitiesToPlace);
                foreach(Entity entity in entities)
                {
                    worldInstance.AddEntity(entity);
                }

                //Do the spawn points
                worldInstance.SpawnPoint = spawnPointPlacer.PlaceSpawnPoint(worldInstance);

                //Use this as our root if we don't have one
                if(root == null)
                {
                    root = worldInstance;
                }

                //Link to the previous floor
                if(current != null)
                {
                    current.AddArea(worldInstance.SpawnPoint, worldInstance);
                }
                current = worldInstance;
            }

            return root;
        }
    }
}
