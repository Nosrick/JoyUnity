﻿using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class SexDecision : TopicData
    {
        protected bool Happening { get; set; }

        public SexDecision(string decision, bool happening)
            : base(
                new ITopicCondition[0],
                "SexDecision",
                new string[0],
                decision,
                0,
                new string[0],
                Speaker.LISTENER)
        {
            Happening = happening;
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            if (Happening == false)
            {
                return base.Interact(instigator, listener);
            }
            
            base.Interact(instigator, listener);
            
            IJoyAction fulfillNeed = instigator.FetchAction("fulfillneedaction");

            int listenerSatisfaction = (
                instigator.Statistics[EntityStatistic.INTELLECT].Value
                + instigator.Statistics[EntityStatistic.ENDURANCE].Value
                + instigator.Statistics[EntityStatistic.PERSONALITY].Value) / 3;
            
            int instigatorSatisfaction = (
                listener.Statistics[EntityStatistic.INTELLECT].Value
                + listener.Statistics[EntityStatistic.ENDURANCE].Value
                + listener.Statistics[EntityStatistic.PERSONALITY].Value) / 3;
            
            fulfillNeed.Execute(
                new IJoyObject[] {instigator},
                new[] {"sex", "need"},
                new object[] {"sex", instigatorSatisfaction, 5});
            fulfillNeed.Execute(
                new IJoyObject[] {listener},
                new[] {"sex", "need"},
                new object[] {"sex", listenerSatisfaction, 5});

            return FetchNextTopics();
        }

        protected override ITopic[] FetchNextTopics()
        {
            if (Happening)
            {
                return new ITopic[0];
            }
            else
            {
                return new ITopic[]
                {
                    new TopicData(
                        new ITopicCondition[0],
                        "SexReject",
                        new string[0],
                        this.Words,
                        0,
                        new string[0],
                        Speaker.LISTENER)
                };
            }
        }
    }
}