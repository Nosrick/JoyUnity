using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Physics;
using JoyLib.Code.Rollers;
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

            EntityTemplate[] templates = EntityTemplateHandler.Templates;
            templates = templates.Where(x => entityTypes.Contains(x.CreatureType)).ToArray();

            int numberToPlace = (worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1)) / 50;
            //int numberToPlace = 1;

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

                int entityIndex = RNG.Roll(0, templates.Length - 1);

                List<CultureType> cultures = CultureHandler.GetByCreatureType(templates[entityIndex].CreatureType);
                CultureType culture = cultures[0];

                JobType jobType = culture.ChooseJob();

                Dictionary<string, int> jobLevels = new Dictionary<string, int>();
                jobLevels.Add(jobType.Name, 1);

                //REPLACE THIS WITH ENTITY CONSTRUCTOR
                BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
                needs.Add(NeedHandler.GetRandomised("hunger"));

                IGrowingValue level = new ConcreteGrowingValue(
                    "level", 
                    1, 
                    100, 
                    0, 
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(), 
                    new NonUniqueDictionary<INeed, float>());

                Entity newEntity = WorldState.EntityHandler.Create(
                    templates[entityIndex], 
                    needs, 
                    level, 
                    jobType, 
                    culture.ChooseSex(), 
                    culture.ChooseSexuality(),
                    availablePoints[pointIndex], 
                    ObjectIconHandler.instance.GetSprites(templates[entityIndex].Tileset, 
                    templates[entityIndex].CreatureType), 
                    worldRef);

                entities.Add(newEntity);

                availablePoints.RemoveAt(pointIndex);
            }

            return entities;
        }
    }
}
