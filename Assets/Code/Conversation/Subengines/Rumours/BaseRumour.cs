using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations.Rumours
{
    public class BaseRumour : IRumour
    {
        public const int DEFAULT_LIFETIME = 5000;
        public JoyObject[] Participants { get; protected set; }
        public string[] Tags { get; protected set; }
        public float ViralPotential { get; protected set; }
        
        public float LifetimeMultiplier { get; protected set; }
        public ITopicCondition[] Conditions { get; protected set; }
        public string[] Parameters { get; protected set; }

        public int Lifetime { get; protected set; }

        public bool IsAlive => Lifetime > 0;

        public string Words
        {
            get
            {
                if (m_Words.Contains("<") == false)
                {
                    return m_Words;
                }
                m_Words = ConstructString();
                return m_Words;
            }
            protected set
            {
                m_Words = value;
            }
        }
        public bool Baseless { get; protected set; }

        protected string m_Words;

        protected static IParameterProcessorHandler ProcessorHandler
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
            float viralPotential,
            ITopicCondition[] conditions,
            string[] parameters,
            string words,
            float lifetimeMultiplier = 1f,
            int lifetime = DEFAULT_LIFETIME,
            bool baseless = false)
        {
            this.Participants = participants;
            this.Tags = tags;
            this.ViralPotential = viralPotential;
            this.Conditions = conditions;
            this.Parameters = parameters;
            this.Words = words;
            this.LifetimeMultiplier = lifetimeMultiplier;
            this.Lifetime = (int)Math.Ceiling(lifetime * this.LifetimeMultiplier);
            this.Baseless = baseless;
            
            Initialise();
        }

        protected void Initialise()
        {
            if (ProcessorHandler is null)
            {
                ProcessorHandler = GlobalConstants.GameManager.ParameterProcessorHandler;
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
                if (values.Any() == false)
                {
                    if (condition.FulfillsCondition(0) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (values.Any(pair => pair.Item1.Equals(condition.Criteria)) == false)
                    {
                        if (condition.FulfillsCondition(0) == false)
                        {
                            return false;
                        }
                    }
                    
                    int value = values.Where(pair =>
                            pair.Item1.Equals(condition.Criteria, StringComparison.OrdinalIgnoreCase))
                        .Max(tuple => tuple.Item2);

                    if (condition.FulfillsCondition(value) == false)
                    {
                        return false;
                    }
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

        public int Tick()
        {
            return --Lifetime;
        }

        public string ConstructString()
        {
            if (Participants is null)
            {
                return m_Words;
            }
            
            int count = 0;
            for (int i = 1; i <= Parameters.Length; i++)
            {
                if (m_Words.Contains("<" + i + ">"))
                {
                    count++;
                }
            }
            
            if (count != Parameters.Length)
            {
                m_Words = "PARAMETER NUMBER MISMATCH. SOMEONE ENTERED THE WRONG NUMBER OF PARAMETER REPLACEMENTS.";
                return m_Words;
            }

            int participantNumber = 0;
            IJoyObject obj = null;
            for (int i = 0; i < count; i++)
            {
                if (Parameters[i].Equals("participant", StringComparison.OrdinalIgnoreCase))
                {
                    obj = Participants[participantNumber];
                    participantNumber++;
                }

                string replacement = ProcessorHandler
                    .Get(Parameters[i])
                    .Parse(Parameters[i], obj);
                
                m_Words = m_Words.Replace("<" + (i + 1) + ">", replacement);
            }

            return m_Words;
        }

        public IRumour Create(
            JoyObject[] participants, 
            string[] tags, 
            float viralPotential, 
            ITopicCondition[] conditions,
            string[] parameters, 
            string words, 
            float lifetimeMultiplier = 1f,
            int lifetime = DEFAULT_LIFETIME,
            bool baseless = false)
        {
            return new BaseRumour(
                participants,
                tags,
                viralPotential,
                conditions,
                parameters,
                words,
                lifetimeMultiplier,
                lifetime,
                baseless);
        }
    }
}