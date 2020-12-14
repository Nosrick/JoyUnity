using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.AI.LOS;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.World.Lighting
{
    public class LightCalculator
    {
        protected LightBoard m_Board;

        protected readonly Vector2Int[] DIAGONALS = { new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

        public LightBoard Do(IEnumerable<IItemInstance> items, IWorldInstance world, Vector2Int dimensions,
            IEnumerable<Vector2Int> walls)
        {
            m_Board = new LightBoard(dimensions.x, dimensions.y, walls);
            foreach (IItemInstance item in items)
            {
                m_Board.ClearVisited();
                m_Board.AddLight(item.WorldPosition, item.ItemType.LightLevel);
                foreach (Vector2Int direction in DIAGONALS)
                {
                    CastLight(item, world, item.WorldPosition, 1, 1, 0, 0, direction.x,direction.y, 0);
                    CastLight(item, world, item.WorldPosition, 1, 1, 0, direction.x, 0, 0, direction.y);
                }
            }

            return m_Board;
        }

        private void CastLight(IItemInstance item, IWorldInstance world, Vector2Int origin, int row, float start, float end, int xx, int xy, int yx, int yy)
        {
            float newStart = 0.0f;
            if(start < end)
            {
                return;
            }

            bool blocked = false;

            for(int distance = row; distance <= item.ItemType.LightLevel && blocked == false; distance++)
            {
                int deltaY = -distance;
                for(int deltaX = -distance; deltaX <= 0; deltaX++)
                {
                    int currentX = origin.x + deltaX * xx + deltaY * xy;
                    int currentY = origin.y + deltaX * yx + deltaY * yy;
                    float leftSlope = (deltaX - 0.5f) / (deltaY + 0.5f);
                    float rightSlope = (deltaX + 0.5f) / (deltaY - 0.5f);

                    int lightLevel = item.ItemType.LightLevel - distance;

                    if (!(currentX >= 0 && currentY >= 0 && currentX < m_Board.Width && currentY < m_Board.Height) || start < rightSlope)
                    {
                        continue;
                    }

                    if (end > leftSlope)
                    {
                        break;
                    }

                    Vector2Int currentPosition = new Vector2Int(currentX, currentY);
                    if (Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= item.ItemType.LightLevel)
                    {
                        m_Board.AddLight(currentPosition, lightLevel);
                    }

                    if (blocked)
                    {
                        m_Board.AddLight(currentPosition, lightLevel);
                        if(m_Board.IsObstacle(currentPosition))
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
                        m_Board.AddLight(currentPosition, lightLevel);
                        if (!m_Board.IsObstacle(currentPosition) || distance >= item.ItemType.LightLevel)
                        {
                            continue;
                        }
                        
                        blocked = true;
                        CastLight(item, world, origin, distance + 1, start, leftSlope, xx, xy, yx, yy);
                        newStart = rightSlope;
                    }
                }
            }
        }

        private void DoDiffuse()
        {
            m_Board.ClearVisited();
            foreach (KeyValuePair<Vector2Int, int> pair in m_Board.LightLevels)
            {
                this.DoAdjacent(pair.Key, pair.Value);
            }
        }

        private void DoAdjacent(Vector2Int position, int lightlevel)
        {
            List<Vector2Int> adjacent = new List<Vector2Int>
            {
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1)
            };
            foreach (Vector2Int point in adjacent)
            {
                if (point.x > 0 && point.x < this.m_Board.Width
                                && point.y > 0 && point.y < this.m_Board.Height
                                && lightlevel > 0)
                {
                    this.m_Board.AddLight(point, lightlevel - 1);
                    this.DoAdjacent(point, lightlevel - 1);
                }
            }
        }

        public LightBoard Light => m_Board;
    }
}