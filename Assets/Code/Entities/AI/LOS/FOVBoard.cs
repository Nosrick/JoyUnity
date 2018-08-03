using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVBoard
    {
        private bool[,] m_Visited;

        private bool[,] m_Visible;
        private List<Vector2Int> m_Walls;

        private int m_Width, m_Height;

        public FOVBoard(int width, int height, List<Vector2Int> walls)
        {
            m_Width = width;
            m_Height = height;

            m_Visited = new bool[width, height];
            m_Visible = new bool[width, height];
            m_Walls = walls;
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
            return m_Walls.Contains(new Vector2Int(x, y));
        }

        public int Radius(int deltaX, int deltaY)
        {
            return (int)Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
        }

        public bool[,] Vision
        {
            get
            {
                return m_Visible;
            }
        }
    }
}
