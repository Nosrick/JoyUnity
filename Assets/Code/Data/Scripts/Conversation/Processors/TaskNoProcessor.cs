﻿using JoyLib.Code.Conversation.Conversations;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskNoProcessor : TopicData
    {
        public TaskNoProcessor() 
            : base(
                new ITopicCondition[0], 
                "TaskNo",
                new []{ "BaseTopics" }, 
                "I can't do that for you.", 
                0,
                new string[0], 
                Speaker.INSTIGATOR)
        {
        }
    }
}