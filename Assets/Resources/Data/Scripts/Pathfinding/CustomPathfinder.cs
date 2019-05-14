using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI
{
    public class CustomPathfinder : IPathfinder
    {
        public Queue<Vector2Int> FindPath(Vector2Int fromPoint, Vector2Int toPoint, List<Vector2Int> walls, RectInt sizes)
        {
            CustomAStar aStar = new CustomAStar(walls, sizes);

            CustomAStarNode2D goal = new CustomAStarNode2D(toPoint.x, toPoint.y, null, null, 0);
            CustomAStarNode2D start = new CustomAStarNode2D(fromPoint.x, fromPoint.y, null, goal, 0);

            aStar.FindPath(start, goal);
            Queue<Vector2Int> path = new Queue<Vector2Int>();

            foreach(CustomAStarNode2D node in aStar.Solution)
            {
                path.Enqueue(new Vector2Int(node.X, node.Y));
            }

            return path;
        }

        public string DetermineSector(Vector2Int from, Vector2Int to)
        {
            float xDiff = to.x - from.x;
            float yDiff = to.y - from.y;
            double angle = Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
            angle += 90;

            if (angle < 0)
            {
                angle += 360;
            }

            if ((angle >= 0 && angle <= 22.5) || (angle <= 360 && angle >= 337.5))
            {
                return "north";
            }
            else if (angle <= 67.5)
            {
                return "north east";
            }
            else if (angle <= 112.5)
            {
                return "east";
            }
            else if (angle <= 157.5)
            {
                return "south east";
            }
            else if (angle <= 202.5)
            {
                return "south";
            }
            else if (angle <= 247.5)
            {
                return "south west";
            }
            else if (angle <= 292.5)
            {
                return "west";
            }
            else if (angle <= 337.5)
            {
                return "north west";
            }

            return "nearby";
        }
    }
}
