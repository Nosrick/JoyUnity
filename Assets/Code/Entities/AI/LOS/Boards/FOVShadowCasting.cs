using System;
using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVShadowCasting : AbstractFOVHandler
    {
        protected FOVBasicBoard m_Board;

        protected readonly Vector2Int[] DIAGONALS = { new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

        public override IFOVBoard Do(IEntity viewer, IWorldInstance world, Vector2Int dimensions,
            IEnumerable<Vector2Int> walls)
        {
            Vector2Int viewerPos = viewer.WorldPosition;
            
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);
            m_Board.Visible(viewerPos.x, viewerPos.y);
            foreach(Vector2Int direction in DIAGONALS)
            {
                CastLight(viewer, world, viewerPos, viewer.VisionMod, 1, 1, 0, 0, direction.x, direction.y, 0);
                CastLight(viewer, world, viewerPos, viewer.VisionMod, 1, 1, 0, direction.x, 0, 0, direction.y);
            }

            return m_Board;
        }

        private void CastLight(IEntity viewer, IWorldInstance world, Vector2Int origin, int sightMod, int row, float start, float end, int xx, int xy, int yx, int yy)
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

                    if (!(currentX >= 0 && currentY >= 0 && currentX < m_Board.Width && currentY < m_Board.Height) || start < rightSlope)
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
                            m_Board.Block(currentX, currentY);
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

        public IEnumerable<Vector2Int> Vision => m_Board.GetVision();
    }
}
