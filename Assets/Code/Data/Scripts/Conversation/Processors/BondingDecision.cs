﻿using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class BondingDecision : TopicData
    {
        public BondingDecision(string decision)
            : base(
                new ITopicCondition[0], 
                "BondingDecision",
                new []{ "BaseTopics" },
                decision,
                0,
                new string[0], 
                Speaker.LISTENER,
                new RNG())
        {}
    }
}