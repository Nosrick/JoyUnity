using System.Collections.Generic;
using System;

namespace JoyLib.Code.Managers
{
    public class GUIDManager
    {
        private static Lazy<GUIDManager> lazy = new Lazy<GUIDManager>(() => new GUIDManager());

        private long m_GUIDCounter = 0;

        private List<long> m_RecycleList = new List<long>();

        public long AssignGUID()
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

        public void ReleaseGUID(long GUIDRef)
        {
            m_RecycleList.Add(GUIDRef);
        }

        public static GUIDManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
    }
}
