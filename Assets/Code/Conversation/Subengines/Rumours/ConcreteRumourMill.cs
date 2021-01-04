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
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations
{
    public class ConcreteRumourMill : IRumourMill
    {
        public List<IRumour> Rumours { get; protected set; }

        public List<IRumour> RumourTypes { get; protected set; }

        public RNG Roller { get; protected set; }

        public ConcreteRumourMill(RNG roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller;
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.RumourTypes is null)
            {
                this.Rumours = new List<IRumour>();
                this.RumourTypes = this.LoadRumours();
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
                            conditions.Add(this.ParseCondition(conditionString));
                        }

                        if (processor.Equals("NONE", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            IRumour processorObject = (IRumour) ScriptingEngine.Instance.FetchAndInitialise(processor);
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
                    GlobalConstants.ActionLog.AddText("Could not load rumours from file " + file);
                    Debug.LogWarning("Could not load rumours from file " + file);
                    Debug.LogWarning(e.Message);
                    Debug.LogWarning(e.StackTrace);
                }
            }

            return rumours;
        }

        public IRumour GetRandom(IWorldInstance overworldRef)
        {
            if (this.Rumours.Count == 0)
            {
                IJoyObject left = overworldRef.GetRandomSentientWorldWide();
                IJoyObject right = left.MyWorld.GetRandomSentient();
                this.Rumours.Add(this.GenerateRandomRumour(new []{left, right}));
            }

            return this.Rumours[this.Roller.Roll(0, this.Rumours.Count)];
        }

        public IRumour GenerateRandomRumour(IJoyObject[] participants)
        {
            if (this.RumourTypes is null)
            {
                this.Initialise();
            }
            
            IRumour rumour = null;
            IRumour[] possibilities = this.RumourTypes.Where(r => r.FulfilsConditions(participants)).ToArray();

            if (possibilities.Length > 0)
            {
                IRumour selected = possibilities[this.Roller.Roll(0, possibilities.Length)];
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
                IRumour selected = this.RumourTypes[this.Roller.Roll(0, this.RumourTypes.Count)];
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

        public IRumour GenerateRumourFromTags(IJoyObject[] participants, string[] tags)
        {
            if (this.RumourTypes is null)
            {
                this.Initialise();
            }
            
            IRumour rumour = null;

            IRumour[] possibilities = this.RumourTypes.Where(r =>
                r.Tags.Intersect(tags, StringComparer.OrdinalIgnoreCase).Any() && r.FulfilsConditions(participants))
                .ToArray();
            
            if (possibilities.Length > 0)
            {
                IRumour resultingRumour = possibilities[this.Roller.Roll(0, possibilities.Length)];
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
                int result = this.Roller.Roll(0, this.RumourTypes.Count);
                IRumour resultingRumour = this.RumourTypes[result];
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

        public IRumour[] GenerateOneRumourOfEachType(IJoyObject[] participants)
        {
            List<IRumour> rumours = new List<IRumour>();
            foreach (IRumour type in this.RumourTypes)
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