using UnityEngine;
using System;

namespace JoyLib.Code
{
    public static class GlobalConstants
    {
        public const string DATA_FOLDER = "/Assets/Data/";
        public const string SCRIPTS_FOLDER = DATA_FOLDER + "Scripts/";

        public const int SPRITE_SIZE = 16;
        public const int DEFAULT_SUCCESS_THRESHOLD = 7;

        public const int MINIMUM_VISION_DISTANCE = 3;

        public static readonly Vector2Int NO_TARGET = new Vector2Int(-1, -1);

        public static readonly StringComparer STRING_COMPARER = StringComparer.OrdinalIgnoreCase;
    }
}
