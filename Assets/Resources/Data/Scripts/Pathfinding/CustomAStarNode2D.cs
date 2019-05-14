using JoyLib.Code.Helpers;
using System;
using System.Collections;

namespace JoyLib.Code.Entities.AI
{
    public class CustomAStarNode2D : IComparable
    {
        public CustomAStarNode2D(int x, int y, CustomAStarNode2D parent, CustomAStarNode2D goal, double cost)
        {
            Parent = parent;
            GoalNode = goal;
            Cost = cost;
            X = x;
            Y = y;
        }

        public bool IsGoal()
        {
            return IsSameState(GoalNode);
        }

        public int CompareTo(object obj)
        {
            return (-TotalCost.CompareTo(((CustomAStarNode2D)obj).TotalCost));
        }

        public override string ToString()
        {
            return " { " + X.ToString() + " : " + Y.ToString() + " } ";
        }

        public bool IsSameState(CustomAStarNode2D node)
        {
            if (node == null)
            {
                return false;
            }
            /*
            string log = "CHECKING FOR SAMENESS BETWEEN " + this.ToString() + " AND " + node.ToString();
            if(node.X == X && node.Y == Y)
            {
                ActionLog.WriteToLog(log + " SAMENESS FOUND");
            }
            else
            {
                ActionLog.WriteToLog(log + " DIFFERENCE FOUND");
            }
            */
            return (node.X == X) && (node.Y == Y);
        }

        public void Calculate()
        {
            if (GoalNode != null)
            {
                double xd = X - GoalNode.X;
                double yd = Y - GoalNode.Y;

                Estimate = Math.Max(Math.Abs(xd), Math.Abs(yd));
            }
            else
            {
                Estimate = 0;
            }
        }

        public void AddSuccessor(ArrayList successors, int x, int y)
        {
            CustomAStarNode2D newNode = new CustomAStarNode2D(x, y, this, GoalNode, Cost + 1);
            if (newNode.IsSameState(Parent))
            {
                return;
            }
            ActionLog.WriteToLog("ADDED SUCCESSOR: " + newNode.ToString());
            successors.Add(newNode);
        }

        public void GetSuccessors(ArrayList arrayList)
        {
            arrayList.Clear();
            AddSuccessor(arrayList, X + 1, Y);
            AddSuccessor(arrayList, X - 1, Y);
            AddSuccessor(arrayList, X, Y + 1);
            AddSuccessor(arrayList, X, Y - 1);
            AddSuccessor(arrayList, X + 1, Y + 1);
            AddSuccessor(arrayList, X + 1, Y - 1);
            AddSuccessor(arrayList, X - 1, Y + 1);
            AddSuccessor(arrayList, X - 1, Y - 1);
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

        public CustomAStarNode2D Parent
        {
            get;
            set;
        }

        public double Cost
        {
            get;
            set;
        }

        protected double m_Estimate;
        public double Estimate
        {
            get
            {
                Calculate();
                return m_Estimate;
            }
            set
            {
                m_Estimate = value;
            }
        }

        public double TotalCost
        {
            get
            {
                return Cost + Estimate;
            }
        }

        public CustomAStarNode2D GoalNode
        {
            get
            {
                return m_GoalNode;
            }
            set
            {
                m_GoalNode = value;
                Calculate();
            }
        }
        protected CustomAStarNode2D m_GoalNode;
    }
}
