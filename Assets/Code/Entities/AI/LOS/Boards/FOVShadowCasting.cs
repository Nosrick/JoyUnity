using System;
using System.Collections.Generic;
using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVShadowCasting : AbstractFOVHandler
    {
        private FOVBasicBoard m_Board;

        private readonly Vector2Int[] DIAGONALS = { new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

        public override IFOVBoard Do(Entity viewer, WorldInstance world, RectInt dimensions, Vector2Int[] walls)
        {
            m_Board = new FOVBasicBoard(dimensions.width, dimensions.height, walls);

            Vector2Int origin = new Vector2Int(dimensions.width / 2, dimensions.height / 2);
            m_Board.Visible(origin.x, origin.y);
            foreach(Vector2Int direction in DIAGONALS)
            {
                CastLight(viewer, world, origin, viewer.VisionMod, 1, 1, 0, 0, direction.x, direction.y, 0);
                CastLight(viewer, world, origin, viewer.VisionMod, 1, 1, 0, direction.x, 0, 0, direction.y);
            }

            return m_Board;
        }

        private void CastLight(Entity viewer, WorldInstance world, Vector2Int origin, int sightMod, int row, float start, float end, int xx, int xy, int yx, int yy)
        {
            float newStart = 0.0f;
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

                    if (!(currentX >= 0 && currentY >= 0 && currentX < m_Board.GetVision().GetLength(0) && currentY < m_Board.GetVision().GetLength(1)) || start < rightSlope)
                    {
                        continue;
                    }

                    if (end > leftSlope)
                    {
                        break;
                    }

                    if (Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= sightMod)
                    {
                        if(viewer.VisionProvider.HasVisibility(viewer, world, currentX, currentY, m_Board.GetVision()))
                        {
                            m_Board.Visible(currentX, currentY);
                        }
                    }

                    if (blocked)
                    {
                        if(m_Board.IsObstacle(currentX, currentY))
                        {
                            newStart = rightSlope;
                        }
                        else
                        {
                            blocked = false;
                            start = newStart;
                        }
                    }
                    else
                    {
                        if (!m_Board.IsObstacle(currentX, currentY) || distance >= sightMod)
                        {
                            continue;
                        }
                        
                        blocked = true;
                        CastLight(viewer, world, origin, sightMod, distance + 1, start, leftSlope, xx, xy, yx, yy);
                        newStart = rightSlope;
                    }
                }
            }
        }

        public override LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            throw new NotImplementedException();
        }

        public bool[,] Vision => m_Board.GetVision();
    }
}
