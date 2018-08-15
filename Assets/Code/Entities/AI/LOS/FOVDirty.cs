using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    /*
     * 
     * NOT SO QUICK AFTER ALL
     * DO NOT USE
     * 
     */
    public class FOVDirty : IFOVHandler
    {
        private FOVBasicBoard m_Board;

        private const int GRANULARITY = 1;

        public IFOVBoard Do(Vector2Int origin, int sightMod)
        {
            m_Board.ClearBoard();

            m_Board.Visible(origin.x, origin.y);

            for (int i = 0; i < 360; i += GRANULARITY)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                DiscoverTile(x, y, origin, sightMod);
            }

            return m_Board;
        }

        public IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int sightMod, List<Vector2Int> walls)
        {
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);

            m_Board.Visible(origin.x, origin.y);

            for (int i = 0; i < 360; i += GRANULARITY)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                DiscoverTile(x, y, origin, sightMod);
            }

            return m_Board;
        }

        protected void DiscoverTile(float x, float y, Vector2Int origin, int perception)
        {
            float oX = origin.x + 0.5f;
            float oY = origin.y + 0.5f;

            for (int i = 0; i <= perception; i++)
            {
                int posX = (int)oX;
                int posY = (int)oY;

                if (m_Board.Contains(posX, posY) == false)
                {
                    return;
                }
                
                m_Board.Visible(posX, posY);
                if (m_Board.IsObstacle(posX, posY) == true)
                {
                    return;
                }

                oX += x;
                oY += y;
            }
        }

        public bool[,] Vision
        {
            get
            {
                return m_Board.Vision;
            }
        }
    }
}
