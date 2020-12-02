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
        protected RNG Roller { get; set; }
        protected ILiveEntityHandler EntityHandler { get; set; }
        protected IEntityTemplateHandler EntityTemplateHandler { get; set; }
        protected IPhysicsManager PhysicsManager { get; set; }
        protected IEntityFactory EntityFactory { get; set; }

        public DungeonEntityPlacer(
            ILiveEntityHandler entityHandler,
            IEntityTemplateHandler templateHandler,
            IPhysicsManager physicsManager,
            IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
            EntityTemplateHandler = templateHandler;
            PhysicsManager = physicsManager;
            EntityHandler = entityHandler;
        }

        public List<IEntity> PlaceEntities(IWorldInstance worldRef, List<string> entityTypes, RNG roller, bool makeNewRollers = true)
        {
            Roller = roller;
            List<IEntity> entities = new List<IEntity>();

            List<IEntityTemplate> templates = EntityTemplateHandler.Templates;
            templates = templates.Where(x => entityTypes.Contains(x.CreatureType)).ToList();

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
                int pointIndex = Roller.Roll(0, availablePoints.Count);

                int entityIndex = Roller.Roll(0, templates.Count);

                RNG newRoller = makeNewRollers ? new RNG() : roller; 
                
                IGrowingValue level = new ConcreteGrowingValue(
                    "level", 
                    1, 
                    100, 
                    0, 
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(newRoller), 
                    new NonUniqueDictionary<INeed, float>());

                IEntity newEntity = EntityFactory.CreateFromTemplate(
                    templates[entityIndex], 
                    availablePoints[pointIndex],
                    level,
                    null,
                    null,
                    null,
                    null,
                    null, 
                    null,
                    null,
                    null, 
                    worldRef);

                EntityHandler.AddEntity(newEntity);
                entities.Add(newEntity);

                availablePoints.RemoveAt(pointIndex);
            }

            return entities;
        }
    }
}
