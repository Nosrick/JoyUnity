using System;
using JoyLib.Code.Helpers;
using UnityEngine;

namespace JoyLib.Code
{
    public static class GlobalConstants
    {
        public const string DATA_FOLDER = "/Data/";
        public const string SCRIPTS_FOLDER = DATA_FOLDER + "Scripts/";
        
        public const int MAX_LIGHT = 32;

        public const int SPRITE_SIZE = 16;
        public const int DEFAULT_SUCCESS_THRESHOLD = 7;
        public const int MINIMUM_SUCCESS_THRESHOLD = 4;
        public const int MAXIMUM_SUCCESS_THRESHOLD = 9;

        public const int MINIMUM_VISION_DISTANCE = 3;
        
        public const int FRAMES_PER_SECOND = 30;

        public static readonly Vector2Int NO_TARGET = new Vector2Int(-1, -1);

        public static readonly StringComparer STRING_COMPARER = StringComparer.OrdinalIgnoreCase;

        public static IGameManager GameManager
        {
            get => m_GameManager;
            set => m_GameManager = value;
        }

        public static ActionLog ActionLog
        {
            get;
            set;
        }

        private static IGameManager m_GameManager;
    }
}
