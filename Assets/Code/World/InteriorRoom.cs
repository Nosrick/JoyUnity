using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.World
{
    public class InteriorRoom
    {
        private RectInt m_Sizes;

        private List<InteriorRoom> m_Neighbours;

        private List<string> m_UsedEntrances;

        public InteriorRoom(RectInt sizesRef, List<InteriorRoom> neighboursRef)
        {
            this.m_Sizes = sizesRef;
            this.m_Neighbours = neighboursRef;
            this.m_UsedEntrances = new List<string>();
        }

        public RectInt sizes
        {
            get
            {
                return this.m_Sizes;
            }
        }

        public List<InteriorRoom> neighbours
        {
            get
            {
                return this.m_Neighbours;
            }
        }

        public List<string> usedEntrances
        {
            get
            {
                return this.m_UsedEntrances;
            }
        }
    }
}
