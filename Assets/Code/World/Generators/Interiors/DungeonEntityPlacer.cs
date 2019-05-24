using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using JoyLib.Code.States;
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

            //int numberToPlace = (worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1)) / 50;
            int numberToPlace = 1;

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
                    List<CultureType> cultures = CultureHandler.GetByCreatureType(templates[entityIndex].CreatureType);
                    CultureType culture = cultures[0];

                    JobType jobType = JobHandler.GetRandom();

                    Dictionary<string, int> jobLevels = new Dictionary<string, int>();
                    jobLevels.Add(jobType.name, 1);

                    //REPLACE THIS WITH ENTITY CONSTRUCTOR
                    Dictionary<string, INeed> needs = new Dictionary<string, INeed>();
                    INeed hunger = NeedHandler.GetRandomised("Hunger");
                    needs.Add(hunger.Name, hunger);

                    newEntity = WorldState.EntityHandler.Create(templates[entityIndex], needs, 1, jobType, culture.ChooseSex(), culture.ChooseSexuality(),
                        new Vector2Int(-1, -1), ObjectIcons.GetSprites(templates[entityIndex].Tileset, templates[entityIndex].CreatureType).ToList(), null);
                }
                else
                {
                    List<CultureType> cultures = CultureHandler.GetByCreatureType(templates[entityIndex].CreatureType);
                    CultureType culture = cultures[0];

                    JobType jobType = JobHandler.GetRandom();

                    Dictionary<string, int> jobLevels = new Dictionary<string, int>();
                    jobLevels.Add(jobType.name, 1);

                    //REPLACE THIS WITH ENTITY CONSTRUCTOR
                    Dictionary<string, INeed> needs = new Dictionary<string, INeed>();
                    INeed hunger = NeedHandler.GetRandomised("Hunger");
                    needs.Add(hunger.Name, hunger);

                    newEntity = WorldState.EntityHandler.Create(templates[entityIndex], needs, 1, jobType, culture.ChooseSex(), culture.ChooseSexuality(),
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
