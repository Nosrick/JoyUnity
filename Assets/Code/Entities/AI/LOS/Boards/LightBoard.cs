using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class LightBoard
    {
        public Dictionary<Vector2Int, int> LightLevels { get; protected set; }
        
        protected HashSet<Vector2Int> Walls { get; set; }
        protected HashSet<Vector2Int> Visited { get; set; }
        
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public LightBoard(int width, int height, IEnumerable<Vector2Int> walls)
        {
            Width = width;
            Height = height;
            Walls = new HashSet<Vector2Int>(walls);
            Visited = new HashSet<Vector2Int>();
            LightLevels = new Dictionary<Vector2Int, int>();
        }

        public bool HasVisited(Vector2Int position)
        {
            return Visited.Contains(position);
        }

        public int AddLight(Vector2Int position, int lightLevel)
        {
            if (HasVisited(position))
            {
                return LightLevels[position];
            }

            if (LightLevels.ContainsKey(position))
            {
                LightLevels[position] = Mathf.Min(LightLevels[position] + lightLevel, GlobalConstants.MAX_LIGHT);
            }
            else
            {
                LightLevels.Add(position, lightLevel);
            }

            Visited.Add(position);
            return LightLevels[position];
        }

        public int GetLight(Vector2Int position)
        {
            if (LightLevels.ContainsKey(position))
            {
                return LightLevels[position];
            }

            return 0;
        }

        public bool IsObstacle(Vector2Int position)
        {
            return Walls.Contains(position);
        }

        public void ClearVisited()
        {
            Visited.Clear();
        }

        public void ClearBoard()
        {
            LightLevels.Clear();
            Visited.Clear();
        }
    }
}