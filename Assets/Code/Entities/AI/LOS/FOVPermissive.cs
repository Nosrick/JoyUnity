using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVPermissive : IFOVHandler
    {
        private FOVBasicBoard m_Board;

        private int m_SightMod;

        public IFOVBoard Do(Vector2Int origin, int sightMod)
        {
            m_Board.ClearBoard();

            m_Board.Visible(origin.x, origin.y);

            int xMin, xMax, yMin, yMax;

            if (origin.x < sightMod)
            {
                xMin = origin.x;
            }
            else
            {
                xMin = sightMod;
            }

            if (m_Board.Width - origin.x - 1 < sightMod)
            {
                xMax = m_Board.Width - origin.x - 1;
            }
            else
            {
                xMax = sightMod;
            }

            if (origin.y < sightMod)
            {
                yMin = origin.y;
            }
            else
            {
                yMin = sightMod;
            }

            if (m_Board.Height - origin.y - 1 < sightMod)
            {
                yMax = m_Board.Height - origin.y - 1;
            }
            else
            {
                yMax = sightMod;
            }

            //Northeast quadrant
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(1, 1), new Vector2Int(xMax, yMax));

            //Southeast
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(1, -1), new Vector2Int(xMax, yMin));

            //Southwest
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(-1, -1), new Vector2Int(xMin, yMin));

            //Northwest
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(-1, 1), new Vector2Int(xMin, yMax));

            return m_Board;
        }

        public IFOVBoard Do(Vector2Int origin, Vector2Int dimensions, int sightMod, List<Vector2Int> walls)
        {
            m_SightMod = sightMod;

            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);

            m_Board.Visible(origin.x, origin.y);

            int xMin, xMax, yMin, yMax;

            if (origin.x < sightMod)
            {
                xMin = origin.x;
            }
            else
            {
                xMin = sightMod;
            }

            if (m_Board.Width - origin.x - 1 < sightMod)
            {
                xMax = m_Board.Width - origin.x - 1;
            }
            else
            {
                xMax = sightMod;
            }

            if (origin.y < sightMod)
            {
                yMin = origin.y;
            }
            else
            {
                yMin = sightMod;
            }

            if (m_Board.Height - origin.y - 1 < sightMod)
            {
                yMax = m_Board.Height - origin.y - 1;
            }
            else
            {
                yMax = sightMod;
            }

            //Northeast quadrant
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(1, 1), new Vector2Int(xMax, yMax));

            //Southeast
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(1, -1), new Vector2Int(xMax, yMin));

            //Southwest
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(-1, -1), new Vector2Int(xMin, yMin));

            //Northwest
            CheckQuadrant(m_Board.Vision, origin, new Vector2Int(-1, 1), new Vector2Int(xMin, yMax));

            return m_Board;
        }

        private void CheckQuadrant(bool[,] visited, Vector2Int origin, Vector2Int dimension, Vector2Int extents)
        {
            Line shallowLine = new Line(0, 1, extents.x, 0);
            Line steepLine = new Line(1, 0, 0, extents.y);

            List<View> activeViews = new List<View>();

            activeViews.Add(new View(shallowLine, steepLine));

            int viewIndex = 0;

            int maxI = extents.x + extents.y;
            int i = 1;
            while(i != maxI + 1 && activeViews.Count > 0)
            {
                int startJ, maxJ;
                if(i - extents.x < 0)
                {
                    startJ = 0;
                }
                else
                {
                    startJ = i - extents.x;
                }

                if(i < extents.y)
                {
                    maxJ = i;
                }
                else
                {
                    maxJ = extents.y;
                }

                int j = startJ;
                while (j != maxJ + 1 && viewIndex < activeViews.Count)
                {
                    int x = i - j;
                    int y = j;

                    VisitCoord(origin, new Vector2Int(x, y), dimension, viewIndex, activeViews);

                    j += 1;
                }

                i += 1;
            }
        }

        private void VisitCoord(Vector2Int origin, Vector2Int current, Vector2Int dimension, int viewIndex, List<View> activeViews)
        {
            Vector2Int topLeft = new Vector2Int(current.x, current.y + 1);
            Vector2Int bottomRight = new Vector2Int(current.x + 1, current.y);

            while(viewIndex < activeViews.Count && activeViews[viewIndex].SteepLine.BelowOrCollinear(bottomRight.x, bottomRight.y))
            {
                viewIndex += 1;
            }

            if(viewIndex == activeViews.Count || activeViews[viewIndex].ShallowLine.AboveOrCollinear(topLeft.x, topLeft.y))
            {
                return;
            }

            bool isBlocked = false;

            int realX = current.x * dimension.x;
            int realY = current.y * dimension.y;

            if(origin.x + realX < 0 || origin.x + realX >= m_Board.Width || origin.y + realY < 0 || origin.y + realY >= m_Board.Height)
            {
                activeViews.RemoveAt(viewIndex);
                return;
            }

            if(m_Board.Visited(realX + origin.x, realY + origin.y) == false && m_Board.Radius(realX, realY) <= m_SightMod)
            {
                m_Board.Visible(realX + origin.x, realY + origin.y);
            }

            isBlocked = m_Board.IsObstacle(realX + origin.x, realY + origin.y);
            if(isBlocked == false)
            {
                return;
            }

            if(activeViews[viewIndex].ShallowLine.Above(bottomRight.x, bottomRight.y) && activeViews[viewIndex].SteepLine.Below(topLeft.x, topLeft.y))
            {
                activeViews.RemoveAt(viewIndex);
            }
            else if(activeViews[viewIndex].ShallowLine.Above(bottomRight.x, bottomRight.y))
            {
                AddShallowBump(bottomRight.x, bottomRight.y, activeViews, viewIndex);
                CheckView(activeViews, viewIndex);
            }
            else if(activeViews[viewIndex].SteepLine.Below(topLeft.x, topLeft.y))
            {
                AddSteepBump(topLeft.x, topLeft.y, activeViews, viewIndex);
                CheckView(activeViews, viewIndex);
            }
            else
            {
                int shallowViewIndex = viewIndex;
                viewIndex += 1;
                int steepViewIndex = viewIndex;

                activeViews.Insert(shallowViewIndex, new View(activeViews[shallowViewIndex]));
                AddSteepBump(bottomRight.x, bottomRight.y, activeViews, shallowViewIndex);

                if(CheckView(activeViews, shallowViewIndex) == false)
                {
                    viewIndex -= 1;
                    steepViewIndex -= 1;
                }

                AddShallowBump(topLeft.x, topLeft.y, activeViews, steepViewIndex);
                CheckView(activeViews, steepViewIndex);
            }
        }

        private void AddShallowBump(int x, int y, List<View> activeViews, int viewIndex)
        {
            activeViews[viewIndex].ShallowLine.End = new Vector2Int(x, y);

            activeViews[viewIndex].ShallowBump = new ViewBump(x, y, activeViews[viewIndex].ShallowBump);

            ViewBump currentBump = activeViews[viewIndex].SteepBump;
            while(currentBump != null)
            {
                if(activeViews[viewIndex].ShallowLine.Above(currentBump.X, currentBump.Y))
                {
                    activeViews[viewIndex].ShallowLine.Begin = new Vector2Int(currentBump.X, currentBump.Y);
                }

                currentBump = currentBump.Parent;
            }
        }

        private void AddSteepBump(int x, int y, List<View> activeViews, int viewIndex)
        {
            activeViews[viewIndex].SteepLine.End = new Vector2Int(x, y);

            activeViews[viewIndex].SteepBump = new ViewBump(x, y, activeViews[viewIndex].SteepBump);

            ViewBump currentBump = activeViews[viewIndex].ShallowBump;
            while (currentBump != null)
            {
                if (activeViews[viewIndex].SteepLine.Below(currentBump.X, currentBump.Y))
                {
                    activeViews[viewIndex].SteepLine.Begin = new Vector2Int(currentBump.X, currentBump.Y);
                }

                currentBump = currentBump.Parent;
            }
        }

        private bool CheckView(List<View> activeViews, int viewIndex)
        {
            Line shallowLine = activeViews[viewIndex].ShallowLine;
            Line steepLine = activeViews[viewIndex].SteepLine;

            if(shallowLine.LineCollinear(steepLine) && (shallowLine.Collinear(0, 1) || shallowLine.Collinear(1, 0)))
            {
                activeViews.RemoveAt(viewIndex);
                return false;
            }

            return true;
        }

        public bool[,] Vision
        {
            get
            {
                return m_Board.Vision;
            }
        }

        class Line
        {
            public Line(int beginX, int beginY, int endX, int endY)
            {
                Begin = new Vector2Int(beginX, beginY);
                End = new Vector2Int(endX, endY);
            }

            public bool Below(int x, int y)
            {
                return RelativeSlope(x, y) > 0;
            }

            public bool BelowOrCollinear(int x, int y)
            {
                return RelativeSlope(x, y) >= 0;
            }

            public bool Above(int x, int y)
            {
                return RelativeSlope(x, y) < 0;
            }

            public bool AboveOrCollinear(int x, int y)
            {
                return RelativeSlope(x, y) <= 0;
            }

            public bool Collinear(int x, int y)
            {
                return RelativeSlope(x, y) == 0;
            }

            public bool LineCollinear(Line line)
            {
                return Collinear(line.Begin.x, line.Begin.y) && Collinear(line.End.x, line.End.y); 
            }

            public int RelativeSlope(int x, int y)
            {
                return (this.DY * (this.End.x - x)) - (this.DX * (this.End.y - y));
            }

            public int DX
            {
                get
                {
                    return End.x - Begin.x;
                }
            }

            public int DY
            {
                get
                {
                    return End.y - Begin.y;
                }
            }

            public Vector2Int Begin
            {
                get;
                set;
            }

            public Vector2Int End
            {
                get;
                set;
            }
        }

        class ViewBump
        {
            public ViewBump(int x, int y, ViewBump parent)
            {
                X = x;
                Y = y;
                Parent = parent;
            }

            public int X
            {
                get;
                set;
            }

            public int Y
            {
                get;
                set;
            }

            public ViewBump Parent
            {
                get;
                set;
            }
        }

        class View
        {
            public View(Line shallowLine, Line steepLine)
            {
                ShallowLine = shallowLine;
                SteepLine = steepLine;

                ShallowBump = null;
                SteepBump = null;
            }

            public View(View copy)
            {
                ShallowLine = copy.ShallowLine;
                SteepLine = copy.SteepLine;

                ShallowBump = copy.ShallowBump;
                SteepBump = copy.SteepBump;
            }

            public Line ShallowLine
            {
                get;
                set;
            }

            public Line SteepLine
            {
                get;
                set;
            }

            public ViewBump ShallowBump
            {
                get;
                set;
            }

            public ViewBump SteepBump
            {
                get;
                set;
            }
        }
    }
}
