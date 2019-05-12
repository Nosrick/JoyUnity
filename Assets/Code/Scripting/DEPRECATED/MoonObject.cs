/*

using MoonSharp.Interpreter;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public class MoonObject
    {
        protected JoyObject m_AssociatedObject;

        [MoonSharpHidden]
        public MoonObject(JoyObject obj)
        {
            m_AssociatedObject = obj;
        }

        [MoonSharpHidden]
        public JoyObject GetAssociatedObject()
        {
            return m_AssociatedObject;
        }

        public bool IsAlive()
        {
            return m_AssociatedObject.Alive;
        }

        public bool IsDestructible()
        {
            return m_AssociatedObject.IsDestructible;
        }

        public bool IsWall()
        {
            return m_AssociatedObject.IsWall;
        }

        public MoonVector2Int GetPosition()
        {
            return new MoonVector2Int(m_AssociatedObject.WorldPosition);
        }

        public string GetName()
        {
            return m_AssociatedObject.JoyName;
        }

        public string GetBaseType()
        {
            return m_AssociatedObject.BaseType;
        }

        public long GetGUID()
        {
            return m_AssociatedObject.GUID;
        }

        public int GetHP()
        {
            return m_AssociatedObject.HitPoints;
        }

        public int GetHPRemaining()
        {
            return m_AssociatedObject.HitPointsRemaining;
        }
    }
}

*/