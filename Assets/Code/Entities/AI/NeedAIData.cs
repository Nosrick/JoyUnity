using System;
using UnityEngine;

namespace JoyLib.Code.Entities.AI
{
    public class NeedAIData
    {
        public IJoyObject target;
        public Vector2Int targetPoint;
        public bool searching;
        public Intent intent;
        public bool idle;
        public string need;
    }

    public class NeedDataSerialisable
    {
        public Guid targetGuid;
        public string targetType;
        public Vector2Int targetPoint;
        public bool searching;
        public Intent intent;
        public bool idle;
        public string need;
    }
}
