using JoyLib.Code.Entities;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Physics
{
    public class PhysicsManager : MonoBehaviour
    {
        public PhysicsResult IsCollision(Vector2Int from, Vector2Int to, WorldInstance worldRef)
        {
            Entity tempEntity = worldRef.GetEntity(to);
            if (tempEntity != null && from != to)
            {
                if(tempEntity.WorldPosition != from)
                {
                    return PhysicsResult.EntityCollision;
                }
            }

            if (worldRef.Walls.ContainsKey(to))
            {
                return PhysicsResult.WallCollision;
            }

            JoyObject obj = worldRef.GetObject(to);
            if (obj != null)
            {
                return PhysicsResult.ObjectCollision;
            }   
            else
            {
                return PhysicsResult.None;
            }
        }
    }
}
