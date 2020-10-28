using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations.Rumours
{
    public class BaseRumour : IRumour
    {
        public JoyObject[] Participants { get; protected set; }
        public string[] Tags { get; protected set; }
        public int ViralPotential { get; protected set; }
        public ITopicCondition[] Conditions { get; protected set; }
        public string[] Parameters { get; protected set; }
        public string Words { get; protected set; }
        public bool Baseless { get; protected set; }

        protected string RegexMatcher => @"/{\d}";

        protected static ParameterProcessorHandler ProcessorHandler
        {
            get;
            set;
        }

        public BaseRumour()
        {
            Initialise();
        }

        public BaseRumour(
            JoyObject[] participants,
            string[] tags,
            int viralPotential,
            ITopicCondition[] conditions,
            string[] parameters,
            string words,
            bool baseless = false)
        {
            this.Participants = participants;
            this.Tags = tags;
            this.ViralPotential = viralPotential;
            this.Conditions = conditions;
            this.Parameters = parameters;
            this.Words = words;
            this.Baseless = baseless;
        }

        protected void Initialise()
        {
            if (ProcessorHandler is null)
            {
                ProcessorHandler = GameObject.Find("GameManager").GetComponent<ParameterProcessorHandler>();
            }
        }

        public bool FulfilsConditions(IEnumerable<Tuple<string, int>> values)
        {
            if (Baseless)
            {
                return true;
            }
            
            foreach (ITopicCondition condition in Conditions)
            {
                int value = values.Where(pair =>
                    pair.Item1.Equals(condition.Criteria, StringComparison.OrdinalIgnoreCase))
                    .Max(tuple => tuple.Item2);
                if (condition.FulfillsCondition(value) == false)
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool FulfilsConditions(IEnumerable<JoyObject> participants)
        {
            if (Baseless)
            {
                return true;
            }
            
            string[] criteria = Conditions.Select(c => c.Criteria).ToArray();

            List<Tuple<string, int>> values = new List<Tuple<string, int>>();
            foreach (JoyObject participant in participants)
            {
                if (participant is Entity entity)
                {
                    JoyObject[] others = participants.Where(p => p.GUID.Equals(participant.GUID) == false).ToArray();
                    values.AddRange(entity.GetData(criteria, others));                    
                }
            }

            return this.FulfilsConditions(values);
        }

        public string ConstructString()
        {
            if (Words.Contains("{") == false)
            {
                return Words;
            }
            
            Regex regex = new Regex(RegexMatcher);

            MatchCollection matches = regex.Matches(Words);
            if (matches.Count != Parameters.Length)
            {
                return "PARAMETER NUMBER MISMATCH. SOMEONE ENTERED THE WRONG NUMBER OF PARAMETER REPLACEMENTS.";
            }

            for (int i = 0; i < matches.Count; i++)
            {
                foreach(JoyObject obj in Participants)
                {
                    string replacement = ProcessorHandler
                        .Get(Parameters[i])
                        .Parse(Parameters[i], obj);

                    i++;
                    Words = Words.Replace("{" + i + "}", replacement);
                }
            }

            return Words;
        }

        public IRumour Create(
            JoyObject[] participants, 
            string[] tags, 
            int viralPotential, 
            ITopicCondition[] conditions,
            string[] parameters, 
            string words, 
            bool baseless = false)
        {
            return new BaseRumour(
                participants,
                tags,
                viralPotential,
                conditions,
                parameters,
                words,
                baseless);
        }
    }
}