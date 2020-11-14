using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

namespace JoyLib.Code.Conversation.Conversations
{
    public class TopicData : ITopic
    {
        public TopicData(
            ITopicCondition[] conditions,
            string ID,
            string[] nextTopics,
            string words,
            int priority,
            string[] cachedActions,
            Speaker speaker,
            string link = "")
        {
            Initialise(
                conditions,
                ID,
                nextTopics,
                words,
                priority,
                cachedActions,
                speaker,
                link);

            if (ConversationEngine is null)
            {
                GameObject gameManager = GameObject.Find("GameManager");
                ConversationEngine = gameManager.GetComponent<ConversationEngine>();
                RelationshipHandler = gameManager.GetComponent<EntityRelationshipHandler>();
            }
        }

        public ITopicCondition[] Conditions { get; protected set; }
        public string ID { get; protected set; }
        public string[] NextTopics { get; protected set; }
        public string Words { get; protected set; }
        public int Priority { get; protected set; }
        
        public Speaker Speaker { get; protected set; }
        
        public string Link { get; protected set; }
        
        public IJoyAction[] CachedActions { get; protected set; }
        
        protected static ConversationEngine ConversationEngine { get; set; }
        
        protected static EntityRelationshipHandler RelationshipHandler { get; set; }
        
        public string[] GetConditionTags()
        {
            return Conditions.Select(c => c.Criteria).ToArray();
        }

        public bool FulfilsConditions(IEnumerable<Tuple<string, int>> values)
        {
            bool any = values.Any();
            
            foreach (ITopicCondition condition in Conditions)
            {
                try
                {
                    if (!any)
                    {
                        if (condition.FulfillsCondition(0) == false)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (values.Any(
                            pair => pair.Item1.Equals(condition.Criteria, StringComparison.OrdinalIgnoreCase)) == false)
                        {
                            if (condition.FulfillsCondition(0) == false)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            int value = values.Where(pair =>
                                    pair.Item1.Equals(condition.Criteria, StringComparison.OrdinalIgnoreCase))
                                .Max(tuple => tuple.Item2);
                            
                            if (condition.FulfillsCondition(value) == false)
                            {
                                return false;
                            }
                        }
                        
                    }
                    
                }
                catch (Exception e)
                {
                    //suppress this
                    return false;
                }
            }

            return true;
        }

        public bool FulfilsConditions(IEnumerable<JoyObject> participants)
        {
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

        public void Initialise(
            ITopicCondition[] conditions, 
            string ID, 
            string[] nextTopics, 
            string words, 
            int priority,
            string[] cachedActions,
            Speaker speaker,
            string link = "")
        {
            this.Conditions = conditions;
            this.ID = ID;
            this.NextTopics = nextTopics;
            this.Words = words;
            this.Priority = priority;

            this.CachedActions = GetCachedActions(cachedActions);

            this.Speaker = speaker;
            this.Link = link;
        }

        public void Initialise(
            ITopicCondition[] conditions,
            string ID,
            string[] nextTopics,
            string words,
            int priority,
            IJoyAction[] actions,
            Speaker speaker,
            string link)
        {
            this.Conditions = conditions;
            this.ID = ID;
            this.NextTopics = nextTopics;
            this.Words = words;
            this.Priority = priority;

            this.CachedActions = actions;

            this.Speaker = speaker;
            this.Link = link;
        }

        protected IJoyAction[] GetCachedActions(string[] actionNames)
        {
            List<IJoyAction> actions = new List<IJoyAction>(ScriptingEngine.instance.FetchActions(actionNames));
            string[] standardActions = new[] {"fulfillneedaction", "modifyrelationshippointsaction"};
            actions.AddRange(ScriptingEngine.instance.FetchActions(standardActions));

            return actions.ToArray();
        }

        public virtual ITopic[] Interact(Entity instigator, Entity listener)
        {
            IJoyAction fulfillNeed = CachedActions.First(action => action.Name.Equals("fulfillneedaction", StringComparison.OrdinalIgnoreCase));
            IJoyAction influence = CachedActions.First(action =>
                action.Name.Equals("modifyrelationshippointsaction", StringComparison.OrdinalIgnoreCase));

            fulfillNeed.Execute(
                new IJoyObject[] { instigator, listener },
                new [] { "friendship" },
                new object[] { "friendship", instigator.Statistics[EntityStatistic.PERSONALITY].Value, 0, true });

            string[] tags = RelationshipHandler.Get(
                new IJoyObject[] {instigator, listener}).SelectMany(relationship => relationship.Tags).ToArray();
            
            influence.Execute(
                new IJoyObject[] { instigator, listener },
                tags,
                new object[] { instigator.Statistics[EntityStatistic.PERSONALITY].Value });

            influence.Execute(
                new IJoyObject[] {listener, instigator},
                tags,
                new object[] { listener.Statistics[EntityStatistic.PERSONALITY].Value });

            if (RelationshipHandler.IsFamily(instigator, listener))
            {
                fulfillNeed.Execute(
                    new IJoyObject[] {instigator, listener},
                    new string[] {"family"},
                    new object[] {"family", instigator.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});
            }
            
            return FetchNextTopics();
        }

        protected virtual ITopic[] FetchNextTopics()
        {
            List<ITopic> nextTopics = ConversationEngine.AllTopics
                .Where(topic => NextTopics.Contains(topic.ID))
                .ToList();

            return nextTopics.ToArray();
        }
    }
}
