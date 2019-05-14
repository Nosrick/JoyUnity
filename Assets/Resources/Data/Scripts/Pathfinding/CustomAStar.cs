using JoyLib.Code.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using Tanis.Collections;
using UnityEngine;

namespace JoyLib.Code.Entities.AI
{
    public class CustomAStar
    {
        protected CustomAStarNode2D m_StartNode;
        protected CustomAStarNode2D m_GoalNode;
        protected Heap m_OpenList;
        protected Heap m_ClosedList;
        protected ArrayList m_Successors;

        public CustomAStar(List<Vector2Int> walls, RectInt sizes)
        {
            m_OpenList = new Heap();
            m_ClosedList = new Heap();
            m_Successors = new ArrayList();
            m_Solution = new ArrayList();

            Walls = walls;
            Sizes = sizes;
        }

        public ArrayList Solution
        {
            get
            {
                return m_Solution;
            }
        }
        protected ArrayList m_Solution;

        public void FindPath(CustomAStarNode2D fromNode, CustomAStarNode2D toNode)
        {
            ActionLog.WriteToLog(fromNode.ToString() + " TO " + toNode.ToString());
            ActionLog.WriteToLog("SIZE IS { " + Sizes.xMin + " : " + Sizes.yMin + " }, " + " { " + Sizes.xMax + " : " + Sizes.yMax + " }");
            m_StartNode = fromNode;
            m_GoalNode = toNode;

            int loopBreak = 0;

            m_OpenList.Add(m_StartNode);

            while (m_OpenList.Count > 0 && loopBreak < 100)
            {
                CustomAStarNode2D currentNode = (CustomAStarNode2D)m_OpenList.Pop();
                ActionLog.WriteToLog("POPPED NODE " + currentNode.ToString());
                ActionLog.WriteToLog("OPEN LIST LENGTH: " + m_OpenList.Count.ToString());

                if (Sizes.Contains(new Vector2Int(currentNode.X, currentNode.Y)) == false)
                {
                    ActionLog.WriteToLog(currentNode.ToString() + " IS OUT OF BOUNDS, DISCARDING");
                    continue;
                }

                if (Walls.Contains(new Vector2Int(currentNode.X, currentNode.Y)) == true)
                {
                    ActionLog.WriteToLog(currentNode.ToString() + " IS A WALL, DISCARDING");
                    continue;
                }

                ActionLog.WriteToLog("CHECKING IF NODE IS GOAL");
                if (currentNode.IsGoal())
                {
                    ActionLog.WriteToLog("FOUND PATH");
                    while (currentNode != null)
                    {
                        ActionLog.WriteToLog("ADDING " + currentNode.ToString() + " TO PATH");
                        m_Solution.Insert(0, currentNode);
                        currentNode = currentNode.Parent;
                    }
                    return;
                }

                ActionLog.WriteToLog("GETTING SUCCESSORS");
                currentNode.GetSuccessors(m_Successors);
                foreach (CustomAStarNode2D successor in m_Successors)
                {
                    // Test if the current successor node is on the open list, if it is and
                    // the TotalCost is higher, we will throw away the current successor.
                    CustomAStarNode2D open = null;
                    if (m_OpenList.Contains(successor))
                    {
                        open = (CustomAStarNode2D)m_OpenList[m_OpenList.IndexOf(successor)];
                    }
                    if ((open != null) && (successor.TotalCost > open.TotalCost))
                    {
                        ActionLog.WriteToLog("DISCARDING " + open.ToString() + " AS IT IS TOO COSTLY AT " + successor.TotalCost.ToString() + " VERSUS " + open.TotalCost.ToString());
                        continue;
                    }

                    // Test if the current successor node is on the closed list, if it is and
                    // the TotalCost is higher, we will throw away the current successor.
                    CustomAStarNode2D closed = null;
                    if (m_ClosedList.Contains(successor))
                    {
                        closed = (CustomAStarNode2D)m_ClosedList[m_ClosedList.IndexOf(successor)];
                    }
                    if ((closed != null) && (successor.TotalCost > closed.TotalCost))
                    {
                        ActionLog.WriteToLog("DISCARDING " + closed.ToString() + " AS IT IS TOO COSTLY AT " + successor.TotalCost.ToString() + " VERSUS " + closed.TotalCost.ToString());
                        continue;
                    }

                    // Remove the old successor from the open list
                    if(open != null)
                    {
                        ActionLog.WriteToLog("REMOVING " + open.ToString() + " FROM OPEN LIST");
                    }
                    m_OpenList.Remove(open);

                    // Remove the old successor from the closed list
                    if(closed != null)
                    {
                        ActionLog.WriteToLog("REMOVING " + closed.ToString() + " FROM CLOSED LIST");
                    }
                    m_ClosedList.Remove(closed);

                    ActionLog.WriteToLog("ADDING SUCCESSOR " + successor.ToString() + " TO OPEN LIST");
                    // Add the current successor to the open list
                    m_OpenList.Push(successor);
                }

                ActionLog.WriteToLog("ADDING " + currentNode.ToString() + " TO CLOSED LIST");
                m_ClosedList.Add(currentNode);
                loopBreak += 1;
            }
            ActionLog.WriteToLog(fromNode.ToString() + " TO " + toNode.ToString() + " FIZZLED OUT, PATH NOT FOUND");
            ActionLog.WriteToLog("OPEN LIST LENGTH: " + m_OpenList.Count.ToString());
        }

        public List<Vector2Int> Walls
        {
            get;
            protected set;
        }

        public RectInt Sizes
        {
            get;
            protected set;
        }
    }
}
