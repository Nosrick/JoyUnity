using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
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
                        //TODO: FINISH THIS
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
            IRumour[] possibilities = RumourTypes.Where(r => r.FulfilsConditions(participants)).ToArray();

            if (possibilities.Length > 0)
            {
                rumour = possibilities[RNG.instance.Roll(0, possibilities.Length)].Create(
                    participants,
                    rumour.Tags,
                    rumour.ViralPotential,
                    rumour.Conditions,
                    rumour.Parameters,
                    rumour.Words);
            }
            else
            {
                int result = RNG.instance.Roll(0, RumourTypes.Count);
                rumour = RumourTypes[result].Create(
                    participants,
                    rumour.Tags,
                    rumour.ViralPotential,
                    rumour.Conditions,
                    rumour.Parameters,
                    rumour.Words,
                    true);
            }

            return rumour;
        }

        public IRumour GenerateRumourFromTags(JoyObject[] participants, string[] tags)
        {
            IRumour rumour = null;

            IRumour[] possibilities = RumourTypes.Where(r =>
                r.Tags.Intersect(tags, StringComparer.OrdinalIgnoreCase).Any() && r.FulfilsConditions(participants))
                .ToArray();
            
            if (possibilities.Length > 0)
            {
                IRumour resultingRumour = possibilities[RNG.instance.Roll(0, possibilities.Length)];
                rumour = resultingRumour.Create(
                    participants,
                    resultingRumour.Tags,
                    resultingRumour.ViralPotential,
                    resultingRumour.Conditions,
                    resultingRumour.Parameters,
                    resultingRumour.Words);
            }
            else
            {
                int result = RNG.instance.Roll(0, RumourTypes.Count);
                IRumour resultingRumour = RumourTypes[result];
                rumour = resultingRumour.Create(
                    participants,
                    resultingRumour.Tags,
                    resultingRumour.ViralPotential,
                    resultingRumour.Conditions,
                    resultingRumour.Parameters,
                    resultingRumour.Words,
                    true);
            }

            return rumour;
        }
    }
}