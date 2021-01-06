using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using JoyLib.Code.Entities;
using UnityEngine;

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

        private bool IsEditor = Application.isEditor;

        public ActionLog()
        {
            this.OpenLog();
        }

        public IEnumerator Update()
        {
            this.ServiceQueue();
            yield return new WaitForSeconds(0.1f);
        }

        public bool OpenLog()
        {
            try
            {
                File.Delete(FILENAME);
                this.WriteToLog("Log Process Started");
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
            this.m_Queue.Add(addition + "\n");
        }

        public void LogAction(Entity actor, string actionString)
        {
            this.AddText(actor.JoyName + " is " + actionString);
        }

        protected void ServiceQueue()
        {
            foreach(string message in this.m_Queue.GetConsumingEnumerable())
            {
                File.AppendAllText(FILENAME, message);
            }
        }

        public void AddText(string stringToAdd, LogType logType = LogType.Information)
        {
            if (this.IsEditor)
            {
                UnityEngine.Debug.Log(stringToAdd);
            }
            this.m_Log.Enqueue(stringToAdd);
            this.WriteToLog(stringToAdd);

            if (this.m_Log.Count > LINES_TO_KEEP)
            {
                this.m_Log.Dequeue();
            }
        }

        public IReadOnlyCollection<string> Log => this.m_Log;
    }

    public enum LogType
    {
        Debug,
        Information,
        Error
    }
}
