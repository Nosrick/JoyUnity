﻿using UnityEngine;

namespace JoyLib.Code.Entities.AI
{
    public struct NeedAIData
    {
        public IJoyObject target;
        public Vector2Int targetPoint;
        public bool searching;
        public Intent intent;
        public bool idle;
        public string need;
    }
}
