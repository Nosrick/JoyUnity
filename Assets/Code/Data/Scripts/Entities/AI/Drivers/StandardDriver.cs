using JoyLib.Code.World;
using System.Collections.Generic;
using JoyLib.Code.Entities.Needs;
using System.Linq;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;

namespace JoyLib.Code.Entities.AI.Drivers
{
    public class StandardDriver : AbstractDriver
    {
        protected static IJoyAction s_WanderAction = ScriptingEngine.instance.FetchAction("wanderaction");

        protected static PhysicsManager s_PhysicsManager;

        public StandardDriver()
        {
            if(s_PhysicsManager is null)
            {
                GameObject.Find("GameManager").GetComponent<PhysicsManager>();
            }
        }

        public override void Locomotion(Entity vehicle)
        {
            //If you're idle
            if (vehicle.CurrentTarget.idle == true)
            {
                //Let's find something to do
                List<INeed> needs = vehicle.Needs.Collection.OrderByDescending(x => x.Priority).ToList();
                //Act on first need

                bool idle = true;
                bool wander = false;
                foreach (INeed need in needs)
                {
                    if (need.ContributingHappiness)
                    {
                        continue;
                    }

                    need.FindFulfilmentObject(vehicle);
                    idle = false;
                    break;
                }

                if(idle == true)
                {
                    int result = RNG.instance.Roll(0, 10);
                    if (result < 1)
                    {
                        wander = true;
                    }
                }

                if(wander == true)
                {
                    s_WanderAction.Execute(
                        new JoyObject[] { vehicle },
                        new string[] {});
                }

                NeedAIData currentTarget = vehicle.CurrentTarget;
                currentTarget.idle = idle;

                vehicle.CurrentTarget = currentTarget;
            }

            //If we're wandering, select a point we can see and wander there
            if (vehicle.CurrentTarget.searching && vehicle.CurrentTarget.targetPoint == GlobalConstants.NO_TARGET)
            {
                List<Vector2Int> visibleSpots = new List<Vector2Int>();
                List<Vector2Int> visibleWalls = vehicle.MyWorld.GetVisibleWalls(vehicle);
                //Check what we can see
                for (int x = 0; x < vehicle.Vision.GetLength(0); x++)
                {
                    for (int y = 0; y < vehicle.Vision.GetLength(0); y++)
                    {
                        Vector2Int newPos = new Vector2Int(x, y);
                        if (vehicle.VisionProvider.CanSee(vehicle, vehicle.MyWorld, x, y) 
                            && visibleWalls.Contains(newPos) == false 
                            && vehicle.WorldPosition != newPos)
                        {
                            visibleSpots.Add(newPos);
                        }
                    }
                }

                //Pick a random spot to wander to
                int result = RNG.instance.Roll(0, visibleSpots.Count - 1);
                NeedAIData currentTarget = vehicle.CurrentTarget;
                currentTarget.targetPoint = visibleSpots[result];
                vehicle.CurrentTarget = currentTarget;
            }

            //If we have somewhere to be, move there
            if (vehicle.WorldPosition != vehicle.CurrentTarget.targetPoint 
                || (vehicle.CurrentTarget.target != null 
                    && AdjacencyHelper.IsAdjacent(vehicle.WorldPosition, vehicle.CurrentTarget.target.WorldPosition)))
            {
                MoveToTarget(vehicle);
            }
            //If we've arrived at our destination, then we do our thing
            if (vehicle.WorldPosition == vehicle.CurrentTarget.targetPoint 
                || (vehicle.CurrentTarget.target != null 
                    && AdjacencyHelper.IsAdjacent(vehicle.WorldPosition, vehicle.CurrentTarget.target.WorldPosition)))
            {
                //If we have a target
                if (vehicle.CurrentTarget.target != null)
                {
                    if (vehicle.CurrentTarget.intent == Intent.Attack)
                    {
                        //CombatEngine.SwingWeapon(this, CurrentTarget.target);
                    }
                    else if (vehicle.CurrentTarget.intent == Intent.Interact)
                    {
                        INeed need = vehicle.Needs[vehicle.CurrentTarget.need];

                        need.Interact(vehicle, vehicle.CurrentTarget.target);
                    }
                }
                //If we do not, we were probably wandering
                else
                {
                    if (vehicle.CurrentTarget.searching == true)
                    {
                        NeedAIData currentTarget = vehicle.CurrentTarget;

                        currentTarget.targetPoint = GlobalConstants.NO_TARGET;

                        //Set idle to true so we look for stuff when we arrive
                        currentTarget.idle = true;

                        vehicle.CurrentTarget = currentTarget;
                    }
                }
            }
        }

        private void MoveToTarget(Entity vehicle)
        {
            if (!vehicle.HasMoved && vehicle.PathfindingData.Count > 0)
            {
                Vector2Int nextPoint = vehicle.PathfindingData.Peek();
                PhysicsResult physicsResult = s_PhysicsManager.IsCollision(vehicle.WorldPosition, nextPoint, vehicle.MyWorld);
                if (physicsResult != PhysicsResult.EntityCollision)
                {
                    vehicle.PathfindingData.Dequeue();
                    vehicle.Move(nextPoint);
                    vehicle.HasMoved = true;
                }
                else if (physicsResult == PhysicsResult.EntityCollision)
                {
                    vehicle.MyWorld.SwapPosition(vehicle, vehicle.MyWorld.GetEntity(nextPoint));
                    vehicle.PathfindingData.Dequeue();
                    vehicle.Move(nextPoint);
                    vehicle.HasMoved = true;
                }
            }
            else if (vehicle.PathfindingData.Count == 0)
            {
                if (vehicle.CurrentTarget.target != null)
                {
                    vehicle.PathfindingData = vehicle.Pathfinder.FindPath(
                        vehicle.WorldPosition, 
                        vehicle.CurrentTarget.target.WorldPosition, 
                        vehicle.MyWorld.Costs, vehicle.VisionProvider.GetFullVisionRect(vehicle));
                }
                else if (vehicle.CurrentTarget.targetPoint != GlobalConstants.NO_TARGET)
                {
                    vehicle.PathfindingData = vehicle.Pathfinder.FindPath(
                        vehicle.WorldPosition, 
                        vehicle.CurrentTarget.targetPoint, 
                        vehicle.MyWorld.Costs, 
                        vehicle.VisionProvider.GetFullVisionRect(vehicle));
                }
            }
        }
    }
}