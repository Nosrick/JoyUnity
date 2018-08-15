using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using System.Collections.Generic;

namespace JoyLib.Code.World.Generators.Interiors
{
    public static class DungeonGenerator
    {
        public static WorldInstance GenerateDungeon(string name, int size, int levels, params string[] dungeonTypes)
        {
            DungeonInteriorGenerator interiorGenerator = new DungeonInteriorGenerator();
            SpawnPointPlacer spawnPointPlacer = new SpawnPointPlacer();

            List<string> entitiesToPlace = new List<string>();
            if (dungeonTypes != null)
            {
                entitiesToPlace.AddRange(dungeonTypes);
            }

            WorldInstance root = null;
            WorldInstance current = null;
            for (int i = 1; i <= levels; i++)
            {
                WorldTile[,] tiles = interiorGenerator.GenerateWorldSpace(size);
                WorldInstance worldInstance = new WorldInstance(tiles, WorldType.Interior, name + " " + i);

                List<JoyObject> walls = interiorGenerator.GenerateWalls(tiles);
                foreach(JoyObject wall in walls)
                {
                    worldInstance.AddObject(wall);
                }

                List<ItemInstance> items = DungeonItemPlacer.PlaceItems(worldInstance);
                foreach(ItemInstance item in items)
                {
                    worldInstance.AddObject(item);
                }

                List<Entity> entities = DungeonEntityPlacer.PlaceEntities(worldInstance, entitiesToPlace);
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
