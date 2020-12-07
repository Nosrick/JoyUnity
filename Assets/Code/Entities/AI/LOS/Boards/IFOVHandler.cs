﻿using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public interface IFOVHandler
    {
        IFOVBoard Do(
            IEntity viewer, 
            IWorldInstance world, 
            Vector2Int dimensions,
            IEnumerable<Vector2Int> walls);

        bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point, IEnumerable<Vector2Int> vision);
        bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y, IEnumerable<Vector2Int> vision);
        LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target);
    }
}
