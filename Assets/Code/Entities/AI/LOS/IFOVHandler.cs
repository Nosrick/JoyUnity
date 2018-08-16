using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public interface IFOVHandler
    {
        IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int visionMod, List<Vector2Int> walls);
        IFOVBoard Do(Vector2Int origin, int visionMod);
        LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target);
    }
}
