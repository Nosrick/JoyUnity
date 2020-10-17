using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;
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
            string[] cachedActions)
        {
            this.Conditions = conditions;
            this.ID = ID;
            this.NextTopics = nextTopics;
            this.Words = words;
            this.Priority = priority;

            List<IJoyAction> actions = new List<IJoyAction>(ScriptingEngine.instance.FetchActions(cachedActions));ScriptingEngine.instance.FetchActions(cachedActions);
            string[] standardActions = new[] {"fulfillneedaction", "modifyrelationshippointsaction"};
            actions.AddRange(ScriptingEngine.instance.FetchActions(standardActions));
            
            this.CachedActions = actions.ToArray();

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
        
        protected IJoyAction[] CachedActions { get; set; }
        
        protected static ConversationEngine ConversationEngine { get; set; }
        
        protected static EntityRelationshipHandler RelationshipHandler { get; set; }
        
        public string[] GetConditionTags()
        {
            return Conditions.Select(c => c.Criteria).ToArray();
        }

        public bool PassesConditions(Tuple<string, int>[] values)
        {
            foreach (Tuple<string, int> value in values)
            {
                ITopicCondition condition = Conditions.First(
                    c => c.Criteria.Equals(value.Item1, StringComparison.OrdinalIgnoreCase));

                if (!condition.FulfillsCondition(value.Item2))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual ITopic[] Interact(Entity instigator, Entity listener)
        {
            IJoyAction fulfillNeed = CachedActions.First(action => action.Name.Equals("fulfillneedaction", StringComparison.OrdinalIgnoreCase));
            IJoyAction influence = CachedActions.First(action =>
                action.Name.Equals("modifyrelationshippointsaction", StringComparison.OrdinalIgnoreCase));

            fulfillNeed.Execute(
                new JoyObject[] { instigator, listener },
                new [] { "friendship" },
                new object[] { "friendship", instigator.Statistics[EntityStatistic.PERSONALITY].Value, 0, true });

            string[] tags = RelationshipHandler.Get(
                new long[] {instigator.GUID, listener.GUID}).SelectMany(relationship => relationship.GetTags()).ToArray();
            
            influence.Execute(
                new JoyObject[] { instigator, listener },
                tags,
                new object[] { instigator.Statistics[EntityStatistic.PERSONALITY].Value });

            influence.Execute(
                new JoyObject[] {listener, instigator},
                tags,
                new object[] { listener.Statistics[EntityStatistic.PERSONALITY].Value });

            if (RelationshipHandler.IsFamily(instigator, listener))
            {
                fulfillNeed.Execute(
                    new JoyObject[] {instigator, listener},
                    new string[] {"family"},
                    new object[] {"family", instigator.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});
            }
            
            return FetchNextTopics();
        }

        protected ITopic[] FetchNextTopics()
        {
            return ConversationEngine.AllTopics
                .Where(topic => NextTopics.Contains(topic.ID))
                .ToArray();
        }
    }
}
