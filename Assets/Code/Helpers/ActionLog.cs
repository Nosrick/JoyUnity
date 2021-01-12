using System;
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

        private Queue<string> m_Queue = new Queue<string>();
        private Thread m_LogProcess;

        private const int LINES_TO_KEEP = 10;

        private const string FILENAME = "log";

        private StreamWriter s_LogFile;

        private bool IsEditor = Application.isEditor;
        
        private StreamWriter Writer { get; set; }

        public ActionLog()
        {
            this.OpenLog();
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
                this.Writer = new StreamWriter(FILENAME);
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError("COULD NOT START LOG PROCESS");
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return false;
            }
        }

        private void WriteToLog(string addition)
        {
            this.m_Queue.Enqueue(addition + "\n");
        }

        public void LogAction(Entity actor, string actionString)
        {
            this.AddText(actor.JoyName + " is " + actionString);
        }

        protected void ServiceQueue()
        {
            bool written = false;
            while (this.m_Queue.Count > 0)
            {
                this.Writer.WriteLine(this.m_Queue.Dequeue());
                written = true;
            }

            if (written)
            {
                this.Writer.Flush();
            }
        }

        public void AddText(string stringToAdd, LogType logType = LogType.Information)
        {
            if (this.IsEditor)
            {
                Debug.Log(stringToAdd);
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
