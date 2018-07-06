using JoyLib.Code.Entities;
using JoyLib.Code.World;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Physics
{
    public static class PhysicsManager
    {
        public static PhysicsResult IsCollision(Vector2Int from, Vector2Int to, WorldInstance worldRef)
        {
            Entity tempEntity = worldRef.GetEntity(to);
            if (tempEntity != null && from != to)
            {
                if(tempEntity.WorldPosition != from)
                    return PhysicsResult.EntityCollision;
            }

            lock(worldRef.Objects)
            {
                if (worldRef.Objects.Any(x => x.WorldPosition.Equals(to) && x.IsWall))
                {
                    return PhysicsResult.WallCollision;
                }

                if (worldRef.Objects.Any(x => x.WorldPosition.Equals(to)))
                {
                    return PhysicsResult.ObjectCollision;
                }
            }
            return PhysicsResult.None;
        }
    }
}
