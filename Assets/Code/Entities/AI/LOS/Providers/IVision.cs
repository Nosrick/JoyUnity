using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public interface IVision
    {
        string Name
        {
            get;
        }

        bool[,] Vision
        {
            get;
        }

        Vector2Int[] GetVisibleWalls(IEntity viewer, IWorldInstance world);

        bool CanSee(IEntity viewer, IWorldInstance world, int x, int y);
        bool CanSee(IEntity viewer, IWorldInstance world, Vector2Int point);

        bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y, bool[,] vision);
        bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point, bool[,] vision);

        RectInt GetVisionRect(IEntity viewer);

        RectInt GetFullVisionRect(IEntity viewer);

        void Update(IEntity viewer, IWorldInstance world);
    }
}