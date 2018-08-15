using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.World
{
    public class InteriorRoom
    {
        private Rect m_Sizes;

        private List<InteriorRoom> m_Neighbours;

        private List<string> m_UsedEntrances;

        public InteriorRoom(Rect sizesRef, List<InteriorRoom> neighboursRef)
        {
            m_Sizes = sizesRef;
            m_Neighbours = neighboursRef;
            m_UsedEntrances = new List<string>();
        }

        public Rect sizes
        {
            get
            {
                return m_Sizes;
            }
        }

        public List<InteriorRoom> neighbours
        {
            get
            {
                return m_Neighbours;
            }
        }

        public List<string> usedEntrances
        {
            get
            {
                return m_UsedEntrances;
            }
        }
    }
}
