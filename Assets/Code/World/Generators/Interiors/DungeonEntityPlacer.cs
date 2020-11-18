using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Physics;
using JoyLib.Code.Rollers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonEntityPlacer
    {
        protected static GameObject s_GameManager;
        protected static LiveEntityHandler s_EntityHandler;
        protected static EntityTemplateHandler s_EntityTemplateHandler;

        protected static PhysicsManager s_PhysicsManager;

        protected static EntityFactory s_EntityFactory = new EntityFactory();

        public DungeonEntityPlacer()
        {
            s_GameManager = GlobalConstants.GameManager;
            s_EntityHandler = s_GameManager.GetComponent<LiveEntityHandler>();
            s_EntityTemplateHandler = s_GameManager.GetComponent<EntityTemplateHandler>();
            s_PhysicsManager = s_GameManager.GetComponent<PhysicsManager>();
        }

        public List<Entity> PlaceEntities(WorldInstance worldRef, List<string> entityTypes)
        {
            List<Entity> entities = new List<Entity>();

            List<EntityTemplate> templates = s_EntityTemplateHandler.Templates;
            templates = templates.Where(x => entityTypes.Contains(x.CreatureType)).ToList();

            //int numberToPlace = (worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1)) / 50;
            int numberToPlace = 1;

            List<Vector2Int> availablePoints = new List<Vector2Int>();

            for (int i = 0; i < worldRef.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < worldRef.Tiles.GetLength(1); j++)
                {
                    Vector2Int point = new Vector2Int(i, j);
                    if (s_PhysicsManager.IsCollision(point, point, worldRef) == PhysicsResult.None && point != worldRef.SpawnPoint)
                    {
                        availablePoints.Add(point);
                    }
                }
            }

            for (int i = 0; i < numberToPlace; i++)
            {
                int pointIndex = RNG.instance.Roll(0, availablePoints.Count);

                int entityIndex = RNG.instance.Roll(0, templates.Count);

                IGrowingValue level = new ConcreteGrowingValue(
                    "level", 
                    1, 
                    100, 
                    0, 
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(), 
                    new NonUniqueDictionary<INeed, float>());

                Entity newEntity = s_EntityFactory.CreateFromTemplate(
                    templates[entityIndex], 
                    level, 
                    availablePoints[pointIndex],
                    null,
                    null,
                    null,
                    null,
                    null, 
                    null,
                    null,
                    worldRef);

                s_EntityHandler.AddEntity(newEntity);
                entities.Add(newEntity);

                availablePoints.RemoveAt(pointIndex);
            }

            return entities;
        }
    }
}
