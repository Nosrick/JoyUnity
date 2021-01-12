using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class ActionLog
    {
        private Queue<LogEntry> m_Queue = new Queue<LogEntry>();

        public const int LINES_TO_KEEP = 10;

        private const string FILENAME = "player.log";

        private readonly bool IsEditor = Application.isEditor;
        
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

        public void LogAction(Entity actor, string actionString)
        {
            this.AddText(actor.JoyName + " is " + actionString, LogLevel.Gameplay);
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

        public void AddText(string stringToAdd, LogLevel logLevel = LogLevel.Information)
        {
            if (this.IsEditor)
            {
                Debug.Log(stringToAdd);
            }
            this.m_Queue.Enqueue(new LogEntry
            {
                m_Data = stringToAdd,
                m_LogLevel = logLevel
            });
        }

        public IEnumerable<string> Log => this.m_Queue
            .Where(data => data.m_LogLevel == LogLevel.Gameplay)
            .Select(data => data.m_Data);
    }

    public enum LogLevel
    {
        Debug,
        Information,
        Gameplay,
        Error
    }

    public struct LogEntry
    {
        public string m_Data;
        public LogLevel m_LogLevel;
    }
}
