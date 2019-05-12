/*

using MoonSharp.Interpreter;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public class MoonVector2Int
    {
        protected Vector2Int m_AssociatedVector;

        [MoonSharpHidden]
        public MoonVector2Int(Vector2Int vector)
        {
            m_AssociatedVector = vector;
        }

        [MoonSharpHidden]
        public Vector2Int GetOriginalVector()
        {
            return m_AssociatedVector;
        }

        public bool Equals(MoonVector2Int other)
        {
            return m_AssociatedVector == other.m_AssociatedVector;
        }

        public int GetX()
        {
            return m_AssociatedVector.x;
        }

        public void SetX(int x)
        {
            m_AssociatedVector.x = x;
        }

        public int GetY()
        {
            return m_AssociatedVector.y;
        }

        public void SetY(int y)
        {
            m_AssociatedVector.y = y;
        }

        public float GetMagnitude()
        {
            return m_AssociatedVector.magnitude;
        }

        public float GetSquaredMagnitude()
        {
            return m_AssociatedVector.sqrMagnitude;
        }

        public float Distance(MoonVector2Int other)
        {
            return Vector2Int.Distance(m_AssociatedVector, other.m_AssociatedVector);
        }

        public MoonVector2Int Scale(MoonVector2Int other)
        {
            return new MoonVector2Int(Vector2Int.Scale(this.m_AssociatedVector, other.m_AssociatedVector));
        }
    }
}
*/