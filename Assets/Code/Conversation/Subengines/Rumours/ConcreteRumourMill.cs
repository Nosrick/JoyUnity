using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Conversation.Conversations.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations
{
    public class ConcreteRumourMill : IRumourMill
    {
        public List<IRumour> Rumours { get; protected set; }

        public List<IRumour> RumourTypes { get; protected set; }

        public ConcreteRumourMill()
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (RumourTypes is null)
            {
                Rumours = new List<IRumour>();
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
                        string words = line.Element("Text").DefaultIfEmpty("SOMEONE FORGOT TO INCLUDE TEXT.");
                        string processor = line.Element("Processor").DefaultIfEmpty("NONE");
                        float viralPotential = line.Element("ViralPotential").DefaultIfEmpty(1.0f);
                        string[] tags = (from tag in line.Elements("Tag")
                            select tag.GetAs<string>()).ToArray();
                        string[] conditionStrings = (from condition in line.Elements("Condition")
                            select condition.GetAs<string>()).ToArray();
                        string[] parameters = (from parameter in line.Elements("Parameter")
                            select parameter.GetAs<string>()).ToArray();
                        bool baseless = line.Element("Baseless").DefaultIfEmpty(false);
                        float lifetimeMultipler = line.Element("LifetimeMultiplier").DefaultIfEmpty(1f);
                        int lifetime = line.Element("Lifetime").DefaultIfEmpty(BaseRumour.DEFAULT_LIFETIME);

                        List<ITopicCondition> conditions = new List<ITopicCondition>();
                        foreach (string conditionString in conditionStrings)
                        {
                            conditions.Add(ParseCondition(conditionString));
                        }

                        if (processor.Equals("NONE", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            IRumour processorObject = (IRumour) ScriptingEngine.instance.FetchAndInitialise(processor);
                            rumours.Add(
                                processorObject.Create(
                                    null,
                                    tags,
                                    viralPotential,
                                    conditions.ToArray(),
                                    parameters,
                                    words,
                                    lifetimeMultipler,
                                    lifetime,
                                    baseless));
                        }
                        else
                        {
                            rumours.Add(
                                new BaseRumour(
                                    null,
                                    tags,
                                    viralPotential,
                                    conditions.ToArray(),
                                    parameters,
                                    words,
                                    lifetimeMultipler,
                                    lifetime,
                                    baseless));
                        }
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
            if (RumourTypes is null)
            {
                Initialise();
            }
            
            IRumour rumour = null;
            IRumour[] possibilities = RumourTypes.Where(r => r.FulfilsConditions(participants)).ToArray();

            if (possibilities.Length > 0)
            {
                IRumour selected = possibilities[RNG.instance.Roll(0, possibilities.Length)];
                rumour = selected.Create(
                    participants,
                    selected.Tags,
                    selected.ViralPotential,
                    selected.Conditions,
                    selected.Parameters,
                    selected.Words,
                    selected.LifetimeMultiplier,
                    selected.Lifetime);
            }
            else
            {
                IRumour selected = RumourTypes[RNG.instance.Roll(0, RumourTypes.Count)];
                rumour = selected.Create(
                    participants,
                    selected.Tags,
                    selected.ViralPotential,
                    selected.Conditions,
                    selected.Parameters,
                    selected.Words,
                    selected.LifetimeMultiplier,
                    selected.Lifetime,
                    true);
            }

            return rumour;
        }

        public IRumour GenerateRumourFromTags(JoyObject[] participants, string[] tags)
        {
            if (RumourTypes is null)
            {
                Initialise();
            }
            
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
                    resultingRumour.Words,
                    resultingRumour.LifetimeMultiplier,
                    resultingRumour.Lifetime);
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
                    resultingRumour.LifetimeMultiplier,
                    resultingRumour.Lifetime,
                    true);
            }

            return rumour;
        }

        public IRumour[] GenerateOneRumourOfEachType(JoyObject[] participants)
        {
            List<IRumour> rumours = new List<IRumour>();
            foreach (IRumour type in RumourTypes)
            {
                rumours.Add(type.Create(
                    participants,
                    type.Tags,
                    type.ViralPotential,
                    type.Conditions,
                    type.Parameters,
                    type.Words,
                    type.LifetimeMultiplier,
                    type.Lifetime,
                    type.Baseless));
            }

            return rumours.ToArray();
        }

        protected ITopicCondition ParseCondition(string conditionString)
        {
            try
            {
                string[] split = conditionString.Split(new char[] {'<', '>', '=', '!'}, StringSplitOptions.RemoveEmptyEntries);

                string criteria = split[0].Trim();
                string operand = conditionString.First(c => c.Equals('!')
                                                            || c.Equals('=')
                                                            || c.Equals('<')
                                                            || c.Equals('>')).ToString();
                string stringValue = split[1].Trim();
            
                TopicConditionFactory factory = new TopicConditionFactory();

                int value = criteria.Equals("relationship", StringComparison.OrdinalIgnoreCase) && operand.Equals("=")
                    ? 1
                    : int.Parse(stringValue);

                criteria = criteria.Equals("relationship", StringComparison.OrdinalIgnoreCase) && operand.Equals("=") ? stringValue : criteria;

                return factory.Create(criteria, operand, value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not parse conversation condition line " + conditionString);
            }
        }
    }
}