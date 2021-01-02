using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Conversation
{
    public class ConversationEngine : IConversationEngine
    {
        protected List<ITopic> m_Topics;
        protected List<ITopic> m_CurrentTopics;

        protected GameObject m_Window;

        protected IGUIManager GUIManager
        {
            get;
            set;
        }

        protected ITopic LastSaid
        {
            get;
            set;
        }

        public string LastSaidWords { get; protected set; }

        public IEntity Instigator
        {
            get;
            protected set;
        }

        public IEntity Listener
        {
            get;
            protected set;
        }
        
        public string ListenerInfo { get; protected set; }

        public event EventHandler OnConverse;
        public event EventHandler OnOpen;
        public event EventHandler OnClose;
        
        protected IEntityRelationshipHandler RelationshipHandler { get; set; }

        public ConversationEngine(
            IEntityRelationshipHandler relationshipHandler)
        {
            this.RelationshipHandler = relationshipHandler;

            this.m_Topics = this.LoadTopics();

            this.m_CurrentTopics = new List<ITopic>();
        }

        protected List<ITopic> LoadTopics()
        {
            List<ITopic> topics = new List<ITopic>();

            string[] files = Directory.GetFiles(
                Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Conversation",
                "*.xml",
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);
                    string topicName = doc.Element("Name").GetAs<string>();

                    foreach (XElement line in doc.Elements("Line"))
                    {
                        string text = line.Element("Text").GetAs<string>("SOMEONE FORGOT TO INCLUDE TEXT.");
                        string processor = line.Element("Processor").DefaultIfEmpty("NONE");

                        string[] conditionStrings = (from conditionElement in line.Elements("Condition")
                            select conditionElement.GetAs<string>()).ToArray();

                        string[] nextTopics = (from nextTopicElement in line.Elements("Next")
                            select nextTopicElement.GetAs<string>()).ToArray();
                        
                        int priority = line.Element("Priority").DefaultIfEmpty(0);

                        string speaker = line.Element("Speaker").DefaultIfEmpty("instigator");

                        string link = line.Element("Link").DefaultIfEmpty("");

                        Speaker speakerEnum = speaker.Equals("instigator", StringComparison.OrdinalIgnoreCase)
                            ? Speaker.INSTIGATOR
                            : Speaker.LISTENER;

                        List<ITopicCondition> conditions = new List<ITopicCondition>();
                        foreach (string condition in conditionStrings)
                        {
                            conditions.Add(this.ParseCondition(condition));
                        }

                        string[] actions = (from actionElement in line.Elements("Action")
                            select actionElement.GetAs<string>()).ToArray();

                        if (processor.Equals("NONE", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            try
                            {
                                ITopic processorObject = (ITopic)ScriptingEngine.instance.FetchAndInitialise(processor);
                                processorObject.Initialise(
                                    conditions.ToArray(),
                                    topicName,
                                    nextTopics.ToArray(),
                                    text,
                                    priority,
                                    actions,
                                    speakerEnum,
                                    link);
                                
                                topics.Add(processorObject);
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning("Could not find topic processor " + processor);
                                topics.Add(new TopicData(
                                    conditions.ToArray(),
                                    topicName,
                                    nextTopics.ToArray(),
                                    text,
                                    priority,
                                    actions,
                                    speakerEnum,
                                    null,
                                    link));
                            }
                        }
                        else
                        {
                            topics.Add(new TopicData(
                                    conditions.ToArray(),
                                    topicName,
                                    nextTopics.ToArray(),
                                    text,
                                    priority,
                                    actions,
                                    speakerEnum,
                                    null,
                                    link));
                        }
                    }
                }
                catch (Exception e)
                {
                    GlobalConstants.ActionLog.AddText("Could not load conversations from file " + file);
                    Debug.LogWarning("Could not load conversations from file " + file);
                    Debug.LogWarning(e.Message);
                    Debug.LogWarning(e.StackTrace);
                }
            }

            topics = this.PerformLinks(topics);

            return topics;
        }

        protected List<ITopic> PerformLinks(List<ITopic> topics)
        {
            List<ITopic> linked = new List<ITopic>(topics);

            foreach (ITopic topic in topics)
            {
                if (topic.Link.IsNullOrEmpty() == false)
                {
                    ITopic[] links = topics.Where(left =>
                            topics.Any(right => right.Link.Equals(left.ID, StringComparison.OrdinalIgnoreCase)))
                        .ToArray();

                    foreach (ITopic link in links)
                    {
                        link.Initialise(
                            link.Conditions,
                            topic.ID,
                            link.NextTopics,
                            link.Words,
                            link.Priority,
                            link.CachedActions,
                            link.Speaker,
                            link.Link);
                    }
                    linked.Remove(topic);
                }
            }
            
            return linked;
        }

        public void SetActors(IEntity instigator, IEntity listener)
        {
            this.Instigator = instigator;
            this.Listener = listener;

            try
            {
                IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(new IJoyObject[] {this.Instigator, this.Listener});

                IRelationship chosenRelationship = null;
                int best = Int32.MinValue;
                foreach (IRelationship relationship in relationships)
                {
                    if (relationship.HasTag("romantic"))
                    {
                        chosenRelationship = relationship;
                        break;
                    }
                    
                    int value = relationship.GetRelationshipValue(this.Instigator.GUID, this.Listener.GUID);
                    if (value > best)
                    {
                        best = value;
                        chosenRelationship = relationship;
                    }
                }

                this.ListenerInfo = this.Listener.JoyName + ", " + chosenRelationship.DisplayName;
            }
            catch (Exception e)
            {
                this.ListenerInfo = this.Listener.JoyName + ", acquaintance.";
            }

            this.OnOpen?.Invoke(this, EventArgs.Empty);
        }

        public ITopic[] Converse(string topic, int index = 0)
        {
            if (this.CurrentTopics.Length == 0)
            {
                this.CurrentTopics = this.m_Topics.Where(t => t.ID.Equals(topic, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                this.CurrentTopics = this.SanitiseTopics(this.CurrentTopics);
            }
            
            ITopic currentTopic = this.CurrentTopics[index];

            this.DoInteractions(currentTopic);

            if (this.Instigator is null == false && this.Listener is null == false)
            {
                this.SetActors(this.Instigator, this.Listener);
                this.OnConverse?.Invoke(this, EventArgs.Empty);
            }
            return this.CurrentTopics;
        }

        public ITopic[] Converse(int index = 0)
        {
            return this.Converse("greeting", index);
        }

        protected void DoInteractions(ITopic currentTopic)
        {
            ITopic[] next = currentTopic.Interact(this.Instigator, this.Listener);

            next = this.SanitiseTopics(next);

            if (next.Length == 0)
            {
                this.OnClose?.Invoke(this, EventArgs.Empty);
                this.CurrentTopics = next;
                this.Listener = null;
                this.Instigator = null;
                return;
            }
            
            switch (currentTopic.Speaker)
            {
                case Speaker.LISTENER:
                    this.LastSaid = currentTopic;
                    this.LastSaidWords = this.LastSaid.Words;
                    break;
                
                case Speaker.INSTIGATOR:
                    currentTopic = next[0];
                    if (currentTopic.Speaker == Speaker.LISTENER)
                    {
                        next = currentTopic.Interact(this.Instigator, this.Listener);
                        next = this.SanitiseTopics(next);
                        this.LastSaid = currentTopic;
                        this.LastSaidWords = this.LastSaid.Words;
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.CurrentTopics = next;

            if (next.Length == 0)
            {
                this.OnClose?.Invoke(this, EventArgs.Empty);
                this.CurrentTopics = next;
                this.Listener = null;
                this.Instigator = null;
            }
        }

        protected ITopic[] SanitiseTopics(ITopic[] topics)
        {
            ITopic[] next = topics;
            next = this.GetValidTopics(next);
            next = this.TrimEmpty(next);
            next = this.SortByPriority(next);

            return next;
        }

        protected ITopic[] GetValidTopics(ITopic[] topics)
        {
            List<ITopic> validTopics = new List<ITopic>();
            foreach (ITopic topic in topics)
            {
                ITopicCondition[] conditions = topic.Conditions;
                List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();

                foreach (ITopicCondition condition in conditions)
                {
                    tuples.AddRange(this.Listener.GetData(
                        new [] {condition.Criteria}, 
                        new object[] {this.Listener }));
                }

                if(topic.FulfilsConditions(tuples.ToArray()))
                {
                    validTopics.Add(topic);
                }
            }

            return validTopics.ToArray();
        }

        protected ITopic[] SortByPriority(ITopic[] topics)
        {
            List<ITopic> sorting = new List<ITopic>(topics);

            return sorting.OrderByDescending(t => t.Priority).ToArray();
        }

        

        protected ITopic[] TrimEmpty(ITopic[] topics)
        {
            List<ITopic> newTopics = new List<ITopic>(topics.Length);

            for(int i = 0; i < topics.Length; i++)
            {
                if(string.IsNullOrWhiteSpace(topics[i].Words) == false)
                {
                    newTopics.Add(topics[i]);
                }
            }

            return newTopics.ToArray();
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

                int value = int.MinValue;
                if (!int.TryParse(stringValue, out value))
                {
                    value = 1;
                    criteria = stringValue;
                }

                return factory.Create(criteria, operand, value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not parse conversation condition line " + conditionString);
            }
        }

        public ITopic[] CurrentTopics
        {
            get
            {
                return this.m_CurrentTopics.ToArray();
            }
            set
            {
                this.m_CurrentTopics = value.ToList();
            }
        }

        public ITopic[] AllTopics => this.m_Topics.ToArray();
    }
}
