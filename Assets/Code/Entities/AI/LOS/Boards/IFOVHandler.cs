using System.Collections.Generic;
using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS
{
    public interface IFOVHandler
    {
        IFOVBoard Do(
            Entity viewer, 
            WorldInstance world, 
            Vector2Int origin, 
            RectInt dimensions,
            Vector2Int[] walls);

        bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point);
        bool HasVisibility(Entity viewer, WorldInstance world, int x, int y);
        LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target);
    }
}
