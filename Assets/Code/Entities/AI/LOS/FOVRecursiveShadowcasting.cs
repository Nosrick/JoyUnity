using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVRecursiveShadowcasting : IFOVHandler
    {
        private FOVBasicBoard Board;

        private static Dictionary<int, List<ArcPoint>> Circles;
        private readonly int MAX_CACHED_RADIUS = 30;

        public FOVRecursiveShadowcasting()
        {
            if(Circles == null)
            {
                Circles = new Dictionary<int, List<ArcPoint>>();
                Vector2Int origin = Vector2Int.zero;
                int radius = MAX_CACHED_RADIUS;

                for(int i = -radius; i <= radius; i++)
                {
                    for(int j = -radius; j <= radius; j++)
                    {
                        int distance = (int)Math.Floor(Vector2Int.Distance(origin, new Vector2Int(i, j)));

                        if(distance <= radius)
                        {
                            if(!Circles.ContainsKey(distance))
                            {
                                List<ArcPoint> circle = new List<ArcPoint>();
                                Circles.Add(distance, circle);
                                circle.Add(new ArcPoint(i, j));
                            }
                            else
                            {
                                List<ArcPoint> circle = Circles[distance];
                                circle.Add(new ArcPoint(i, j));
                            }
                        }
                    }
                }

                foreach(List<ArcPoint> list in Circles.Values)
                {
                    list.Sort();
                }
            }
        }

        public IFOVBoard Do(Vector2Int origin, int visionMod)
        {
            Board.ClearBoard();

            Board.Visible(origin.x, origin.y);
            Go(origin, 1, visionMod, 0, 359.9d);

            return Board;
        }
        
        public IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int visionMod, List<Vector2Int> walls)
        {
            Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);

            Board.Visible(origin.x, origin.y);
            Go(origin, 1, visionMod, 0, 359.9d);

            return Board;
        }

        private void Go(Vector2Int origin, int recursion, int visionDistance, double theta1, double theta2)
        {
            if(recursion > visionDistance)
            {
                return;
            }
            else if(recursion <= 0)
            {
                return;
            }

            List<ArcPoint> circle = Circles[recursion];
            int circleSize = circle.Count;

            bool wasObstacle = false;
            bool foundClear = false;
            for(int i = 0; i < circleSize; i++)
            {
                ArcPoint point = circle[i];
                int pX = origin.x + point.X;
                int pY = origin.y + point.Y;

                if(!Board.Contains(pX, pY))
                {
                    wasObstacle = true;
                    continue;
                }

                if(point.Lagging < theta1 && point.Theta != theta1 && point.Theta != theta2)
                {
                    continue;
                }

                if(point.Leading > theta2 && point.Theta != theta1 && point.Theta != theta2)
                {
                    continue;
                }

                Board.Visible(pX, pY);

                bool isObstacle = Board.IsObstacle(pX, pY);

                if(isObstacle == true)
                {
                    if(wasObstacle == true)
                    {
                        continue;
                    }
                    else if(foundClear == true)
                    {
                        double runEndTheta = point.Leading;
                        double runStartTheta = theta1;

                        if(recursion < visionDistance)
                        {
                            Go(origin, recursion + 1, visionDistance, runStartTheta, runEndTheta);
                        }

                        wasObstacle = true;
                    }
                    else
                    {
                        if (point.Theta == 0.0f)
                        {
                            theta1 = 0.0f;
                        }
                        else
                        {
                            theta1 = point.Leading;
                        }
                    }
                }
                else
                {
                    foundClear = true;
                    if(wasObstacle == true)
                    {
                        ArcPoint last = circle[i - 1];
                        theta1 = last.Lagging;

                        wasObstacle = false;
                    }
                    else
                    {
                        wasObstacle = false;
                        continue;
                    }
                }
                wasObstacle = isObstacle;
            }
            if(recursion < visionDistance)
            {
                Go(origin, recursion + 1, visionDistance, theta1, theta2);
            }
        }
    }

    public class ArcPoint : IComparable
    {
        public ArcPoint(int dX, int dY)
        {
            X = dX;
            Y = dY;

            Theta = Angle(Y, X);

            if (X < 0 && Y < 0)
            {
                Leading = Angle(Y - 0.5, X + 0.5);
                Lagging = Angle(Y + 0.5, X - 0.5);
            }
            // bottom left
            else if (X < 0)
            {
                Leading = Angle(Y - 0.5, X - 0.5);
                Lagging = Angle(Y + 0.5, X + 0.5);
            }
            // bottom right
            else if (Y > 0)
            {
                Leading = Angle(Y + 0.5, X - 0.5);
                Lagging = Angle(Y - 0.5, X + 0.5);
            }
            // top right
            else
            {
                Leading = Angle(Y + 0.5, X + 0.5);
                Lagging = Angle(Y - 0.5, X - 0.5);
            }
        }

        public double Angle(double y, double x)
        {
            double angle = Math.Atan2(y, x);
            angle = RadToDeg(angle);
            angle = 360.0d - angle;
            angle %= 360.0d;

            if(angle < 0)
            {
                angle += 360.0d;
            }

            return angle;
        }

        private double RadToDeg(double rads)
        {
            return (rads * (180.0d / Math.PI));
        }

        public int CompareTo(object obj)
        {
            ArcPoint temp = (ArcPoint)obj;
            if(Theta > temp.Theta)
            {
                return 1;
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            ArcPoint temp = (ArcPoint)obj;
            return Theta == temp.Theta;
        }

        public override int GetHashCode()
        {
            return X * Y;
        }

        public override string ToString()
        {
            return ("[" + X + ", " + Y + " = " + Theta + " | " + (int)Leading + " | " + (int)Lagging + "]");
        }

        public int X
        {
            get;
            protected set;
        }

        public int Y
        {
            get;
            protected set;
        }

        public double Theta
        {
            get;
            protected set;
        }

        public double Leading
        {
            get;
            protected set;
        }

        public double Lagging
        {
            get;
            protected set;
        }
    }
}
