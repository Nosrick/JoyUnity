using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.Drivers
{
    public class StandardDriver : AbstractDriver
    {
        protected static IJoyAction s_WanderAction = ScriptingEngine.Instance.FetchAction("wanderaction");

        protected static IPhysicsManager s_PhysicsManager;
        
        protected RNG Roller { get; set; }

        public StandardDriver(IPhysicsManager physicsManager = null, RNG roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller;
            if(s_PhysicsManager is null)
            {
                s_PhysicsManager = physicsManager is null ? null : physicsManager;
            }
        }

        public override void Locomotion(Entity vehicle)
        {
            //If you're idle
            if (vehicle.CurrentTarget.idle == true)
            {
                //Let's find something to do
                List<INeed> needs = vehicle.Needs.Values.OrderByDescending(x => x.Priority).ToList();
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
                    break;
                }

                if(idle == true)
                {
                    int result = this.Roller.Roll(0, 10);
                    if (result < 1)
                    {
                        wander = true;
                    }
                }

                if(wander == true)
                {
                    s_WanderAction.Execute(
                        new JoyObject[] { vehicle },
                        new[] { "wander", "idle"});
                }
            }

            //If we're wandering, select a point we can see and wander there
            if (vehicle.CurrentTarget.searching && vehicle.CurrentTarget.targetPoint == GlobalConstants.NO_TARGET)
            {
                List<Vector2Int> visibleSpots = new List<Vector2Int>(vehicle.Vision);

                //Pick a random spot to wander to
                int result = this.Roller.Roll(0, visibleSpots.Count);
                NeedAIData currentTarget = vehicle.CurrentTarget;
                currentTarget.targetPoint = visibleSpots[result];
                vehicle.CurrentTarget = currentTarget;
            }

            //If we have somewhere to be, move there
            if (vehicle.WorldPosition != vehicle.CurrentTarget.targetPoint 
                || vehicle.CurrentTarget.target != null)
            {
                if (vehicle.CurrentTarget.target is ItemInstance 
                    && vehicle.WorldPosition != vehicle.CurrentTarget.target.WorldPosition)
                {
                    if (vehicle.CurrentTarget.targetPoint.Equals(GlobalConstants.NO_TARGET))
                    {
                        NeedAIData newData = new NeedAIData
                        {
                            idle = vehicle.CurrentTarget.idle,
                            intent = vehicle.CurrentTarget.intent,
                            need = vehicle.CurrentTarget.need,
                            searching = vehicle.CurrentTarget.searching,
                            target = vehicle.CurrentTarget.target,
                            targetPoint = vehicle.CurrentTarget.target.WorldPosition
                        };
                        vehicle.CurrentTarget = newData;
                    }

                    this.MoveToTarget(vehicle);
                }
                else if(vehicle.CurrentTarget.target is Entity
                    && AdjacencyHelper.IsAdjacent(vehicle.WorldPosition, vehicle.CurrentTarget.target.WorldPosition) == false)
                {
                    this.MoveToTarget(vehicle);
                }
                else if (vehicle.CurrentTarget.target is null 
                && vehicle.CurrentTarget.targetPoint.Equals(GlobalConstants.NO_TARGET) == false)
                {
                    this.MoveToTarget(vehicle);
                }
            }
            //If we've arrived at our destination, then we do our thing
            if ((vehicle.WorldPosition == vehicle.CurrentTarget.targetPoint
                && (vehicle.CurrentTarget.target is ItemInstance || vehicle.CurrentTarget.target is null)
                || (vehicle.CurrentTarget.target is Entity 
                    && AdjacencyHelper.IsAdjacent(vehicle.WorldPosition, vehicle.CurrentTarget.target.WorldPosition))))
            {
                //If we have a target
                if (vehicle.CurrentTarget.target is null == false)
                {
                    if (vehicle.CurrentTarget.intent == Intent.Attack)
                    {
                        //CombatEngine.SwingWeapon(this, CurrentTarget.target);
                    }
                    else if (vehicle.CurrentTarget.intent == Intent.Interact)
                    {
                        INeed need = vehicle.Needs[vehicle.CurrentTarget.need];

                        need.Interact(vehicle, vehicle.CurrentTarget.target);
                        vehicle.CurrentTarget = new NeedAIData
                        {
                            idle = true,
                            intent = Intent.Interact,
                            need = "none",
                            searching = false,
                            target = null,
                            targetPoint = GlobalConstants.NO_TARGET
                        };
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