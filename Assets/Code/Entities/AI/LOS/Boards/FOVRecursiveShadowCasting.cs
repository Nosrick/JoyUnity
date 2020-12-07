using System;
using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVRecursiveShadowCasting : AbstractFOVHandler
    {
        protected FOVBasicBoard m_Board;

        protected IEntity Viewer { get; set; }

        List<int> VisibleOctants = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8};

        public FOVRecursiveShadowCasting()
        {
            m_Board = new FOVBasicBoard(0, 0, new Vector2Int[0]);
        }

        public override IFOVBoard Do(IEntity viewer, IWorldInstance world, Vector2Int dimensions,
            IEnumerable<Vector2Int> walls)
        {
            Viewer = viewer;
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);
            m_Board.Visible(Viewer.WorldPosition.x, Viewer.WorldPosition.y);
            foreach (int octant in VisibleOctants)
            {
                ScanOctant(1, octant, 1.0, 0.0);
            }

            return m_Board;
        }

        protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
        {
            int visionModSqr = Viewer.VisionMod * Viewer.VisionMod;
            int x, y;
            x = y = 0;

            Vector2Int viewerPosition = Viewer.WorldPosition;

            switch (pOctant)
            {
                case 1: //nnw
                    y = viewerPosition.y - pDepth;
                    if (y < 0)
                    {
                        return;
                    }

                    x = viewerPosition.x - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0)
                    {
                        x = 0;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, false) >= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y)) //current cell blocked
                                {
                                    if (x - 1 >= 0 && m_Board.IsObstacle(x - 1, y) == false
                                    ) //prior cell within range AND open...
                                    {
                                        //...incremenet the depth, adjust the endslope and recurse
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x - 0.5, y + 0.5, viewerPosition.x, viewerPosition.y, false));
                                    }
                                }
                                else
                                {
                                    if (x - 1 >= 0 && m_Board.IsObstacle(x - 1, y) == false
                                    ) //prior cell within range AND open...
                                    {
                                        //..adjust the startslope
                                        pStartSlope = GetSlope(x - 0.5, y - 0.5, viewerPosition.x, viewerPosition.y,
                                            false);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        x++;
                    }

                    x--;
                    break;

                case 2: //nne

                    y = viewerPosition.y - pDepth;
                    if (y < 0)
                    {
                        return;
                    }

                    x = viewerPosition.x + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= m_Board.Width)
                    {
                        x = m_Board.Width - 1;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, false) <= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (x + 1 < m_Board.Width && m_Board.IsObstacle(x + 1, y) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x + 0.5, y + 0.5, viewerPosition.x, viewerPosition.y, false));
                                    }
                                }
                                else
                                {
                                    if (x + 1 < m_Board.Width && m_Board.IsObstacle(x + 1, y) == false)
                                    {
                                        pStartSlope = -GetSlope(x + 0.5, y - 0.5, viewerPosition.x, viewerPosition.y,
                                            false);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        x--;
                    }

                    x++;
                    break;

                case 3:

                    x = viewerPosition.x + pDepth;
                    if (x >= m_Board.Width)
                    {
                        return;
                    }

                    y = viewerPosition.y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0)
                    {
                        y = 0;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, true) <= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (y - 1 >= 0 && m_Board.IsObstacle(x, y - 1) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x - 0.5, y - 0.5, viewerPosition.x, viewerPosition.y, true));
                                    }
                                }
                                else
                                {
                                    if (y - 1 >= 0 && m_Board.IsObstacle(x, y - 1) == false)
                                    {
                                        pStartSlope = -GetSlope(x + 0.5, y - 0.5, viewerPosition.x, viewerPosition.y,
                                            true);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        y++;
                    }

                    y--;
                    break;

                case 4:

                    x = viewerPosition.x + pDepth;
                    if (x >= m_Board.Width)
                    {
                        return;
                    }

                    y = viewerPosition.y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= m_Board.Height)
                    {
                        y = m_Board.Height - 1;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, false) >= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (y + 1 < m_Board.Height && m_Board.IsObstacle(x, y + 1) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x - 0.5, y + 0.5, viewerPosition.x, viewerPosition.y, true));
                                    }
                                }
                                else
                                {
                                    if (y + 1 < m_Board.Height && m_Board.IsObstacle(x, y + 1) == false)
                                    {
                                        pStartSlope = GetSlope(x + 0.5, y + 0.5, viewerPosition.x, viewerPosition.y,
                                            true);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        y--;
                    }

                    y++;
                    break;

                case 5:

                    y = viewerPosition.y + pDepth;
                    if (y >= m_Board.Height)
                    {
                        return;
                    }

                    x = viewerPosition.x + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= m_Board.Width)
                    {
                        x = m_Board.Width - 1;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, false) >= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (x + 1 < m_Board.Height && m_Board.IsObstacle(x + 1, y) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x + 0.5, y - 0.5, viewerPosition.x, viewerPosition.y, false));
                                    }
                                }
                                else
                                {
                                    if (x + 1 < m_Board.Height
                                        && m_Board.IsObstacle(x + 1, y) == false)
                                    {
                                        pStartSlope = GetSlope(x + 0.5, y + 0.5, viewerPosition.x, viewerPosition.y,
                                            false);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        x--;
                    }

                    x++;
                    break;

                case 6:

                    y = viewerPosition.y + pDepth;
                    if (y >= m_Board.Height)
                    {
                        return;
                    }

                    x = viewerPosition.x - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0)
                    {
                        x = 0;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, false) <= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (x - 1 >= 0 && m_Board.IsObstacle(x - 1, y) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x - 0.5, y - 0.5, viewerPosition.x, viewerPosition.y, false));
                                    }
                                }
                                else
                                {
                                    if (x - 1 >= 0
                                        && m_Board.IsObstacle(x - 1, y) == false)
                                    {
                                        pStartSlope = -GetSlope(x - 0.5, y + 0.5, viewerPosition.x, viewerPosition.y,
                                            false);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        x++;
                    }

                    x--;
                    break;

                case 7:

                    x = viewerPosition.x - pDepth;
                    if (x < 0)
                    {
                        return;
                    }

                    y = viewerPosition.y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= m_Board.Height)
                    {
                        y = m_Board.Height - 1;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, true) <= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (y + 1 < m_Board.Height && m_Board.IsObstacle(x, y + 1) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x + 0.5, y + 0.5, viewerPosition.x, viewerPosition.y, true));
                                    }
                                }
                                else
                                {
                                    if (y + 1 < m_Board.Height && m_Board.IsObstacle(x, y + 1) == false)
                                    {
                                        pStartSlope = -GetSlope(x - 0.5, y + 0.5, viewerPosition.x, viewerPosition.y,
                                            true);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        y--;
                    }

                    y++;
                    break;

                case 8: //wnw

                    x = viewerPosition.x - pDepth;
                    if (x < 0)
                    {
                        return;
                    }

                    y = viewerPosition.y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0)
                    {
                        y = 0;
                    }

                    while (GetSlope(x, y, viewerPosition.x, viewerPosition.y, true) >= pEndSlope)
                    {
                        if (GetDistance(x, y, viewerPosition.x, viewerPosition.y) <= visionModSqr)
                        {
                            if (m_Board.HasVisited(x, y) == false)
                            {
                                if (m_Board.IsObstacle(x, y))
                                {
                                    if (y - 1 >= 0 && m_Board.IsObstacle(x, y - 1) == false)
                                    {
                                        ScanOctant(pDepth + 1, pOctant, pStartSlope,
                                            GetSlope(x + 0.5, y - 0.5, viewerPosition.x, viewerPosition.y, true));
                                    }
                                }
                                else
                                {
                                    if (y - 1 >= 0 && m_Board.IsObstacle(x, y - 1) == false)
                                    {
                                        pStartSlope = GetSlope(x - 0.5, y - 0.5, viewerPosition.x, viewerPosition.y,
                                            true);
                                    }

                                    m_Board.Visible(x, y);
                                }
                            }
                        }

                        y++;
                    }

                    y--;
                    break;
            }
            
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= m_Board.Width)
            {
                x = m_Board.Width - 1;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y >= m_Board.Height)
            {
                y = m_Board.Height - 1;
            }

            if (pDepth < visionModSqr & m_Board.IsVisible(x, y))
            {
                ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);
            }
        }

        protected double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
        {
            if (pInvert)
            {
                return (pY1 - pY2) / (pX1 - pX2);
            }
            else
            {
                return (pX1 - pX2) / (pY1 - pY2);
            }
        }

        protected int GetDistance(int pX1, int pY1, int pX2, int pY2)
        {
            return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
        }
    }
}