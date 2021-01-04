using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public interface IVision
    {
        int MinimumLightLevel { get; }
        int MaximumLightLevel { get; }
        
        Color DarkColour { get; }
        Color LightColour { get; }
        
        string Name
        {
            get;
        }

        IEnumerable<Vector2Int> Vision
        {
            get;
        }

        Vector2Int[] GetVisibleWalls(IEntity viewer, IWorldInstance world);

        bool CanSee(IEntity viewer, IWorldInstance world, int x, int y);
        bool CanSee(IEntity viewer, IWorldInstance world, Vector2Int point);

        bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y);
        bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point);

        RectInt GetVisionRect(IEntity viewer);

        RectInt GetFullVisionRect(IEntity viewer);

        void Update(IEntity viewer, IWorldInstance world);
    }
}