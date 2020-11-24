﻿using System;
using System.Linq;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomanceNo : TopicData
    {
        public RomanceNo()
            : base(
                new ITopicCondition[0], 
                "RomanceNo",
                new []{ "BaseTopics" },
                "No thanks.",
                0,
                new string[0], 
                Speaker.INSTIGATOR)
        {}

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            IJoyAction influence = CachedActions.First(action =>
                action.Name.Equals("modifyrelationshippointsaction", StringComparison.OrdinalIgnoreCase));

            influence.Execute(
                new IJoyObject[]
                {
                    listener,
                    instigator
                },
                new[] {"friendship"},
                new object[] {-instigator.Statistics[EntityStatistic.PERSONALITY].Value});
            
            return FetchNextTopics();
        }
    }
}