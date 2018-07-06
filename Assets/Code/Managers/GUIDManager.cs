using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoyLib.Code.Managers
{
    public static class GUIDManager
    {
        private static int m_GUIDCounter = 0;

        private static List<int> m_RecycleList = new List<int>();

        public static int AssignGUID()
        {
            if (m_RecycleList.Count > 0)
            {
                int GUID = m_RecycleList[0];
                m_RecycleList.RemoveAt(0);
                return GUID;
            }
            m_GUIDCounter += 1;
            return m_GUIDCounter;
        }

        public static void ReleaseGUID(int GUIDRef)
        {
            m_RecycleList.Add(GUIDRef);
        }
    }
}
