using UnityEngine;
using System;

namespace JoyLib.Code
{
    public static class GlobalConstants
    {
        public const string DATA_FOLDER = "/Assets/Code/Data/";
        public const string SCRIPTS_FOLDER = DATA_FOLDER + "Scripts/";

        public const int SPRITE_SIZE = 16;
        public const int DEFAULT_SUCCESS_THRESHOLD = 7;

        public const int MINIMUM_VISION_DISTANCE = 3;
        
        public const int FRAMES_PER_SECOND = 30;

        public static readonly Vector2Int NO_TARGET = new Vector2Int(-1, -1);

        public static readonly StringComparer STRING_COMPARER = StringComparer.OrdinalIgnoreCase;

        public static GameObject GameManager
        {
            get
            {
                if (m_GameManager is null)
                {
                    m_GameManager = GameObject.Find("GameManager");
                }

                return m_GameManager;
            }
            set => m_GameManager = value;
        }

        private static GameObject m_GameManager;
    }
}
