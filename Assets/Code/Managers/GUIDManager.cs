using System.Collections.Generic;

namespace JoyLib.Code.Managers
{
    public static class GUIDManager
    {
        private static long m_GUIDCounter = 0;

        private static List<long> m_RecycleList = new List<long>();

        public static long AssignGUID()
        {
            if (m_RecycleList.Count > 0)
            {
                long GUID = m_RecycleList[0];
                m_RecycleList.RemoveAt(0);
                return GUID;
            }
            m_GUIDCounter += 1;
            return m_GUIDCounter;
        }

        public static void ReleaseGUID(long GUIDRef)
        {
            m_RecycleList.Add(GUIDRef);
        }
    }
}
