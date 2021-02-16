using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Physics;
using JoyLib.Code.Rollers;
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
            this.EntityFactory = entityFactory;
            this.EntityTemplateHandler = templateHandler;
            this.PhysicsManager = physicsManager;
            this.EntityHandler = entityHandler;
        }

        public List<IEntity> PlaceEntities(IWorldInstance worldRef, List<string> entityTypes, RNG roller, bool makeNewRollers = true)
        {
            this.Roller = roller;
            List<IEntity> entities = new List<IEntity>();

            List<IEntityTemplate> templates = this.EntityTemplateHandler.Values.ToList();
            templates = templates.Where(x => entityTypes.Contains(x.CreatureType)).ToList();

            int numberToPlace = (worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1)) / 50;
            //int numberToPlace = 1;

            List<Vector2Int> availablePoints = new List<Vector2Int>();

            for (int i = 0; i < worldRef.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < worldRef.Tiles.GetLength(1); j++)
                {
                    Vector2Int point = new Vector2Int(i, j);
                    if (this.PhysicsManager.IsCollision(point, point, worldRef) == PhysicsResult.None && point != worldRef.SpawnPoint)
                    {
                        availablePoints.Add(point);
                    }
                }
            }

            for (int i = 0; i < numberToPlace; i++)
            {
                int pointIndex = this.Roller.Roll(0, availablePoints.Count);

                int entityIndex = this.Roller.Roll(0, templates.Count);

                RNG newRoller = makeNewRollers ? new RNG() : roller; 

                IEntity newEntity = this.EntityFactory.CreateFromTemplate(
                    templates[entityIndex], 
                    availablePoints[pointIndex],
                    null,
                    null,
                    null, 
                    null,
                    null,
                    null,
                    null,
                    null, 
                    null,
                    null,
                    null, 
                    null,
                    worldRef);

                this.EntityHandler.AddEntity(newEntity);
                entities.Add(newEntity);

                availablePoints.RemoveAt(pointIndex);
            }

            return entities;
        }
    }
}
