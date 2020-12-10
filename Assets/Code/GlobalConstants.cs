using System;
using UnityEngine;

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

        public const string NEEDSRECT = "NeedsRect";
        public const string INVENTORY = "Inventory";
        public const string EQUIPMENT = "Equipment";
        public const string CONVERSATION = "Conversation Window";
        public const string CONTEXT_MENU = "Context Menu";
        public const string TRADE = "Trade";
        public const string QUEST_JOURNAL = "Quest Journal";
        public const string CHARACTER_CREATION_PART_1 = "Character Creation Part 1";
        public const string CHARACTER_CREATION_PART_2 = "Character Creation Part 2";
        public const string TOOLTIP = "Tooltip";
        public const string JOB_MANAGEMENT = "Job Management";
        public const string CHARACTER_SHEET = "Character Sheet";

        public static IGameManager GameManager
        {
            get => m_GameManager;
            set => m_GameManager = value;
        }

        private static IGameManager m_GameManager;
    }
}
