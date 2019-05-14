using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI
{
    public interface IPathfinder
    {
        Queue<Vector2Int> FindPath(Vector2Int fromPoint, Vector2Int toPoint, List<Vector2Int> visibleWalls, RectInt sizes);

        string DetermineSector(Vector2Int fromPoint, Vector2Int toPoint);
    }
}
