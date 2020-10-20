﻿using System;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using DevionGames;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity.GUI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using MenuItem = DevionGames.UIWidgets.MenuItem;

namespace JoyLib.Code.Conversation
{
    public class ConversationEngine : MonoBehaviour
    {
        protected List<ITopic> m_Topics;
        protected List<ITopic> m_CurrentTopics;

        [SerializeField]
        protected GameObject s_Window;

        protected GUIManager GUIManager
        {
            get;
            set;
        }

        protected List<ConversationMenu> MenuList
        {
            get;
            set;
        }

        protected TextMeshProUGUI LastSaidGUI
        {
            get;
            set;
        }

        protected ITopic LastSaid
        {
            get;
            set;
        }

        protected Image LastSpokeIcon
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

                Window = Window is null ? GameObject.Find("Conversation Window") : Window;

                Transform title = Window.FindChild("Title", true).transform;
                LastSaidGUI = title.GetComponentInChildren<TextMeshProUGUI>();
                LastSpokeIcon = title.GetComponentInChildren<Image>();
                MenuItem = Window.FindChild("Menu Item", true);
                
                MenuList = new List<ConversationMenu>();

                this.GUIManager = this.gameObject.GetComponent<GUIManager>();
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

                        string speaker = line.Element("Speaker").DefaultIfEmpty("instigator");

                        string link = line.Element("Link").DefaultIfEmpty("");

                        Speaker speakerEnum = speaker.Equals("instigator", StringComparison.OrdinalIgnoreCase)
                            ? Speaker.INSTIGATOR
                            : Speaker.LISTENER;

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
                                ITopic processorObject = (ITopic) Activator.CreateInstance(type);
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
                                    link));
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

            topics = PerformLinks(topics);

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

        public void SetActors(Entity instigator, Entity listener)
        {
            Instigator = instigator;
            Listener = listener;

            LastSpokeIcon.sprite = Listener.Icon;
        }

        protected void SetLastSpoke(Entity speaker)
        {
            LastSpokeIcon.sprite = speaker.Icon;
        }
        
        public ITopic[] Converse(int index = 0)
        {
            if (CurrentTopics.Length == 0)
            {
                CurrentTopics = m_Topics.Where(topic => topic.ID.Equals("greeting", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                CurrentTopics = SanitiseTopics(CurrentTopics);
            }
            
            ITopic currentTopic = CurrentTopics[index];
            
            //Listener speaks
            //Instigator speaks
            //Listener speaks
            //Instigator speaks
            //HALT
            ITopic[] next = currentTopic.Interact(Instigator, Listener);

            next = SanitiseTopics(next);

            if (next.Length == 0)
            {
                this.GUIManager.CloseGUI(Window.name);
                CurrentTopics = next;
                return next;
            }
            
            switch (currentTopic.Speaker)
            {
                case Speaker.LISTENER:
                    LastSaid = currentTopic;
                    SetTitle(LastSaid.Words);
                    break;
                
                case Speaker.INSTIGATOR:
                    currentTopic = next[0];
                    if (currentTopic.Speaker == Speaker.LISTENER)
                    {
                        next = currentTopic.Interact(Instigator, Listener);
                        next = SanitiseTopics(next);
                        LastSaid = currentTopic;
                        SetTitle(LastSaid.Words);
                    }
                    break;
            }

            CurrentTopics = next;

            CreateMenuItems(CurrentTopics);
            return CurrentTopics;
        }

        protected ITopic[] SanitiseTopics(ITopic[] topics)
        {
            ITopic[] next = topics;
            next = GetValidTopics(next);
            next = TrimEmpty(next);
            next = SortByPriority(next);

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
                    tuples.AddRange(Listener.GetData(
                        new [] {condition.Criteria}, 
                        new object[] { Listener }));
                }

                if(topic.PassesConditions(tuples.ToArray()))
                {
                    validTopics.Add(topic);
                }
            }

            return validTopics.ToArray();
        }

        protected void SetTitle(string text)
        {
            LastSaidGUI.text = text;
        }

        protected ITopic[] SortByPriority(ITopic[] topics)
        {
            List<ITopic> sorting = new List<ITopic>(topics);

            return sorting.OrderByDescending(t => t.Priority).ToArray();
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
                MenuList[i].Index = i;
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

        public ITopic[] CurrentTopics
        {
            get
            {
                return m_CurrentTopics.ToArray();
            }
            set
            {
                m_CurrentTopics = value.ToList();
            }
        }

        public ITopic[] AllTopics => m_Topics.ToArray();
    }
}
