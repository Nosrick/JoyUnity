using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public interface IVision
    {
        string Name
        {
            get;
        }

        HashSet<Vector2Int> Vision
        {
            get;
        }

        Vector2Int[] GetVisibleWalls(IEntity viewer, IWorldInstance world);

        bool CanSee(IEntity viewer, IWorldInstance world, int x, int y);
        bool CanSee(IEntity viewer, IWorldInstance world, Vector2Int point);

        bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y, IEnumerable<Vector2Int> vision);
        bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point, IEnumerable<Vector2Int> vision);

        RectInt GetVisionRect(IEntity viewer);

        RectInt GetFullVisionRect(IEntity viewer);

        void Update(IEntity viewer, IWorldInstance world);
    }
}