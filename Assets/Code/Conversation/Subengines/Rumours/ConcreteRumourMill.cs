using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Helpers;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations
{
    public class ConcreteRumourMill : IRumourMill
    {
        public List<IRumour> Rumours { get; protected set; }

        public List<IRumour> RumourTypes { get; protected set; }

        public ConcreteRumourMill()
        {
            Rumours = new List<IRumour>();
            if (RumourTypes is null)
            {
                RumourTypes = LoadRumours();
            }
        }

        protected List<IRumour> LoadRumours()
        {
            List<IRumour> rumours = new List<IRumour>();

            string[] files = Directory.GetFiles(
                Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Rumours",
                "*.xml",
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    foreach (XElement line in doc.Elements("Line"))
                    {
                        string text = line.Element("Text").GetAs<string>("SOMEONE FORGOT TO INCLUDE TEXT.");
                    }
                }
                catch (Exception e)
                {
                    ActionLog.instance.AddText("Could not load rumours from file " + file);
                    Debug.LogWarning("Could not load rumours from file " + file);
                    Debug.LogWarning(e.Message);
                    Debug.LogWarning(e.StackTrace);
                }
            }

            return rumours;
        }

        public IRumour GenerateRandomRumour(JoyObject[] participants)
        {
            IRumour rumour = null;
            int breakout = 0;
            while (rumour is null && breakout < 100)
            {
                IRumour[] possibilities = RumourTypes.Where(r => r.FulfilsConditions(participants)).ToArray();
                
                
                
                breakout++;
            }

            return rumour;
        }

        public IRumour GenerateRumourFromTags(JoyObject[] participants, string[] tags)
        {
            throw new System.NotImplementedException();
        }
    }
}