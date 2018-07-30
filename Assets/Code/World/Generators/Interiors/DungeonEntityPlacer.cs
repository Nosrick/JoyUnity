using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public static class DungeonEntityPlacer
    {
        public static List<Entity> PlaceEntities(WorldInstance worldRef, List<string> entityTypes)
        {
            List<Entity> entities = new List<Entity>();

            List<EntityTemplate> templates = EntityTemplateHandler.Templates;
            templates = templates.Where(x => entityTypes.Contains(x.CreatureType)).ToList();

            int numberToPlace = (worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1)) / 50;

            List<Vector2Int> availablePoints = new List<Vector2Int>();

            for (int i = 0; i < worldRef.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < worldRef.Tiles.GetLength(1); j++)
                {
                    Vector2Int point = new Vector2Int(i, j);
                    if (PhysicsManager.IsCollision(point, point, worldRef) == PhysicsResult.None && point != worldRef.SpawnPoint)
                    {
                        availablePoints.Add(point);
                    }
                }
            }

            for (int i = 0; i < numberToPlace; i++)
            {
                int pointIndex = RNG.Roll(0, availablePoints.Count - 1);

                int entityIndex = RNG.Roll(0, templates.Count - 1);

                Entity newEntity = null;
                if (templates[entityIndex].Sentient)
                {
                    CultureType culture = CultureHandler.Get(templates[entityIndex].CreatureType);

                    JobType jobType = JobHandler.GetRandom();

                    Dictionary<string, int> jobLevels = new Dictionary<string, int>();
                    jobLevels.Add(jobType.name, 1);

                    newEntity = new Entity(templates[entityIndex], EntityNeed.GetFullRandomisedNeeds(), 1, jobType, culture.Choosesex(), culture.ChooseSexuality(),
                        new Vector2Int(-1, -1), ObjectIcons.GetSprites(templates[entityIndex].Tileset, templates[entityIndex].CreatureType).ToList(), null);
                }
                else
                {
                    CultureType culture = CultureHandler.Get(templates[entityIndex].CreatureType);

                    JobType jobType = JobHandler.GetRandom();

                    Dictionary<string, int> jobLevels = new Dictionary<string, int>();
                    jobLevels.Add(jobType.name, 1);

                    newEntity = new Entity(templates[entityIndex], EntityNeed.GetBasicRandomisedNeeds(), 1, jobType, culture.Choosesex(), culture.ChooseSexuality(),
                        new Vector2Int(-1, -1), ObjectIcons.GetSprites(templates[entityIndex].Tileset, templates[entityIndex].CreatureType).ToList(), null);
                }
                newEntity.Move(availablePoints[pointIndex]);
                newEntity.MyWorld = worldRef;
                entities.Add(newEntity);

                availablePoints.RemoveAt(pointIndex);
            }

            return entities;
        }
    }
}
