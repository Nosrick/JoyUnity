using System.Collections.Generic;
using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS
{
    public abstract class AbstractFOVHandler : IFOVHandler
    {
        public virtual bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point, bool[,] vision)
        {
            return viewer.VisionProvider.HasVisibility(viewer, world, point, vision);
        }

        public virtual bool HasVisibility(Entity viewer, WorldInstance world, int x, int y, bool[,] vision)
        {
            return viewer.VisionProvider.HasVisibility(viewer, world, x, y, vision);
        }

        public virtual IFOVBoard Do(Entity viewer, WorldInstance world, Vector2Int origin, RectInt dimensions, Vector2Int[] walls)
        {
            throw new System.NotImplementedException("Someone forgot to implement Do()");
        }

        public virtual LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            throw new System.NotImplementedException("Someone forgot to implement HasLOS()");
        }
    }
}