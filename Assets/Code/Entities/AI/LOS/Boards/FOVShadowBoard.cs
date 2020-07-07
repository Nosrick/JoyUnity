using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVShadowBoard : IFOVBoard
    {
        private int[,] m_Visible;
        private bool[,] m_Visited;

        private bool[,] m_Walls;

        private int m_Width, m_Height;

        private const int LIT = 16;

        public FOVShadowBoard(int width, int height, List<Vector2Int> walls)
        {
            m_Walls = new bool[width, height];
            foreach (Vector2Int point in walls)
            {
                m_Walls[point.x, point.y] = true;
            }
            m_Width = width;
            m_Height = height;

            m_Visible = new int[width, height];
            m_Visited = new bool[width, height];
        }

        public bool IsBlocked(int x, int y)
        {
            return Get(x, y) == 0 || IsObstacle(x, y);
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

        public void Visible(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                m_Visited[x, y] = true;
                m_Visible[x, y] = LIT;
            }
        }

        public void Visit(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                m_Visited[x, y] = true;
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

        public int Get(int x, int y)
        {
            if (x >= 0 && x < m_Width && y >= 0 && y < m_Height)
            {
                return m_Visible[x, y];
            }
            return 0;
        }

        public void ClearBoard()
        {
            m_Visible = new int[m_Width, m_Height];
            m_Visited = new bool[m_Width, m_Height];
        }

        public bool Contains(int x, int y)
        {
            return (x >= 0 && x < m_Width && y >= 0 && y < m_Height);
        }

        public bool[,] Vision
        {
            get
            {
                bool[,] vision = new bool[m_Width, m_Height];
                for(int i = 0; i < m_Visible.GetLength(0); i++)
                {
                    for(int j = 0; j < m_Visible.GetLength(1); j++)
                    {
                        if(m_Visible[i,j] > 0)
                        {
                            vision[i, j] = true;
                        }
                    }
                }

                return vision;
            }
        }
    }
}
