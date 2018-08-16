using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    /*
     * 
     * Made it faster, better, stronger
     * 
     */
    public class FOVDirty : IFOVHandler
    {
        private FOVBasicBoard m_Board;

        private const int GRANULARITY = 4;

        public IFOVBoard Do(Vector2Int origin, int sightMod)
        {
            m_Board.ClearBoard();

            m_Board.Visible(origin.x, origin.y);

            UnityEngine.Profiling.Profiler.BeginSample("FOV Calculations");
            for (int i = 0; i < 360; i += GRANULARITY)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                DiscoverTile(x, y, origin, sightMod);
            }
            UnityEngine.Profiling.Profiler.EndSample();

            return m_Board;
        }

        public IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int sightMod, List<Vector2Int> walls)
        {
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);

            m_Board.Visible(origin.x, origin.y);

            UnityEngine.Profiling.Profiler.BeginSample("FOV Calculations");
            for (int i = 0; i < 360; i += GRANULARITY)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                DiscoverTile(x, y, origin, sightMod);
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return m_Board;
        }

        protected void DiscoverTile(float x, float y, Vector2Int origin, int perception)
        {
            float oX = origin.x + 0.5f;
            float oY = origin.y + 0.5f;

            for (int i = 0; i <= perception; i++)
            {
                Vector2Int position = new Vector2Int((int)oX, (int)oY);
                
                if (m_Board.Contains(position.x, position.y) == false)
                {
                    continue;
                }

                m_Board.Visible(position.x, position.y);

                if (m_Board.IsObstacle(position.x, position.y) == true)
                {
                    continue;
                }

                oX += x;
                oY += y;
            }
        }

        /// <summary>
        /// Uses (mildly permissive) Bresenham LOS to calculate LOS
        /// </summary>
        /// <param name="origin">The viewer's position</param>
        /// <param name="target">The target position</param>
        /// <returns>The path (or lack thereof) to the target</returns>
        public LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            LinkedList<Vector2Int> path = new LinkedList<Vector2Int>();

            List<Vector2Int> targets = new List<Vector2Int>(3);
            Vector2 leftTemp = new Vector2(target.y, -target.x);
            leftTemp.Normalize();
            Vector2 rightTemp = -leftTemp;
            Vector2Int left = new Vector2Int((int)(target.x * leftTemp.x), (int)(target.y * leftTemp.y));
            Vector2Int right = new Vector2Int((int)(target.x * rightTemp.x), (int)(target.y * rightTemp.y));

            targets.Add(target);
            targets.Add(left);
            targets.Add(right);

            for(int i = 0; i < targets.Count; i++)
            {
                Vector2Int point = targets[i];
                bool steep = Mathf.Abs(point.y - origin.y) > Mathf.Abs(point.x - origin.x);
                if (steep == true)
                {
                    //Swap the X and Y co-ords for the two vectors
                    int tempX = origin.x;
                    int tempY = origin.y;
                    origin.x = tempY;
                    origin.y = tempX;

                    tempX = point.x;
                    tempY = point.y;
                    point.x = tempY;
                    point.y = tempX;
                }

                //If the origin is further along the X axis than the target,
                //swap the two vectors
                bool swapped = false;
                if (origin.x > point.x)
                {
                    int tempX = origin.x;
                    origin.x = point.x;
                    target.x = tempX;

                    int tempY = origin.y;
                    origin.y = point.y;
                    point.y = tempY;
                    swapped = true;
                }

                int dX = point.x - origin.x;
                int dY = Math.Abs(point.y - origin.y);

                int error = dX / 2;

                //If the origin Y is less than the target Y, we're taking positive steps
                //otherwise, negative steps
                int yStep = origin.y < point.y ? 1 : -1;
                int y = origin.y;

                for (int x = origin.x; x < point.x; x++)
                {
                    Vector2Int coord = Vector2Int.zero;
                    if (steep == true)
                    {
                        coord = new Vector2Int(y, x);
                    }
                    else
                    {
                        coord = new Vector2Int(x, y);
                    }
                    path.AddLast(coord);
                    error -= Mathf.Abs(dY);
                    if (error < 0)
                    {
                        y += yStep;
                        error += dX;
                    }
                }

                if (swapped == true)
                {
                    LinkedList<Vector2Int> reversed = new LinkedList<Vector2Int>();
                    foreach (Vector2Int node in path)
                    {
                        reversed.AddFirst(node);
                    }
                    path = reversed;
                }
            }
            return path;
        }
    }
}
