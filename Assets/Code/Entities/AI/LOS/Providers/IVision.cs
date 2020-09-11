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

        Vector2Int[] GetVisibleWalls(Entity viewer, WorldInstance world);

        bool CanSee(Entity viewer, WorldInstance world, int x, int y);
        bool CanSee(Entity viewer, WorldInstance world, Vector2Int point);

        bool HasVisibility(Entity viewer, WorldInstance world, int x, int y, bool[,] vision);
        bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point, bool[,] vision);

        RectInt GetVisionRect(Entity viewer);

        RectInt GetFullVisionRect(Entity viewer);

        void Update(Entity viewer, WorldInstance world);
    }
}