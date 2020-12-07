using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public abstract class AbstractFOVHandler : IFOVHandler
    {
        public virtual bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point,
            IEnumerable<Vector2Int> vision)
        {
            return viewer.VisionProvider.HasVisibility(viewer, world, point);
        }

        public virtual bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y,
            IEnumerable<Vector2Int> vision)
        {
            return viewer.VisionProvider.HasVisibility(viewer, world, x, y);
        }

        public virtual IFOVBoard Do(IEntity viewer, IWorldInstance world, Vector2Int dimensions,
            IEnumerable<Vector2Int> walls)
        {
            throw new System.NotImplementedException("Someone forgot to implement Do()");
        }

        public virtual LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            throw new System.NotImplementedException("Someone forgot to implement HasLOS()");
        }
    }
}