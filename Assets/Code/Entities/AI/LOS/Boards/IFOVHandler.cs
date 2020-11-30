using System.Collections.Generic;
using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS
{
    public interface IFOVHandler
    {
        IFOVBoard Do(
            IEntity viewer, 
            IWorldInstance world, 
            Vector2Int dimensions,
            Vector2Int[] walls);

        bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point, bool[,] vision);
        bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y, bool[,] vision);
        LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target);
    }
}
