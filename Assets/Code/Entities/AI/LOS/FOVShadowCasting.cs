using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVShadowCasting : IFOVHandler
    {
        private FOVBasicBoard m_Board;

        private readonly Vector2Int[] DIAGONALS = { new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

        public IFOVBoard Do(Vector2Int origin, int sightMod)
        {
            m_Board.ClearBoard();

            m_Board.Visible(origin.x, origin.y);
            foreach (Vector2Int direction in DIAGONALS)
            {
                CastLight(origin, sightMod, 1, 1, 0, 0, direction.x, direction.y, 0);
                CastLight(origin, sightMod, 1, 1, 0, direction.x, 0, 0, direction.y);
            }

            return m_Board;
        }

        public IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int sightMod, List<Vector2Int> walls)
        {
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);

            m_Board.Visible(origin.x, origin.y);
            foreach(Vector2Int direction in DIAGONALS)
            {
                CastLight(origin, sightMod, 1, 1, 0, 0, direction.x, direction.y, 0);
                CastLight(origin, sightMod, 1, 1, 0, direction.x, 0, 0, direction.y);
            }

            return m_Board;
        }

        private void CastLight(Vector2Int origin, int sightMod, int row, int start, int end, int xx, int xy, int yx, int yy)
        {
            int newStart = 0;
            if(start < end)
            {
                return;
            }

            bool blocked = false;

            for(int distance = row; distance <= sightMod && blocked == false; distance++)
            {
                int deltaY = -distance;
                for(int deltaX = -distance; deltaX <= 0; deltaX++)
                {
                    int currentX = origin.x + deltaX * xx + deltaY * xy;
                    int currentY = origin.y + deltaX * yx + deltaY * yy;
                    float leftSlope = (deltaX - 0.5f) / (deltaY + 0.5f);
                    float rightSlope = (deltaX + 0.5f) / (deltaY - 0.5f);

                    if (!(currentX >= 0 && currentY >= 0 && currentX < m_Board.Vision.GetLength(0) && currentY < m_Board.Vision.GetLength(1)) || start < rightSlope)
                    {
                        continue;
                    }
                    else if (end > leftSlope)
                    {
                        break;
                    }

                    if (Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= sightMod)
                    {
                        m_Board.Visible(currentX, currentY);
                    }

                    if (blocked)
                    {
                        if(m_Board.IsObstacle(currentX, currentY))
                        {
                            newStart = (int)rightSlope;
                            continue;
                        }
                        else
                        {
                            blocked = false;
                            start = newStart;
                        }
                    }
                    else
                    {
                        if(m_Board.IsObstacle(currentX, currentY) && distance < sightMod)
                        {
                            blocked = true;
                            CastLight(origin, sightMod, distance + 1, start, (int)leftSlope, xx, xy, yx, yy);
                            newStart = (int)rightSlope;
                        }
                    }
                }
            }
        }

        public LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            throw new NotImplementedException();
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
