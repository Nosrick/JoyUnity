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
    }
}
