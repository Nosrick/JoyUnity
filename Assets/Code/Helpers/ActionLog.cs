using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Helpers
{
    public class ActionLog
    {
        private Queue<string> m_Log = new Queue<string>(10);

        private BlockingCollection<string> m_Queue = new BlockingCollection<string>();
        private Thread m_LogProcess;

        private const int LINES_TO_KEEP = 10;

        private const string FILENAME = "log";

        private StreamWriter s_LogFile;

        public ActionLog()
        {
            OpenLog();
        }

        public void Update()
        {
            this.ServiceQueue();
        }

        public bool OpenLog()
        {
            try
            {
                File.Delete(FILENAME);
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

        private void WriteToLog(string addition)
        {
            m_Queue.Add(addition + "\n");
        }

        public void LogAction(Entity actor, string actionString)
        {
            AddText(actor.JoyName + " is " + actionString);
        }

        protected void ServiceQueue()
        {
            foreach(string message in m_Queue.GetConsumingEnumerable())
            {
                File.AppendAllText(FILENAME, message);
            }
        }

        public void AddText(string stringToAdd, LogType logType = LogType.Information)
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

        public void LogDamage(int damage, Entity attacker, Entity defender, ItemInstance weapon)
        {
            string damageString = attacker.JoyName + " " + weapon.ItemType.ActionString + " " + defender.JoyName + " for " + damage + ".";
            AddText(damageString);
        }

        public Queue<string> Log
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
