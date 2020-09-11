using System;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVBasicBoard : IFOVBoard
    {
        private bool[,] m_Visited;

        private bool[,] m_Visible;
        private bool[,] m_Walls;

        private int m_Width, m_Height;

        public FOVBasicBoard(int width, int height, Vector2Int[] walls)
        {
            m_Width = width;
            m_Height = height;

            m_Visited = new bool[width, height];
            m_Visible = new bool[width, height];
            m_Walls = new bool[width, height];
            foreach(Vector2Int point in walls)
            {
                m_Walls[point.x, point.y] = true;
            }
        }

        public bool Visited(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                return m_Visited[x, y];
            }
            return true;
        }

        public void Visit(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                m_Visited[x, y] = true;
            }
        }

        public void Invisible(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                m_Visited[x, y] = true;
                m_Visible[x, y] = false;
            }
        }

        public void Visible(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                m_Visited[x, y] = true;
                m_Visible[x, y] = true;
            }
        }

        public bool IsBlocked(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                return (m_Visible[x, y] == false) || IsObstacle(x, y);
            }
            return true;
        }

        public bool IsObstacle(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                return m_Walls[x, y];
            }
            return true;
        }

        public double Radius(int deltaX, int deltaY)
        {
            return Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
        }

        public void ClearBoard()
        {
            m_Visible = new bool[m_Width, m_Height];
            m_Visited = new bool[m_Width, m_Height];
        }

        public bool Contains(int x, int y)
        {
            return (x >= 0 && x < m_Width && y >= 0 && y < m_Height);
        }

        public bool[,] GetVision()
        {
            return m_Visible;
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
