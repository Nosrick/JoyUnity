using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace JoyLib.Code.Helpers
{
    public static class ActionLog
    {
        private static Queue<string> m_Log = new Queue<string>(10);

        private static BlockingCollection<string> s_Queue = new BlockingCollection<string>();
        private static Thread s_LogProcess;

        private const int LINES_TO_KEEP = 10;

        private const string FILENAME = "log";

        private static StreamWriter s_LogFile;

        public static bool OpenLog()
        {
            try
            {
                File.Delete(FILENAME);
                s_LogProcess = new Thread(new ThreadStart(ServiceQueue));
                s_LogProcess.Start();
                WriteToLog("Log Process Started");
                return true;
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogError("COULD NOT START LOG PROCESS");
                UnityEngine.Debug.LogError(ex.Message);
                UnityEngine.Debug.LogError(ex.StackTrace);
                return false;
            }
        }

        public static void WriteToLog(string addition)
        {
            s_Queue.Add(addition + "\n");
        }

        public static void ServiceQueue()
        {
            while(true)
            {
                foreach(string message in s_Queue.GetConsumingEnumerable())
                {
                    File.AppendAllText(FILENAME, message);
                }
            }
        }

        public static void AddText(string stringToAdd, LogType logType = LogType.Information)
        {
            if (logType == LogType.Information || (logType == LogType.Debug && Debugger.IsAttached))
            {
                m_Log.Enqueue(stringToAdd);
                WriteToLog(stringToAdd);
            }

            if (m_Log.Count > LINES_TO_KEEP)
            {
                m_Log.Dequeue();
            }
        }

        public static void LogDamage(int damage, Entity attacker, Entity defender, ItemInstance weapon)
        {
            string damageString = attacker.JoyName + " " + weapon.ItemType.ActionString + " " + defender.JoyName + " for " + damage + ".";
            AddText(damageString);
        }

        public static Queue<string> Log
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
