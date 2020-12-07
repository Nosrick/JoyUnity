using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVBasicBoard : IFOVBoard
    {
        protected HashSet<Vector2Int> Visited { get; set; } 
        protected HashSet<Vector2Int> VisiblePoints { get; set; }
        protected HashSet<Vector2Int> Walls { get; set; }

        protected int m_Width, m_Height;

        public FOVBasicBoard(int width, int height, IEnumerable<Vector2Int> walls)
        {
            m_Width = width;
            m_Height = height;
            
            Visited = new HashSet<Vector2Int>();
            VisiblePoints = new HashSet<Vector2Int>();
            Walls = new HashSet<Vector2Int>(walls);
        }

        public bool HasVisited(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                return Visited.Contains(new Vector2Int(x, y));
            }
            return true;
        }

        public void Visit(int x, int y)
        {
            Vector2Int point = new Vector2Int(x, y);
            if (x >= 0 && x < m_Width 
               && y >= 0 && y < m_Height 
               && Visited.Contains(point) == false)
            {
                Visited.Add(point);
            }
        }

        public void Visible(int x, int y)
        {
            Vector2Int point = new Vector2Int(x, y);
            if (x >= 0 && x < m_Width 
                && y >= 0 && y < m_Height)
            {
                Visited.Add(point);
                VisiblePoints.Add(point);
            }
        }

        public void Block(int x, int y)
        {
            Vector2Int point = new Vector2Int(x, y);
            if (x >= 0 && x < m_Width 
                && y >= 0 && y < m_Height)
            {
                Visited.Add(point);
                VisiblePoints.Remove(point);
            }
        }

        public bool IsBlocked(int x, int y)
        {
            Vector2Int point = new Vector2Int(x, y);
            if (x >= 0 && x < m_Width 
                && y >= 0 && y < m_Height)
            {
                return (VisiblePoints.Contains(point) == false) || IsObstacle(x, y);
            }
            return true;
        }

        public bool IsObstacle(int x, int y)
        {
            Vector2Int point = new Vector2Int(x, y);
            if (x >= 0 && x < m_Width 
                && y >= 0 && y < m_Height)
            {
                return Walls.Contains(point);
            }
            return true;
        }

        public double Radius(int deltaX, int deltaY)
        {
            return Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
        }

        public void ClearBoard()
        {
            VisiblePoints = new HashSet<Vector2Int>();
            Visited = new HashSet<Vector2Int>();
        }

        public bool Contains(int x, int y)
        {
            return (x >= 0 && x < m_Width && y >= 0 && y < m_Height);
        }

        public HashSet<Vector2Int> GetVision()
        {
            return VisiblePoints;
        }

        public int Width
        {
            get
            {
                return m_Width;
            }
        }

        public int Height
        {
            get
            {
                return m_Height;
            }
        }
    }
}
