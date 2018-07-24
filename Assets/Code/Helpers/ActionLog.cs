using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JoyLib.Code.Helpers
{
    public static class ActionLog
    {
        private static List<string> m_Log = new List<string>();
        private const int LINES_TO_KEEP = 10;

        public static void AddText(string stringToAdd, LogType logType = LogType.Information)
        {
            if (logType == LogType.Information || (logType == LogType.Debug && Debugger.IsAttached))
            {
                m_Log.Add(stringToAdd);
                Console.WriteLine(stringToAdd);
            }

            if (m_Log.Count > LINES_TO_KEEP)
                m_Log.RemoveAt(0);
        }

        public static void LogDamage(int damage, Entity attacker, Entity defender, ItemInstance weapon)
        {
            string damageString = attacker.JoyName + " " + weapon.ItemType.ActionString + " " + defender.JoyName + " for " + damage + ".";
            AddText(damageString);
        }

        public static List<string> Log
        {
            get
            {
                return m_Log;
            }
        }
    }

    public enum LogType
    {
        Debug,
        Information
    }
}
