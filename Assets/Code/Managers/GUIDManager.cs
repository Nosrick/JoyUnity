using System;
using System.Collections.Generic;

namespace JoyLib.Code.Managers
{
    public class GUIDManager
    {
        private static Lazy<GUIDManager> lazy = new Lazy<GUIDManager>(() => new GUIDManager());

        private long m_GUIDCounter = 0;

        private List<long> m_RecycleList = new List<long>();

        public long AssignGUID()
        {
            if (this.m_RecycleList.Count > 0)
            {
                long GUID = this.m_RecycleList[0];
                this.m_RecycleList.RemoveAt(0);
                return GUID;
            }

            this.m_GUIDCounter += 1;
            return this.m_GUIDCounter;
        }

        public void ReleaseGUID(long GUIDRef)
        {
            this.m_RecycleList.Add(GUIDRef);
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
