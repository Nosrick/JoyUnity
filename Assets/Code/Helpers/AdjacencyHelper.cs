using System;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class AdjacencyHelper
    {
        public static bool IsAdjacent(Vector2Int left, Vector2Int right)
        {
            Rect adjacency = new Rect(left.x - 1, left.y - 1, 3, 3);
            return adjacency.Contains(right);
        }

        public static bool IsInRange(Vector2Int left, Vector2Int right, int range)
        {
            return range >= Math.Abs((left.x - right.x) + (left.y - right.y));
        }
    }
}
