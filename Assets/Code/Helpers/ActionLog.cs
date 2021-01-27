﻿using System;
using System.Collections.Generic;
using System.IO;
using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class ActionLog : IDisposable
    {
        public List<string> History { get; protected set; } 
        
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
                this.m_Queue = new Queue<LogEntry>();
                this.History = new List<string>();
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

            while (this.History.Count > LINES_TO_KEEP)
            {
                this.History.RemoveAt(0);
            }
        }

        public void AddText(string stringToAdd, LogLevel logLevel = LogLevel.Information)
        {
            if (this.IsEditor)
            {
                switch (logLevel)
                {
                    case LogLevel.Warning:
                        Debug.LogWarning(stringToAdd);
                        break;
                    
                    case LogLevel.Error:
                        Debug.LogError(stringToAdd);
                        break;
                    
                    default:
                        Debug.Log(stringToAdd);
                        break;
                }
            }

            LogEntry entry = new LogEntry
            {
                m_Data = stringToAdd,
                m_LogLevel = logLevel
            };
            this.m_Queue.Enqueue(entry);
            if (logLevel == LogLevel.Gameplay)
            {
                this.History.Add(stringToAdd);
            }
        }

        ~ActionLog()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Writer?.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public enum LogLevel
    {
        Debug,
        Information,
        Gameplay,
        Warning,
        Error
    }

    public class LogEntry
    {
        public string m_Data;
        public LogLevel m_LogLevel;

        public override string ToString()
        {
            return "[" + this.m_LogLevel + "]: " + this.m_Data;
        }
    }
}
