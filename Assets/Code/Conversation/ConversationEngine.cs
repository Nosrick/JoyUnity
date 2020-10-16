using System;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Conversation
{
    public class ConversationEngine : MonoBehaviour
    {
        protected List<ITopic> m_Topics;
        protected List<ITopic> m_CurrentTopics;
        protected List<ITopic> m_PreviousTopics;

        public void Awake()
        {
            if (m_Topics is null)
            {
                m_Topics = LoadTopics();
                
                m_CurrentTopics = new List<ITopic>();

                m_PreviousTopics = new List<ITopic>();
            }
        }

        public List<ITopic> LoadTopics()
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

                        List<ITopicCondition> conditions = new List<ITopicCondition>();
                        foreach (string condition in conditionStrings)
                        {
                            conditions.Add(ParseCondition(condition));
                        }

                        string[] actions = (from actionElement in line.Elements("Action")
                            select actionElement.GetAs<string>()).ToArray();

                        if (processor.Equals("NONE", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            try
                            {
                                Type type = ScriptingEngine.instance.FetchType(processor);
                                topics.Add((ITopic)Activator.CreateInstance(type));
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
                                    actions));
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
                                    actions));
                        }
                    }
                }
                catch (Exception e)
                {
                    ActionLog.instance.AddText("Could not load conversations from file " + file);
                    ActionLog.instance.AddText(e.Message);
                    ActionLog.instance.AddText(e.StackTrace);
                }
            }

            return topics;
        }
        
        public List<ITopic> Converse(Entity instigator, Entity listener, int selectedItem = 0)
        {
            ITopic[] next = m_CurrentTopics[selectedItem].Interact(instigator, listener);
            
            List<ITopic> validTopics = new List<ITopic>();

            foreach (ITopic topic in next)
            {
                ITopicCondition[] conditions = topic.Conditions;
                List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();

                foreach (ITopicCondition condition in conditions)
                {
                    tuples.AddRange(listener.GetData(
                        new [] {condition.Criteria}, 
                        new object[] { listener }));
                }

                if(topic.PassesConditions(tuples.ToArray()))
                {
                    validTopics.Add(topic);
                }
            }

            validTopics = TrimEmpty(validTopics);
            
            return validTopics;
        }

        protected List<ITopic> TrimEmpty(List<ITopic> topics)
        {
            List<ITopic> newTopics = new List<ITopic>(topics.Count);

            for(int i = 0; i < topics.Count; i++)
            {
                if(string.IsNullOrWhiteSpace(topics[i].Words) == false)
                {
                    newTopics.Add(topics[i]);
                }
            }

            return newTopics;
        }

        protected ITopicCondition ParseCondition(string conditionString)
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

        public ReadOnlyCollection<ITopic> CurrentTopics => m_CurrentTopics.AsReadOnly();

        public ReadOnlyCollection<ITopic> AllTopics => m_Topics.AsReadOnly();
    }
}
