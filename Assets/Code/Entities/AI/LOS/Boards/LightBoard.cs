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
            this.Width = width;
            this.Height = height;
            this.Walls = new HashSet<Vector2Int>(walls);
            this.Visited = new HashSet<Vector2Int>();
            this.LightLevels = new Dictionary<Vector2Int, int>();
        }

        public bool HasVisited(Vector2Int position)
        {
            return this.Visited.Contains(position);
        }

        public int AddLight(Vector2Int position, int lightLevel)
        {
            if (this.HasVisited(position))
            {
                return this.LightLevels[position];
            }

            if (this.LightLevels.ContainsKey(position))
            {
                this.LightLevels[position] = Mathf.Min(this.LightLevels[position] + lightLevel, GlobalConstants.MAX_LIGHT);
            }
            else
            {
                this.LightLevels.Add(position, lightLevel);
            }

            this.Visited.Add(position);
            return this.LightLevels[position];
        }

        public int GetLight(Vector2Int position)
        {
            if (this.LightLevels.ContainsKey(position))
            {
                return this.LightLevels[position];
            }

            return 0;
        }

        public bool IsObstacle(Vector2Int position)
        {
            return this.Walls.Contains(position);
        }

        public void ClearVisited()
        {
            this.Visited.Clear();
        }

        public void ClearBoard()
        {
            this.LightLevels.Clear();
            this.Visited.Clear();
        }
    }
}