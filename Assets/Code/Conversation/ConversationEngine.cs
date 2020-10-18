using System;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using DevionGames;
using DevionGames.UIWidgets;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JoyLib.Code.Conversation
{
    public class ConversationEngine : MonoBehaviour
    {
        protected List<ITopic> m_Topics;
        protected List<ITopic> m_CurrentTopics;
        protected List<ITopic> m_PreviousTopics;

        [SerializeField]
        protected GameObject s_Window;

        protected List<ConversationMenu> MenuList
        {
            get;
            set;
        }

        protected TextMeshProUGUI LastSaid
        {
            get;
            set;
        }

        protected Image ListenerIcon
        {
            get;
            set;
        }

        protected GameObject Window
        {
            get => s_Window;
            set => s_Window = value;
        }

        protected GameObject MenuItem
        {
            get;
            set;
        }

        protected Entity Instigator
        {
            get;
            set;
        }

        protected Entity Listener
        {
            get;
            set;
        }

        public void Awake()
        {
            if (m_Topics is null)
            {
                m_Topics = LoadTopics();
                
                m_CurrentTopics = new List<ITopic>();

                m_PreviousTopics = new List<ITopic>();

                Window = Window is null ? GameObject.Find("Conversation Window") : Window;

                Transform title = Window.FindChild("Title", true).transform;
                LastSaid = title.GetComponentInChildren<TextMeshProUGUI>();
                ListenerIcon = title.GetComponentInChildren<Image>();
                MenuItem = Window.FindChild("Menu Item", true);
                
                MenuList = new List<ConversationMenu>();
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

        public void SetActors(Entity instigator, Entity listener)
        {
            Instigator = instigator;
            Listener = listener;

            ListenerIcon.sprite = Listener.Icon;
        }
        
        public List<ITopic> Converse(string topicID)
        {
            ITopic currentTopic;
            
            List<ITopic> validTopics = new List<ITopic>();
            
            if (m_CurrentTopics.IsNullOrEmpty())
            {
                ITopic[] topics = m_Topics.Where(t => t.ID.Equals(topicID, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                
                foreach (ITopic topic in topics)
                {
                    ITopicCondition[] conditions = topic.Conditions;
                    List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();

                    foreach (ITopicCondition condition in conditions)
                    {
                        tuples.AddRange(Listener.GetData(
                            new [] {condition.Criteria}, 
                            new object[] { Listener }));
                    }

                    if(topic.PassesConditions(tuples.ToArray()))
                    {
                        validTopics.Add(topic);
                    }
                }
                
                currentTopic = validTopics[RNG.instance.Roll(0, validTopics.Count)];
            }
            else
            {
                currentTopic = m_CurrentTopics.First(t => t.ID.Equals(topicID, StringComparison.OrdinalIgnoreCase));
            }

            SetTitle(currentTopic.Words);
            
            ITopic[] next = currentTopic.Interact(Instigator, Listener);

            validTopics = new List<ITopic>();
            foreach (ITopic topic in next)
            {
                ITopicCondition[] conditions = topic.Conditions;
                List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();

                foreach (ITopicCondition condition in conditions)
                {
                    tuples.AddRange(Listener.GetData(
                        new [] {condition.Criteria}, 
                        new object[] { Listener }));
                }

                if(topic.PassesConditions(tuples.ToArray()))
                {
                    validTopics.Add(topic);
                }
            }

            validTopics = TrimEmpty(validTopics);
            
            CreateMenuItems(validTopics.ToArray());
            
            return validTopics;
        }

        protected void SetTitle(string text)
        {
            LastSaid.text = text;
        }

        protected void CreateMenuItems(ITopic[] topics)
        {
            if (topics.Length > MenuList.Count)
            {
                for (int i = MenuList.Count; i < topics.Length; i++)
                {
                    ConversationMenu child = GameObject.Instantiate(MenuItem, Window.transform).GetComponent<ConversationMenu>();
                    MenuList.Add(child);
                }
            }
            
            for(int i = 0; i < topics.Length; i++)
            {
                MenuList[i].TopicID = topics[i].ID;
                MenuList[i].SetText(topics[i].Words);
                MenuList[i].gameObject.SetActive(true);
                ConversationMenu menu = MenuList[i].GetComponent<ConversationMenu>();
                MenuItem menuItem = MenuList[i].GetComponent<MenuItem>();
                Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
                buttonClickedEvent.AddListener(menu.OnMouseDown);
                menuItem.onTrigger = buttonClickedEvent;
            }

            for (int i = topics.Length; i < MenuList.Count; i++)
            {
                MenuList[i].gameObject.SetActive(false);
            }
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
